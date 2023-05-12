using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using CSML;
//using Matrix;
using Accord.Math;
using System.Threading.Tasks;

//revert commit

namespace SLN
{
    /// <summary>
    /// The network itself
    /// </summary>
    [Serializable]
    public class Network
    {
        private List<NetworkInput> countInput;
        private int simulationNumber;
        public int current_epoch;
        public int current_learning;
        public int primo_test = 0;

        // Lists of synapses
        private LinkedList<Synapse> _firstToFirst1;
        private LinkedList<Synapse> _firstToFirstSTDP;
        private LinkedList<Synapse> _firstToLiquid;
        private LinkedList<Synapse> _liquidToLiquid;
        private LinkedList<LinkedList<Synapse>> _liquidToOut;
        private LinkedList<LinkedList<SynapseProportional>> _samenessToFirst;

        // Layers
        private Layers _layers;
        private LiquidState _liquid;
        private OutputLayerExternal _outExt;



        //winner first
        Neuron[] winnerFirst;
        Neuron[] winnerFirstA;




        public StreamWriter file; //file in cui è salvato il network (vedere Network())
        public StreamWriter input_connection; //in cui sono salvate le connessioni del network


        //Current input
        protected NetworkInput currentInput;
        protected NetworkInput currentInputOld;
        private int indexWinOut;

        //random's seed
        private Random rand;

        //Path Pc
        String pathPc;

        //link input Morris-Lecar
        public int[,] MLtoInput;

        /// <summary>
        /// Constructor (with configuration saved to file)
        /// </summary>
        /// <param name="synSav">The synapse configuration saver object</param>
        private Network(SynapseSaver synSav)
        {
            pathPc = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            file = new StreamWriter(pathPc + @"\Dati\EI.txt");
            input_connection = new StreamWriter(pathPc + @"\Dati\Input_connection.txt");
            this.init();
            this.initSynapseLayers(synSav);

        }


        /// <summary>
        /// Constructor
        /// </summary>
        private Network()
            : this((SynapseSaver)null)
        { }

        /// <summary>
        /// Initializes all the class members
        /// </summary>
        private void init()
        {
            current_epoch = 0;
            current_learning = -1;
            _layers = new Layers();
            _liquid = new LiquidState();
            _outExt = new OutputLayerExternal();

            _firstToFirst1 = new LinkedList<Synapse>();
            _firstToFirstSTDP = new LinkedList<Synapse>();
            _liquidToLiquid = new LinkedList<Synapse>();
            _firstToLiquid = new LinkedList<Synapse>();
            _liquidToOut = new LinkedList<LinkedList<Synapse>>();
            _samenessToFirst = new LinkedList<LinkedList<SynapseProportional>>();



            rand = new Random(10000);
            simulationNumber = 0;
        }


        /// <summary>
        /// Initializes the first-to-Liquid-layer synapses, saving the configuration
        /// </summary>
        /// <param name="synSav">The configuration saver object</param>
        private void initFirstToLiquid(SynapseSaver synSav)
        {
            Random rnd = new Random();

            for (int i1 = 0; i1 < Constants.FIRST_LAYER_DIMENSION_I; i1++)
            {
                for (int j1 = 0; j1 < Constants.FIRST_LAYER_DIMENSION_J; j1++)
                {
                    Neuron start1 = _layers.getFirstLayerNeuron1(i1, j1);

                    for (int i2 = 0; i2 < Constants.LIQUID_DIMENSION_I; i2++)
                    {
                        for (int j2 = 0; j2 < Constants.LIQUID_DIMENSION_J; j2++)
                        {
                            Neuron dest1 = _liquid.getLiquidLayerNeuron(i2, j2);

                            #region First-to-Liquid-layer synapses #1
                            double p = rand.NextDouble();
                            if (p < Constants.FIRST_TO_SECOND_P)
                            {
                                Synapse syn = new Synapse(start1,
                                    dest1,
                                    Constants.FIRST_TO_SECOND_W * (1 + (rnd.NextDouble() - 0.5) * 0.0),
                                    Constants.FIRST_TO_SECOND_TAU,
                                    Constants.FIRST_TO_SECOND_DELAY_STEP,
                                    Constants.FIRST_TO_SECOND_SYNAPTIC_GAIN);
                                _firstToLiquid.AddLast(syn);

                                input_connection.WriteLine((dest1.ROW + 1) + " " + (dest1.COLUMN + 1));
                            }

                            input_connection.Flush();
                            #endregion
                        }
                    }
                }
            }

            input_connection.Close();

            // Saving configuration...
            if (synSav != null)
            {
                synSav.saveSynapseConfig(_firstToLiquid);
            }
        }



