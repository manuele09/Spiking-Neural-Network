﻿using System;


namespace SLN
{
    /// <summary>
    /// A synapse between two neurons
    /// </summary>
    [Serializable]
    internal class Synapse
	{
		#region Fields and properties

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

		/// <summary>
		/// The weight of the synapse (without local excitation)
		/// </summary>
		protected double _Wsec;

		/// <summary>
		/// The weight of the synapse (without local excitation)
		/// </summary>
		internal double Wsec
		{
			get { return _Wsec; }
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

		protected double _Iprev;

		/// <summary>
		/// The current generated by the synapse
		/// </summary>
		internal double IPrev
		{
			get { return _Iprev; }
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
		internal Synapse(Neuron start, Neuron dest, double w, double wSec, double tau, int delay, double gain)
		{
			_start = start;
			_dest = dest;
			_W = w;
			_Wsec = wSec;
			_tau = tau;
			_delay = delay;
			_gain = gain;
		}

		/// <summary>
		/// Constructor (with the same excitation weights specified)
		/// </summary>
		/// <param name="start">The starting neuron</param>
		/// <param name="dest">The destination neuron</param>
		/// <param name="w">The synaptic weight</param>
		/// <param name="tau">The synapse time constant</param>
		/// <param name="delay">The synaptic delay (in steps of simulation)</param>
		/// <param name="gain">Gain in the calculation of the current</param>
		internal Synapse(Neuron start, Neuron dest, double w, double tau, int delay, double gain)
			: this(start, dest, w, w, tau, delay, gain)
		{ }

        //https://stackoverflow.com/questions/10552280/fast-exp-calculation-possible-to-improve-accuracy-without-losing-too-much-perfo
        public static double exp1(double x)
        {
            var tmp = (long)(1512775 * x + 1072632447);
            return BitConverter.Int64BitsToDouble(tmp << 32);
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
                int tspk = Start.getSpikeAt(k);
                if (tspk <= step) // Necessary in the learning phase
                {
                    double t = (Constants.INTEGRATION_STEP) * (step - tspk + Delay);
                    double Itmp = _gain * (t / Tau) * exp1(1 - t / Tau);
                    
                    I += Itmp;
                }
            }

            //update the output current of the start neuron
            this.Start._iOut = I;
            I *= W;
            Dest.updateI(I);
            _Iprev = I;
            return 0;
        }
        internal virtual double simulate_morris_to_motor(int step)
        {

            double I = 0;

            //Calculating the current generated by each spike of the starting neuron
            int start_index = Start.NSpikes - 50;
            if (Start.NSpikes <= 50)
                start_index = 0;


            for (int k = 0; k < Start.NSpikes; k++)
            {
                int tspk = Start.getSpikeAt(k);
                if (tspk <= step && (step - tspk) < 200) // Necessary in the learning phase
                {
                    double t = (Constants.INTEGRATION_STEP) * (step - tspk + Delay);
                    double Itmp = _gain * (t / Tau) * Math.Exp(1 - t / Tau);
                    I += Itmp;
                }
            }

            //update the output current of the start neuron
            this.Start._iOut = I;
            I *= W;
            //else
            //    I *= Wsec;

            //Forwarding the current to the destination neuron
            Dest.updateI(I);
            //Saving the current for debugging purposes
            _Iprev = I;

            return 0;


        }

        internal virtual double simulate_context_to_end(int step)
        {

            double I = 0;

            //Calculating the current generated by each spike of the starting neuron
            int start_index = Start.NSpikes - 50;
            if (Start.NSpikes <= 50)
                start_index = 0;


            for (int k = 0; k < Start.NSpikes; k++)
            {
                int tspk = Start.getSpikeAt(k);
                if (tspk <= step && (step - tspk) < 200) // Necessary in the learning phase
                {
                    double t = (Constants.INTEGRATION_STEP) * (step - tspk + Delay);
                    double Itmp = _gain * (t / Tau) * Math.Exp(1 - t / Tau);
                    I += Itmp;
                }
            }

            //update the output current of the start neuron
            this.Start._iOut = I;
            I *= W;
            //else
            //    I *= Wsec;

            //Forwarding the current to the destination neuron
            Dest.updateI(I);
            //Saving the current for debugging purposes
            _Iprev = I;

            return 0;


        }

        /// <summary>
        /// Simulates the synapse
        /// </summary>
        /// <param name="step">The current simulation step</param>
        internal virtual double simulate(int step, int k0)
        {

            double I = 0;

            //Calculating the current generated by each spike of the starting neuron
            for (int k = k0; k < Start.NSpikes; k++)
            {
                int tspk = Start.getSpikeAt(k);
                if (tspk <= step) // Necessary in the learning phase
                {
                    double t = (Constants.INTEGRATION_STEP) * (step - tspk + Delay);
                    double Itmp = _gain * (t / Tau) * Math.Exp(1 - t / Tau);
                    I += Itmp;
                }
            }

            //update the output current of the start neuron
            this.Start._iOut = I;

            //if (step <= Constants.LOCAL_EXCITATORY_WINDOW || (step >= Constants.SIMULATION_STEPS_DETOUR && step <= Constants.LOCAL_EXCITATORY_WINDOW + Constants.SIMULATION_STEPS_DETOUR))
            //if (step <= Constants.LOCAL_EXCITATORY_WINDOW)
            I *= W;
            //else
            //    I *= Wsec;

            //Forwarding the current to the destination neuron
            Dest.updateI(I);
            //Saving the current for debugging purposes
            _Iprev = I;

            //if (Dest.COLUMN == 1 && Dest.ROW == 0 && Dest.LAYER == LayerNumbers.SOSL_1)
            //{
            //    //Dest.file.WriteLine(step + "\t" + I);
            //    //Dest.file.Flush();
            //    return I;
            //}
            //else 

            return 0;


        }
        internal virtual double simulate_morris_to_ring(int step, int k0)
        {

            double I = 0;

            //Calculating the current generated by each spike of the starting neuron
            for (int k = k0; k < Start.NSpikes; k++)
            {
                int tspk = Start.getSpikeAt(k);
                if (tspk <= step) // Necessary in the learning phase
                {
                    double t = (Constants.INTEGRATION_STEP) * (step - tspk + Delay);
                    double Itmp = _gain * (t / Tau) * Math.Exp(1 - t / Tau);
                    I += Itmp;
                }
            }

            //update the output current of the start neuron
            this.Start._iOut = I;

            //if (step <= Constants.LOCAL_EXCITATORY_WINDOW || (step >= Constants.SIMULATION_STEPS_DETOUR && step <= Constants.LOCAL_EXCITATORY_WINDOW + Constants.SIMULATION_STEPS_DETOUR))
            //if (step <= Constants.LOCAL_EXCITATORY_WINDOW)
            I *= W;
            //else
            //    I *= Wsec;

            //Forwarding the current to the destination neuron
            Dest.updateI(I);
            //Saving the current for debugging purposes
            _Iprev = I;

            //if (Dest.COLUMN == 1 && Dest.ROW == 0 && Dest.LAYER == LayerNumbers.SOSL_1)
            //{
            //    //Dest.file.WriteLine(step + "\t" + I);
            //    //Dest.file.Flush();
            //    return I;
            //}
            //else 

            return 0;


        }

        /// <summary>
        /// Simulates the synapse
        /// </summary>
        /// <param name="step">The current simulation step</param>
        internal virtual double simulatecontext(int step)
        {

            double I = 1;//60 + 90; //we set the current over the threshold of the Morris Lecar for having a continuous spikes: I_const=60

            //update the output current of the start neuron
            this.Start._iOut = I;

            //if (step <= Constants.LOCAL_EXCITATORY_WINDOW || (step >= Constants.SIMULATION_STEPS_DETOUR && step <= Constants.LOCAL_EXCITATORY_WINDOW + Constants.SIMULATION_STEPS_DETOUR))
            //if (step <= Constants.LOCAL_EXCITATORY_WINDOW)
            I *= W;
            //else
            //    I *= Wsec;

            //Forwarding the current to the destination neuron
            Dest.updateI(I);
            //Saving the current for debugging purposes
            _Iprev = I;

            //if (Dest.COLUMN == 1 && Dest.ROW == 0 && Dest.LAYER == LayerNumbers.SOSL_1)
            //{
            //    //Dest.file.WriteLine(step + "\t" + I);
            //    //Dest.file.Flush();
            //    return I;
            //}
            //else 

            return 0;


        }

		/// <summary>
		/// Sets the starting neuron of the synapse
		/// </summary>
		/// <param name="neuron">The neuron to be set as starting one</param>
		internal void setStartingNeuron(Neuron neuron)
		{
			_start = neuron;
		}

        /// <summary>
        /// reset the state of the synapses with the initial value
        /// </summary>
        internal void resetState()
        {
            _Iprev = 0;									
        }
	}
}