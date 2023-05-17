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


        public double A;
        public double B;
        public double C;
        public double D;

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





        #endregion


        /// <summary>
        /// Constructor
        /// </summary>
        public Neuron()
        {

            V = Constants.INITIAL_STATE_V_LIQUID;
            U = Constants.INITIAL_STATE_U_LIQUID;

            _spikeList = new SpikeList();
            _iBias = 0;
            _i = 0;

        }


        /// <summary>
        /// Simulates the neuron behavior
        /// </summary>
        /// <param name="step">The current simulation step</param>
        /// <returns><i>true</i> if the neuron fired a spike, <i>false</i> otherwise</returns>
        internal virtual bool simulate(int step)
        {
            double v;
            double u;


            //V and U are the values at the previous simulation step, 
            //v and u are the values at the current simulation step
            v = V + Constants.INTEGRATION_STEP * ((0.04 * V * V) + (5.0 * V) + 140.0 - U + I + IBias);
            u = U + Constants.INTEGRATION_STEP * A * ((B * V) - U);
            bool spike = false;
            if (v > 30)
            {
                V = C;
                U += D;
                _spikeList.addSpike(step, true);
                //_spikeList.addSpike(step);
                spike = true;
            }
            else
            {
                _spikeList.addSpike(step, false);
                V = v;
                U = u;
            }


            resetI();

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
            V = Constants.INITIAL_STATE_V_LIQUID;
            U = Constants.INITIAL_STATE_U_LIQUID;

            _spikeList = new SpikeList();
            _iBias = 0;
            _i = 0;




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
            
            //Console.WriteLine(step + ") Steps difference: " + (step - _spikeList.getLastSpike(step)) + "; Value: " + Math.Exp(-(step - _spikeList.getLastSpike(step)) / tau));
            return Math.Exp(-(step - _spikeList.getLastSpike(step)) / tau);
            //Console.WriteLine(step + ") Steps difference: " + (step - _spikeList.Last()) + "; Value: " + Math.Exp(-(step - _spikeList.Last()) / tau));
           
            //return Math.Exp(-(step - _spikeList.Last()) / tau);

        }
    }
}
