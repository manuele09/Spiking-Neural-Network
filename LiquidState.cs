using System;
using System.Collections.Generic;
using System.Text;

namespace SLN
{
    [Serializable]
    internal class LiquidState
    {

        private Neuron[,] _liquidState;

        /// <summary>
        /// Constructor
        /// </summary>
        internal LiquidState()
        {
            _liquidState = new Neuron[
                Constants.LIQUID_DIMENSION_I,
                Constants.LIQUID_DIMENSION_J];

            //Populating liquid layer
            for (int i = 0; i < Constants.LIQUID_DIMENSION_I; i++)
                for (int j = 0; j < Constants.LIQUID_DIMENSION_J; j++)
                {
                    _liquidState[i, j] = new Class1Neuron();
                    _liquidState[i, j].IBias = 0; //32
                    _liquidState[i, j].setCoord(i, j, LayerNumbers.LIQUID_STATE);
                    _liquidState[i, j].V = Constants.INITIAL_STATE_V_LIQUID;
                    _liquidState[i, j].U = Constants.INITIAL_STATE_U_LIQUID;

                    if ((new Random(1234)).NextDouble() < Constants.EX_PROB)
                        _liquidState[i, j].is_exec = true;
                    else
                        _liquidState[i, j].is_exec = false;
                }
        }


        /// <summary>
        /// Finds the neuron at the specific coordinates in the Liquid State
        /// </summary>
        /// <param name="i">The row coordinate</param>
        /// <param name="j">The column coordinate</param>
        /// <returns>The neuron at the specified coordinates</returns>
        internal Neuron getLiquidLayerNeuron(int i, int j)
        {
            return _liquidState[i, j];
        }



        //perché chiamare simulateLiquid con simulate???
        //tra l'altro si perde un argomento.
        /// <summary>
        /// Simulates the Network connected to the Liquid State
        /// </summary>
        /// <param name="step">The current step of simulation</param>
        /// <param name="log">The logger object</param>
        /// <param name="sim_number">The number of the simulation</param>
        internal void simulate(int step, StateLogger log)
        {
            simulateLiquid(step, log);

        }



        /// <summary>
        /// Simulates the Liquid State
        /// </summary>
        /// <param name="step">The current step of simulation</param>
        /// <param name="log">The logger object</param>
        /// <param name="sim_number">The number of the simulation</param>
        internal void simulateLiquid(int step, StateLogger log)
        {
            for (int i = 0; i < Constants.LIQUID_DIMENSION_I; i++)
                for (int j = 0; j < Constants.LIQUID_DIMENSION_J; j++)
                {
                    Class1Neuron n = (Class1Neuron)_liquidState[i, j];
                    n.simulate(step);
                    if (log != null)
                        log.logNeuron(step, n);
                }
        }

        internal void resetState()
        {
            foreach (Class1Neuron n in _liquidState)
                n.resetState();
        }

    }
}
