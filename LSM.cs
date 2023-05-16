using Accord.Math;
using CSML;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SLN
{
    [Serializable]
    internal class LSM
    {
        [Serializable]
        public struct Connection
        {
            public int index;  // Neuron in the input layer
            public double w;  // Weight of the connection

            public Connection(int index, double w)
            {
                this.index = index;
                this.w = w;
            }
        }

        public Neuron[] all_neurons;
        public Neuron[] exc_neurons;
        public Neuron[] inh_neurons;
        public Neuron[] rec_neurons;

        public int n_exc;
        public int n_inh;
        public int n_rec;

        public LinkedList<Synapse> all_synapses;
        public LinkedList<Connection>[] connectivityMatrix;


        private Random rand = new Random(123);
        public int current_step;
        public int input_size;


        /// <summary>
        /// Initializes a new instance of the Liquid State Machine (LSM) with specified numbers of excitatory,
        /// inhibitory and readout neurons.
        /// Populates the LSM with neurons and initializes the synaptic connections between them.
        /// </summary>
        /// <param name="n_exc">Number of excitatory neurons</param>
        /// <param name="n_inh">Number of inhibitory neurons</param>
        /// <param name="n_rec">Number of readout neurons</param
        internal LSM(int n_exc, int n_inh, int n_rec, int input_size)
        {
            current_step = 0;
            this.input_size = input_size;
            connectivityMatrix = new LinkedList<Connection>[input_size];

            this.n_exc = n_exc;
            this.n_inh = n_inh;
            this.n_rec = n_rec;

            all_neurons = new Neuron[n_exc + n_inh];
            exc_neurons = new Neuron[n_exc];
            inh_neurons = new Neuron[n_inh];
            rec_neurons = new Neuron[n_rec];

            //Populating liquid layer
            for (int i = 0; i < n_exc + n_inh; i++)
            {
                all_neurons[i] = new Class1Neuron();
                all_neurons[i].IBias = 0; //32
                all_neurons[i].V = Constants.INITIAL_STATE_V_LIQUID;
                all_neurons[i].U = Constants.INITIAL_STATE_U_LIQUID;
            }

            int indice = 0;
            for (int i = 0; i < n_exc; i++)
                exc_neurons[i] = all_neurons[indice++];
            for (int i = 0; i < n_inh; i++)
                inh_neurons[i] = all_neurons[indice++];
            for (int i = 0; i < n_rec; i++)
                rec_neurons[i] = exc_neurons[i];

            all_synapses = new LinkedList<Synapse>();

            Connect(exc_neurons, exc_neurons, 2, 10 * 5);
            Connect(exc_neurons, inh_neurons, 2, 10 * 25);
            Connect(inh_neurons, exc_neurons, 1, 10 * (-20));
            Connect(inh_neurons, inh_neurons, 1, 10 * (-20));

            ConnectInputToLiquid();
        }



        /// <summary>
        /// Connects the pre-synaptic and post-synaptic neurons with specified degree and synaptic efficacy. 
        /// Assigns weights and delays to the synapses.
        /// </summary>
        /// <param name="pre">Array of pre-synaptic neurons</param>
        /// <param name="post">Array of post-synaptic neurons</param>
        /// <param name="indegree">Degree of the connections</param>
        /// <param name="J">Synaptic efficacy</param>
        public void Connect(Neuron[] pre, Neuron[] post, int indegree, double J)
        {
            double delay = NextGaussian(10, 20, 3, 200);
            double w, tau;
            if (J >= 0)
            {
                w = NextGaussian(J, 0.7 * J, 0, 100000);
                tau = 3;
            }
            else
            {
                w = NextGaussian(J, 0.7 * (-J), -100000, 0);
                tau = 2;
            }

            for (int i = 0; i < post.Length; i++)
            {
                for (int j = 0; j < indegree; j++)
                {
                    Synapse syn = new Synapse(pre[rand.Next(pre.Length)], post[i], w, 0, tau, (int)(delay / Constants.INTEGRATION_STEP), 1);
                    all_synapses.AddLast(syn);
                }
            }
        }

        /// <summary>
        /// Simulates a single time step of the entire LSM. Updates the state of all neurons and synapses.
        /// </summary>
        /// <param name="step">Current time step</param>
        /// <param name="parallel">Flag indicating whether to use parallel computation</param>
        internal void simulateLiquidStep(int step, bool parallel)
        {
            if (!parallel)
            {
                for (int i = 0; i < all_neurons.Length; i++)
                {
                    all_neurons[i].simulate(step);
                }

                foreach (Synapse syn in all_synapses)
                {
                    syn.simulate(step);
                }
            }
            else
            {
                for (int i = 0; i < all_neurons.Length; i++)
                {
                    all_neurons[i].simulate(step);
                }

                Parallel.ForEach(all_synapses, syn =>
                {
                    syn.simulate(step);
                });

            }


        }

        /// <summary>
        /// Simulates the LSM for a specified number of time steps.
        /// </summary>
        /// <param name="nsteps">Number of time steps to simulate</param>
        /// <param name="parallel">Flag indicating whether to use parallel computation</param>
        public void simulateLiquid(int nsteps, bool parallel)
        {
            for (int step = 0; step < nsteps; step++)
            {
                simulateLiquidStep(current_step++, parallel);
            }
        }

        /// <summary>
        /// Connects the input layer to the liquid (LSM) with Gaussian weights.
        /// </summary>
        public void ConnectInputToLiquid()
        {
            double weight;
            for (int i = 0; i < input_size; i++)
            {
                connectivityMatrix[i] = new LinkedList<Connection>();

                for (int e = 0; e < n_exc; e++)
                {
                    weight = NextGaussian(2.65, 0.025, 0, 14.9);
                    connectivityMatrix[i].AddLast(new Connection(e, weight));
                }

            }
        }

        /// <summary>
        /// Sets the current of the neurons in the LSM based on the provided input current and gain.
        /// </summary>
        /// <param name="input_current">2D array of input currents</param>
        /// <param name="gain">Scaling factor for the currents</param>
        public void SetLiquidCurrent(double[] input_current, double gain)
        {
            this.ResetLiquidBiasCurrent();
            for (int i = 0; i < input_current.Length; i++)
                foreach (Connection conn in connectivityMatrix[i])
                    exc_neurons[conn.index].IBias += input_current[i] * conn.w * gain;
            return;
        }

        /// <summary>
        /// Resets the bias current of all neurons in the LSM to zero.
        /// </summary>
        public void ResetLiquidBiasCurrent()
        {
            for (int i = 0; i < n_exc; i++)
                all_neurons[i].IBias = 0;
            return;
        }

        /// <summary>
        /// Resets the state of all neurons in the LSM.
        /// </summary>
        internal void resetState()
        {
            for (int i = 0; i < all_neurons.Length; i++)
                all_neurons[i].resetState();
            foreach (Synapse syn in all_synapses)
                syn.resetState();
            current_step = 0;
        }

        /// <summary>
        /// Generates a Gaussian random number with specified mean and standard deviation.
        /// </summary>
        /// <param name="mu">Mean of the Gaussian distribution</param>
        /// <param name="sigma">Standard deviation of the Gaussian distribution</param>
        /// <returns>A Gaussian random number</returns>
        public double NextGaussian(double mu, double sigma)
        {
            // These are uniform(0,1) random doubles
            double u1 = rand.NextDouble();
            double u2 = rand.NextDouble();
            // Using Box-Muller transform to get two standard normally distributed numbers
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) *
                Math.Sin(2.0 * Math.PI * u2);

            // Scaling and shifting standard normal random numbers to get desired distribution
            double randNormal = mu + sigma * randStdNormal;

            return randNormal;
        }

        /// <summary>
        /// Generates a Gaussian random number with specified mean, standard deviation, 
        /// minimum and maximum values.
        /// Keeps generating until the generated number falls within the specified range.
        /// </summary>
        /// <param name="mu">Mean of the Gaussian distribution</param>
        /// <param name="sigma">Standard deviation of the Gaussian distribution</param>
        /// <param name="minValue">Minimum value of the generated number</param>
        /// <param name="maxValue">Maximum value of the generated number</param>
        /// <returns>A Gaussian random number within the specified range</returns>
        public double NextGaussian(double mu, double sigma, double minValue, double maxValue)
        {
            double randNormal;
            do
            {
                // These are uniform(0,1) random doubles
                double u1 = rand.NextDouble();
                double u2 = rand.NextDouble();

                // Using Box-Muller transform to get two standard normally distributed numbers
                double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) *
                    Math.Sin(2.0 * Math.PI * u2);

                // Scaling and shifting standard normal random numbers to get desired distribution
                randNormal = mu + sigma * randStdNormal;
            }
            while (randNormal < minValue || randNormal > maxValue);

            return randNormal;
        }


        /// <summary>
        /// Retrieves the states of the readout neurons at a specified time step.
        /// </summary>
        /// <param name="step">Time step at which to retrieve the states</param>
        /// <param name="tau">Time constant</param>
        /// <returns>Array of neuron states</returns>
        public double[] GetLiquidStates(int step, double tau)
        {
            double[] states = new double[n_rec];
            for (int i = 0; i < n_rec; i++)
                states[i] = rec_neurons[i].getState(step, tau);

            return states;
        }

        public void SaveState(int step, double tau, int dim1, int dim2, String filePath)
        {
            double[] state = GetLiquidStates(step, tau);
            using (StreamWriter file = new StreamWriter(filePath))
            {


                for (int i = 0; i < dim1; i++)
                {
                    for (int j = 0; j < dim2; j++)
                        file.Write(state[i * dim2 + j].ToString(CultureInfo.InvariantCulture) + " ");
                    // Write a newline character after each row except the last one
                    if (i < dim1 - 1)
                        file.WriteLine();
                }
            }
        }

        public void SavePotential(int dim1, int dim2, String filePath)
        {
            using (StreamWriter file = new StreamWriter(filePath))
            {
                for (int i = 0; i < dim1; i++)
                {
                    for (int j = 0; j < dim2; j++)
                        file.Write((all_neurons[i * dim2 + j].V).ToString(CultureInfo.InvariantCulture) + " ");
                    // Write a newline character after each row except the last one
                    if (i < dim1 - 1)
                        file.WriteLine();
                }
            }
        }

    }
}
