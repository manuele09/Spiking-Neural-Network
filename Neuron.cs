using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SLN
{
    /// <summary>
    /// An Izhikevich's generic neuron
    /// </summary>
    [Serializable]
    public class Neuron
    {
        #region Neuron variables

        public bool is_winner_old;
        public bool is_winner_now;
        public bool is_exec;
        /// <summary>
        /// The <b>A</b> parameter in Izhikevich's model
        /// </summary>
        protected double A;

        /// <summary>
        /// The <b>B</b> parameter in Izhikevich's model
        /// </summary>
        protected double B;

        /// <summary>
        /// The <b>C</b> parameter in Izhikevich's model
        /// </summary>
        protected double C;

        /// <summary>
        /// The <b>D</b> parameter in Izhikevich's model
        /// </summary>
        protected double D;

        private double _v;

        /// <summary>
        /// Membrane potential
        /// </summary>
        internal double V
        {
            get { return _v; }
            set { _v = value; }
        }

        private double _u;

        /// <summary>
        /// Membrane recovery variable
        /// </summary>
        internal double U
        {
            get { return _u; }
            set { _u = value; }
        }

        private double _i;

        /// <summary>
        /// Neuron input current
        /// </summary>
        internal double I
        {
            get { return _i; }
            set { _i = value; }
        }

        private double _iPrev;

        /// <summary>
        /// Neuron total input current before resetting
        /// (for logging purposes only)
        /// </summary>
        internal double IPrev
        {
            get { return _iPrev; }
            set { _iPrev = value; }
        }

        private double _iBias;

        /// <summary>
        /// Neuron Bias current
        /// </summary>
        internal double IBias
        {
            get { return _iBias; }
            set { _iBias = value; }
        }

        /// <summary>
        /// List with the timestemp (i.e. simulation step) at which
        /// each spike occurred
        /// </summary>
        public SpikeList _spikeList;

        /// <summary>
        /// Number of the spikes the neuron fired
        /// </summary>
        internal int NSpikes
        {
            get { return _spikeList.NSpikes; }
        }


        public double _idetour;
        /// <summary>
        /// Neuron input current used for developing the "Detour paradigm" 
        /// </summary>
        internal double Idetour
        {
            get { return _idetour; }
            set { _idetour = value; }
        }

        public int _countdetour;
        /// <summary>
        /// Neuron counter used for developing the "Detour paradigm" 
        /// </summary>
        internal int Countdetour
        {
            get { return _countdetour; }
            set { _countdetour = value; }
        }

        public double _iOut;
        /// <summary>
        /// Neuron output current 
        /// </summary>
        internal double IOut
        {
            get { return _iOut; }
            set { _iOut = value; }
        }

        #endregion

        #region Position variables
        protected int _row;

        /// <summary>
        /// The zero-based row coordinate of the neuron
        /// </summary>
        internal int ROW
        {
            get { return _row; }
        }

        protected int _col;

        /// <summary>
        /// The zero-based column coordinate of the neuron
        /// </summary>
        internal int COLUMN
        {
            get { return _col; }
        }

        protected LayerNumbers _layer;

        /// <summary>
        /// The layer the neuron belongs to
        /// </summary>
        internal LayerNumbers LAYER
        {
            get { return _layer; }
        }
        #endregion

        public StreamWriter file;

        /// <summary>
        /// Sets the position of the neuron in the network
        /// </summary>
        /// <param name="i">The zero-based row coordinate</param>
        /// <param name="j">The zero-based column coordinate</param>
        /// <param name="layer">The layer the neuron belongs to</param>
        internal void setCoord(int i, int j, LayerNumbers layer)
        {
            _row = i;
            _col = j;
            _layer = layer;
            //if (COLUMN == 2 && ROW == 3 && LAYER == LayerNumbers.SOSL_1)
            //{
            //    //file = new StreamWriter("Corrente di B.txt");
            //    //Console.WriteLine("\n\n creazione file ok \n\n");
            //}
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public Neuron()
        {

            V = Constants.INITIAL_STATE_V;
            U = Constants.INITIAL_STATE_U;

            _spikeList = new SpikeList();
            Countdetour = -1;
            _iBias = 0;

        }


        /// <summary>
        /// Simulates the neuron behavior
        /// </summary>
        /// <param name="step">The current simulation step</param>
        /// <returns><i>true</i> if the neuron fired a spike, <i>false</i> otherwise</returns>
        internal virtual bool simulate(int step)
        {
            //if (COLUMN == 1 && ROW == 0 && step < 10)
            //    Console.WriteLine("\n\n Iinit= " + I + "\tU="+ U + "\tV=" + V + "\n\n");


            Random rnd = new Random();
            double v;
            double u;

            if (this.LAYER == LayerNumbers.LIQUID_STATE)
            {
                //V and U are the values at the previous simulation step, 
                //v and u are the values at the current simulation step
                v = V + Constants.INTEGRATION_STEP_LIQUID * ((0.04 * V * V) + (5.0 * V) + 140.0 - U + I + IBias);
                u = U + Constants.INTEGRATION_STEP_LIQUID * A * ((B * V) - U);
            }
            else
            {
                //V and U are the values at the previous simulation step, 
                //v and u are the values at the current simulation step
                v = V + Constants.INTEGRATION_STEP * ((0.04 * V * V) + (5.0 * V) + 140.0 - U + I + IBias);
                u = U + Constants.INTEGRATION_STEP * A * ((B * V) - U);
            }


            //if (COLUMN == 2 && ROW == 3 && LAYER==LayerNumbers.SOSL_1)
            //{
            //    //file.WriteLine(step + "\t" + I /*+   "\t" + V + "\n"*/);
            //    //file.Flush();
            //}
            //    Console.WriteLine("\n\n V= " + V + "  U= " + U + "    I=" + I + "   step=" + step + "\n\n");

            double vPrev = V;
            bool spike = false;

            //Updating to the new values of V and U...
            //if (COLUMN == 1 && ROW == 0 && step<500)
            //    Console.WriteLine("\n\n tensione: " + v + "\n\n");

            //(v>30)
            if (v > 30)
            {
                V = C;
                U += D;
                _spikeList.addSpike(step);
                spike = true;
            }
            else
            {
                V = v;
                U = u;
            }

            //Now we have "used up" the input current, so we set it to zero after saving
            //its value for logging purposes.
            _iPrev = I;
            resetI();

            //if (COLUMN == 1 && ROW == 0 && step < 50)
            //    Console.WriteLine("\n\n I= " + I + "\tU=" + U + "\tV=" + V + "\n\n");

            return spike;
        }

        /// <summary>
        /// Returns the timestamp of the <i>n</i>-th spike of the neuron
        /// </summary>
        /// <param name="n">The index of the spike (beginning from 0)</param>
        /// <returns>The timestamp of the spike (i.e. the simulation step when it occurred)</returns>
        internal int getSpikeAt(int n)
        {
            return _spikeList.getSpikeAt(n);
        }

        /// <summary>
        /// Sets the neuron's input current to 0
        /// </summary>
        protected void resetI()
        {
            _i = 0;
        }

        /// <summary>
        /// Adds the specified current value to neuron's input current
        /// </summary>
        /// <param name="i"></param>
        internal void updateI(double i)
        {
            _i += i;
        }


        /// <summary>
        /// Returns the spiking frequency of the neuron
        /// </summary>
        /// <param name="lastTimestamp">Last timestamp (i.e. integration step) on which
        /// calculate the frequency</param>
        /// <returns>The spiking frequency of the neuron</returns>
        internal double getFrequencyWindow(int window)
        {
            /*int lastTimestamp = 0;

            if (_spikeList.Count == 0)
				lastTimestamp = _spikeList.Last();
            int firstTimestamp = lastTimestamp - window + 1;
            int firstIndex = _spikeList.FindIndex(
                delegate (int spk)
                {
                    return spk >= firstTimestamp;
                }
            );

            //if (COLUMN == 1 && ROW == 0 && lastTimestamp > 998)
            //    Console.WriteLine("\n\n SpikeList: " + _spikeList + "\n\n");


            if (firstIndex > -1)
            {
                int nspk = NSpikes - firstIndex + 1;

                double freq = nspk / (Constants.INTEGRATION_STEP * window); // in kHz
                return freq * 1000; // in Hz
            }
            else
                return 0;*/
            return _spikeList.Count;
        }


        /// <summary>
        /// Returns the spiking frequency of the neuron
        /// </summary>
        /// <param name="lastTimestamp">Last timestamp (i.e. integration step) on which
        /// calculate the frequency</param>
        /// <returns>The spiking frequency of the neuron</returns>
        internal double getFrequency(int lastTimestamp)
        {
            int firstTimestamp = lastTimestamp - Constants.FREQUENCY_WINDOW_SIZE + 1;
            int firstIndex = _spikeList.FindIndex(
                delegate (int spk)
                {
                    return spk >= firstTimestamp;
                }
            );

            //if (COLUMN == 1 && ROW == 0 && lastTimestamp > 998)
            //    Console.WriteLine("\n\n SpikeList: " + _spikeList + "\n\n");


            if (firstIndex > -1)
            {
                int nspk = NSpikes - firstIndex + 1;

                double freq = nspk / Constants.FREQUENCY_WINDOW_TIME; // in kHz
                return freq * 1000; // in Hz
            }
            else
                return 0;
        }



        /// <summary>
        /// Returns the spiking frequency of the neuron
        /// </summary>
        /// <param name="lastTimestamp">Last timestamp (i.e. integration step) on which
        /// calculate the frequency</param>
        /// <returns>The spiking frequency of the neuron</returns>
        internal double getFrequencyEnd(int lastTimestamp)
        {
            //int firstTimestamp = lastTimestamp - Constants.FREQUENCY_WINDOW_SIZE + 1;
            //int firstIndex = _spikeList.FindIndex(
            //    delegate(int spk)
            //    {
            //        return spk >= firstTimestamp;
            //    }
            //);

            ////if (COLUMN == 1 && ROW == 0 && lastTimestamp > 998)
            ////    Console.WriteLine("\n\n SpikeList: " + _spikeList + "\n\n");
            int window;
            if (lastTimestamp < Constants.SIMULATION_STEPS_FEEDFORWARD)
                window = lastTimestamp;
            else
                window = lastTimestamp - Constants.SIMULATION_STEPS_FEEDFORWARD;

            if (window == 0)           // questa condizione serve solo se si vorrebbe chiamare questa funzione al primo step della fase Liquid (il che sarebbe stupido ma non si sa mai xD) 
                window = 1;           //se la si chiama invece in uno step della fase feedforward si avrebbe che la frequenza finale è zero perchè Nspike è zero in quanto non vengono simulati i neuroni di end


            //if (firstIndex > -1)
            //{
            //int nspk = ;

            double freq = NSpikes / (window * Constants.INTEGRATION_STEP); // in kHz
            return freq * 1000; // in Hz
            //}
            //else
            //    return 0;
        }

        /// <summary>
        /// Neset the neuron's state
        /// </summary>
        internal void resetState()
        {
            if (this.LAYER == LayerNumbers.LIQUID_STATE)
            {
                _v = Constants.INITIAL_STATE_V_LIQUID;
                _u = Constants.INITIAL_STATE_U_LIQUID;
            }
            else
            {
                _v = Constants.INITIAL_STATE_V;
                _u = Constants.INITIAL_STATE_U;
                //_iPrev = 0;
            }

            resetI();
            //_iOut = 0;
            //_iPrev = 0;

            _spikeList = new SpikeList();


        }

        internal void resetSpikeList()
        {
            _spikeList = new SpikeList();
        }
        /// <summary>
        /// Returns the list of the spikes
        /// </summary>
        /// <returns>The list of the spikes the neuron fired</returns>
        internal SpikeList getSpikeList()
        {
            return _spikeList;
        }

        internal double getState(int step, double tau)
        {
            return Math.Exp(-(step - _spikeList.Last()) / tau);
        }

    }
}
