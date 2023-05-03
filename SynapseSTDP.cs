using System;
using System.IO;

namespace SLN
{
    /// <summary>
    /// Spike-Time-Dependent-Plasticity synapse
    /// </summary>
    [Serializable]
    internal class SynapseSTDP : Synapse
	{
		private double _multFactorSTDP;


        /// <summary>
        /// The weight of the synapse (with local excitation)
        /// </summary>
        protected double _W_temp;

        /// <summary>
        /// The min weight of the synapse 
        /// </summary>
        protected double stdp_w_lo;

        /// <summary>
        /// The max weight of the synapse 
        /// </summary>
        protected double stdp_w_hi;

        /// <summary>
        /// The inertial of a synapse
        /// </summary>
        protected double _DWold;

        /// <summary>
        /// The weight of the synapse (with local excitation)
        /// </summary>
        internal double W_temp
        {
            get { return _W_temp; }
        }

		/// <summary>
		/// Constructor (with default initial weight)
		/// </summary>
		/// <param name="start">The starting neuron</param>
		/// <param name="dest">The destination neuron</param>
		/// <param name="multFactorSTDP">Common multiplication factor of STDP gains</param>
		/// <param name="multFactorI">Multiplication factor of synapse current gain</param>
		internal SynapseSTDP(Neuron start, Neuron dest, double multFactorSTDP, double multFactorI)
			: base(start, dest, Constants.STDP_INIT_W, Constants.STDP_TAU,
				Constants.STDP_DELAY_STEP, multFactorI)
		{
			_multFactorSTDP = multFactorSTDP;
            _W_temp = _W;
            _DWold = 0;
            stdp_w_lo = Constants.STDP_W_LO;
            stdp_w_hi = Constants.STDP_W_HI;
            
		}

        /// <summary>
        /// Constructor (with default initial weight)
        /// </summary>
        /// <param name="start">The starting neuron</param>
        /// <param name="dest">The destination neuron</param>
        /// <param name="multFactorSTDP">Common multiplication factor of STDP gains</param>
        /// <param name="multFactorI">Multiplication factor of synapse current gain</param>
        internal SynapseSTDP(Neuron start, Neuron dest, double multFactorSTDP, double multFactorI,double max_w, double min_w)
            : base(start, dest, Constants.STDP_INIT_W, Constants.STDP_TAU,
                Constants.STDP_DELAY_STEP, multFactorI)
        {
            _multFactorSTDP = multFactorSTDP;
            _W_temp = _W;
            _DWold = 0;
            stdp_w_hi = max_w;
            stdp_w_lo = min_w;
            

        }

       

        /// <summary>
        /// Constructor (with modified time costant)
        /// </summary>
        /// <param name="start">The starting neuron</param>
        /// <param name="dest">The destination neuron</param>
        /// <param name="multFactorSTDP">Common multiplication factor of STDP gains</param>
        /// <param name="multFactorI">Multiplication factor of synapse current gain</param>
        /// <param name="tau">Time costant of STDP</param>
        internal SynapseSTDP(double tau,Neuron start, Neuron dest, double multFactorSTDP, double multFactorI)
            : base(start, dest, Constants.STDP_INIT_W, Constants.STDP_TAU_EXPECTATION,
                Constants.STDP_DELAY_STEP, multFactorI)
        {
            _multFactorSTDP = multFactorSTDP;
            _W_temp = _W;
            _DWold = 0;
            stdp_w_lo = Constants.STDP_W_LO;
            stdp_w_hi = Constants.STDP_W_HI;
        }

