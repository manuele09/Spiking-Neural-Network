using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using CSML;
//using Matrix;
using Accord.Math;
using System.Threading.Tasks;


//revert commit

namespace SLN
{
    /// <summary>
    /// The network itself
    /// </summary>
    [Serializable]
    public class Network
    {
        [Serializable]
        public struct Connection
        {
            public int i;  // Neuron in the input layer
            public int j;  // Neuron in the liquid layer
            public double w;  // Weight of the connection

            public Connection(int i, int j, double w)
            {
                this.i = i;
                this.j = j;
                this.w = w;
            }
        }


        private LinkedList<Synapse> _liquidToLiquid;
        private LinkedList<Connection>[,] connectivityMatrix;
        private LiquidState _liquid;


        public double[,] W = new double[Constants.LIQUID_DIMENSION_I * Constants.LIQUID_DIMENSION_J, Constants.FIRST_LAYER_DIMENSION_I * Constants.FIRST_LAYER_DIMENSION_J];
        private Random rand;

        /// <summary>
        /// Constructor (with configuration saved to file)
        /// </summary>
        /// <param name="synSav">The synapse configuration saver object</param>
        private Network()
        {
            _liquid = new LiquidState();
            _liquidToLiquid = new LinkedList<Synapse>();
            rand = new Random();
            ConnectInputToLiquid();
            initLiquidToLiquid();
        }
        public void resetLiquid()
        {
            _liquid.resetState();
        }
        internal void saveWeights(double[,] w)
        {
            W = (double[,])w.Clone();
        }
        internal Neuron getLiquidLayerNeuron(int i, int j)
        {
            return _liquid.getLiquidLayerNeuron(i, j);
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

        /// <summary>
        /// Initializes the Liquid-To-Liquid-layer synapses
        /// </summary>
        private void initLiquidToLiquid()
        {
            double w = 0;
            Synapse syn;
            double tau = 0;
            double delay = 0;

            for (int i1 = 0; i1 < Constants.LIQUID_DIMENSION_I; i1++)
                for (int j1 = 0; j1 < Constants.LIQUID_DIMENSION_J; j1++)
                {
                    Neuron start = _liquid.getLiquidLayerNeuron(i1, j1);

                    for (int i2 = 0; i2 < Constants.LIQUID_DIMENSION_I; i2++)
                        for (int j2 = 0; j2 < Constants.LIQUID_DIMENSION_J; j2++)
                        {
                            bool first_exc = _liquid.getLiquidLayerNeuron(i1, j1).is_exec;
                            bool second_exc = _liquid.getLiquidLayerNeuron(i2, j2).is_exec;

                            Neuron dest = _liquid.getLiquidLayerNeuron(i2, j2);

                            if (rand.NextDouble() < Constants.LIQUID_CONNETTIVITY)
                            {
                                if (first_exc && second_exc)
                                    w = NextGaussian(5, 0.7 * 5);
                                if (first_exc && !second_exc)
                                    w = NextGaussian(25, 0.7 * 25);
                                if (!first_exc)
                                    w = NextGaussian(-20, 0.7 * (-20));

                                if (first_exc)
                                    tau = 3; //ms
                                else
                                    tau = 2; //ms
                                delay = NextGaussian(10, 20, 3, 200); //ms da trasformare in steps
                                syn = new Synapse(start, dest, w, 0, tau, (int)(delay / Constants.INTEGRATION_STEP), 1);
                                _liquidToLiquid.AddLast(syn);
                            }
                        }
                }
        }


        public void ConnectInputToLiquid()
        {
            Random rnd = new Random();
            connectivityMatrix = new LinkedList<Connection>[Constants.FIRST_LAYER_DIMENSION_I, Constants.FIRST_LAYER_DIMENSION_J];
            for (int i = 0; i < Constants.FIRST_LAYER_DIMENSION_I; i++)
                for (int j = 0; j < Constants.FIRST_LAYER_DIMENSION_J; j++)
                {
                    connectivityMatrix[i, j] = new LinkedList<Connection>();

                    for (int l1 = 0; l1 < Constants.LIQUID_DIMENSION_I; l1++)
                        for (int l2 = 0; l2 < Constants.LIQUID_DIMENSION_J; l2++)
                            //if (rnd.NextDouble() < ((double)Constants.Single_INPUT_TO_LIQUID_CONNECTIONS / (Constants.LIQUID_DIMENSION_I * Constants.LIQUID_DIMENSION_J)))
                            if (rnd.NextDouble() < 0.5)
                            {
                                double weight = NextGaussian(2.65, 0.025, 0, 14.9);
                                connectivityMatrix[i, j].AddLast(new Connection(l1, l2, weight));
                            }
                }
        }

        public void SetLiquidCurrent(double[,] input_current, double gain)
        {
            this.ResetLiquidBiasCurrent();
            for (int i = 0; i < Constants.FIRST_LAYER_DIMENSION_I; i++)
                for (int j = 0; j < Constants.FIRST_LAYER_DIMENSION_J; j++)
                    foreach (Connection conn in connectivityMatrix[i, j])
                        getLiquidLayerNeuron(conn.i, conn.j).IBias += input_current[i, j] * conn.w * gain;
            return;
        }
        public void ResetLiquidBiasCurrent()
        {
            for (int i = 0; i < Constants.LIQUID_DIMENSION_I; i++)
                for (int j = 0; j < Constants.LIQUID_DIMENSION_J; j++)
                    getLiquidLayerNeuron(i, j).IBias = 0;
            return;
        }

        public double[,] GetLiquidStates(int step, double tau)
        {
            double[,] states = new double[Constants.LIQUID_DIMENSION_I, Constants.LIQUID_DIMENSION_J];
            for (int i = 0; i < Constants.LIQUID_DIMENSION_I; i++)
                for (int j = 0; j < Constants.LIQUID_DIMENSION_J; j++)
                {
                    states[i, j] = getLiquidLayerNeuron(i, j).getState(step, tau);
                }
            return states;
        }

        public void AddLiquidStates(double[,] states, int row, int step, double tau)
        {
            for (int i = 0; i < Constants.LIQUID_DIMENSION_I; i++)
                for (int j = 0; j < Constants.LIQUID_DIMENSION_J; j++)
                {
                    states[row, i * Constants.LIQUID_DIMENSION_J + j] = getLiquidLayerNeuron(i, j).getState(step, tau);
                }

        }




        /// <summary>
        /// Simulates the entire network with the Liquid State
        /// </summary>
        /// <param name="simNumber">Number of the simulation</param>
        /// <param name="learning">If <i>true</i> sets the learning on</param>
        //ritorna un intero, non un double
        public double simulateLiquid(int start_step, int nsteps)
        {
            for (int step = 0; step < nsteps; step++)
            {
                _liquid.simulate(step + start_step, null);

                Parallel.ForEach(_liquidToLiquid, syn =>
                {
                    syn.simulate(step);
                });

                /*foreach (Synapse syn in _liquidToLiquid)
                {
                    syn.simulate(step);
                }*/
            }

            return 1;
        }








        /// <summary>
        /// Creates a new network without information
        /// </summary>
        /// <returns>A reference to the new <code>Network</code> object</returns>
        public static Network generateNetwork()
        {
            return new Network();
        }


    }
}
