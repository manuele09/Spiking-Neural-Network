namespace SLN
{
	/// <summary>
	/// Global constants
	/// </summary>
	public class Constants
	{
        public const int LOGGING_RATE = 100; //salvo su file ogni 100 passi
        public const int DEBUG = 0;
        public const int MORRIS_FREQUENCIES = 0;
        public const double start_freq = 5; //con step 5 riesce fino a 6 input, con step 3 almeno il doppio
        public const double incr_freq = 3;
        public const double KF = 0.35; 
        public const double KI = 70;

        public const double EX_PROB = 0.8;
        public const double LIQUID_CONNETTIVITY = 0.5;
        public const double INPUT_TO_LIQUID_PROB = 0.5;
        public const int Single_INPUT_TO_LIQUID_CONNECTIONS = 25;


        #region Network geometry
        /// <summary>
        /// Number of rows in the first layer
        /// </summary>
        public const int FIRST_LAYER_DIMENSION_I = 64;

		/// <summary>
		/// Number of columns in the first layer
		/// </summary>
		public const int FIRST_LAYER_DIMENSION_J = 64;

	
    

        /// <summary>
        /// Number of rows in the Liquid state
        /// </summary>
        public const int LIQUID_DIMENSION_I = 10;

        /// <summary>
        /// Number of columns in the Liquid
        /// </summary>
        public const int LIQUID_DIMENSION_J = 10;

		/// <summary>
		/// Radius of the neighborhood for each neuron in the second layer
		/// </summary>
		public const int NEIGHBORHOOD_SIZE = 1;

		/// <summary>
		/// Probability of creation of a first-to-second-layer 
		/// between two neurons
		/// </summary>
        public const double FIRST_TO_SECOND_P = 0.25;

        /// <summary>
        /// Probability of creation of a second-to-third layer 
        /// between two neurons
        /// </summary>
        public const double SECOND_TO_THIRD_P = 0.25;

		/// <summary>
		/// Number of the neurons in the winner cluster (including the central one)
		/// </summary>
		public const int WINNER_CLUSTER_SIZE = 5;

        /// <summary>
        /// Number of ring in the context layer
        /// </summary>
        public const int RINGS = 4;
        public const int CLASSES = 6;

        /// <summary>
        /// Number of ring in the context layer
        /// </summary>
        public const int MOTOR = 2;
		#endregion

		#region Simulation parameters
		/// <summary>
		/// Steps of simulation (phase A)
		/// </summary>
		public const int SIMULATION_STEPS_A = 1000;    //1000

		/// <summary>
		/// Steps of simulation (phase B)
		/// </summary>
        public const int SIMULATION_STEPS_B = 500; //500;

        /// <summary>
        /// Steps of simulation (phase DETOUR)
        /// </summary>
        public const int SIMULATION_STEPS_DETOUR = 1000; //1000;

        /// <summary>
        /// Steps of simulation (phase FEEDFORWARD)
        /// </summary>
        public const int SIMULATION_STEPS_FEEDFORWARD = 200; //1000;

        /// <summary>
        /// Steps of an epoch (Liquid State)
        /// </summary>
        public const int SIMULATION_STEPS_LIQUID = 1000;//1000;

		/// <summary>
		/// Value of each step of integration in ms
		/// </summary>
        public const double INTEGRATION_STEP = 1;     //0.08;

        /// <summary>
        /// Value of each step of integration in ms for the liquid state
        /// </summary>
        public const double INTEGRATION_STEP_LIQUID = 0.08;     //1.5;

        /// <summary>
        /// Default value of the current in input for the neuron in the Liquid State
        /// </summary>
        public const double DEFAULT_CURRENT_LIQUID = 35.0;


        /// <summary>
        /// Value of each step of integration in ms for the Morris-Lecar neuron
        /// </summary>
        public const double INTEGRATION_STEP_MORRIS_LECAR = 1;    


		/// <summary>
		/// The offset (in steps of simulation) when the simulation starts. 
		/// </summary>
		public const int SIMULATION_STEP_OFFSET = 13;

		/// <summary>
		/// Number of consecutive simulations started by the Main
		/// </summary>
		public const int SIMULATION_NUMBER = 21; //////////
         
		/// <summary>
		/// Number of consecutive simulations started used for learning
		/// </summary>
        public const int LEARNING_NUMBER = 32;

        /// <summary>
        /// Number of consecutive simulations started used for testing
        /// </summary>
        public const int TESTING_NUMBER = 4;

		/// <summary>
		/// Set to <i>true</i>, enables output to console
		/// </summary>
		public const bool ENABLE_OUTPUT = false;
		#endregion

		#region Neuron parameters
		/// <summary>
		/// Initial value of neurons' membrane potential
		/// </summary>
		public const double INITIAL_STATE_V = -70.0;

		/// <summary>
		/// Initial value of neurons' recovery variable
		/// </summary>
		public const double INITIAL_STATE_U = -14.0;

        /// <summary>
        /// Initial value of neurons' membrane potential for the Liquid State
        /// </summary>
        public const double INITIAL_STATE_V_LIQUID = -60.0;

        /// <summary>
        /// Initial value of neurons' recovery variable for the Liquid State
        /// </summary>
        public const double INITIAL_STATE_U_LIQUID = 6.0;

		/// <summary>
		/// Parameter <b>A</b> in Izhikevich's spiking neuron model
		/// </summary>
		public const double A_IZH_SPK = 0.02;

		/// <summary>
		/// Parameter <b>B</b> in Izhikevich's spiking neuron model
		/// </summary>
		public const double B_IZH_SPK = 0.2;

		/// <summary>
		/// Parameter <b>C</b> in Izhikevich's spiking neuron model
		/// </summary>
		public const double C_IZH_SPK = -65.0;

		/// <summary>
		/// Parameter <b>D</b> in Izhikevich's spiking neuron model
		/// </summary>
		public const double D_IZH_SPK = 1.5;

		/// <summary>
		/// Parameter <b>A</b> in Izhikevich's "alternative" neuron model
		/// </summary>
		public const double A_IZH_ALT = 0.0;

		/// <summary>
		/// Parameter <b>B</b> in Izhikevich's "alternative" neuron model
		/// </summary>
		public const double B_IZH_ALT = -0.1;

		/// <summary>
		/// Parameter <b>C</b> in Izhikevich's "alternative" neuron model
		/// </summary>
		public const double C_IZH_ALT = -55.0;

		/// <summary>
		/// Parameter <b>D</b> in Izhikevich's "alternative" neuron model
		/// </summary>
		public const double D_IZH_ALT = 0.0;

        /// <summary>
        /// Parameter <b>A</b> in Izhikevich's Inhibition-Induced Spiking neuron model
        /// </summary>
        public const double A_IZH_INH = -0.02;

        /// <summary>
        /// Parameter <b>B</b> in Izhikevich's Inhibition-Induced Spiking neuron model
        /// </summary>
        public const double B_IZH_INH = -1;

        /// <summary>
        /// Parameter <b>C</b> in Izhikevich's Inhibition-Induced Spiking neuron model
        /// </summary>
        public const double C_IZH_INH = -60;

        /// <summary>
        /// Parameter <b>D</b> in Izhikevich's Inhibition-Induced Spiking neuron model
        /// </summary>
        public const double D_IZH_INH = 8;

        /// <summary>
        /// Parameter <b>A</b> in Izhikevich's Class1 Spiking neuron model
        /// </summary>
        public const double A_IZH_CLASS1 = 0.02;

        /// <summary>
        /// Parameter <b>B</b> in Izhikevich's Class1 Spiking neuron model
        /// </summary>
        public const double B_IZH_CLASS1 = 0.2;

        /// <summary>
        /// Parameter <b>C</b> in Izhikevich's Class1 Spiking neuron model
        /// </summary>
        public const double C_IZH_CLASS1 = -60;

        /// <summary>
        /// Parameter <b>D</b> in Izhikevich's Class1 Spiking neuron model
        /// </summary>
        public const double D_IZH_CLASS1 = 2;

        #region Morris-Lecar neuron

        /// <summary>
        /// Parameter <b>Vk</b> in Morris-Lecar neuron model
        /// </summary>
        public const double VK_VALUE = -84;

        /// <summary>
        /// Parameter <b>Vl</b> in Morris-Lecar neuron model
        /// </summary>
        public const double Vl_VALUE = -60;

        /// <summary>
        /// Parameter <b>Vca</b> in Morris-Lecar neuron model
        /// </summary>
        public const double VCA_VALUE = 120;

        /// <summary>
        /// Parameter <b>gca</b> in Morris-Lecar neuron model
        /// </summary>
        public const double GCA_VALUE = 4.4;

        /// <summary>
        /// Parameter <b>gk</b> in Morris-Lecar neuron model
        /// </summary>
        public const double GK_VALUE = 8;

        /// <summary>
        /// Parameter <b>gl</b> in Morris-Lecar neuron model
        /// </summary>
        public const double GL_VALUE = 2;

        /// <summary>
        /// Parameter <b>V1</b> in Morris-Lecar neuron model
        /// </summary>
        public const double V1_VALUE = -1.2;

        /// <summary>
        /// Parameter <b>V2</b> in Morris-Lecar neuron model
        /// </summary>
        public const double V2_VALUE = 18;

        /// <summary>
        /// Parameter <b>V3</b> in Morris-Lecar neuron model
        /// </summary>
        public const double V3_VALUE = 2;

        /// <summary>
        /// Parameter <b>V4</b> in Morris-Lecar neuron model
        /// </summary>
        public const double V4_VALUE = 30;

        /// <summary>
        /// Parameter <b>V_INITIAL_VALUE</b> in Morris-Lecar neuron model
        /// </summary>
        public const double V_INITIAL_VALUE = -60;

        /// <summary>
        /// Parameter <b>W_INITIAL_VALUE</b> in Morris-Lecar neuron model
        /// </summary>
        public const double W_INITIAL_VALUE = 0.01;

        /// <summary>
        /// Parameter <b>I</b> in Morris-Lecar neuron model
        /// </summary>
        public const double I_8Hz = 61.35;

        /// <summary>
        /// Parameter <b>kf</b> in Morris-Lecar neuron model
        /// </summary>
        public const double kf_8Hz = 0.082;


        #endregion


        #endregion

        #region Synaptic parameters
        #region First-to-second synapses
        /// <summary>
		/// Synaptic weight of the first-to-second-layer synapses
		/// </summary>
        public const double FIRST_TO_SECOND_W = 30;   //30.0;

		/// <summary>
		/// Value in steps of simulation of first-to-second-layer synaptic delay
		/// </summary>
		public const int FIRST_TO_SECOND_DELAY_STEP = 0;

		/// <summary>
		/// Time constant of a first-to-second-layer synapse
		/// </summary>
		public const double FIRST_TO_SECOND_TAU = 1.0;

		/// <summary>
		/// Gain in the calculation of the current for first-to-second-layer synapses
		/// </summary>
		public const double FIRST_TO_SECOND_SYNAPTIC_GAIN = 1.0;
		#endregion

		#region Second-to-second synapses
		/// <summary>
		/// Excitatory synaptic weight of the second-to-second-layer synapses
		/// </summary>
		public const double EXCITATORY_W = 10.0;

		/// <summary>
		/// Inhibitory synaptic weight of the second-to-second-layer synapses
		/// </summary>
		public const double INHIBITORY_W = -10.0;

		/// <summary>
		/// Weight for local synapses <i>after</i> the local excitatory window
		/// </summary>
		public static readonly double LOCAL_INHIBITORY_W =
			EXCITATORY_W / (System.Math.Pow((2 * NEIGHBORHOOD_SIZE + 1), 2) - 1);

		/// <summary>
		/// Window size (in simulation steps) in which to mantain the local excitation
		/// </summary>
		public const int LOCAL_EXCITATORY_WINDOW = 750;

		/// <summary>
		/// Value in seconds of inhibitory synaptic delay
		/// </summary>
		public const double INH_DELAY_TIME = 1.0;

		/// <summary>
		/// Value in steps of simulation of inhibitory synaptic delay
		/// </summary>
		public const int INH_DELAY_STEP = 0;

		/// <summary>
		/// Value in steps of simulation of excitatory synaptic delay
		/// </summary>
		public const int EXC_DELAY_STEP = 0;

		/// <summary>
		/// Time constant of a second-to-second-layer "<i>near</i>" synapse
		/// </summary>
		public const double NEAR_TAU = 1.0;

		/// <summary>
		/// Time constant of a second-to-second-layer "<i>far</i>" synapse
		/// </summary>
		public const double FAR_TAU = 1.0;

		/// <summary>
		/// Gain in the calculation of the current for second-to-second-layer synapses
		/// </summary>
		public const double SECOND_TO_SECOND_SYNAPTIC_GAIN = 0.5;
		#endregion

		#region First-to-first synapses
		/// <summary>
		/// Excitatory synaptic weight for first-to-first-layer synapses
		/// </summary>
		public const double FIRST_TO_FIRST_W_EXC = 0;       //7;

        /// <summary>
        /// MAX synaptic weight for first-to-first-layer STDP synapses
        /// </summary>
        public const double FIRST_TO_FIRST_STDP_W_HI = 6.9;

        /// <summary>
        /// MIN synaptic weight for first-to-first-layer STDP synapses
        /// </summary>
        public const double FIRST_TO_FIRST_STDP_W_LO = 0.05;       

		/// <summary>
		/// Inhibitory synaptic weight for first-to-first-layer synapses
		/// </summary>
		public const double FIRST_TO_FIRST_W_INH = -30;

		/// <summary>
		/// Time constant of a first-to-first-layer
		/// </summary>
		public const double FIRST_TO_FIRST_TAU = 1;

		/// <summary>
		/// Value in steps of simulation of first-to-first-layer synaptic delay 
		/// </summary>
		public const int FIRST_TO_FIRST_DELAY_STEP = 0;

		/// <summary>
		/// Gain in the calculation of the current for first-to-first-layer synapses
		/// </summary>
		public const double FIRST_TO_FIRST_SYNAPTIC_GAIN = 1.0;

        /// <summary>
        /// Gain in the calculation of the current for first-to-first-layer synapses
        /// </summary>
        public const double FIRST_TO_FIRST_SYNAPTIC_GAIN_STDP = 0;// 1.5 3*0; //4.0; //2
		#endregion

		#region Second-to-first synapses
		/// <summary>
		///Sinaptic weight from second layer to first layer 
		/// </summary>
		public const double SECOND_TO_FIRST_W = 0;

		/// <summary>
		///Time constant of a second-to-first-layer
		/// </summary>
		public const double SECOND_TO_FIRST_TAU = 1;

		/// <summary>
		/// Value in steps of simulation of second-to-first-layer synaptic delay 
		/// </summary>
		public const int SECOND_TO_FIRST_DELAY_STEP = 0;

		/// <summary>
		/// Gain in the calculation of the current for second-to-first-layer synapses
		/// </summary>
		public const double SECOND_TO_FIRST_SYNAPTIC_GAIN = 2.0;

        /// <summary>
        /// Gain in the calculation of the current for second2-to-second1-layer synapses
        /// </summary>
        public const double SECOND2_TO_SECOND1_SYNAPTIC_GAIN = 1.0;
		#endregion

		#region Feedback synapses
		/// <summary>
		/// Weight of the feedback synapses between the two SOSL
		/// </summary>
		public const double FEEDBACK_W = 5.0;

		/// <summary>
		/// Time constant of feedback synapses
		/// </summary>
		public const double FEEDBACK_TAU = 1.0;

		/// <summary>
		/// Value in steps of simulation of feedback synaptic delay
		/// </summary>
		public const int FEEDBACK_DELAY_STEP = 0;

		/// <summary>
		/// Gain in the calculation of the current for feedback synapses
		/// </summary>
		public const double FEEDBACK_SYNAPTIC_GAIN = 1.0;
		#endregion

		#region Reward-to-premotor
		/// <summary>
		/// Synaptic weight between the reward neuron and the premotor one
		/// </summary>
		public const double REWARD_TO_PREMOTOR_W = 40.0;

		/// <summary>
		/// Time constant of the synapse between the reward neuron and the premotor one
		/// </summary>
		public const double REWARD_TO_PREMOTOR_TAU = 1.0;

		/// <summary>
		/// Value in steps of simulation of reward-to-premotor synaptic delay
		/// </summary>
		public const int REWARD_TO_PREMOTOR_DELAY_STEP = 0;

		/// <summary>
		/// Gain in the calculation of the current for reward-to-premotor synapse
		/// </summary>
		public const double REWARD_TO_PREMOTOR_GAIN = 1.0;
		#endregion

		#region Sameness-to-premotor
		/// <summary>
		/// Time constant of the synapse between the sameness neuron and the premotor one
		/// </summary>
		public const double SAMENESS_TO_PREMOTOR_TAU = 1.0;

		/// <summary>
		/// Value in steps of simulation of sameness-to-premotor synaptic delay
		/// </summary>
		public const int SAMENESS_TO_PREMOTOR_DELAY_STEP = 0;

		/// <summary>
		/// Gain in the calculation of the current for sameness-to-premotor synapse
		/// </summary>
		public const double SAMENESS_TO_PREMOTOR_GAIN = 1.0;
		#endregion

        #region Sameness-to-First

        /// <summary>
        /// Synaptic weight of the sameness to first synapses
        /// </summary>
        public const double SAMENESS_TO_FIRST_W = 0.1;
        
        #endregion

        #region Morris-Lecar to Sameneness
        /// <summary>
        /// Synaptic weight of the first-to-second-layer synapses
        /// </summary>
        public const double MORRIS_TO_SAMENESS_W = 0.1;

        /// <summary>
        /// Value in steps of simulation of first-to-second-layer synaptic delay
        /// </summary>
        public const int MORRIS_TO_SAMENESS_DELAY_STEP = 0;

        /// <summary>
        /// Time constant of a first-to-second-layer synapse
        /// </summary>
        public const double MORRIS_TO_SAMENESS_TAU = 50;

        /// <summary>
        ///decay of a liquid-to-sameness synapse
        /// </summary>
        public const double MORRIS_TO_SAMENESS_DECAY = 600;
        /// <summary>
        /// Time constant of a first-to-second-layer synapse
        /// </summary>
        public const double MORRIS_TO_SAMENESS_TAU1 = 5;

        // <summary>
        /// Time constant of a first-to-second-layer synapse
        /// </summary>
        public const double MORRIS_TO_SAMENESS_GAIN = 0;

        /// <summary>
        /// Gain in the calculation of the current for first-to-second-layer synapses
        /// </summary>
        public const double MORRIS_TO_SAMENESS_SYNAPTIC_GAIN = 1.0;
        #endregion

        #region STDP synapses
        /// <summary>
		/// Initial weight of STDP synapses
		/// </summary>
		public const double STDP_INIT_W = 0.05;

        /// <summary>
        /// Initial weight of STDP synapses
        /// </summary>
        public const double STDP_CONTEXT_END_INIT_W = 0.0;

		/// <summary>
		/// Positive time constant for STDP synapses
		/// </summary>
		public const double STDP_TAU_P = 20; //0.2;

		/// <summary>
		/// Negative time constant for STDP synapses
		/// </summary>
        public const double STDP_TAU_N = 20;  //0.2;

		/// <summary>
		/// "Memory" time constant for STDP synapses
		/// </summary>
		public const double STDP_TAU_OLD = 3.0;

		/// <summary>
		/// Positive gain for STDP synapses
		/// </summary>
        public const double STDP_A_P = 0.005;//0.000075;  //0.1 * 0.25;

		/// <summary>
		/// Negative gain for STDP synapses
		/// </summary>
        public const double STDP_A_N = 0.00505;//0.00007575; //0.1 * 0.25;

		/// <summary>
		/// "Memory" gain for STDP synapses
		/// </summary>
		public const double STDP_A_N_OLD = 7.5e-3 * 0.025;

		/// <summary>
		/// Lower weight limit for STDP synapses
		/// </summary>
		public const double STDP_W_LO = 0.05;

		/// <summary>
		/// Upper weight limit for STDP synapses
		/// </summary>
		public const double STDP_W_HI = 8.0;

		/// <summary>
		/// Multiplier of STDP weight for decay (e.g. 0.9 = 10% decay)
		/// </summary>
        public const double STDP_DECAY = 1;//0.9;

        /// <summary>
        /// Multiplier of STDP Expectation  weight for decay (e.g. 0.7 = 30% decay)
        /// </summary>
        public const double STDP_DECAY_EXPECTATION = 0.7;

		/// <summary>
		/// Value in steps of simulation of STDP synaptic delay
		/// </summary>
		public const int STDP_DELAY_STEP = 0;

		/// <summary>
		/// Gain in the calculation of the current for STDP synapse
		/// </summary>
		public const double STDP_GAIN = 10;

        /// <summary>
        /// Gain in the calculation of the current for STDP synapse 
        /// in the feedback connection between the Second to First Layer
        /// </summary>
        public const double STDP_EXPECTATION_GAIN = 1.0;    //0.8; // se = 20 la frequenza premotore=6250

		/// <summary>
		/// Time constant
		/// </summary>
		public const double STDP_TAU = 1.0;

        /// <summary>
        /// Time constant for expectation's STDP synapse
        /// </summary>
        public const double STDP_TAU_EXPECTATION = 1.0;

		/// <summary>
		/// Common multiplication factor of the gains 
		/// for sameness-to-premotor synapse
		/// </summary>
		public const double STDP_SAMENESS_MULT_FACTOR = 4;

		/// <summary>
		/// Common multiplication factor of the gains 
		/// for SOSL-to-premotor synapse
		/// </summary>
		public const double STDP_SOSL_MULT_FACTOR = 1;

		/// <summary>
		/// Multiplication factor of current gain for
		/// sameness-to-premotor synapse
		/// </summary>
		public const double STDP_SAMENESS_GAIN = 3;

		/// <summary>
		/// Multiplication factor of current gain for
		/// SOSL-to-premotor synapses
		/// </summary>
		public const double STDP_SOSL_GAIN = 0.5;
		#endregion

        #region First-to-Inhibitory
        /// <summary>
        /// Synaptic weight of the first-to-Inhibitory neuron synapses
        /// </summary>
        public const double FIRST_TO_INHIBITORY_W = 30.0;
        #endregion

        #region Second-to-Third synapses
        /// <summary>
        /// Synaptic weight of the second-to-third-layer synapses
        /// </summary>
        public const double SECOND_TO_THIRD_W = 10.0;       //30

        /// <summary>
        /// Value in steps of simulation of second-to-third-layer synaptic delay
        /// </summary>
        public const int SECOND_TO_THIRD_DELAY_STEP = 0;

        /// <summary>
        /// Time constant of a second-to-third-layer synapse
        /// </summary>
        public const double SECOND_TO_THIRD_TAU = 1.0;

        /// <summary>
        /// Gain in the calculation of the current for second-to-third-layer synapses
        /// </summary>
        public const double SECOND_TO_THIRD_SYNAPTIC_GAIN = 1.0;
        #endregion

        #region Third-to-third synapses
        /// <summary>
        /// Excitatory synaptic weight for third-to-third-layer synapses
        /// </summary>
        public const double THIRD_TO_THIRD_W_EXC = (7/2);       //7/2;

        /// <summary>
        /// Inhibitory synaptic weight for third-to-third-layer synapses
        /// </summary>
        public const double THIRD_TO_THIRD_W_INH = -(7/2);    //-7

        /// <summary>
        /// Excitatory synaptic weight for third-to-third-layer synapses outer the excitatory window
        /// </summary>
        public const double THIRD_TO_THIRD_WSEC = -20;       //7;

        /// <summary>
        /// Time constant of a third-to-third-layer
        /// </summary>
        public const double THIRD_TO_THIRD_TAU = 1;

        /// <summary>
        /// Time constant of a third-to-third-layer
        /// </summary>
        public const double THIRD_TO_THIRD_TAU_INH = 2;

        /// <summary>
        /// Value in steps of simulation of third-to-third-layer synaptic delay 
        /// </summary>
        public const int THIRD_TO_THIRD_DELAY_STEP = 0;

        /// <summary>
        /// Gain in the calculation of the current for third-to-third-layer synapses
        /// </summary>
        public const double THIRD_TO_THIRD_SYNAPTIC_GAIN = 10.0; //2

        public const double THIRD_TO_THIRD_SYNAPTIC_GAIN_END = 20.0;

        /// <summary>
        /// Gain in the calculation of the current for third-to-third-layer synapses
        /// </summary>
        public const double THIRD_TO_THIRD_SYNAPTIC_GAIN_STDP = 1; //2.0;

        /// <summary>
        /// Gain in the calculation of the current for third-to-third-layer synapses
        /// </summary>
        public const double THIRD_TO_END_SYNAPTIC_GAIN_STDP = 2; //2;
        #endregion

        #region Third-to-Second
        /// <summary>
        /// Excitatory synaptic weight for third-to-second-layer synapses
        /// </summary>
        public const double THIRD_TO_SECOND_W = 10;      
        #endregion

        #region Liquid-to-Output
        /// <summary>
        /// Synaptic weight of the Liquid-layer to output neurons synapses
        /// </summary>
        public const double LIQUID_TO_OUT_W = 1.0;       //30

        /// <summary>
        /// Value in steps of simulation of Liquid-layer to output neurons synaptic delay
        /// </summary>
        public const int LIQUID_TO_OUT_DELAY_STEP = 0;

        /// <summary>
        /// Time constant of a Liquid-layer to output neurons synapse
        /// </summary>
        public const double LIQUID_TO_OUT_THIRD_TAU = 1.0;

        /// <summary>
        /// Gain in the calculation of the current for Liquid-layer to output neurons synapses
        /// </summary>
        public const double LIQUID_TO_OUT_SYNAPTIC_GAIN = 1.0;

        /// <summary>
        /// Parameter for the updating of the weight for Liquid-layer to output neurons synapses
        /// </summary>
        public const double LIQUID_TO_OUT_NI = 0.0001;//0.0001; //-->a epoche      //0.00001;//--->per 5Hz
        /// <summary>
        /// Threshold for the updating of the weight for Liquid-layer to output neurons synapses
        /// </summary>
        public const double LIQUID_TO_OUT_EPS = 0.0000000005;

        /// <summary>
        /// Selected target for Liquid-layer to output neurons synapses
        /// </summary>
        public const int LIQUID_TO_OUT_TARGET = 8;//0;

        /// <summary>
        /// Selected option for the updating rules of the Liquid-layer to output neurons synapses
        /// case 0: // campioni classico				 
        /// case 1: // campioni EPOCHE		
        /// case 2: // campioni segno d'errore
        /// case 3 : // pseudo inversa
        /// case 4: // segno d'errore EPOCHE
        /// </summary>
        public const int LIQUID_TO_OUT_OPTION = 0;

        /// <summary>
        /// Selected error threshold for Liquid-layer to output neurons synapses
        /// </summary>
        public const double LIQUID_TO_OUT_THRESHOLD = 0.0006;

        /// <summary>
        /// Set the constant for the calculation of the pseudoinverse from file
        /// </summary>
        public const bool LIQUID_TO_OUT_PSEUDOINV_FILE = false;

        #endregion
        
          #region Liquid-to-Liquid
        /// <summary>
        /// Synaptic weight of the Liquid-to-liquid layer synapses
        /// </summary>
        public const double LIQUID_TO_LIQUID_W = 1.0;       //30

        /// <summary>
        /// Value in steps of simulation of Liquid-to-liquid layer synaptic delay
        /// </summary>
        public const int LIQUID_TO_LIQUID_DELAY_STEP = 0;

        /// <summary>
        /// Time constant of a Liquid-to-liquid layer synapse
        /// </summary>
        public const double LIQUID_TO_LIQUID_TAU = 1.0;

        /// <summary>
        /// Gain in the calculation of the current for Liquid-to-liquid layer synapses
        /// </summary>
        public const double LIQUID_TO_LIQUID_SYNAPTIC_GAIN = 1.0;

        /// <summary>
        /// Probability for the internal synapses in the Liquid layer
        /// </summary>
        public const double LIQUID_E_I = 0.20;       //0.1

        /// <summary>
        /// Connection for the internal synapses in the Liquid layer
        /// </summary>
        public const int LIQUID_CONNECTION = 2;

        /// <summary>
        /// Weight for the connection of the internal synapses in the Liquid layer
        /// </summary>
        public const double LIQUID_WEIGHT = 0.25;

        /// <summary>
        /// Input Ratio for the input to the the Liquid layer
        /// </summary>
        public const double LIQUID_INPUT_RATIO = 0.50;          //0.15

        /// <summary>
        /// Input weight for the the Liquid layer
        /// </summary>
        public const double LIQUID_INPUT_WEIGHT = 1;
        
        /// <summary>
        /// Output Neuron for the the Liquid layer
        /// </summary>
        public const int LIQUID_OUTPUT = 1;

        /// <summary>
        /// Target for the learning in the the Liquid layer
        /// </summary>
        public const int LIQUID_TARGET = 0;

        /// <summary>
        /// Number of Spikes of the morris lecar for having the winner object
        /// </summary>
        public const int MORRIS_WINNER_SPIKE = 3;

        #endregion

        #region Context-to-Morris
        /// <summary>
        /// Gain in the calculation of the current for context-to-morris synapses
        /// </summary>
        public const double CONTEXT_TO_MORRIS_GAIN_STDP = 2.0;

        /// <summary>
        /// Gain in the calculation of the current for context-to-morris synapses
        /// </summary>
        public const double CONTEXT_TO_MORRIS_GAIN = 10.0;

        public const double CONTEXT_TO_MORRIS_Weight = 0;

        #endregion

        #region Motor-to-Motor
        /// <summary>
        /// Gain in the calculation of the current for context-to-morris synapses
        /// </summary>
        public const double MOTOR_TO_MOTOR_W_INH = -7/3;

        /// <summary>
        /// Gain in the calculation of the current for context-to-morris synapses
        /// </summary>
        public const double MOTOR_TO_MOTOR_TAU_INH = 2;

        public const int MOTOR_TO_MOTOR_DELAY_STEP = 0;

        public const double MOTOR_TO_MOTOR_SYNAPTIC_GAIN = 10.0;

        #endregion

        #region Morris-to-Motor
        public const double MORRIS_TO_MOTOR_SYNAPTIC_GAIN_STDP = 2; //2;

        public const double MORRIS_TO_MOTOR_SYNAPTIC_GAIN = 14.0;

        public const double MORRIS_MOTOR_INIT_W_STDP= 0.0;

        public const double MORRIS_MOTOR_MAX_W_STDP = 8.0;

        public const double MORRIS_TO_MOTOR_TAU = 15;

        #endregion

        #region Morris-to-Contex
        /// <summary>
        /// Gain in the calculation of the current for morris-to-contex synapses
        /// </summary>
        public const double MORRIS_TO_CONTEX_W_EXC = (7/2);

        /// <summary>
        /// Time constant of a third-to-third-layer
        /// </summary>
        public const double MORRIS_TO_CONTEX_TAU = 15;

        #endregion



        #region Context-to_End

        /// <summary>
        /// Threshold of the end sequence neuron
        /// </summary>
        public const double THRESHOLD_END_SEQUENCE = 12.5;

        #endregion

        #region Context-to-Motor

        public const double CONTEXT_TO_MOTOR_SYNAPTIC_GAIN_STDP = 2; //2;

        public const double CONTEXT_TO_MOTOR_SYNAPTIC_GAIN = 4.0;

        public const double STDP_CONTEXT_MOTOR_INIT_W = 0.0;

        /// <summary>
        /// Threshold of the end sequence neuron
        /// </summary>
        //public const double THRESHOLD_END_SEQUENCE = 12.5;

        #endregion


        /// <summary>
        /// Level of noise in the sinaptic weight
        /// </summary>
        public static double NOISE_LVL = 0;

        #endregion

        #region Input currents
        /// <summary>
		/// Value of the input current of the first layer neurons
		/// </summary>
		public const double INPUT_CURRENT = 40.0;

		/// <summary>
		/// Value of the feedback current of the first layer neurons
		/// </summary>
		public const double FEEDBACK_CURRENT = 10;

		/// <summary>
		/// Input current of the reward neuron when reward is <b>on</b>
		/// </summary>
		public const double I_REWARD_ON = 100.0;

		/// <summary>
		/// Input current of the reward neuron when reward is <b>off</b>
		/// </summary>
		public const double I_REWARD_OFF = 0.0;

		/// <summary>
		/// Input current of the sameness neuron when there is sameness
		/// </summary>
		public const double I_SAMENESS_ON = 100.0;

		/// <summary>
		/// Input current of the sameness neuron when there is <i>no</i> sameness
		/// </summary>
		public const double I_SAMENESS_OFF = 0.0;
		#endregion

		#region Frequencies
		/// <summary>
		/// Frequency threshold for the synaptic loop between the two SOSL
		/// </summary>
		public const int FREQUENCY_LOOP_THRESHOLD = 2000;

        /// <summary>
        /// Frequency threshold for detecting the end of the sequence between the SOSL#3 and the end sequence neuron synapses
        /// </summary>
        public const int FREQUENCY_END_SEQUENCE_THRESHOLD = 500;

        /// <summary>
        /// Frequency threshold for detecting the end of the sequence between the First Layer1 and the end sequence neuron synapses
        /// </summary>
        public const int FREQUENCY_FIRST_END_SEQUENCE_THRESHOLD = 150;

		/// <summary>
		/// Size of the window (in integration steps) used for frequency estimation
		/// </summary>
		public const int FREQUENCY_WINDOW_SIZE = 250;

		/// <summary>
		/// Duration of the time window (in ms) used for frequency estimation
		/// </summary>
		public static readonly double FREQUENCY_WINDOW_TIME = INTEGRATION_STEP * FREQUENCY_WINDOW_SIZE;

        /// <summary>
        /// Frequency threshold for the Oja's rule applied in the input layer
        /// </summary>
        public const int FREQUENCY_THRESHOLD_FIRST_LAYER = 2000;

        /// <summary>
        /// Frequency threshold for the Oja's rule applied in the SOSL#1
        /// </summary>
        public const int FREQUENCY_THRESHOLD_SECOND_LAYER = 10000;

		#endregion

        #region Noise
        /// <summary>
        /// Number of lvl of noise to test
        /// </summary>
        public const int NUM_PROVE = 1;

        /// <summary>
        /// Number of trials with a specifed lv of noise 
        /// </summary>
        public const int ITERATIONS = 1;

        /// <summary>
        /// Increment of noise between one trial to the other 0.1=+-5% 
        /// </summary>
        public const double INCREMENT = 0.1; //0.1;

        public const int SEQU_ELEMENTS = 3;


        #endregion
    }
}
