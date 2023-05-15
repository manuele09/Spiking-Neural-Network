using System;
using System.Collections.Generic;
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
            public int i;  // Neuron in the input layer
            public double w;  // Weight of the connection

            public Connection(int i, double w)
            {
                this.i = i;
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

        private Random rand = new Random();

        public LinkedList<Synapse> all_synapses;
        public LinkedList<Connection>[,] connectivityMatrix;
        public double[,] W = new double[Constants.LIQUID_DIMENSION_I * Constants.LIQUID_DIMENSION_J, Constants.FIRST_LAYER_DIMENSION_I * Constants.FIRST_LAYER_DIMENSION_J];


        /// <summary>
        /// Constructor
        /// </summary>

        internal LSM(int n_exc, int n_inh, int n_rec)
        {
            this.n_exc = n_exc;
            this.n_inh = n_inh;
            this.n_rec = n_rec;

            all_neurons = new Neuron[n_exc + n_inh + n_rec];
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
        }

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

            foreach (Neuron post_neuron in post)
            {
                for (int j = 0; j < indegree; j++)
                {
                    Synapse syn = new Synapse(pre[rand.Next(pre.Length)], post_neuron, w, 0, tau, (int)(delay / Constants.INTEGRATION_STEP), 1);
                    all_synapses.AddLast(syn);
                }
            }
        }

        internal void simulateLiquid(int step, bool parallel)
        {
            if (!parallel)
            {
                foreach (Class1Neuron n in all_neurons)
                {
                    n.simulate(step);
                }

                foreach (Synapse syn in all_synapses)
                {
                    syn.simulate(step);
                }
            }
            else
            {
                Parallel.ForEach(all_neurons, n =>
                {
                    n.simulate(step);
                });

                Parallel.ForEach(all_synapses, syn =>
                {
                    syn.simulate(step);
                });
            }


        }

        public void simulateLiquid(int start_step, int nsteps, bool parallel)
        {
            for (int step = 0; step < nsteps; step++)
                simulateLiquid(step + start_step, parallel);
        }


        public void ConnectInputToLiquid()
        {
            connectivityMatrix = new LinkedList<Connection>[Constants.FIRST_LAYER_DIMENSION_I, Constants.FIRST_LAYER_DIMENSION_J];
            double weight;
            for (int i = 0; i < Constants.FIRST_LAYER_DIMENSION_I; i++)
                for (int j = 0; j < Constants.FIRST_LAYER_DIMENSION_J; j++)
                {
                    connectivityMatrix[i, j] = new LinkedList<Connection>();

                    for (int e = 0; e < n_exc; e++)
                    {
                        weight = NextGaussian(2.65, 0.025, 0, 14.9);
                        connectivityMatrix[i, j].AddLast(new Connection(e, weight));
                    }

                }
        }

        public void SetLiquidCurrent(double[,] input_current, double gain)
        {
            this.ResetLiquidBiasCurrent();
            for (int i = 0; i < Constants.FIRST_LAYER_DIMENSION_I; i++)
                for (int j = 0; j < Constants.FIRST_LAYER_DIMENSION_J; j++)
                    foreach (Connection conn in connectivityMatrix[i, j])
                        all_neurons[conn.i].IBias += input_current[i, j] * conn.w * gain;
            return;
        }
        public void ResetLiquidBiasCurrent()
        {
            for (int i = 0; i < n_exc; i++)
                    all_neurons[i].IBias = 0;
            return;
        }
        internal void resetState()
        {
            foreach (Class1Neuron n in all_neurons)
                n.resetState();
        }
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

    }
}
