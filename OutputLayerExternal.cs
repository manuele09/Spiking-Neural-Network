using System;
using System.Collections.Generic;
using System.Text;

namespace SLN
{
    [Serializable]
    class OutputLayerExternal
    {

        
        private LinkedList<NeuronMorrisLecar> _neurons; //morris lecar
        private LinkedList<SumNeuron> _neuronSum;
        private LinkedList<SumNeuron> _neuronPersistance;
        private LinkedList<SumNeuron> _neuronSameness;

        private LinkedList<Synapse> _morrisToPersistance;
        private LinkedList<Synapse> _morrisToSameness;

        private int index = 0;
        private double[,] matrixKI;
        private double[] matrixf;


         /// <summary>
		/// Constructor
		/// </summary>
        internal OutputLayerExternal()
        {

            this.init();

        }

        /// <summary>
        /// initialization of the output layer
        /// </summary>
        private void init() {

            _neurons = new LinkedList<NeuronMorrisLecar>();
            _neuronSum = new LinkedList<SumNeuron>();
            _neuronPersistance = new LinkedList<SumNeuron>();
            _neuronSameness = new LinkedList<SumNeuron>();

            matrixKI = new double[Constants.CLASSES,2]{ {0.055, 61.5},/* {0.067, 61.45}, {0.074, 61.4}, {0.082, 61.35}, {0.095, 61.25}, */{0.105, 61.2}, {0.155, 60.8}, {0.2, 60.5}};
            matrixf = new double[Constants.CLASSES] {5, 10, 15, 20 };

           // matrixKI = new double[6, 2] { { 0.055, 61.5 }, {0.067, 61.45}, /*{0.074, 61.4}, {0.082, 61.35},*/ {0.095, 61.35}, { 0.105, 61.2 }, { 0.155, 60.8 }, { 0.2, 60.5 } };
           // matrixf = new double[6] { 5, 10, 15, 20, 25, 30};

            NeuronMorrisLecar m = new NeuronMorrisLecar(matrixKI[index, 0], matrixKI[index,1]);
            m.setCoord(1, index, LayerNumbers.OUTPUT_LAYER);         
            _neurons.AddLast(m);

            SumNeuron n = new SumNeuron(0, matrixKI[index, 1]);  //uso la variabile gain nel caso del sommatore per dire a che valore deve saturare l'ingresso (in pratico stabilisco il valore della saturazione della funzione di heaviside
            n.setCoord(0, index, LayerNumbers.OUTPUT_LAYER);
            _neuronSum.AddLast(n);

            SumNeuron o = new SumNeuron();
            o.setCoord(2, index, LayerNumbers.OUTPUT_LAYER);
            _neuronPersistance.AddLast(o);

            SumNeuron p = new SumNeuron(Constants.MORRIS_TO_SAMENESS_DECAY,Constants.MORRIS_TO_SAMENESS_GAIN);
            p.setCoord(3, index, LayerNumbers.OUTPUT_LAYER);
            _neuronSameness.AddLast(p);

           

            _morrisToPersistance = new LinkedList<Synapse>();
            _morrisToSameness = new LinkedList<Synapse>();

            Synapse syn = new SynapseSameness(m, o, Constants.MORRIS_TO_SAMENESS_W/matrixf[index],
                                    Constants.MORRIS_TO_SAMENESS_TAU,
                                    Constants.MORRIS_TO_SAMENESS_DELAY_STEP,
                                    Constants.MORRIS_TO_SAMENESS_SYNAPTIC_GAIN);
            _morrisToPersistance.AddLast(syn);

            Synapse syn2 = new SynapseSameness(m, p, Constants.MORRIS_TO_SAMENESS_W / matrixf[index],
                                   Constants.MORRIS_TO_SAMENESS_TAU1,
                                   Constants.MORRIS_TO_SAMENESS_DELAY_STEP,
                                   Constants.MORRIS_TO_SAMENESS_SYNAPTIC_GAIN);
            _morrisToSameness.AddLast(syn2);

            index = index + 1;

            //LinkedList<Synapse> _liquidToOut1 = new LinkedList<Synapse>();
            //NeuronMorrisLecar dest = new NeuronMorrisLecar(0,0);
            //foreach (NeuronMorrisLecar a in _neurons)
            //    dest = a;
            //Random randOut = new Random();
            //Random rand1 = new Random();
            //for (int i1 = 0; i1 < Constants.LIQUID_DIMENSION_I; i1++)
            //    for (int j1 = 0; j1 < Constants.LIQUID_DIMENSION_J; j1++)
            //    {
            //        Neuron start = _liquid.getLiquidLayerNeuron(i1, j1);

            //        double w = ((((randOut.Next(100))) / 100.0) - 0.5) * 0.0001;
            //        w = Math.Abs(w);

            //        double[] value = new double[4];
            //        value[0] = 5;
            //        value[1] = 10;
            //        value[2] = 30;
            //        value[3] = 50;
            //        double tau = 1;

            //        double wheel = (((rand1.Next(100))) / 100.0);
            //        if (wheel < 0.25)
            //            tau = value[0];
            //        if ((wheel >= 0.25) && (wheel < 0.5))
            //            tau = value[1];
            //        if ((wheel >= 0.5) && (wheel < 0.75))
            //            tau = value[2];
            //        if (wheel >= 0.75)
            //            tau = value[3];

                    
            //        //Synapse syn = new SynapseSTDP(
            //        //    start,
            //        //    dest, 
            //        //    Constants.LIQUID_TO_OUT_SYNAPTIC_GAIN,1, w);
            //        ////Constants.LIQUID_TO_OUT_THIRD_TAU, Constants.LIQUID_TO_LIQUID_DELAY_STEP,

            //        Synapse syn = new SynapseSTDP(tau,
            //                    start,
            //                    dest,
            //                    1, 1, w);
            //        //Constants.LIQUID_TO_OUT_THIRD_TAU, Constants.LIQUID_TO_LIQUID_DELAY_STEP,


            //        _liquidToOut1.AddLast(syn);

            //    }

            //_liquidToOut.AddLast(_liquidToOut1);

        }