        /// <summary>
        /// Initializes the first-to-Liquid-layer synapses, loading the configuration
        /// from file
        /// </summary>
        /// <param name="configPath">The path of the configuration file</param>
        private void initFirstToLiquid(string configPath)
        {
            using (StreamReader sr = new StreamReader(configPath))
            {
                String str = sr.ReadLine();

                while (str != null)
                {
                    string[] coords = str.Split(new char[] { '\t', ' ' });

                    int layer = int.Parse(coords[0]);
                    int stRow = int.Parse(coords[1]);
                    int stCol = int.Parse(coords[2]);
                    int deRow = int.Parse(coords[3]);
                    int deCol = int.Parse(coords[4]);

                    Neuron start;
                    Neuron dest;

                    if (layer == (int)LayerNumbers.FirstLayer_1)
                    {
                        start = _layers.getFirstLayerNeuron1(stRow, stCol);
                        dest = _liquid.getLiquidLayerNeuron(deRow, deCol);
                        Synapse syn = new Synapse(start,
                                    dest,
                                    Constants.FIRST_TO_SECOND_W,
                                    Constants.FIRST_TO_SECOND_TAU,
                                    Constants.FIRST_TO_SECOND_DELAY_STEP,
                                    Constants.FIRST_TO_SECOND_SYNAPTIC_GAIN);
                        _firstToLiquid.AddLast(syn);
                    }

                    str = sr.ReadLine();
                }
            }
        }



        /// <summary>
        /// Initializes the first-to-first-layer (to SOSL #1) synapses
        /// </summary>
        private void initFirstToFirst1()
        {
            Random rnd = new Random();
            for (int i1 = 0; i1 < Constants.FIRST_LAYER_DIMENSION_I; i1++) // Feature
                for (int j1 = 0; j1 < Constants.FIRST_LAYER_DIMENSION_J; j1++) // Value
                {
                    Neuron start = _layers.getFirstLayerNeuron1(i1, j1);
                    for (int i2 = 0; i2 < Constants.FIRST_LAYER_DIMENSION_I; i2++)
                        for (int j2 = 0; j2 < Constants.FIRST_LAYER_DIMENSION_J; j2++)
                        {
                            double w;
                            Neuron dest = _layers.getFirstLayerNeuron1(i2, j2);

                            /*
							if (i1 == i2 && j1 == j2) 
								w = 0; // No autosynapse
							//w = Constants.FIRST_TO_FIRST_W_EXC;
                            else*/
                            if (i1 == i2) // Same feature
                                w = Constants.FIRST_TO_FIRST_W_INH * (1 + (rnd.NextDouble() - 0.5) * Constants.NOISE_LVL);
                            else
                            {
                                w = Constants.FIRST_TO_FIRST_W_EXC;
                                Synapse synStdp = new SynapseSTDP(start,
                                dest,
                                Constants.FIRST_TO_FIRST_SYNAPTIC_GAIN_STDP,
                                Constants.STDP_GAIN - 5,
                                Constants.FIRST_TO_FIRST_STDP_W_HI - 4,
                                Constants.FIRST_TO_FIRST_STDP_W_LO);
                                _firstToFirstSTDP.AddLast(synStdp);
                            }

                            Synapse syn = new Synapse(start, dest, w,
                                Constants.FIRST_TO_FIRST_TAU,
                                Constants.FIRST_TO_FIRST_DELAY_STEP,
                                Constants.FIRST_TO_FIRST_SYNAPTIC_GAIN);

                            //potremmo non aggiungere la syn se il neurone start è anche il dest
                            if (i1 != i2 || j1 != j2)
                                _firstToFirst1.AddLast(syn);
                        }
                }
        }


