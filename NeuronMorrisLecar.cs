using System;
using System.Collections.Generic;
using System.Text;


namespace SLN
{
    [Serializable]
    public class NeuronMorrisLecar : Neuron
    {

        
		#region Neuron variables

		/// <summary>
		/// The <b>Vk</b> parameter in Morris Lecar model
		/// </summary>
		protected double Vk;

        /// <summary>
		/// The <b>Vl</b> parameter in Morris Lecar model
		/// </summary>
		protected double Vl;

        /// <summary>
		/// The <b>Vca</b> parameter in Morris Lecar model
		/// </summary>
		protected double Vca;

        /// <summary>
		/// The <b>gca</b> parameter in Morris Lecar model
		/// </summary>
		protected double gca;

        /// <summary>
		/// The <b>gk</b> parameter in Morris Lecar model
		/// </summary>
		protected double gk;

        /// <summary>
		/// The <b>gl</b> parameter in Morris Lecar model
		/// </summary>
		protected double gl;

        /// <summary>
		/// The <b>V1</b> parameter in Morris Lecar model
		/// </summary>
		protected double V1;

        /// <summary>
		/// The <b>V2</b> parameter in Morris Lecar model
		/// </summary>
		protected double V2;

        /// <summary>
		/// The <b>V3</b> parameter in Morris Lecar model
		/// </summary>
		protected double V3;

        /// <summary>
		/// The <b>V4</b> parameter in Morris Lecar model
		/// </summary>
		protected double V4;

        /// <summary>
		/// The <b>kf</b> parameter in Morris Lecar model
		/// </summary>
		protected double kf;

        /// <summary>
        /// KF
        /// </summary>
        internal double KF
        {
            get { return kf; }
            set { kf = value; }
        }

        //private double _v;

        ///// <summary>
        ///// Membrane potential
        ///// </summary>
        //internal new double V
        //{
        //    get { return _v; }
        //    set { _v = value; }
        //}

		private double _w;

		/// <summary>
		/// Membrane variable
		/// </summary>
		internal double W
		{
			get { return _w; }
			set { _w = value; }
		}

        ///// <summary>
        ///// Input current 
        ///// </summary>
        //private new double _i;

        ///// <summary>
        ///// Neuron input current
        ///// </summary>
        //internal new double I
        //{
        //    get { return _i; }
        //    set { _i = value; }
        //}

        /// <summary>
        /// Input current set for the resonance frequency
        /// </summary>
        private double _i_Const;
        /// <summary>
        /// Neuron input current for the resonance frequency
        /// </summary>
        internal double I_Const
        {
            get { return _i_Const; }
            set { _i_Const = value; }
        }

        /// <summary>
        /// Input current set for the resonance frequency
        /// </summary>
        private double _i_Sum;
        /// <summary>
        /// Neuron input current for the resonance frequency
        /// </summary>
        internal double I_Sum
        {
            get { return _i_Sum; }
            set { _i_Sum = value; }
        }

        //private double _iPrev;

        ///// <summary>
        ///// Neuron total input current before resetting
        ///// (for logging purposes only)
        ///// </summary>
        //internal new double IPrev
        //{
        //    get { return _iPrev; }
        //}

        
        ///// <summary>
        ///// List with the timestemp (i.e. simulation step) at which
        ///// each spike occurred
        ///// </summary>
        //public new SpikeList _spikeList;

        ///// <summary>
        ///// Number of the spikes the neuron fired
        ///// </summary>
        //internal new int NSpikes
        //{
        //    get { return _spikeList.NSpikes; }
        //}

        
       

        public new double _iOut;
        /// <summary>
        /// Neuron output current 
        /// </summary>
        internal new double IOut
        {
            get { return _iOut; }
            set { _iOut = value; }
        }

        public int countSpike;

		#endregion

		#region Position variables
        //private int _row;

        ///// <summary>
        ///// The zero-based row coordinate of the neuron
        ///// </summary>
        //internal new int ROW
        //{
        //    get { return _row; }
        //}

        //private int _col;

        ///// <summary>
        ///// The zero-based column coordinate of the neuron
        ///// </summary>
        //internal new int COLUMN
        //{
        //    get { return _col; }
        //}

        //private LayerNumbers _layer;

        ///// <summary>
        ///// The layer the neuron belongs to
        ///// </summary>
        //internal new LayerNumbers LAYER
        //{
        //    get { return _layer; }
        //}
		#endregion

        

