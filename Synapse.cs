﻿using System;


namespace SLN
{
    /// <summary>
    /// A synapse between two neurons
    /// </summary>
    [Serializable]
    public class Synapse
    {
        #region Fields and properties
        protected double current;

        /// <summary>
        /// The starting neuron of the synapse
        /// </summary>
        protected Neuron _start;

        /// <summary>
        /// The starting neuron of the synapse
        /// </summary>
        internal Neuron Start
        {
            get { return _start; }
        }

        protected Neuron _dest;

        /// <summary>
        /// The destination neuron of the synapse
        /// </summary>
        internal Neuron Dest
        {
            get { return _dest; }
        }

        /// <summary>
        /// The weight of the synapse (with local excitation)
        /// </summary>
        protected double _W;

        /// <summary>
        /// The weight of the synapse (with local excitation)
        /// </summary>
        internal double W
        {
            get { return _W; }
            set { _W = value; }
        }

        private double _tau;

        /// <summary>
        /// The time constant of the synapse
        /// </summary>
        internal double Tau
        {
            // [MethodImpl(MethodImplOptions.NoInlining)]
            get { return _tau; }
        }

        private int _delay;

        /// <summary>
        /// The delay of the synapse (in steps of simulation)
        /// </summary>
        internal int Delay
        {
            get { return _delay; }
        }


        protected double _gain;
        #endregion

        /// <summary>
        /// Constructor (with different excitation weights specified)
        /// </summary>
        /// <param name="start">The starting neuron</param>
        /// <param name="dest">The destination neuron</param>
        /// <param name="w">The synaptic weight (with local excitation)</param>
        /// <param name="wSec">The synaptic weight (without local excitation)</param>
        /// <param name="tau">The synapse time constant</param>
        /// <param name="delay">The synaptic delay (in steps of simulation)</param>
        /// <param name="gain">Gain in the calculation of the current</param>
        internal Synapse(Neuron start, Neuron dest, double w, double tau, int delay, double gain)
        {
            _start = start;
            _dest = dest;
            _W = w;
            _tau = tau;
            _delay = delay;
            _gain = gain;
        }

        /// <summary>
        /// Simulates the synapse
        /// </summary>
        /// <param name="step">The current simulation step</param>
        internal virtual double simulate(int step)
        {

            double I = 0;

            //Calculating the current generated by each spike of the starting neuron
            for (int k = 0; k < Start.NSpikes; k++)
            {
                //int tspk = Start._spikeList._spikelist[k];
                int tspk = Start.getSpikeAt(k);
                if (tspk != -1 && tspk <= step) // Necessary in the learning phase
                //if (tspk <= step) // Necessary in the learning phase
                {
                    double t = (Constants.INTEGRATION_STEP) * (step - tspk + Delay);
                    double Itmp = _gain / 1.5 * (t / Tau) * Math.Exp(1 - t / Tau); //exp1

                    I += Itmp;
                }
            }

            I *= W;
            current = I;
            //Dest.updateI(I);
            return 0;
        }
        
        internal virtual void update_current()
        {
            Dest.updateI(current);
        }
        internal virtual void resetState()
        {
            ;
        }
    }
}