        /// <summary>
        /// Initializes the Liquid-To-Liquid-layer synapses
        /// </summary>
        private void initLiquidToLiquid()
        {
            double[,] W;
            W = new double[Constants.LIQUID_DIMENSION_I * Constants.LIQUID_DIMENSION_J, Constants.LIQUID_DIMENSION_I * Constants.LIQUID_DIMENSION_J];

            // determinazione eccitatori-inibitori
            bool[] ei = new bool[Constants.LIQUID_DIMENSION_I * Constants.LIQUID_DIMENSION_J];

            for (int i = 0; i < Constants.LIQUID_DIMENSION_I * Constants.LIQUID_DIMENSION_J; i++)
            {

                double EI = (((rand.Next(100))) / 100.0); // eccitatorio/inibitorio
                if (EI < Constants.LIQUID_E_I)  // caso inibitorio
                {
                    ei[i] = false;
                    file.WriteLine(i + "\t" + 0);
                }
                else		 //  caso eccitatorio
                {
                    ei[i] = true;
                    file.WriteLine(i + "\t" + 1);
                }
                file.Flush();
            }

            file.Close();


            for (int i1 = 0; i1 < Constants.LIQUID_DIMENSION_I; i1++)
                for (int j1 = 0; j1 < Constants.LIQUID_DIMENSION_J; j1++)
                {
                    Neuron start = _liquid.getLiquidLayerNeuron(i1, j1);
                    for (int i2 = 0; i2 < Constants.LIQUID_DIMENSION_I; i2++)
                        for (int j2 = 0; j2 < Constants.LIQUID_DIMENSION_J; j2++)
                        {

                            Neuron dest = _liquid.getLiquidLayerNeuron(i2, j2);

                            double deltaX = i1 - i2, deltaY = j1 - j2;
                            if (Math.Abs(i1 - i2) > Constants.LIQUID_DIMENSION_I / 2)
                                deltaX = Math.Abs(i1 - i2) - Constants.LIQUID_DIMENSION_I;
                            if (Math.Abs(j1 - j2) > Constants.LIQUID_DIMENSION_J / 2)
                                deltaY = Math.Abs(j1 - j2) - Constants.LIQUID_DIMENSION_J;

                            double distance = Math.Sqrt(Math.Pow(deltaX, 2) + Math.Pow(deltaY, 2));
                            double C = 0;
                            double prob = 0;

                            if ((ei[i1 * Constants.LIQUID_DIMENSION_I + j1]) && (ei[i2 * Constants.LIQUID_DIMENSION_I + j2])) // EE
                                C = 0.3;
                            if ((ei[i1 * Constants.LIQUID_DIMENSION_I + j1]) && (!(ei[i2 * Constants.LIQUID_DIMENSION_I + j2]))) // EI
                                C = 0.2;
                            if ((!(ei[i1 * Constants.LIQUID_DIMENSION_I + j1])) && (ei[i2 * Constants.LIQUID_DIMENSION_I + j2])) // IE
                                C = 0.4;
                            if (!((ei[i1 * Constants.LIQUID_DIMENSION_I + j1])) && (!(ei[i2 * Constants.LIQUID_DIMENSION_I + j2]))) // II
                                C = 0.1;
                            C = C * 2;

                            switch (Constants.LIQUID_CONNECTION)
                            {
                                case 1:
                                    if (distance <= 1)
                                        prob = 1 * C;
                                    else
                                        prob = 0;
                                    break;
                                case 2:
                                    if (distance <= 1)
                                        prob = 1 * C;
                                    if ((distance > 1) && (distance <= 2))
                                        prob = 0.5 * C;
                                    if (distance > 2)
                                        prob = 0;
                                    break;
                                case 3:
                                    break;
                                case 4:
                                    break;
                                case 5:
                                    break;
                            }

                            if (distance <= 1)
                                prob = 1 * C;
                            if ((distance > 1) && (distance <= 2))
                                prob = 0.5 * C;
                            if (distance > 2)
                                prob = 0;

                            double weightL = ((((rand.Next(100))) / 100.0) + 0) / 2;     //prima era +1

                            double p = (((rand.Next() % 100)) / 100.0); // eccitatorio/inibitorio
                            if (p < prob)
                                if (ei[i1 * Constants.LIQUID_DIMENSION_I + j1])  // se il neurone di interesse è eccitatorio
                                    W[i1 * Constants.LIQUID_DIMENSION_I + j1, i2 * Constants.LIQUID_DIMENSION_I + j2] = weightL;
                                else       // se il neurone di interesse è inibitorio
                                    W[i1 * Constants.LIQUID_DIMENSION_I + j1, i2 * Constants.LIQUID_DIMENSION_I + j2] = -weightL;
                            else
                                W[i1 * Constants.LIQUID_DIMENSION_I + j1, i2 * Constants.LIQUID_DIMENSION_I + j2] = 0;


                            double[] value = new double[4];
                            value[0] = 5 / 2;
                            value[1] = 10 / 2;
                            value[2] = 30 / 2;
                            value[3] = 50 / 2;
                            double tau = 1;

                            double wheel = (((rand.Next(100))) / 100.0);
                            if (wheel < 0.25)
                                tau = value[0];
                            if ((wheel >= 0.25) && (wheel < 0.5))
                                tau = value[1];
                            if ((wheel >= 0.5) && (wheel < 0.75))
                                tau = value[2];
                            if (wheel >= 0.75)
                                tau = value[3];



                            Synapse syn = new Synapse(start, dest, W[i1 * Constants.LIQUID_DIMENSION_I + j1, i2 * Constants.LIQUID_DIMENSION_I + j2],
                                tau,
                                Constants.LIQUID_TO_LIQUID_DELAY_STEP,
                                Constants.LIQUID_TO_LIQUID_SYNAPTIC_GAIN);

                            _liquidToLiquid.AddLast(syn);
                        }
                }
        }