        /// <summary>
        /// Constructor (with modified time costant)
        /// </summary>
        /// <param name="start">The starting neuron</param>
        /// <param name="dest">The destination neuron</param>
        /// <param name="multFactorSTDP">Common multiplication factor of STDP gains</param>
        /// <param name="multFactorI">Multiplication factor of synapse current gain</param>
        /// <param name="tau">Time costant of STDP</param>
        internal SynapseSTDP(double tau, Neuron start, Neuron dest, double multFactorSTDP, double multFactorI, double weight)
            : base(start, dest, Constants.STDP_INIT_W, tau,
                Constants.STDP_DELAY_STEP, multFactorI)
        {
            _multFactorSTDP = multFactorSTDP;
            _W = weight;
            _Wsec = weight;
            _W_temp = _W;
            _DWold = 0;
            stdp_w_lo = Constants.STDP_W_LO;
            stdp_w_hi = Constants.STDP_W_HI;
        }


        /// <summary>
		/// Constructor (with given weight)
		/// </summary>
		/// <param name="start">The starting neuron</param>
		/// <param name="dest">The destination neuron</param>
		/// <param name="multFactorSTDP">Common multiplication factor of STDP gains</param>
		/// <param name="multFactorI">Multiplication factor of synapse current gain</param>
		/// <param name="weight">The weight of the synapse</param>
        internal SynapseSTDP(Neuron start, Neuron dest, double multFactorSTDP, double multFactorI, double weight)
			: this(start, dest, multFactorSTDP, multFactorI)
		{
			_W = weight;
			_Wsec = weight;
            _W_temp = _W;
            _DWold = 0;
            stdp_w_lo = Constants.STDP_W_LO;
            stdp_w_hi = Constants.STDP_W_HI;
		}


         /// <summary>
		/// Constructor (with given weight)
		/// </summary>
		/// <param name="start">The starting neuron</param>
		/// <param name="dest">The destination neuron</param>
		/// <param name="multFactorSTDP">Common multiplication factor of STDP gains</param>
		/// <param name="multFactorI">Multiplication factor of synapse current gain</param>
		/// <param name="weight">The weight of the synapse</param>
        internal SynapseSTDP(Neuron start, Neuron dest, double multFactorSTDP, double multFactorI, double weight, double max_w, double min_w)
			: this(start, dest, multFactorSTDP, multFactorI)
		{
			_W = weight;
			_Wsec = weight;
            _W_temp = _W;
            _DWold = 0;
            stdp_w_lo = min_w;
            stdp_w_hi = max_w;
		}

        internal SynapseSTDP(Neuron start, Neuron dest, double multFactorSTDP, double multFactorI, double weight, double max_w, double min_w,double tau)
            : base(start, dest, weight, tau, Constants.STDP_DELAY_STEP, multFactorI)
        {
            _W = weight;
            _Wsec = weight;
            _W_temp = _W;
            _DWold = 0;
            stdp_w_lo = min_w;
            stdp_w_hi = max_w;
            _multFactorSTDP = multFactorSTDP;
        }

        

		/// <summary>
		/// Updates the synaptic weight according to the status of starting and
		/// destination neurons, and to the previous input history
		/// </summary>
		/// <param name="rewardOffset">Difference between the last epoch when a reward
		/// was triggered and the current one</param>
		/// <param name="nOldSpikes">Number of the spikes the premotor neuron fired the last
		/// time a reward was triggered</param>
		internal void learn(int rewardOffset, int nOldSpikes)
		{
			for (int k = 0; k < Start.NSpikes; k++)
			{
				int tSpkStart = Start.getSpikeAt(k);
				for (int m = 0; m < Dest.NSpikes; m++)
				{
					int tSpkDest = Dest.getSpikeAt(m);
					//double dt = Constants.INTEGRATION_STEP * (tSpkStart - tSpkDest); // SBAGLIATO!!!
					double dt = Constants.INTEGRATION_STEP * (tSpkDest - tSpkStart);

					if (dt >= 0)
						_W += _multFactorSTDP * Constants.STDP_A_P * Math.Exp(-dt / Constants.STDP_TAU_P);
					else
						_W -= _multFactorSTDP * Constants.STDP_A_N * Math.Exp(+dt / Constants.STDP_TAU_N);
				}
			}

			// "Memory"
			if (rewardOffset > 0)
			{
				int nCurrSpikes = Start.NSpikes;
				_W -= nOldSpikes * nCurrSpikes * _multFactorSTDP * Constants.STDP_A_N_OLD *
					Math.Exp(-rewardOffset / Constants.STDP_TAU_OLD);
			}

			// Decay
			_W *= Constants.STDP_DECAY;

			// Saturation values
			_W = _W < stdp_w_lo ? stdp_w_lo : _W;
			_W = _W > stdp_w_hi ? stdp_w_hi : _W;

            //if (_W > 0.05)
            //{
            //    _W += 0;
            //}

			_Wsec = _W;
		}