        /// <summary>
        /// Finds the sameness neuron at the specific coordinates 
        /// </summary>
        /// <returns>The neuron at the specified coordinates</returns>
        internal SumNeuron getNeuronPersistance(int indexSameness)
        {
            foreach (SumNeuron a in _neuronPersistance)
            {
                if(a.COLUMN==indexSameness)
                    return a;
            }
            return null;
        }

        /// <summary>
        /// Finds the sameness neuron at the specific coordinates 
        /// </summary>
        /// <returns>The neuron at the specified coordinates</returns>
        internal NeuronMorrisLecar getNeuronMorris(int indexSameness)
        {
            foreach (NeuronMorrisLecar a in _neurons)
            {
                if (a.COLUMN == indexSameness)
                    return a;
            }
            return null;
        }

        /// <summary>
        /// Finds the neuron at the specific coordinates in the output of the Liquid
        /// </summary>
        /// <returns>The neuron at the specified coordinates</returns>
        internal SumNeuron getOutNeuronFirst()
        {
            SumNeuron dest = new SumNeuron();
            foreach (SumNeuron a in _neuronSum){
                dest = a;
                return dest;
            }
            return null;
        }

        public int getWinnerNeuron() {

            //int spikemax = 0;// Constants.MORRIS_WINNER_SPIKE;
            int indNeuron = -1;
            foreach (NeuronMorrisLecar n in _neurons) {
                double spikemax = getMorrisThreshold(n.COLUMN);
                if (n.countSpike > spikemax)
                {
                    spikemax = n.countSpike;
                    indNeuron = n.COLUMN;
                }
            }
            return indNeuron;
        }

        /// <summary>
        /// Simulate one Morris Lecar of the output layer
        /// </summary>
        /// <param name="step"></param>
        /// <param name="target"></param>
        /// <param name="Z"></param>
        public int simulateMorrisLecar(int step, int target, double Z) {

            int cSpike = 0;
                foreach (NeuronMorrisLecar m in _neurons) {
                    if (m.COLUMN == target)
                    {
                        m.I_Sum = Z;
                        m.simulate(step);
                        cSpike = m.countSpike;
                        return cSpike;
                    }
                }
           return -1;
        }


