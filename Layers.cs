using Accord.Math.Decompositions;
using System;
using System.Collections.Generic;

namespace SLN
{
    /// <summary>
    /// The input and SOSLs layers of the network:
    /// <list type="table">
    /// <listheader>
    /// <term>Type</term>
    /// <description>Description</description>
    /// </listheader>
    /// <item>
    /// <term>first</term>
    /// <description>the input layer;</description>
    /// </item>
    /// <item>
    /// <term>second_1</term>
    /// <description>the clustering layer at t;</description>
    /// </item>
    /// <item>
    /// <term>second_2</term>
    /// <description>the clustering layer at t-1.</description>
    /// </item>
    /// </list>
    /// </summary>
    [Serializable]
    internal class Layers
    {
        private Neuron[,] _firstLayer1;
        protected bool[,] _inputs1;

        /// <summary>
        /// Constructor
        /// </summary>
        internal Layers()
        {
            _firstLayer1 = new Neuron[
                Constants.FIRST_LAYER_DIMENSION_I,
                Constants.FIRST_LAYER_DIMENSION_J];

            //All values are initialized to false by default
            _inputs1 = new bool[
                Constants.FIRST_LAYER_DIMENSION_I,
                Constants.FIRST_LAYER_DIMENSION_J];

            //Populating first layer
            for (int i = 0; i < Constants.FIRST_LAYER_DIMENSION_I; i++)
                for (int j = 0; j < Constants.FIRST_LAYER_DIMENSION_J; j++)
                {
                    _firstLayer1[i, j] = new SpikingNeuron();
                    _firstLayer1[i, j].setCoord(i, j, LayerNumbers.FirstLayer_1);
                }


        }

        /// <summary>
        /// Sets the inputs of the network
        /// </summary>
        /// <param name="input">The input of the network</param>
        internal void setInput(NetworkInput input)
        {
            //_inputCur = input;
            if (input.COLOR >= 0)
                _inputs1[0, input.COLOR] = true;

            if (input.SIZE >= 0)
                _inputs1[1, input.SIZE] = true;

            if (input.HDISTR >= 0)
                _inputs1[2, input.HDISTR] = true;

            if (input.VDISTR >= 0)
                _inputs1[3, input.VDISTR] = true;

        }

        /// <summary>
        /// Resets the inputs of the network
        /// </summary>
        internal void resetInputs()
        {
            for (int i = 0; i < _inputs1.GetLength(0); i++)
                for (int j = 0; j < _inputs1.GetLength(1); j++)
                    _inputs1[i, j] = false;

        }





        /// <summary>
        /// Simulates the first layer (to SOSL #1)
        /// </summary>
        /// <param name="step">The current step of simulation</param>
        /// <param name="log">The logger object</param>
        public void simulateFirst1(int step, StateLogger log)
        {
            Random rnd = new Random();

            //If the neuron in the first layer is triggered, its input 
            //current is a continuous external current, so it has 
            //to be set to its value before each step of simulation, 
            //because it was set to 0 in the previous step.
            double iIn1 = Constants.INPUT_CURRENT;
            double iFb1 = 0 * Constants.FEEDBACK_CURRENT * (1 + 0.2 * rnd.NextDouble());

            for (int i = 0; i < Constants.FIRST_LAYER_DIMENSION_I; i++)
                for (int j = 0; j < Constants.FIRST_LAYER_DIMENSION_J; j++)
                {
                    if (_inputs1[i, j])
                    {
                        _firstLayer1[i, j].I += iIn1;
                        if (_firstLayer1[i, j].I > 80)
                            _firstLayer1[i, j].I = 80;

                    }
                    Neuron n = _firstLayer1[i, j];
                    n.simulate(step);
                    if (log != null)
                        log.logNeuron(step, n);
                }
        }


        /// <summary>
        /// Finds the neuron at the specific coordinates in the first layer (to SOSL #1)
        /// </summary>
        /// <param name="i">The row coordinate</param>
        /// <param name="j">The column coordinate</param>
        /// <returns>The neuron at the specified coordinates</returns>
        internal Neuron getFirstLayerNeuron1(int i, int j)
        {
            return _firstLayer1[i, j];
        }



        /// Resets the neurons' state in the network
        /// </summary>
        internal void resetNeuronsState()
        {
            foreach (Neuron n in _firstLayer1)
                n.resetState();
        }

        /// <summary>
        /// 		/// Ritorna una lista di 4 neuroni vincitori, uno per ogni Feature.
        /// Non è detto che ogni feature abbia un vincitore, quindi alcuni campi 
        /// della lista possono essere non inizializzati. Nel caso in cui non c'è
        /// nemmeno un neuorone vincitore (ossia con una frequenza di spiking maggiore
        /// di una certa soglia) viene ritornato Null.
        /// </summary>
        /// <param name="lastTimestamp">Last timestamp (i.e. integration step) on which
        /// calculate the frequency</param>
        /// <returns>The winner neuron of said layer</returns>
        internal Neuron[] getWinnerFirstActive(int lastTimestamp)
        {
            double winnerFreq = 0;
            Neuron[] nWin = new Neuron[4];
            bool input = false;

            for (int i = 0; i < _firstLayer1.GetLength(0); i++)
            {
                winnerFreq = 160; //questa soglia era 100, l'ho aumentata perchè alcuni neuroni
                //dell'input si triggeravano anche se senza input (a 150)
                for (int j = 0; j < _firstLayer1.GetLength(1); j++)
                {
                    Neuron n = _firstLayer1[i, j];
                    double freq = n.getFrequency(lastTimestamp);
                    if (freq > winnerFreq)
                    {
                        nWin[i] = n;
                        winnerFreq = freq;
                        input = true;
                    }
                }
                //Console.WriteLine("Frequenza ("+i+"): " + winnerFreq + input);
            }

            if (!input)
                return null;
            else
                return nWin;
        }

    }
}
