using System;
using System.Collections.Generic;
using System.Text;

namespace SLN
{
    [Serializable]
    class OutputLayerExternal
    {
        private LinkedList<Neuron> _neurons; //morris lecar
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
        private void init()
        {
            _neurons = new LinkedList<Neuron>();
            _neuronSum = new LinkedList<SumNeuron>();
            _neuronPersistance = new LinkedList<SumNeuron>();
            _neuronSameness = new LinkedList<SumNeuron>();

            matrixKI = new double[Constants.CLASSES, 2] { { 0.055, 61.5 },/* {0.067, 61.45}, {0.074, 61.4}, {0.082, 61.35}, {0.095, 61.25}, */{ 0.105, 61.2 }, { 0.155, 60.8 }, { 0.2, 60.5 } };
            matrixf = new double[Constants.CLASSES] { 5, 10, 15, 20 }; //matrice delle frequenze


            Neuron m = new Neuron();
            m.A = 0.1;
            m.B = 0.26;
            m.C = -65;
            m.D = 8;
            m.setCoord(1, index, LayerNumbers.OUTPUT_LAYER);
            _neurons.AddLast(m);

            SumNeuron n = new SumNeuron(0, matrixKI[index, 1]);  //uso la variabile gain nel caso del sommatore per dire a che valore deve saturare l'ingresso (in pratico stabilisco il valore della saturazione della funzione di heaviside
            n.setCoord(0, index, LayerNumbers.OUTPUT_LAYER);
            _neuronSum.AddLast(n);

            SumNeuron o = new SumNeuron();
            o.setCoord(2, index, LayerNumbers.OUTPUT_LAYER);
            _neuronPersistance.AddLast(o);

            SumNeuron p = new SumNeuron(Constants.MORRIS_TO_SAMENESS_DECAY, Constants.MORRIS_TO_SAMENESS_GAIN);
            p.setCoord(3, index, LayerNumbers.OUTPUT_LAYER);
            _neuronSameness.AddLast(p);



            _morrisToPersistance = new LinkedList<Synapse>();
            _morrisToSameness = new LinkedList<Synapse>();

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
        }


        /// <summary>
        /// Finds the sameness neuron at the specific coordinates 
        /// </summary>
        /// <returns>The neuron at the specified coordinates</returns>
        internal SumNeuron getNeuronPersistance(int indexSameness)
        {
            foreach (SumNeuron a in _neuronPersistance)
            {
                if (a.COLUMN == indexSameness)
                    return a;
            }
            return null;
        }

        /// <summary>
        /// Finds the sameness neuron at the specific coordinates 
        /// </summary>
        /// <returns>The neuron at the specified coordinates</returns>
        internal Neuron getNeuronMorris(int indexMorris)
        {
            foreach (Neuron a in _neurons)
            {
                if (a.COLUMN == indexMorris)
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
            foreach (SumNeuron a in _neuronSum)
            {
                dest = a;
                return dest;
            }
            return null;
        }

        public int getWinnerNeuron()
        {
            int indNeuron = -1;
            foreach (Neuron n in _neurons)
            {
                double spikemax = getMorrisThreshold(n.COLUMN);
                if (n.NSpikes > spikemax)
                {
                    spikemax = n.NSpikes;
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
        public int simulateMorrisLecar(int step, int target, double Z)
        {

            int cSpike = 0;
            foreach (Neuron m in _neurons)
            {
                if (m.COLUMN == target)
                {
                    m.I = Z;
                    m.simulate(step);
                    cSpike = m.NSpikes;
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
        public void simulate(int step, StateLogger log, int simNumberInternal, bool integration)
        {

            foreach (SumNeuron n in _neuronSum)
            {
                n.simulate(step);                 //scelgo questo o l'opzione sotto in base al fatto che voglio o no un neurone sommatore non lineare.
                //n.simulateSquare(step);
                if (log != null)
                    log.logNeuron(step, n);

                foreach (Neuron m in _neurons)
                {
                    if (m.COLUMN == n.COLUMN)
                    {
                        m.I = n.V * 1000;
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

        public SumNeuron addNeuron()
        {
            Neuron m = new Neuron();
            m.A = 0.1;
            m.B = 0.26;
            m.C = -65;
            m.D = 8;
            m.setCoord(1, index, LayerNumbers.OUTPUT_LAYER);

            SumNeuron n = new SumNeuron(0, matrixKI[index, 1]);
            n.setCoord(0, index, LayerNumbers.OUTPUT_LAYER);

            SumNeuron o = new SumNeuron();
            o.setCoord(2, index, LayerNumbers.OUTPUT_LAYER);

            SumNeuron p = new SumNeuron(Constants.MORRIS_TO_SAMENESS_DECAY, Constants.MORRIS_TO_SAMENESS_GAIN);
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

        public void resetState()
        {
            foreach (Neuron n in _neurons)
                n.resetState();

            foreach (SumNeuron n in _neuronSum)
                n.resetState();


        }

        public double getMorrisThreshold(int target)
        {
            //potremmo usare un vettore di treshold
            foreach (Neuron m in _neurons)
                if (m.COLUMN == target)
                    return matrixf[target];

            return Double.MaxValue; //ritorna infinito se al target non corrisponde un neurone
        }

        public double getMatrixf(int index)
        {
            if (index > matrixf.Length - 1)
                return -1;
            else
                return matrixf[index];
        }

    }
}