        /// <summary>
        /// Simulate the output layer
        /// </summary>
        /// <param name="step"></param>
        /// <param name="log"></param>
        public void simulate(int step, StateLogger log, int simNumberInternal, bool integration) {

            foreach (SumNeuron n in _neuronSum)
            {
                n.simulate(step);                 //scelgo questo o l'opzione sotto in base al fatto che voglio o no un neurone sommatore non lineare.
                //n.simulateSquare(step);
                if (log != null)
                    log.logNeuron(step, n);

                foreach (NeuronMorrisLecar m in _neurons) {
                    if (m.COLUMN == n.COLUMN)
                    {
                        m.I_Sum = n.V;
                        m.simulate(step);
                        if (log != null)
                            log.logNeuronMorrisLecar(step, m);
                    }
                }

            }

            if (simNumberInternal == -1)
            {
                foreach (SynapseSameness s in _morrisToPersistance)
                {
                    s.simulate(step);
                    if (log != null)
                        log.logSynapse(s, step);
                }

                foreach (SynapseSameness s in _morrisToSameness)
                {
                    s.simulate(step);
                    if (log != null)
                        log.logSynapse(s, step);
                }

                foreach (SumNeuron n in _neuronPersistance)
                {
                    n.simulateSameness(step, integration);
                    if (log != null)
                        log.logNeuron(step, n);
                }

                foreach (SumNeuron n in _neuronSameness)
                {
                    n.simulateSameness(step, true);
                    if (log != null)
                        log.logNeuron(step, n);
                }
            }
           
        }

        public SumNeuron addNeuron() {
            NeuronMorrisLecar m = new NeuronMorrisLecar(matrixKI[index, 0], matrixKI[index,1]);
            m.setCoord(1, index, LayerNumbers.OUTPUT_LAYER);

            SumNeuron n = new SumNeuron(0, matrixKI[index, 1]);
            n.setCoord(0, index, LayerNumbers.OUTPUT_LAYER);

            SumNeuron o = new SumNeuron();
            o.setCoord(2, index, LayerNumbers.OUTPUT_LAYER);

            SumNeuron p = new SumNeuron(Constants.MORRIS_TO_SAMENESS_DECAY,Constants.MORRIS_TO_SAMENESS_GAIN);
            p.setCoord(3, index, LayerNumbers.OUTPUT_LAYER);

            Synapse syn = new SynapseSameness(m, o, Constants.MORRIS_TO_SAMENESS_W / matrixf[index],
                                    Constants.MORRIS_TO_SAMENESS_TAU,
                                    Constants.MORRIS_TO_SAMENESS_DELAY_STEP,
                                    Constants.MORRIS_TO_SAMENESS_SYNAPTIC_GAIN);
            
            _morrisToPersistance.AddLast(syn);

            Synapse syn2 = new SynapseSameness(m, p, Constants.MORRIS_TO_SAMENESS_W / matrixf[index],
                                    Constants.MORRIS_TO_SAMENESS_TAU1,
                                    Constants.MORRIS_TO_SAMENESS_DELAY_STEP,
                                    Constants.MORRIS_TO_SAMENESS_SYNAPTIC_GAIN);

            _morrisToSameness.AddLast(syn2);

            index = index + 1;
            _neurons.AddLast(m);
            _neuronSum.AddLast(n);
            _neuronSameness.AddLast(p);
            _neuronPersistance.AddLast(o);

            return n;

        }

        public void resetState() {
            foreach (NeuronMorrisLecar n in _neurons)
                n.resetState();

            foreach (SumNeuron n in _neuronSum)
                n.resetState();
            
        
        }

        public double getMorrisThreshold(int target) {
            foreach (NeuronMorrisLecar m in _neurons)
            {
                if (m.COLUMN == target)
                {
                    //double calculus = matrixf[target] / 2 + 1;
                    double threshold = Math.Round(matrixf[target] / 2, 0, MidpointRounding.ToEven) + 1;
                    return threshold;
                }
            }
            return Double.MaxValue;     //cioè faccio ritornare infinito se non trova nessun MorrisLecar con quel Target in modo tale che la simulate Test non esce indice che c'è un errore da qualche parte perchè si cerca un morrislecar che non esiste
        
        }

        public double getMatrixf(int index) {
            if (index > matrixf.Length-1)
                return -1;
            else
                return matrixf[index];
        }

    }
}
