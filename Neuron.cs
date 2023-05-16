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


        #endregion

        public StreamWriter file;



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
            double v;
            double u;


            //V and U are the values at the previous simulation step, 
            //v and u are the values at the current simulation step
            v = V + Constants.INTEGRATION_STEP * ((0.04 * V * V) + (5.0 * V) + 140.0 - U + I + IBias);
            u = U + Constants.INTEGRATION_STEP * A * ((B * V) - U);


            double vPrev = V;
            bool spike = false;
            if (v > 30)
            {
                V = C;
                U += D;
                _spikeList.addSpike(step, true);
                spike = true;
            }
            else
            {
                _spikeList.addSpike(step, false);
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
        /// Neset the neuron's state
        /// </summary>
        internal void resetState()
        {

            _v = Constants.INITIAL_STATE_V;
            _u = Constants.INITIAL_STATE_U;
            IBias = 0;
            //_iPrev = 0;


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
           // Console.WriteLine(_spikeList.getLastSpike(step));
           // Console.WriteLine(step);
            return Math.Exp(-(step - _spikeList.getLastSpike(step)) / tau);
        }

    }
}
