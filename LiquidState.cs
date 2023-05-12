using System;
using System.Collections.Generic;
using System.Text;

namespace SLN
{
    [Serializable]
    internal class LiquidState
    {
      
        private Neuron[,] _liquidState;
        //private Neuron[,] _firstLayer1;
        //protected bool[,] _inputs1;

        private Neuron output1;

        private int option;
        /// <summary>
        /// Neuron input current
        /// </summary>
        internal int Option
        {
            get { return option; }
            set { option = value; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        internal LiquidState() {

            _liquidState = new Neuron[
                Constants.LIQUID_DIMENSION_I,
                Constants.LIQUID_DIMENSION_J];

            //_firstLayer1 = new Neuron[
            //    Constants.FIRST_LAYER_DIMENSION_I,
            //    Constants.FIRST_LAYER_DIMENSION_J];

            ////All values are initialized to false by default
            //_inputs1 = new bool[
            //    Constants.FIRST_LAYER_DIMENSION_I,
            //    Constants.FIRST_LAYER_DIMENSION_J];

            //Populating first layer
            //for (int i = 0; i < Constants.FIRST_LAYER_DIMENSION_I; i++)
            //    for (int j = 0; j < Constants.FIRST_LAYER_DIMENSION_J; j++)
            //    {
            //        _firstLayer1[i, j] = new SpikingNeuron();
            //        _firstLayer1[i, j].setCoord(i, j, LayerNumbers.FirstLayer_1);

            //    }

            //Populating liquid layer
            for (int i = 0; i < Constants.LIQUID_DIMENSION_I; i++)
                for (int j = 0; j < Constants.LIQUID_DIMENSION_J; j++)
                {
                    _liquidState[i, j] = new Class1Neuron();
                    _liquidState[i, j].IBias = 22+10;   //22 è il valore di base, il 10 è stato inserito per migliorare la sensibilità
                    _liquidState[i, j].setCoord(i, j, LayerNumbers.LIQUID_STATE);
                    _liquidState[i, j].V = Constants.INITIAL_STATE_V_LIQUID;
                    _liquidState[i, j].U = Constants.INITIAL_STATE_U_LIQUID;

                }

            output1 = new SumNeuron();
            output1.setCoord(100, 100, LayerNumbers.LIQUID_STATE);
            output1.V = Constants.INITIAL_STATE_V_LIQUID;
            output1.U = Constants.INITIAL_STATE_U;

            option = Constants.LIQUID_TO_OUT_OPTION;


        }

        ///// <summary>
        ///// Sets the inputs of the network
        ///// </summary>
        ///// <param name="input">The input of the network</param>
        //internal void setInput(NetworkInput input)
        //{
        //    //_inputCur = input;
        //    if (input.COLOR >= 0)
        //        _inputs1[0, input.COLOR] = true;

        //    if (input.SIZE >= 0)
        //        _inputs1[1, input.SIZE] = true;

        //    if (input.HDISTR >= 0)
        //        _inputs1[2, input.HDISTR] = true;

        //    if (input.VDISTR >= 0)
        //        _inputs1[3, input.VDISTR] = true;

        //}

        ///// <summary>
        ///// Resets the inputs of the network
        ///// </summary>
        //internal void resetInputs()
        //{
        //    for (int i = 0; i < _inputs1.GetLength(0); i++)
        //        for (int j = 0; j < _inputs1.GetLength(1); j++)
        //            _inputs1[i, j] = false;

        //}

        ///// <summary>
        ///// Finds the neuron at the specific coordinates in the first layer 
        ///// </summary>
        ///// <param name="i">The row coordinate</param>
        ///// <param name="j">The column coordinate</param>
        ///// <returns>The neuron at the specified coordinates</returns>
        //internal Neuron getFirstLayerNeuron1(int i, int j)
        //{
        //    return _firstLayer1[i, j];
        //}

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

        /// <summary>
        /// Finds the neuron at the specific coordinates in the output of the Liquid
        /// </summary>
        /// <returns>The neuron at the specified coordinates</returns>
        internal Neuron getOutNeuron()
        {
            return output1;
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
        /// Reset the active variable in the Liquid State
        internal void resetActive()
        {
            foreach (Class1Neuron n in _liquidState)
                n.Active=false;

        }

        /// <summary>
        /// Simulates the Liquid State
        /// </summary>
        /// <param name="step">The current step of simulation</param>
        /// <param name="log">The logger object</param>
        /// <param name="sim_number">The number of the simulation</param>
        internal void simulateLiquid(int step, StateLogger log)
        {
            //double V = 0; 


            for (int i = 0; i < Constants.LIQUID_DIMENSION_I; i++)
            {
                for (int j = 0; j < Constants.LIQUID_DIMENSION_J; j++)
                {
                    
                    Class1Neuron n = (Class1Neuron)_liquidState[i, j]; 
                    //if (input[i * Constants.LIQUID_DIMENSION_I + j] > 0)
                    //    n.I += 5;        //corrente di input

                    //normalizzazione dell'ingresso secondo la proporzione n.Inputcurrent:140=x:35 --> x=n.Inputcurrent*35/140 o altre prove
                    if (step == 100)
                        n.INPUTCURRENT = n.INPUTCURRENT*35/140;

                    if (n.Active)
                        n.I += n.INPUTCURRENT;
                    //n.I += 10;  //BIAS in corrente per tutti i neuroni della liquid--> è stato inserito nel costruttore del Class1
                    n.simulate(step);

                    //if (n.I > 0) {
                    //    n.I = n.I;
                    //}

                    if (log != null)
                        log.logNeuron(step, n);
                    //V += n.V;
                    
                }
            }


            //simulazione del neurone di output
            //Neuron n_o = output1;
            //n_o.simulate(step);
            //if (log != null)
            //    log.logNeuron(step, n_o);


        }

        internal void resetState() {
            foreach (Class1Neuron n in _liquidState)
            {
                n.resetState();
                n.INPUTCURRENT = 0;
            }
           
            output1.resetState();
        }

       


    }
}