        /// <summary>
        /// Updates the synaptic weight according to the status of starting and
        /// destination neurons, and to the previous input history
        /// </summary>
        /// <param name="i">step, sembra non avere alcun ruolo</param>
        /// <param name="ni">coefficente d'apprendimento</param>
        /// <param name="eps">treshold per l'aggiornamento dei pesi</param>
        /// <param name="option">Modalità di learning: a campioni, ad epoch, ...</param>
        /// <param name="save">Parametro Inutile</param>
        /// <param name="t">Target</param>

        internal double learnLiquid(int i,double ni,double eps,int option,bool save,double t){
	        //obbiettivo: fare in modo che la tensione del neurone somma (OutExt) 
            //coincida il più possibile con quella di riferimento del target.

            //l'algoritmo di aggiornamento dei pesi è praticamente quello del Perceptron
	        double I_out = this.Dest.IPrev; // E> restituisce la v di out della rete ad un dato istante
            double V_out = this.Dest.V;
            double target =t ;//Math::Exp(-i*Clock::DeT()/1000)/100;
	
	        double E;
            //double up =0;
	        //double gamma=0.05,u =1;

	        double segno = 1;

           

            //switch(t)
            //{
            //     //E> target fissi, impostati qui
            //    //case 0: target =(double)Math.Sin(i*Math.PI/100)/100  ; break;
            //    //case 1: target =(double)Math.Sin(2*i*Math.PI/100)/100+0.01; break;
            //    //case 2: target =(double)Math.Sin(3*i*Math.PI/100)/100+0.01; break;
            //    //case 3: target =(double)Math.Sin(4*i*Math.PI/100)/100; break;
            //    //case 4 :target =(double)Math.Cos(4*i*Math.PI/100)/100; break;
            //    //case 5 :target =(double)Math.Exp(-i/100)/100; break;  // da MODIFICARE
            //    //case 6: target =(double)(1-((Math.Cos(i*Math.PI/(100-1))+1)/2))/200;break;
            //    //case 7: target =(double)(1-((Math.Cos(i*Math.PI/(100-1))+1)/2))/100;break;

            //    //sinusoidal with f=5Hz
            //    case 0: target = (double)Math.Sin((i * 2 * Math.PI * (5 * 0.001)) / Constants.INTEGRATION_STEP_MORRIS_LECAR) / 100; break;
            //    //sinusoidal with f=10Hz
            //    case 1: target = (double)Math.Sin((i * 2 * Math.PI * (10 * 0.001)) / Constants.INTEGRATION_STEP_MORRIS_LECAR) / 100; break;
            //    //sinusoidal with f=15Hz
            //    case 2: target = (double)Math.Sin((i * 2 * Math.PI * (15 * 0.001)) / Constants.INTEGRATION_STEP_MORRIS_LECAR) / 100; break;
            //    //sinusoidal with f=20Hz
            //    case 3: target = (double)Math.Sin((i * 2 * Math.PI * (20 * 0.001)) / Constants.INTEGRATION_STEP_MORRIS_LECAR) / 100; break;
                        
            //}


            

   	        E =  target - V_out;
            if (i == 0 && E == 70) E = 0;

            //ricordiamo che a tutto il liquido è associato un unico neurone di output
            //(escludendo quelli per la classificazione)    
	        for (int k=0;k<Constants.LIQUID_OUTPUT;k++)
	        {
                    Class1Neuron start = (Class1Neuron)this.Start;
                    //double I_liquid = start.IOut_array[i];
                    double I_tmp = this.Start.IOut;
			         // E> vedi form1.h ()riga 1179
		            switch(option)
		            {
			        case 0:
                            //aggiornamento a campioni
                            if (Math.Abs(E) > eps)
                            {
                                //if (this.Start.ROW == 0 && this.Start.COLUMN == 1)
                                //{
                                    //if(i>100)
                                    //   i = i;
                                    _W = _W + ni * I_tmp * E; //+ 0.2* _DWold;
                                    _DWold = ni * I_tmp * E;
                                //}

                            }
                            break;

                    case 1:
                            //aggiornamento a campioni a epoche
                            if (Math.Abs(E) > eps)
                            {
                                    _W_temp = _W_temp + ni * I_tmp * E;                             
                            } 
                            break;
			   
			        case 2:
                            //aggiornamento a campioni a segno di errore
                            if (E >= 0) segno = 1;
                            else segno = -1;

				            if(Math.Abs(E)>eps){
                                
                                    _W = _W + ni * I_tmp * segno;
                            }
                            break;
			  
			        case -1: break; // E> implementata in simulate, è la pseudo-inversa
			   
			        case 4: 
                            //aggiornamento a segno d'errore con epoche
                            if (E >= 0) segno = 1;
                            else segno = -1;

				            if(Math.Abs(E)>eps){
                                
                                    _W_temp = _W_temp + ni * I_tmp * segno;
                            }
                            break;
		            }

		  		 
	            }
		

            return E;
  
        }