        ///// <summary>
        ///// Sets the position of the neuron in the network
        ///// </summary>
        ///// <param name="i">The zero-based row coordinate</param>
        ///// <param name="j">The zero-based column coordinate</param>
        ///// <param name="layer">The layer the neuron belongs to</param>
        //internal new void setCoord(int i, int j, LayerNumbers layer)
        //{
        //    _row = i;
        //    _col = j;
        //    _layer = layer;
        //}

		/// <summary>
		/// Constructor
		/// </summary>
        public NeuronMorrisLecar(double kf_freq, double I_freq)
        {
            
                V = Constants.V_INITIAL_VALUE;
                W = Constants.W_INITIAL_VALUE;

                Vk = Constants.VK_VALUE;
                Vl = Constants.Vl_VALUE;
                Vca = Constants.VCA_VALUE;
                gca = Constants.GCA_VALUE;
                gk = Constants.GK_VALUE;
                gl = Constants.GL_VALUE;
                V1 = Constants.V1_VALUE;
                V2 = Constants.V2_VALUE;
                V3 = Constants.V3_VALUE;
                V4 = Constants.V4_VALUE;
            
            _spikeList = new SpikeList();

            kf = kf_freq;
            I_Const = I_freq;
            countSpike = 0;
            
           
        }


		/// <summary>
		/// Simulates the neuron behavior
		/// </summary>
		/// <param name="step">The current simulation step</param>
		/// <returns><i>true</i> if the neuron fired a spike, <i>false</i> otherwise</returns>
		internal new bool simulate(int step)
		{
            
            //Random rnd = new Random();
            double v;
            double w;
                   
			double vPrev = V;
			bool spike = false;

            double y = I_Const * I_Sum * 100; //the factor 100 is due to the normalization of the liquid output
            //if (Math.Abs(y) > (I_Const))
            //{
            //    y = Math.Sign(y) * (I_Const);
            //}
            if (y > 0)
                y = I_Const;
            else y = -I_Const;

            y += I;



			double minf = 0.5*(1+Math.Tanh((V-V1)/V2));
            double winf = 0.5*(1+Math.Tanh((V-V3)/V4));
            double tauw = 3/((Math.Cosh((V-V3)/(2*V4))));
          
            v=V+Constants.INTEGRATION_STEP_MORRIS_LECAR*kf*(y - gca*minf*(V-Vca)-gk*W*(V-Vk)-gl*(V-Vl));
            w=W+Constants.INTEGRATION_STEP_MORRIS_LECAR*kf*(winf-W)/tauw;

            int indspike;
            if (_spikeList.Count == 0)
            {
                indspike = -1;
            }
            else
                indspike = _spikeList.Count - 1;
			if ( v > 30 && (indspike==-1||(step-_spikeList.getSpikeAt(indspike))>15))
			{
				_spikeList.addSpike(step);
				spike = true;
                countSpike++;
			}


            V = v;
            W = w;
			//Now we have "used up" the input current, so we set it to zero after saving
			//its value for logging purposes.
            IPrev = y;
            I = 0;

			return spike;
		}

		/// <summary>
		/// Returns the timestamp of the <i>n</i>-th spike of the neuron
		/// </summary>
		/// <param name="n">The index of the spike (beginning from 0)</param>
		/// <returns>The timestamp of the spike (i.e. the simulation step when it occurred)</returns>
		internal new int getSpikeAt(int n)
		{
			return _spikeList.getSpikeAt(n);
		}

		

		/// <summary>
		/// Returns the spiking frequency of the neuron
		/// </summary>
		/// <param name="lastTimestamp">Last timestamp (i.e. integration step) on which
		/// calculate the frequency</param>
		/// <returns>The spiking frequency of the neuron</returns>
		internal new double getFrequency(int lastTimestamp)
		{
			int firstTimestamp = lastTimestamp - Constants.FREQUENCY_WINDOW_SIZE + 1;
			int firstIndex = _spikeList.FindIndex(
				delegate(int spk)
				{
					return spk >= firstTimestamp;
				}
			);

           
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
		/// Neset the neuron's state
		/// </summary>
		internal new void resetState()
		{
           
                V = Constants.V_INITIAL_VALUE;
                _w = Constants.W_INITIAL_VALUE;

                countSpike = 0;
			_spikeList = new SpikeList();

		}

		/// <summary>
		/// Returns the list of the spikes
		/// </summary>
		/// <returns>The list of the spikes the neuron fired</returns>
		internal new SpikeList getSpikeList()
		{
			return _spikeList;
		}



    }
}
