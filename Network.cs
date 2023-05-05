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
        private bool _endSequenceReward;
        private int _levelReward;
        private int _motor;
        private bool _expCheck;
        private int[] feat;
        private List<NetworkInput> countInput;
        private int simulationNumber;
        private int simNumberInternal;
        private double frequencyRewardSequence = -12;

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
        private ContextLayer _context;


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
        /// Constructor (with configuration taken from file)
        /// </summary>
        /// <param name="configPath"></param>
        private Network(string configPath)
        {
            pathPc = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            this.init();
            this.initSynapseLayers(configPath);
            file = new StreamWriter(pathPc + @"\Dati\EI.txt");
            input_connection = new StreamWriter(pathPc + @"\Dati\Input_connection.txt");


        }

        /// <summary>
        /// Constructor (with configuration taken from files)
        /// </summary>
        /// <param name="configPath"></param>
        /// <param name="configStdp"></param>
        private Network(string configPath, string configStdp)
        {
            pathPc = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            this.init();
            this.initSynapseLayers(configPath, configStdp);
            file = new StreamWriter(pathPc + @"\Dati\EI.txt");
            input_connection = new StreamWriter(pathPc + @"\Dati\Input_connection.txt");


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
            _layers = new Layers();
            _liquid = new LiquidState();
            _outExt = new OutputLayerExternal();
            _context = new ContextLayer(_outExt);
            _context.addPath(_outExt.getNeuronMorris(0));

            _firstToFirst1 = new LinkedList<Synapse>();
            _firstToFirstSTDP = new LinkedList<Synapse>();
            _liquidToLiquid = new LinkedList<Synapse>();
            _firstToLiquid = new LinkedList<Synapse>();
            _liquidToOut = new LinkedList<LinkedList<Synapse>>();
            _samenessToFirst = new LinkedList<LinkedList<SynapseProportional>>();

            feat = new int[5];              //le prime 4 locazioni contengono le feature dell'elemento vincitore mentre la quinta l'indice del neurone motore vincitore


            rand = new Random(10000);


            _endSequenceReward = false;
            _levelReward = 1;
            _expCheck = false;
            _motor = -1;
            countInput = new List<NetworkInput>();

            simulationNumber = 0;
            simNumberInternal = -1;
            frequencyRewardSequence = 0;
            indexWinOut = -1;
            MLtoInput = new int[Constants.CLASSES, 4];
            for (int i = 0; i < Constants.CLASSES; i++)
                for (int j = 0; j < 4; j++)
                {
                    MLtoInput[i, j] = -1;
                }



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
        /// Initializes the Sameness to first layer synapses
        /// </summary>
        /// <param name="synSav">The configuration saver object</param>
        private void initSamenessToFirst(int indexSameness)
        {
            LinkedList<SynapseProportional> _samenessToFirst1 = new LinkedList<SynapseProportional>();
            SumNeuron start = _outExt.getNeuronPersistance(indexSameness);

            foreach (Neuron n in winnerFirst)
            {
                if (n != null)
                {
                    SynapseProportional syn = new SynapseProportional(start,
                        n,
                        Constants.SAMENESS_TO_FIRST_W);
                    _samenessToFirst1.AddLast(syn);
                }

            }

            _samenessToFirst.AddLast(_samenessToFirst1);

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


                    //W=Zinverse*target
                    double[] _target = new double[Constants.SIMULATION_STEPS_LIQUID];
                    //String pathTarget = "target" + simulationNumber + ".txt";
                    String pathTarget = "target_default_" + target + ".txt";
                    StreamReader sr2 = new StreamReader(pathTarget);
                    int index = -1;
                    str = sr2.ReadLine();

                    while (str != null)
                    {
                        //string[] coords = str.Split(new char[] { '\t', ' ' });
                        //coords[0] = coords[0].Replace('.', ',');
                        str = str.Replace('.', ',');
                        index++;

                        if (index < Constants.SIMULATION_STEPS_LIQUID)
                        {
                            //double valueTarget = double.Parse(coords[0]);
                            double valueTarget = double.Parse(str);
                            _target[index] = valueTarget;
                        }
                        else
                            break;

                        str = sr2.ReadLine();
                    }

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
        /// Initializes the Liquid-to-Output synapses
        /// </summary>
        private void initLiquidToOutput()
        {
            LinkedList<Synapse> _liquidToOut1 = new LinkedList<Synapse>();
            //Random randOut = new Random(10000); //
            Random rand1 = new Random(10000); //
            for (int i1 = 0; i1 < Constants.LIQUID_DIMENSION_I; i1++)
                for (int j1 = 0; j1 < Constants.LIQUID_DIMENSION_J; j1++)
                {
                    Neuron start = _liquid.getLiquidLayerNeuron(i1, j1);

                    double w = ((((rand.Next(100))) / 100.0) - 0.5) * 0.0001;  //* 0.0001;
                    //w = Math.Abs(w);

                    double[] value = new double[4];
                    value[0] = 5 / 2;
                    value[1] = 10 / 2;
                    value[2] = 30 / 2;
                    value[3] = 50 / 2;
                    double tau = 1;

                    double wheel = (((rand1.Next(100))) / 100.0);
                    if (wheel < 0.25)
                        tau = value[0];
                    if ((wheel >= 0.25) && (wheel < 0.5))
                        tau = value[1];
                    if ((wheel >= 0.5) && (wheel < 0.75))
                        tau = value[2];
                    if (wheel >= 0.75)
                        tau = value[3];


                    SumNeuron dest = _outExt.getOutNeuronFirst();

                    Synapse syn = new SynapseSTDP(tau,
                                start,
                                dest,
                                1, 1, w);


                    _liquidToOut1.AddLast(syn);

                }

            _liquidToOut.AddLast(_liquidToOut1);
        }

        /// <summary>
        /// Initializes the Liquid-to-Output synapses
        /// </summary>
        private void initLiquidToOutput(SumNeuron destNeuron)
        {
            LinkedList<Synapse> _liquidToOut1 = new LinkedList<Synapse>();
            Random rand1 = new Random(10000); //
            for (int i1 = 0; i1 < Constants.LIQUID_DIMENSION_I; i1++)
                for (int j1 = 0; j1 < Constants.LIQUID_DIMENSION_J; j1++)
                {
                    Neuron start = _liquid.getLiquidLayerNeuron(i1, j1);

                    double w = ((((rand.Next(100))) / 100.0) - 0.5) * 0.0001;
                    //w = Math.Abs(w);

                    double[] value = new double[4];
                    value[0] = 5 / 2;
                    value[1] = 10 / 2;
                    value[2] = 30 / 2;
                    value[3] = 50 / 2;
                    double tau = 1;

                    double wheel = (((rand1.Next(100))) / 100.0);
                    if (wheel < 0.25)
                        tau = value[0];
                    if ((wheel >= 0.25) && (wheel < 0.5))
                        tau = value[1];
                    if ((wheel >= 0.5) && (wheel < 0.75))
                        tau = value[2];
                    if (wheel >= 0.75)
                        tau = value[3];


                    SumNeuron dest = destNeuron;

                    Synapse syn = new SynapseSTDP(tau,
                                start,
                                dest,
                                1, 1, w);

                    _liquidToOut1.AddLast(syn);

                }

            _liquidToOut.AddLast(_liquidToOut1);
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
            initLiquidToOutput();
        }

        /// <summary>
        /// Initializes the synapses between layers, loading the configuration
        /// of first-to-second synapses from file
        /// <param name="configPath">The path of the configuration file</param>
        /// </summary>
        private void initSynapseLayers(string configPath)
        {
            initFirstToFirst1();
            initFirstToLiquid(configPath);
            initLiquidToLiquid();
            initLiquidToOutput();

        }

        /// <summary>
        /// Initializes the synapses between layers, loading the configuration
        /// of first-to-second synapses from file and for the stdp synapses
        /// <param name="configPath">The path of the configuration file</param>
        /// <param name="configStdp">The path of the configuration file for the Stdp</param>
        /// </summary>
        private void initSynapseLayers(string configPath, string configStdp)
        {
            initFirstToFirst1();
            initFirstToLiquid(configPath);
            initLiquidToLiquid();
            initLiquidToOutput();
        }

        /// <summary>
        /// Sets the inputs of the network all features, plus the reward condition.
        /// Should be used only once in a simulation
        /// </summary>
        /// <param name="input">The input of the network</param>
        public void setInput(NetworkInput input)
        {
            //questo if rappresenta il primo input della fase di test
            //nota: il simulation number non è stato ancora aggiornato, questo è
            //il motivo per cui si usa LEARNING_NUMBER -1.
            if ((simulationNumber == (Constants.LEARNING_NUMBER - 1) && simNumberInternal == -1) || (Constants.LEARNING_NUMBER == 0 && simulationNumber == 0))
            {
                countInput.Add(input);
            }
            //questo significa che quando vado a fare il primo test
            //tutti gli input dati prima della prima chiamata a testLiquid 
            //saranno contenuti in countInput.
            else countInput.Clear();
            // Console.WriteLine(countInput.Count);
            currentInput = input;
            _layers.setInput(input);
            //_liquid.setInput(input);
            _endSequenceReward = input.END;
            _levelReward = input.REWARDLEVEL;
            _motor = input.MOTOR;
        }


        /// <summary>
        /// Sets the inputs of the network all features, plus the reward condition.
        /// Should be used only once in a simulation
        /// </summary>
        /// <param name="input">The input of the network</param>
        public void setInput(NetworkInput input, int nLearn)
        {
            if ((simulationNumber == (nLearn - 1) && simNumberInternal == -1) || (nLearn == 0))
                countInput.Add(input);
            else countInput.Clear();

            currentInput = input;
            _layers.setInput(input);
            //_liquid.setInput(input);
            _endSequenceReward = input.END;
            _levelReward = input.REWARDLEVEL;
            _motor = input.MOTOR;
        }


        /// <summary>
        /// Sets the inputs of the network all features, plus the reward condition.
        /// Should be used only once in a simulation
        /// </summary>
        /// <param name="color">The value of the <b>COLOR</b> feature</param>
        /// <param name="size">The value of the <b>SIZE</b> feature</param>
        /// <param name="hdistr">The value of the <b>HORIZONTAL DISTRIBUTEDNESS</b> feature</param>
        /// <param name="vdistr">The value of the <b>VERTICAL DISTRIBUTEDNESS</b> feature</param>
        /// <param name="reward"><i>true</i> if there's a reward associated to this input, <i>false</i> otherwise</param>
        public void setInput(int color, int size, int hdistr, int vdistr, bool reward, bool end)
        {
            currentInput = new NetworkInput(color, size, hdistr, vdistr, reward, end);
            this.setInput(currentInput);
        }

        /// <summary>
        /// Resets the inputs of the network
        /// </summary>
        public void resetInputs()
        {
            _layers.resetInputs();
            //_liquid.resetInputs();
            _endSequenceReward = false;
        }




        /// <summary>
        /// Simulates the entire network with the Liquid State
        /// </summary>
        /// <param name="log">The logger object</param>
        /// <param name="simNumber">Number of the simulation</param>
        /// <param name="learning">If <i>true</i> sets the learning on</param>
        //ritorna un intero, non un double
        private double simulateLiquid(StateLogger log, StateLogger logSTDP, int simNumber, int simNumberInternal, bool learning, int signedTarget, bool test)
        {
            //inizialmente simNumberInternal = -1, Option = 0, signedtarget=0
            String targetS;

            if (simNumberInternal == -1)
                targetS = "target" + simNumber + ".txt";
            else
                targetS = "target" + simNumber + "-" + simNumberInternal + ".txt";
            StreamWriter fileTarget = new StreamWriter(pathPc + @"\Dati\" + targetS);
            double target = 0;
            double error = 0;
            double errorMean = 0;
            bool integration = false;

            //signedTarget = 1;

            //resettiamo i log
            if (log != null)
                log.newIteration();

            if (logSTDP != null)
                logSTDP.newIteration();


            //Caso di Pseudo-inversa Classica
            if (_liquid.Option == 3)
            {
                setWLiquidToOutput("W.txt", signedTarget);
                _liquid.Option = -1;
            }

            //Caso di Pseudo-Inversa con rumore
            if (_liquid.Option == 5 && simNumberInternal > 1)
            {
                setWLiquidToOutput("W.txt", signedTarget);
                //_liquid.Option = -1;
            }

            if (simNumberInternal == -1 || (_liquid.Option == 5 && simNumberInternal <= 2))      //io devo entrare sempre in questa if se sto eseguendo le simulazioni con input rumoroso per la pseudoinversa 
            {
                for (int step = 0; step < Constants.SIMULATION_STEPS_FEEDFORWARD; step++)
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

                    foreach (Synapse s in _firstToFirst1)
                    {
                        s.simulate(step);
                        if (log != null)
                            log.logSynapse(s, step);
                    }

                    foreach (Synapse s in _firstToFirstSTDP)
                    {
                        s.simulate(step);
                        if (log != null)
                            logSTDP.logSynapse(s, step);
                    }

                    _layers.simulateFirst1(step, log);

                    //non ci sono connessione sameness to first inizialmente
                    foreach (LinkedList<SynapseProportional> s1 in _samenessToFirst)
                        foreach (SynapseProportional s in s1)
                        {
                            s.simulate(step);
                        }

                }

                if (log != null)
                {
                    log.printLog();
                    log.newIteration();
                }
                if (logSTDP != null)
                {
                    logSTDP.printLog();
                    logSTDP.newIteration();
                }



                //lista contenente un neurone vincitore per ogni feature. Ci possono essere
                //valori non inizializzati.
                winnerFirst = _layers.getWinnerFirstActive(Constants.SIMULATION_STEPS_FEEDFORWARD);

                _liquid.resetActive();
                StreamWriter input_active = new StreamWriter(pathPc + @"\Dati\Input_active_" + simNumber + ".txt");



                if (winnerFirst != null) //se c'è almeno un neurone vincitore
                {
                    //in base all'attività dei neuroni si stabilisce se questa sia dovuta
                    //all'input corrente oppure all'input precedente. Sulla base di questo
                    //nel primo caso persistance sarà false, e nel secondo sarà true perché
                    //l'input è permasto.
                    bool persistance = false;

                    foreach (Neuron n in winnerFirst)
                        if (n != null)
                            //ricordiamo che la riga è la Feature, 
                            //la colonna è il valore che assume
                            switch (n.ROW)
                            {
                                case 0:
                                    if (n.COLUMN == currentInput.COLOR || currentInput.COLOR == -1)
                                        persistance = false;
                                    else persistance = true;
                                    break;
                                case 1:
                                    if (n.COLUMN == currentInput.SIZE || currentInput.SIZE == -1)
                                        persistance = false;
                                    else persistance = true;
                                    break;
                                case 2:
                                    if (n.COLUMN == currentInput.HDISTR || currentInput.HDISTR == -1)
                                        persistance = false;
                                    else persistance = true;
                                    break;
                                case 3:
                                    if (n.COLUMN == currentInput.VDISTR || currentInput.VDISTR == -1)
                                        persistance = false;
                                    else persistance = true;
                                    break;
                            }
                    //in teoria la parentesi mancante del foreach sopra dovrebbe finire qui
                    //però se la cosa è veramente così allora l'ultimo neurone analizzato avrebbe
                    //l'ultima parola su che valore debba avere persistance, il che non ha senso.

                    //Forse l'effetto voluto era che il valore di persistance sia relativo alle singole feature.
                    //Quindi o si dovrebbbe conservare un vettore di 4 booleani, oppure si dovrebbe cambiare 
                    //leggermente il control-flow della procedura.

                    //Però mettiamo il caso che l'effetto voluto sia quello che se presento un oggetto
                    //una seconda volta (permanenza) allora voglio appunto che la permanenza scatti.
                    //Ossia la permanenza dovrebbe avvenire su TUTTE le feature, e quindi si dovrebbe
                    //memorizzare true solo se tutte le feature non nulle abbiano registrato una permanenza.
                    foreach (Synapse s in _firstToLiquid)
                    {
                        foreach (Neuron n1 in winnerFirst)
                            //qui si va a selezionare la sinapsi che parte dal neurone vincitore n1
                            if (n1 != null && n1.ROW == s.Start.ROW && n1.COLUMN == s.Start.COLUMN && s.Start.NSpikes >= 2)
                            {
                                Class1Neuron n = (Class1Neuron)s.Dest;

                                //Prima versione in cui la corrente della Liquid si setta al valore diverso dal Default
                                //if (n1.ROW == 0 && n1.COLUMN == currentInput.COLOR)
                                //    if (n.Active == false || currentInput.COLORVALUE < n.INPUTCURRENT || (currentInput.COLORVALUE != Constants.DEFAULT_CURRENT_LIQUID && currentInput.COLORVALUE > n.INPUTCURRENT))   //così do al neurone della liquid la corrente non di default anche se ha più inputs
                                //        n.INPUTCURRENT = currentInput.COLORVALUE;
                                //if (n1.ROW == 1 && n1.COLUMN == currentInput.SIZE)
                                //    if (n.Active == false || currentInput.SIZEVALUE < n.INPUTCURRENT || (currentInput.SIZEVALUE != Constants.DEFAULT_CURRENT_LIQUID && currentInput.SIZEVALUE > n.INPUTCURRENT))
                                //        n.INPUTCURRENT = currentInput.SIZEVALUE;
                                //if (n1.ROW == 2 && n1.COLUMN == currentInput.HDISTR)
                                //    if (n.Active == false || currentInput.HDISTRVALUE < n.INPUTCURRENT || (currentInput.HDISTRVALUE != Constants.DEFAULT_CURRENT_LIQUID && currentInput.HDISTRVALUE > n.INPUTCURRENT))
                                //        n.INPUTCURRENT = currentInput.HDISTRVALUE;
                                //if (n1.ROW == 3 && n1.COLUMN == currentInput.VDISTR)
                                //    if (n.Active == false || currentInput.VDISTRVALUE < n.INPUTCURRENT || (currentInput.VDISTRVALUE != Constants.DEFAULT_CURRENT_LIQUID && currentInput.VDISTRVALUE > n.INPUTCURRENT))
                                //        n.INPUTCURRENT = currentInput.VDISTRVALUE;

                                if (!persistance)
                                {
                                    //Seconda versione in cui arriva la corrente nei neuroni della Liquid come somma degli ingressi dagli AL

                                    //la terza condizione di questi if sembra superflua, perché nessuna colonna può essere -1.
                                    if (n1.ROW == 0 && n1.COLUMN == currentInput.COLOR && currentInput.COLOR != -1)
                                        n.INPUTCURRENT += currentInput.COLORVALUE;
                                    else if (n1.ROW == 0 && currentInput.COLOR == -1)
                                        n.INPUTCURRENT += Constants.DEFAULT_CURRENT_LIQUID;

                                    if (n1.ROW == 1 && n1.COLUMN == currentInput.SIZE && currentInput.SIZE != -1)
                                        n.INPUTCURRENT += currentInput.SIZEVALUE;
                                    else if (n1.ROW == 1 && currentInput.SIZE == -1)
                                        n.INPUTCURRENT += Constants.DEFAULT_CURRENT_LIQUID;

                                    if (n1.ROW == 2 && n1.COLUMN == currentInput.HDISTR && currentInput.HDISTR != -1)
                                        n.INPUTCURRENT += currentInput.HDISTRVALUE;
                                    else if (n1.ROW == 2 && currentInput.HDISTR == -1)
                                        n.INPUTCURRENT += Constants.DEFAULT_CURRENT_LIQUID;

                                    if (n1.ROW == 3 && n1.COLUMN == currentInput.VDISTR && currentInput.VDISTR != -1)
                                        n.INPUTCURRENT += currentInput.VDISTRVALUE;
                                    else if (n1.ROW == 3 && currentInput.VDISTR == -1)
                                        n.INPUTCURRENT += Constants.DEFAULT_CURRENT_LIQUID;
                                }
                                else
                                {

                                    if (n1.ROW == 0 && n1.COLUMN == currentInputOld.COLOR)
                                        n.INPUTCURRENT += currentInputOld.COLORVALUE;
                                    if (n1.ROW == 1 && n1.COLUMN == currentInputOld.SIZE)
                                        n.INPUTCURRENT += currentInputOld.SIZEVALUE;
                                    if (n1.ROW == 2 && n1.COLUMN == currentInputOld.HDISTR)
                                        n.INPUTCURRENT += currentInputOld.HDISTRVALUE;
                                    if (n1.ROW == 3 && n1.COLUMN == currentInputOld.VDISTR)
                                        n.INPUTCURRENT += currentInputOld.VDISTRVALUE;

                                }

                                n.Active = true;

                                input_active.WriteLine((n.ROW + 1) + " " + (n.COLUMN + 1) + " " + (n.INPUTCURRENT));
                                input_active.Flush();

                            }
                    }

                }

                input_active.Close();
                currentInputOld = currentInput;

                _layers.resetNeuronsState();

                for (int step = Constants.SIMULATION_STEPS_FEEDFORWARD; step < Constants.SIMULATION_STEPS_LIQUID + Constants.SIMULATION_STEPS_FEEDFORWARD; step++)
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
                    //with target spiking
                    //_ntarget.I = (signedTarget + 1) * 10;
                    //_ntarget.simulate(step);

                    //integration=true se si sono attivati gli stessi neuroni nel layer di input
                    if (step == 2 * Constants.SIMULATION_STEPS_FEEDFORWARD)
                    {
                        winnerFirstA = _layers.getWinnerFirstActive(2 * Constants.SIMULATION_STEPS_FEEDFORWARD);
                        for (int i = 0; i < 4; i++)
                            if (winnerFirstA != null && winnerFirstA[i] != null && winnerFirst[i].COLUMN == winnerFirstA[i].COLUMN)
                                integration = true;
                            else
                            {
                                integration = false;
                                break;
                            }

                    }

                    target = (double)Math.Sin(((step - Constants.SIMULATION_STEPS_FEEDFORWARD) * 2 * Math.PI * ((signedTarget+1)*5 * 0.001)) / Constants.INTEGRATION_STEP_MORRIS_LECAR) / 100;
                    ////Da mettere se vogliamo squadrare la sinusoide
                    //if (target > 0)
                    //    target = value;
                    //else
                    //    target = -value;

                    foreach (Synapse s in _firstToFirst1)
                    {
                        s.simulate(step);
                        if (log != null)
                            log.logSynapse(s, step);
                    }

                    foreach (Synapse s in _firstToFirstSTDP)
                    {
                        s.simulate(step);
                        //if (log != null)
                        //    logSTDP.logSynapse(s, step);
                    }

                    foreach (Synapse s in _liquidToLiquid)
                    {
                        s.simulate(step);
                        if (log != null)
                            log.logSynapse(s, step);
                    }

                    foreach (LinkedList<Synapse> s1 in _liquidToOut)
                        foreach (SynapseSTDP s in s1)
                        {
                            //s rappresenta una sinapsi culminante in un neurone somma
                            s.simulate(step);

                            if (s.Dest.COLUMN == signedTarget)
                                //nel caso della pseudo-inversa il calcolo viene poi effettuato su Matlab
                                if (learning && (_liquid.Option != 3) && (_liquid.Option != 5))
                                    error = s.learnLiquid(step, Constants.LIQUID_TO_OUT_NI, Constants.LIQUID_TO_OUT_EPS, _liquid.Option, true, target);
                            if (logSTDP != null)
                                logSTDP.logSynapse(s, step);
                        }
                    //error è l'ultimo errore calcolato, era voluto tralasciare quelli precedenti?
                    errorMean += Math.Abs(error); //non è una media
                    _layers.simulateFirst1(step, log);
                    _liquid.simulate(step, log, simNumber);
                    _outExt.simulate(step, log, simNumberInternal, integration);
                    _context.simulate(step, log, logSTDP, simNumber, simNumberInternal, _endSequenceReward, test, _levelReward, _motor);



                    String dot = target.ToString(CultureInfo.CreateSpecificCulture("en-GB"));
                    String dotError = error.ToString(CultureInfo.CreateSpecificCulture("en-GB"));
                    fileTarget.Write(dot + "\t" + dotError + "\r\n");
                    fileTarget.Flush();

                }

                if (log != null)
                {
                    log.printLog();
                    log.newIteration();
                }
                if (logSTDP != null)
                {
                    logSTDP.printLog();
                    logSTDP.newIteration();
                }
                //aggiornamento a epoche
                if (!test)
                {
                    foreach (SynapseSTDP s in _firstToFirstSTDP)
                    {
                        s.learn(0, 0);
                        if (log != null)
                            logSTDP.logSynapse(s, Constants.SIMULATION_STEPS_LIQUID + Constants.SIMULATION_STEPS_FEEDFORWARD + 1);

                    }




                    if (_liquid.Option == 1 || _liquid.Option == 4)
                    {
                        foreach (LinkedList<Synapse> s1 in _liquidToOut)
                            foreach (SynapseSTDP s in s1)
                            {
                                if (s.Dest.COLUMN == signedTarget)
                                    s.UpdateSyn();
                                if (logSTDP != null)
                                    logSTDP.logSynapse(s, Constants.SIMULATION_STEPS_LIQUID);
                            }

                    }
                }

                fileTarget.Close();
                indexWinOut = _outExt.getWinnerNeuron();
            }
            else
            {

                for (int step = Constants.SIMULATION_STEPS_FEEDFORWARD; step < Constants.SIMULATION_STEPS_LIQUID + Constants.SIMULATION_STEPS_FEEDFORWARD; step++)
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
                    //with target spiking
                    //_ntarget.I = (signedTarget + 1) * 10;
                    //_ntarget.simulate(step);

                    target = (double)Math.Sin(((step - Constants.SIMULATION_STEPS_FEEDFORWARD) * 2 * Math.PI * ((signedTarget + 1) * 5 * 0.001)) / Constants.INTEGRATION_STEP_MORRIS_LECAR) / 100;

                    //if (target > 0)
                    //    target = value;
                    //else
                    //    target = -value;


                    foreach (LinkedList<Synapse> s1 in _liquidToOut)
                        foreach (SynapseSTDP s in s1)
                        {
                            s.simulate(step);

                            if (s.Dest.COLUMN == signedTarget)
                                //nel caso della pseudo-inversa il calcolo viene poi effettuato su Matlab
                                if (learning && (_liquid.Option != -1) && (_liquid.Option != 3) && (_liquid.Option != 5))
                                    error = s.learnLiquid(step, Constants.LIQUID_TO_OUT_NI, Constants.LIQUID_TO_OUT_EPS, _liquid.Option, true, target);
                            if (logSTDP != null)
                                logSTDP.logSynapse(s, step);
                        }

                    errorMean += Math.Abs(error);
                    _outExt.simulate(step, log, simNumberInternal, integration);
                    _context.simulate(step, log, logSTDP, simNumber, simNumberInternal, _endSequenceReward, test, _levelReward, _motor);  //risimulo il contesto anche in questo occasione perchè se sono arrivato qui significa che il morris lecar associato all'elemento non ha sparato nel caso precedente e quindi il contesto non si è aggiornato correttamente



                    String dot = target.ToString(CultureInfo.CreateSpecificCulture("en-GB"));
                    String dotError = error.ToString(CultureInfo.CreateSpecificCulture("en-GB"));
                    fileTarget.Write(dot + "\t" + dotError + "\r\n");
                    fileTarget.Flush();



                }

                if (log != null)
                {
                    log.printLog();
                    log.newIteration();
                }
                if (logSTDP != null)
                {
                    logSTDP.printLog();
                    logSTDP.newIteration();
                }

                //aggiornamento a epoche
                if (!test)
                {

                    if (_liquid.Option == 1 || _liquid.Option == 4)
                    {
                        foreach (LinkedList<Synapse> s1 in _liquidToOut)
                            foreach (SynapseSTDP s in s1)
                            {
                                if (s.Dest.COLUMN == signedTarget)
                                    s.UpdateSyn();
                                if (logSTDP != null)
                                    logSTDP.logSynapse(s, Constants.SIMULATION_STEPS_LIQUID);
                            }

                    }
                }

                fileTarget.Close();
                indexWinOut = _outExt.getWinnerNeuron();



            }
            //ricordo che indexWinOut è l'indice del neurone di Morris Lecar Vincente
            if (indexWinOut != -1)
            {

                //test
                if (test || simulateLiquidTest(indexWinOut, simNumber, simNumberInternal))
                {

                    if (MLtoInput[indexWinOut, 0] == -1)
                    {
                        for (int i = 0; i < 4; i++)
                            if (winnerFirst[i] != null)
                                MLtoInput[indexWinOut, i] = winnerFirst[i].COLUMN;
                    }

                    for (int i = 0; i < 4; i++)
                        feat[i] = MLtoInput[indexWinOut, i];

                    feat[feat.Length - 1] = _context.getWinnerMotor(Constants.SIMULATION_STEPS_FEEDFORWARD + Constants.SIMULATION_STEPS_LIQUID);
                    frequencyRewardSequence = _context.getFrequencyEndSequence(Constants.SIMULATION_STEPS_FEEDFORWARD + Constants.SIMULATION_STEPS_LIQUID);
                    //reset of all neuron 
                    _liquid.resetState();
                    _layers.resetNeuronsState();
                    if (!test)
                        _context.learn(logSTDP, Constants.SIMULATION_STEPS_LIQUID + Constants.SIMULATION_STEPS_FEEDFORWARD - 1, _endSequenceReward, _motor);
                    _context.winContextOld = _context.getWinnerContext(Constants.SIMULATION_STEPS_LIQUID + Constants.SIMULATION_STEPS_FEEDFORWARD - 1, 1);
                    if (_context.isEndSequence())
                    {
                        // Console.WriteLine("Simulate network: Resetto il contesto");
                        _context.endSequence();
                    }
                }
                else
                {
                    indexWinOut = -1;
                }
            }
            else
            {
                frequencyRewardSequence = -1;
                _context.nullValid();
                //non si dovrebbe anche chiamare end sequence?

            }
            //else if (test) {
            //    _context.winContextOld = _context.getWinnerContext(Constants.SIMULATION_STEPS_LIQUID + Constants.SIMULATION_STEPS_FEEDFORWARD - 1);                   
            //}
            _outExt.resetState();
            _context.resetNeuronsState();
            //_ntarget.resetState();

            //Se sono nelle simulazioni del Test devo resettare in ogni caso perchè non ho la possibilità di fare simulazioni interne
            if (simNumber >= Constants.LEARNING_NUMBER || _liquid.Option == 5)
            {
                //reset of all neuron 
                _liquid.resetState();

                _layers.resetNeuronsState();
            }


            if (log != null)
                log.printLog();
            if (logSTDP != null)
                logSTDP.printLog();

            if (log != null)
            {
                log.closeLog();
                log.newIteration();
            }
            if (logSTDP != null)
            {
                logSTDP.closeLog();
                logSTDP.newIteration();
            }


            //return (errorMean / Constants.SIMULATION_STEPS_LIQUID);
            return indexWinOut;

        }


        /// <summary>
		/// Simulates the liquid in a test phase 
		/// </summary>
        private bool simulateLiquidTest(int signedTarget, int simNumber, int simNumberInternal)
        {
            double[] target = new double[Constants.SIMULATION_STEPS_LIQUID];
            double[] ZW = new double[Constants.SIMULATION_STEPS_LIQUID];
            double errorMeanTest = 0;
            String targetS;
            int countSpike = 0;

            if (simNumberInternal == -1)
                targetS = "Z_pesi_fissi" + simNumber + ".txt";
            else
                targetS = "Z_pesi_fissi" + simNumber + "-" + simNumberInternal + ".txt";

            StreamWriter fileTarget = new StreamWriter(pathPc + @"\Dati\" + targetS);

            _outExt.resetState();

            for (int step = Constants.SIMULATION_STEPS_FEEDFORWARD; step < Constants.SIMULATION_STEPS_LIQUID + Constants.SIMULATION_STEPS_FEEDFORWARD; step++)
            {
                target[step - Constants.SIMULATION_STEPS_FEEDFORWARD] = (double)Math.Sin(((step - Constants.SIMULATION_STEPS_FEEDFORWARD) * 2 * Math.PI * ((signedTarget + 1) * 5 * 0.001)) / Constants.INTEGRATION_STEP_MORRIS_LECAR) / 100;

                ////Se usiamo il target squadrato
                //if (target[step - Constants.SIMULATION_STEPS_FEEDFORWARD] > 0)
                //    target[step - Constants.SIMULATION_STEPS_FEEDFORWARD] = value;
                //else
                //    target[step - Constants.SIMULATION_STEPS_FEEDFORWARD] = -value;

                foreach (LinkedList<Synapse> s1 in _liquidToOut)
                    foreach (SynapseSTDP s in s1)
                    {
                        if (s.Dest.COLUMN == signedTarget)
                        {
                            s.simulate(step);
                            ZW[step - Constants.SIMULATION_STEPS_FEEDFORWARD] += s.IPrev;
                        }
                    }


                ////Se usiamo il sommatore squadrato:
                //if (ZW[step - Constants.SIMULATION_STEPS_FEEDFORWARD] > 0)
                //    ZW[step - Constants.SIMULATION_STEPS_FEEDFORWARD] = value;
                //else
                //    ZW[step - Constants.SIMULATION_STEPS_FEEDFORWARD] = -value;


                errorMeanTest += target[step - Constants.SIMULATION_STEPS_FEEDFORWARD] - ZW[step - Constants.SIMULATION_STEPS_FEEDFORWARD];

                String dot = ZW[step - Constants.SIMULATION_STEPS_FEEDFORWARD].ToString(CultureInfo.CreateSpecificCulture("en-GB"));


                countSpike = _outExt.simulateMorrisLecar(step, signedTarget, ZW[step - Constants.SIMULATION_STEPS_FEEDFORWARD]);

                fileTarget.Write(dot + "\t" + countSpike + "\r\n");
                fileTarget.Flush();


            }


            fileTarget.Close();
            double threshold = _outExt.getMorrisThreshold(signedTarget);

            //if (countSpike > (Constants.MORRIS_WINNER_SPIKE))     //vecchia versione con soglia costante per tutti i morris

            if (countSpike > threshold)                             //nuova versione con la soglia che varia in base alla frequenza del morris
                return true;
            else
                return false;

            //if ((errorMeanTest / Constants.SIMULATION_STEPS_LIQUID) > Constants.LIQUID_TO_OUT_THRESHOLD)
            //    return false;
            //else return true;

        }




        /// <summary>
        /// Simulates the network for learning (logging the results)
        /// </summary>
        /// <param name="log">The logger object</param>
        /// <param name="simNumber">Number of the simulation (0-based)</param>
        public void learnLiquid(StateLogger log, int simNumber)
        {
            int target = 0;
            if (simNumber != 0)
                target = addNewNeuron(simNumber);

            int simNumberInternal = 0;
            double errorLearn = this.simulateLiquid(log, null, simNumber, simNumberInternal, true, target, false);
            while (errorLearn == -1)
            {
                StateLogger sl = new StateLogger("Neurons" + (simNumber) + "-" + simNumberInternal + ".txt", "Synapse" + (simNumber) + "-" + simNumberInternal + ".txt", "NeuronMorrisLecar" + (simNumber) + "-" + simNumberInternal + ".txt", true, false, true);

                errorLearn = this.simulateLiquid(sl, null, simNumber, simNumberInternal, true, target, false);
                simNumberInternal++;
            }
        }

        /// <summary>
        /// Simulates the network for learning (logging the results for STDP synapse)
        /// </summary>
        /// <param name="log">The logger object</param>
        /// <param name="simNumber">Number of the simulation (0-based) (epoch)</param>
        /// <param name="input">vector for the input (without the simulation of the input layer) </param>
        public void learnLiquid(StateLogger log, StateLogger logSTDP, int simNumber)
        {
            int target = 0;
            bool nofeature = false;

            simulationNumber = simNumber;

            double errorLearn = this.simulateLiquid(log, logSTDP, simNumber, simNumberInternal, false, target, false);

            target = addNewNeuron(simNumber);

            simNumberInternal++;
            while (errorLearn == -1 && simNumberInternal <= 2)
            {
                Console.WriteLine("Learning non riuscito, in atto un nuovo tentantivo.");

               
                setOption(3);   
                                


                //PER PSEUDO-INVERSA CON RUMORE
                if (_liquid.Option == 5 && simNumberInternal < 2)
                {

                    StateLogger sl = new StateLogger(pathPc + @"\Dati\Neurons" + (simNumber) + "-" + simNumberInternal + ".txt", pathPc + @"\Dati\Synapse" + (simNumber) + "-" + simNumberInternal + ".txt", pathPc + @"\Dati\NeuronMorrisLecar" + (simNumber) + "-" + simNumberInternal + ".txt", true, false, true);
                    StateLogger slStdp = new StateLogger(pathPc + @"\Dati\NeuronsStdp" + (simNumber) + "-" + simNumberInternal + ".txt", pathPc + @"\Dati\SynapseSTDP" + (simNumber) + "-" + simNumberInternal + ".txt", false, true);
                    this.resetInputs();

                    //Se abbiamo l'ultima feature mancante usiamo questa variazione per la pseudoInversa rumorosa -> caso if se no else
                    if (currentInput.VDISTR == -1)
                    {
                        this.setInput(new NetworkInput(currentInput.COLOR, currentInput.SIZE, currentInput.HDISTR, 1, currentInput.REWARD, currentInput.END, currentInput.REWARDLEVEL, 0, 0, 0, -60, currentInput.MOTOR));  //setto lo stesso input di prima semplicemente modificando una feature,perturbandola con del rumore prima additivo e poi sottrattivo                            
                        nofeature = true;
                    }
                    else
                        this.setInput(new NetworkInput(currentInput.COLOR, currentInput.SIZE, currentInput.HDISTR, currentInput.VDISTR, currentInput.REWARD, currentInput.END, currentInput.REWARDLEVEL, 0, 0, 0, -40, currentInput.MOTOR));  //setto lo stesso input di prima semplicemente modificando una feature,perturbandola con del rumore prima additivo e poi sottrattivo

                    this.simulateLiquid(sl, slStdp, simNumber, simNumberInternal, false, target, false);
                    simNumberInternal++;
                    StateLogger sl1 = new StateLogger(pathPc + @"\Dati\Neurons" + (simNumber) + "-" + simNumberInternal + ".txt", pathPc + @"\Dati\Synapse" + (simNumber) + "-" + simNumberInternal + ".txt", pathPc + @"\Dati\NeuronMorrisLecar" + (simNumber) + "-" + simNumberInternal + ".txt", true, false, true);
                    StateLogger slStdp1 = new StateLogger(pathPc + @"\Dati\NeuronsStdp" + (simNumber) + "-" + simNumberInternal + ".txt", pathPc + @"\Dati\SynapseSTDP" + (simNumber) + "-" + simNumberInternal + ".txt", false, true);
                    this.resetInputs();

                    //Se abbiamo l'ultima feature mancante usiamo questa variazione per la pseudoInversa rumorosa -> caso if se no else
                    if (nofeature)
                        this.setInput(new NetworkInput(currentInput.COLOR, currentInput.SIZE, currentInput.HDISTR, 1, currentInput.REWARD, currentInput.END, currentInput.REWARDLEVEL, 0, 0, 0, -80, currentInput.MOTOR));
                    else
                        this.setInput(new NetworkInput(currentInput.COLOR, currentInput.SIZE, currentInput.HDISTR, currentInput.VDISTR, currentInput.REWARD, currentInput.END, currentInput.REWARDLEVEL, 0, 0, 0, -20, currentInput.MOTOR));

                    this.simulateLiquid(sl1, slStdp1, simNumber, simNumberInternal, false, target, false);
                    simNumberInternal++;

                    if (nofeature)
                        this.setInput(new NetworkInput(currentInput.COLOR, currentInput.SIZE, currentInput.HDISTR, -1, currentInput.REWARD, currentInput.END, currentInput.REWARDLEVEL));
                    else
                        this.setInput(new NetworkInput(currentInput.COLOR, currentInput.SIZE, currentInput.HDISTR, currentInput.VDISTR, currentInput.REWARD, currentInput.END, currentInput.REWARDLEVEL));

                    if (Constants.LIQUID_TO_OUT_PSEUDOINV_FILE)
                    {
                        Console.WriteLine("\n Aggiornare file W da Matlab con Simulation=" + simNumber + " target=" + target);
                        Console.WriteLine("\n Scrivere su prompt Matlab: PseudoInvNoise(" + simNumber + "," + target + ") \n");
                    }
                    string op = System.Console.ReadLine();
                }



                StateLogger sl2 = new StateLogger(pathPc + @"\Dati\Neurons" + (simNumber) + "-" + simNumberInternal + ".txt", pathPc + @"\Dati\Synapse" + (simNumber) + "-" + simNumberInternal + ".txt", pathPc + @"\Dati\NeuronMorrisLecar" + (simNumber) + "-" + simNumberInternal + ".txt", true, false, true);
                StateLogger slStdp2 = new StateLogger(pathPc + @"\Dati\NeuronsStdp" + (simNumber) + "-" + simNumberInternal + ".txt", pathPc + @"\Dati\SynapseSTDP" + (simNumber) + "-" + simNumberInternal + ".txt", false, true);
                errorLearn = this.simulateLiquid(sl2, slStdp2, simNumber, simNumberInternal, true, target, false);
                simNumberInternal++;
            }

            simNumberInternal = -1;
            Console.WriteLine("\t\t\t\t\t\t\t\tNeurone vincitoree: " + indexWinOut);
            Console.WriteLine("\t\t\t\t\t\t\t\tFrequenza End Neuronn: " + frequencyRewardSequence);
            Console.WriteLine("\t\t\t\t\t\t\t\tNeurone Motore vincitoree: " + feat[feat.Length - 1] + "\n\n");



        }

        /// <summary>
        /// Simulates the network for learning
        /// </summary>
        /// <param name="simNumber">Number of the simulation (0-based)</param>
        public void learnLiquid(int simNumber)
        {
            int target = 0;
            if (simNumber != 0)
                target = addNewNeuron(simNumber);


            double errorLearn = this.simulateLiquid(null, null, simNumber, 0, true, target, false);
            while (errorLearn == -1)
            {
                errorLearn = this.simulateLiquid(null, null, simNumber, 0, true, target, false);
            }
        }

        /// <summary>
        /// Simulates the network for testing (logging the results)
        /// </summary>
        /// <param name="log">The logger object</param>
        /// <param name="simNumber">Number of the simulation (0-based)</param>
        public void testLiquid(StateLogger log, int simNumber)
        {
            this.simulateLiquid(log, null, simNumber, 0, false, 0, true);
        }


        //prova
        /// <summary>
        /// Simulates the network for testing (logging the results for STDP synapses)
        /// </summary>
        /// <param name="log">The logger object</param>
        /// <param name="simNumber">Number of the simulation (0-based)</param>
        public void testLiquid(StateLogger log, StateLogger logSTDP, int simNumber)
        {
            // Console.WriteLine("\t\tsim: " + simulationNumber);   
            simulationNumber = simNumber;
            double end_freq = 0;
            int winner = 0;
            int sim;
            int contatore;
            List<NetworkInput> lista_input = new List<NetworkInput>(countInput);
            //reset the context
            if (simNumber == Constants.LEARNING_NUMBER) //eseguito per il primo test in assoluto
            {
                //_context.reset_win_context_old();
                _context.endSequence();
                _context.nullValid();
            }

            /*if (simNumber == Constants.LEARNING_NUMBER && countInput.Count > 1)
                this.imaginationLiquid(countInput);*/
            if (simNumber == Constants.LEARNING_NUMBER && lista_input.Count > 1)
            {
                Console.WriteLine("********************Imagination**************************");
                for (int i = 0; i < lista_input.Count; i++)
                {
                    Console.WriteLine("-------------------Simulo Input---------------: " + i);
                    sim = 0;
                    resetInputs();
                    _context.endSequence();
                    _context.nullValid();
                    setInput(lista_input[i]);
                    contatore = 0;
                    do
                    {
                        Console.WriteLine("     Simulo internamente: " + sim);
                        if (sim != 0)
                        {
                            resetInputs();
                            setInput(new NetworkInput(-1, -1, -1, -1));
                        }
                        this.simulateLiquid(null, null, simNumber + i, -1, false, 0, true);
                        Console.WriteLine("\t\t\t\t\t\t\t\tNeurone vincitore: " + indexWinOut);
                        Console.WriteLine("\t\t\t\t\t\t\t\tFrequenza End Neuron: " + frequencyRewardSequence);
                        Console.WriteLine("\t\t\t\t\t\t\t\tNeurone Motore vincitore: " + feat[feat.Length - 1] + "\n\n");
                        sim++;
                        contatore++;
                        if (this.getEndSequenceFrequency() > end_freq)
                        {
                            end_freq = this.getEndSequenceFrequency();
                            winner = i;
                        }

                    } while (this.getEndSequenceFrequency() == 0 || contatore < 4);
                }
                resetInputs();
                setInput(lista_input[winner]);
                _context.endSequence();
                _context.nullValid();
                Console.WriteLine("Input scelto = " + winner);
                Console.WriteLine("********************Fine Imagination**************************");
            }
            /*if (indexWinOut == -1) //se l'input non è stato riconoscuto resetto il context
            {
                _context.endSequence();
                _context.nullValid();
            }*/
            this.simulateLiquid(log, logSTDP, simNumber, -1, false, 0, true);
            Console.WriteLine("\t\t\t\t\t\t\t\tNeurone vincitore: " + indexWinOut);
            Console.WriteLine("\t\t\t\t\t\t\t\tFrequenza End Neuron: " + frequencyRewardSequence);
            Console.WriteLine("\t\t\t\t\t\t\t\tNeurone Motore vincitore: " + feat[feat.Length - 1] + "\n\n");

        }


        /// <summary>
        /// Simulates the network for testing (logging the results for STDP synapses)
        /// </summary>
        /// <param name="log">The logger object</param>
        /// <param name="simNumber">Number of the simulation (0-based)</param>
        public void testLiquid(StateLogger log, StateLogger logSTDP, int simNumber, int nLearn)
        {
            simulationNumber = simNumber;

            //reset the context
            if (simNumber == nLearn)
                _context.endSequence();

            if (simNumber == nLearn && countInput.Count > 1)
                this.imaginationLiquid(countInput);

            this.simulateLiquid(log, logSTDP, simNumber, -1, false, 0, true);
        }

        /// <summary>
        /// Simulates the network for testing
        /// <param name="simNumber">Number of the simulation (0-based)</param>
        /// </summary>
        public void testLiquid(int simNumber)
        {
            simulationNumber = simNumber;
            this.simulateLiquid(null, null, simNumber, 0, false, 0, true);
        }







        /// <summary>
        /// Returns the frequency of the end neuron
        /// </summary>
        /// <returns>The frequency of the end neuron, calculated
        /// at the last step of simulation</returns>
        public double getEndSequenceFrequency()
        {
            //return _layers._endSequenceNeuron.getFrequency(Constants.SIMULATION_STEPS_A);
            return frequencyRewardSequence;
        }

        /// <summary>
        /// Creates a new network, reading its status information from file
        /// </summary>
        /// <param name="configPath">The path of the configuration file</param>
        /// <returns>A reference to the new <code>Network</code> object</returns>
        public static Network generateNetwork(String configPath)
        {
            return new Network(configPath);
        }

        /// <summary>
        /// Creates a new network without information
        /// </summary>
        /// <returns>A reference to the new <code>Network</code> object</returns>
        public static Network generateNetwork()
        {
            return new Network();
        }

        /// <summary>
        /// Creates a new network, reading its status information from files
        /// </summary>
        /// <param name="configPath">The path of the configuration file</param>
        /// <param name="configStdp">The path of the configuration file for the STDP synapses</param>
        /// <returns>A reference to the new <code>Network</code> object</returns>
        public static Network generateNetwork(String configPath, String configStdp)
        {
            return new Network(configPath, configStdp);
        }



        /// <summary>
        /// Return the value of _expCheck variable which contains the result of the comparison beetwen the expected input and the presented one
        /// </summary>
        public bool getExpCheck()
        {
            return _expCheck;
        }

        /// <summary>
        /// Return the value of feat variable which contains the features of the winner input 
        /// </summary>
        public int[] getFeat()
        {
            return feat;
        }

        /// <summary>
        /// Set the value of the option for the liquid state
        /// </summary>
        public void setOption(int op)
        {
            _liquid.Option = op;
        }




        /// <summary>
        /// Function to indentify the greater reward sequence 
        /// </summary>
        private void imaginationLiquid(List<NetworkInput> listInput)
        {
            //_layers.resetInputs();
            double[] rewardSeq = new double[listInput.Count];
            int simNumberImg;
            NetworkInput inputNull = new NetworkInput(-1, -1, -1, -1);
            List<NetworkInput> lista_input = new List<NetworkInput>();
            for (int i = 0; i < listInput.Count; i++)
                lista_input.Add(listInput[i]);


            for (int i = 0; i < lista_input.Count; i++)
            {
                _context.endSequence(); //aggiunta io
                _context.nullValid(); //aggiunta io, provare a metterle alla fine
                resetInputs();
                setInput(lista_input[i]);
                currentInput = lista_input[i];
                //_layers.setInput(listInput[i]);

                //_reward = listInput[i].REWARD;
                //_endSequenceReward = listInput[i].END;

                if (Constants.ENABLE_OUTPUT)
                    System.Console.WriteLine("\n *** *** *** *** *** *** *** *** IMAGINATION *** *** *** *** *** *** *** *** \n");
                simNumberImg = 0;
                frequencyRewardSequence = 0;
                while (simNumberImg == 0 || frequencyRewardSequence == 0)
                {
                    if (simNumberImg > 0)
                    {
                        //currentInput = inputNull;
                        resetInputs();
                        setInput(inputNull);
                        //_layers.setInput(inputNull);
                        //_reward = false;
                        //_endSequenceReward = false;
                    }
                    Console.WriteLine("Input dato while: " + currentInput);
                    if (Constants.ENABLE_OUTPUT)
                        System.Console.WriteLine("\n *** *** *** *** Simulazione Imagination {0} *** *** *** ***\n", simNumberImg + 1);
                    this.simulateLiquid(null, null, simNumberImg, -1, false, 0, true);
                    //this.resetInputs();
                    simNumberImg++;
                }

                //take the reward sequence
                rewardSeq[i] = frequencyRewardSequence;
                Console.WriteLine(" end: " + frequencyRewardSequence + ", input " + i);

            }

            double maxRew = rewardSeq[0];
            int indexWinnerInput = 0;
            for (int i = 1; i < lista_input.Count; i++)
            {
                if (rewardSeq[i] > maxRew)
                {
                    maxRew = rewardSeq[0];
                    indexWinnerInput = i;
                }
            }

            //set the winner Input for the sequence with a greater reward frequency of a sequence
            //_layers.setInput(listInput[indexWinnerInput]);
            //_reward = listInput[indexWinnerInput].REWARD;
            //_endSequenceReward = listInput[indexWinnerInput].END;
            currentInput = lista_input[indexWinnerInput];
            setInput(currentInput);


            Console.WriteLine(currentInput); //vedere se contiene il valore giusto
            Console.WriteLine("Imagination : Input vincente = " + indexWinnerInput);
            if (Constants.ENABLE_OUTPUT)
                System.Console.WriteLine("\n *** *** *** *** *** *** *** *** Testing *** *** *** *** *** *** *** ***");
            if (Constants.ENABLE_OUTPUT)
                System.Console.WriteLine("*** *** *** *** Simulazione {0} *** *** *** ***", 1);
        }


        /// <summary>
        /// This function establish if there is a correct output neuron for the liquid and if not it creates new one
        /// </summary>
        /// <returns></returns>
        public int addNewNeuron(int simNumber)
        {
            //in generale questa funzione serve per creare una nuova classe
            //di output se necessario
            int target = 0;
            SumNeuron dest;

            //dopo che una simulazione (1200 steps) si va a vedere quale dei neuroni di Morris-Lecar
            //si è attivato. L'indice di questo neurone è contenuto in indexWinOut.
            //in particolare se nessun neurone si è attivato indexWinOut è posto a -1.


            //se siamo alla prima simulazione non può che esistere per ora
            //una sola classe, con i neuroni (morris, sum, persist., samen.) già creati
            if (simNumber == 0)
            {
                //connettiamo il neurone di sameness con tutti i neuroni vincitori del layer di input
                initSamenessToFirst(0); //feedback
                return 0;
            }
            else
            //in questo caso c'è bisogno di creare una nuova classe
            if (indexWinOut == -1)
            {
                //andiamo a creare i neuroni della nuova classe, inizializzando le connessioni
                dest = _outExt.addNeuron();
                target = dest.COLUMN;
                initSamenessToFirst(target); //feedback
                initLiquidToOutput(dest);
                _context.addPath(_outExt.getNeuronMorris(target));
                if (Constants.DEBUG==1)
                    Console.WriteLine("E' stata aggiunta una nuova classe. Target: " + target);
                return target;
            }
            else
            {
                //dovrebbe essere il caso nel quale non serve alcuna classe nuova
                return indexWinOut;
            }
        }

        /// <summary>
        /// This function establish if there is a correct output neuron for the liquid and if not it creates new one
        /// </summary>
        /// <returns></returns>
        public int[] getValid()
        {
            return _context.valid;
        }

    }
}