        /// <summary>
        /// Set the Liquid-to-Output synapses' weights
        /// </summary>
        private void setWLiquidToOutput(string configpath, int target)
        {
            if (Constants.LIQUID_TO_OUT_PSEUDOINV_FILE)
            {
                StreamReader sr = new StreamReader(configpath);

                foreach (LinkedList<Synapse> s1 in _liquidToOut)
                    foreach (SynapseSTDP s2 in s1)
                    {
                        if (s2.Dest.COLUMN == target)
                        {
                            String str = sr.ReadLine();

                            if (str != null)
                            {
                                str = str.Replace('.', ',');
                                double w = double.Parse(str);
                                s2.W = w;
                            }
                        }
                    }
            }
            else
            {

                if (_liquid.Option == 3)
                {
                    //PSEUDO-INVERSA SENZA RUMORE
                    double[,] Z = new double[Constants.LIQUID_DIMENSION_I * Constants.LIQUID_DIMENSION_J, Constants.SIMULATION_STEPS_LIQUID];
                    String path = "Neurons" + simulationNumber + ".txt";
                    StreamReader sr = new StreamReader(pathPc + @"\Dati\" + path);

                    //heading
                    String str = sr.ReadLine();
                    str = sr.ReadLine();
                    //first reading
                    str = sr.ReadLine();


                    while (str != null)
                    {
                        string[] coords = str.Split(new char[] { '\t', ' ' });

                        int step = int.Parse(coords[0]);
                        int layer = int.Parse(coords[1]);
                        int Row = int.Parse(coords[2]);
                        int Col = int.Parse(coords[3]);
                        coords[8] = coords[8].Replace('.', ',');
                        double iOut = double.Parse(coords[8]);

                        if (layer == (int)LayerNumbers.LIQUID_STATE && step >= Constants.SIMULATION_STEPS_FEEDFORWARD)
                        {
                            Z[Row * Constants.LIQUID_DIMENSION_I + Col, step - Constants.SIMULATION_STEPS_FEEDFORWARD] = iOut;
                        }


                        str = sr.ReadLine();
                        if (str[0] == '#')
                            break;
                    }

                    var Ztraspost = Z.Transpose();
                    var Zinverse = Ztraspost.PseudoInverse();

                    double[] _target = new double[Constants.SIMULATION_STEPS_LIQUID];

                    for (int i = 0; i < Constants.SIMULATION_STEPS_LIQUID; i++)
                        _target[i] = (double)Math.Sin(i * 2 * Math.PI * (Constants.start_freq + target * Constants.incr_freq) * 0.001 / Constants.INTEGRATION_STEP_MORRIS_LECAR) / 100;

                    double[] W;

                    W = Zinverse.Multiply(_target);

                    int ind = 0;
                    foreach (LinkedList<Synapse> s1 in _liquidToOut)
                        foreach (SynapseSTDP s2 in s1)
                        {
                            if (s2.Dest.COLUMN == target)
                            {
                                s2.W = W[ind];
                                ind++;
                            }
                        }


                }

                if (_liquid.Option == 5)
                {

                    //NEW VERSION - PSEUDO-INVERSA CON RUMORE
                    double[,] Z = new double[64, 3 * Constants.SIMULATION_STEPS_LIQUID];

                    String path = "Neurons" + simulationNumber + ".txt";
                    StreamReader sr = new StreamReader(pathPc + @"\Dati\" + path);
                    String path1 = "Neurons" + simulationNumber + "-0.txt";
                    StreamReader sr1 = new StreamReader(pathPc + @"\Dati\" + path);
                    String path2 = "Neurons" + simulationNumber + "-1.txt";
                    StreamReader sr2 = new StreamReader(pathPc + @"\Dati\" + path);

                    //heading
                    String str = sr.ReadLine();
                    str = sr.ReadLine();
                    //first reading
                    str = sr.ReadLine();


                    while (str != null)
                    {
                        string[] coords = str.Split(new char[] { '\t', ' ' });

                        int step = int.Parse(coords[0]);
                        int layer = int.Parse(coords[1]);
                        int Row = int.Parse(coords[2]);
                        int Col = int.Parse(coords[3]);
                        coords[8] = coords[8].Replace('.', ',');
                        double iOut = double.Parse(coords[8]);

                        if (layer == (int)LayerNumbers.LIQUID_STATE && step >= Constants.SIMULATION_STEPS_FEEDFORWARD)
                        {
                            Z[Row * Constants.LIQUID_DIMENSION_I + Col, step - Constants.SIMULATION_STEPS_FEEDFORWARD] = iOut;
                        }


                        str = sr.ReadLine();
                        if (str[0] == '#')
                            break;
                    }


                    //A partire da questo momento leggo il secondo file ovvero quello con la feature con il rumore additivo
                    //heading
                    str = sr1.ReadLine();
                    str = sr1.ReadLine();
                    //first reading
                    str = sr1.ReadLine();


                    while (str != null)
                    {
                        string[] coords = str.Split(new char[] { '\t', ' ' });

                        int step = int.Parse(coords[0]);
                        int layer = int.Parse(coords[1]);
                        int Row = int.Parse(coords[2]);
                        int Col = int.Parse(coords[3]);
                        coords[8] = coords[8].Replace('.', ',');
                        double iOut = double.Parse(coords[8]);

                        if (layer == (int)LayerNumbers.LIQUID_STATE && step >= Constants.SIMULATION_STEPS_FEEDFORWARD)
                        {
                            Z[Row * Constants.LIQUID_DIMENSION_I + Col, step - Constants.SIMULATION_STEPS_FEEDFORWARD + Constants.SIMULATION_STEPS_LIQUID] = iOut;
                        }


                        str = sr1.ReadLine();
                        if (str[0] == '#')
                            break;
                    }

                    //A partire da questo momento leggo il secondo file ovvero quello con la feature con il rumore sottrattivo
                    //heading
                    str = sr2.ReadLine();
                    str = sr2.ReadLine();
                    //first reading
                    str = sr2.ReadLine();


                    while (str != null)
                    {
                        string[] coords = str.Split(new char[] { '\t', ' ' });

                        int step = int.Parse(coords[0]);
                        int layer = int.Parse(coords[1]);
                        int Row = int.Parse(coords[2]);
                        int Col = int.Parse(coords[3]);
                        coords[8] = coords[8].Replace('.', ',');
                        double iOut = double.Parse(coords[8]);

                        if (layer == (int)LayerNumbers.LIQUID_STATE && step >= Constants.SIMULATION_STEPS_FEEDFORWARD)
                        {
                            Z[Row * Constants.LIQUID_DIMENSION_I + Col, step - Constants.SIMULATION_STEPS_FEEDFORWARD + 2 * Constants.SIMULATION_STEPS_LIQUID] = iOut;
                        }


                        str = sr2.ReadLine();
                        if (str[0] == '#')
                            break;
                    }

                    Z = Z.Round(4);
                    var Ztraspost = Z.Transpose();
                    var Zinverse = Ztraspost.PseudoInverse();


                    //W=Zinverse*target
                    double[] _target = new double[3 * Constants.SIMULATION_STEPS_LIQUID];
                    //String pathTarget = "target" + simulationNumber + ".txt";
                    String pathTarget = "target_default_" + target + ".txt";
                    StreamReader srt = new StreamReader(pathTarget);
                    int index = -1;
                    str = srt.ReadLine();

                    while (str != null)
                    {
                        //string[] coords = str.Split(new char[] { '\t', ' ' });
                        //coords[0] = coords[0].Replace('.', ',');
                        str = str.Replace('.', ',');
                        index++;

                        //double valueTarget = double.Parse(coords[0]);
                        double valueTarget = double.Parse(str);
                        _target[index] = valueTarget;

                        str = srt.ReadLine();
                    }

                    double[] W = new double[Constants.LIQUID_DIMENSION_I * Constants.LIQUID_DIMENSION_J];

                    W = Zinverse.Multiply(_target);

                    int ind = 0;
                    foreach (LinkedList<Synapse> s1 in _liquidToOut)
                        foreach (SynapseSTDP s2 in s1)
                        {
                            if (s2.Dest.COLUMN == target)
                            {
                                s2.W = W[ind];
                                ind++;
                            }
                        }


                }



            }

        }


        /// <summary>
        /// Initializes the synapses between layers, saving the configuration
        /// of first-to-second synapses
        /// </summary>
        /// <param name="synSav">The configuration saver object</param>
        private void initSynapseLayers(SynapseSaver synSav)
        {
            initFirstToFirst1();
            initFirstToLiquid(synSav);
            initLiquidToLiquid();
        }


        /// <summary>
        /// Sets the inputs of the network all features, plus the reward condition.
        /// Should be used only once in a simulation
        /// </summary>
        /// <param name="input">The input of the network</param>
        public void setInput(NetworkInput input)
        {
            countInput.Add(input);

            currentInput = input;
            _layers.setInput(input);
        }




        /// <summary>
        /// Resets the inputs of the network
        /// </summary>
        public void resetInputs()
        {
            _layers.resetInputs();
        }




        /// <summary>
        /// Simulates the entire network with the Liquid State
        /// </summary>
        /// <param name="log">The logger object</param>
        /// <param name="simNumber">Number of the simulation</param>
        /// <param name="learning">If <i>true</i> sets the learning on</param>
        //ritorna un intero, non un double
        public double simulateLiquid(StateLogger log, StateLogger logSTDP)
        {
            //resettiamo i log
            if (log != null)
                log.newIteration();
            if (logSTDP != null)
                logSTDP.newIteration();


            for (int step = 0; step < 100; step++)
            {
                if (log != null && (step % Constants.LOGGING_RATE) == 0)
                {
                    log.printLog();
                    log.newIteration();
                }
                if (logSTDP != null && (step % Constants.LOGGING_RATE) == 0)
                {
                    logSTDP.printLog();
                    logSTDP.newIteration();
                }

                _liquid.simulate(step, log);
            }

            if (log != null)
            {
                log.printLog();
                log.newIteration();
                log.closeLog();
            }
            if (logSTDP != null)
            {
                logSTDP.printLog();
                logSTDP.newIteration();
                logSTDP.closeLog();
            }



            _liquid.resetState();
            //_layers.resetNeuronsState();


            return 1;
        }







        /// <summary>
        /// Creates a new network without information
        /// </summary>
        /// <returns>A reference to the new <code>Network</code> object</returns>
        public static Network generateNetwork()
        {
            return new Network();
        }


    }
}