        /// <summary>
        /// Update the value of the weight of the synapse from the temporany value stored
        /// </summary>
        public void UpdateSyn() {
            _W = _W_temp;
        }

        /// <summary>
        /// Update the value of the weight of the synapse from the temporany value stored
        /// </summary>
        public void setW(double weight)
        {
            _W += weight;
            _W = _W < stdp_w_lo ? stdp_w_lo : _W;
            _W = _W > stdp_w_hi ? stdp_w_hi : _W;
        }

        /// <summary>
        /// Update the value of the weight of the synapse in an incremental manner respect to the percentage value selected
        /// </summary>
        public void setW(double weight,double percentage)
        {
            if(Math.Abs(_W) < Math.Abs(weight))
                _W += weight * percentage / 100 ;
            
            
        }

		/// <summary>
		/// Prints the weight of the synapse
		/// </summary>
		/// <returns>The synaptic weight</returns>
		public override string ToString()
		{
			return String.Format("{0:F2}", _W);
		}

        /// <summary>
        /// Decay the synaptic weight during the test phase
        /// </summary>
        internal void decay()
        {
            // Decay
            _W *= Constants.STDP_DECAY_EXPECTATION;

            // Saturation values
            _W = _W < stdp_w_lo ? stdp_w_lo : _W;
            _W = _W > stdp_w_hi ? stdp_w_hi : _W;

            _Wsec = _W;
        }

        /// <summary>
        /// Decay the synaptic weight during the test phase in according to the Oja rule
        /// </summary>
        internal void decayOja(double threshold)
        {
            double decay;
            double freqStart = Start.getFrequency(Constants.SIMULATION_STEPS_A+Constants.SIMULATION_STEPS_DETOUR);
            double freqDest = Dest.getFrequency(Constants.SIMULATION_STEPS_A+Constants.SIMULATION_STEPS_DETOUR);
            freqDest = freqDest / threshold;
            freqStart = freqStart / threshold;
            //if (freqDest > 0.2)
            //    freqStart = 0;
            if (freqDest > 1)
                decay = -(freqDest - 1) * freqStart;
            else
                decay = 0;
            
            // Decay
            _W += decay;

            // Saturation values
            _W = _W < stdp_w_lo ? stdp_w_lo : _W;
            _W = _W > stdp_w_hi ? stdp_w_hi : _W;

            _Wsec = _W;
        }


	}
}
