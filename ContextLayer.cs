using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLN
{
    [Serializable]
    class ContextLayer
    {
        /// <summary>
        /// numero massimo di elementi in una sequenza
        /// </summary>
        //private int dimension = 6; 

        /// <summary>
        /// epoca attiva del contesto
        /// </summary>
        private int t;

        /// <summary>
        /// contatore univoco neuroni contesto
        /// </summary>
        private int count_id;

        /// <summary>
        /// neuroni vincitori per ogni anello nell'epoca precedente
        /// </summary>
        public AltNeuron[] winContextOld;

        public int[] valid;


        LinkedList<AltNeuron>[] neurons = new LinkedList<AltNeuron>[Constants.RINGS];
        // LinkedList<Synapse> _ringToring;
        LinkedList<Synapse> _morrisToRing;
        LinkedList<SynapseSTDP> _ringToOtherRingSTDP;
        LinkedList<Synapse> _ringToOtherRing;
        OutputLayerExternal outlayer;
        Layers inputlayer;
        AltNeuron[] _endSequenceNeurons;
        AltNeuron[] _motorNeurons;
        LinkedList<SynapseSTDP> _contextToEndSequence;
        LinkedList<SynapseSTDP> _contextToMotor;
        LinkedList<SynapseSTDP> _morrisToMotor;
        LinkedList<Synapse> _motorToMotor;

        LinkedList<SynapseSTDP> _ringToMorris;
        LinkedList<SynapseSTDP> _ringToInput;

        /// <summary>
        /// Constructor
        /// </summary>
        internal ContextLayer(OutputLayerExternal o, Layers l)
        {

            this.init(o, l);

        }

        private void init(OutputLayerExternal o, Layers l)
        {
            t = -1;
            count_id = -1;
            //winContextOld = null;
            winContextOld = new AltNeuron[Constants.RINGS];
            for (int i = 0; i < Constants.RINGS; i++)
                winContextOld[i] = null;
            valid = new int[Constants.RINGS + 1];                 // metto un posto in più così conservo pure il risultato dell'end sequence
            for (int i = 0; i < Constants.RINGS; i++)
                neurons[i] = new LinkedList<AltNeuron>();
            // _ringToring = new LinkedList<Synapse>();
            _ringToOtherRingSTDP = new LinkedList<SynapseSTDP>();
            _morrisToMotor = new LinkedList<SynapseSTDP>();
            _ringToOtherRing = new LinkedList<Synapse>();
            _morrisToRing = new LinkedList<Synapse>();
            _endSequenceNeurons = new AltNeuron[Constants.RINGS];
            _motorNeurons = new AltNeuron[Constants.MOTOR];
            for (int i = 0; i < Constants.RINGS; i++)
            {
                _endSequenceNeurons[i] = new AltNeuron();
                _endSequenceNeurons[i].setCoord(Constants.RINGS, i, -1, -1, LayerNumbers.CONTEXT);
            }

            for (int i = 0; i < Constants.MOTOR; i++)
            {
                _motorNeurons[i] = new AltNeuron();
                _motorNeurons[i].setCoord(1, i, -1, -1, LayerNumbers.Premotor);
            }
            _contextToEndSequence = new LinkedList<SynapseSTDP>();
            _contextToMotor = new LinkedList<SynapseSTDP>();
            _motorToMotor = new LinkedList<Synapse>();
            

            _ringToMorris = new LinkedList<SynapseSTDP>();
            outlayer = o;

            Random rnd = new Random();
            for (int i = 0; i < Constants.MOTOR; i++)
            {
                for (int j = 0; j < Constants.MOTOR; j++)
                {
                    if (i != j)
                        _motorToMotor.AddLast(new Synapse(_motorNeurons[i], _motorNeurons[j], Constants.MOTOR_TO_MOTOR_W_INH * (1 + (rnd.NextDouble() - 0.5) * Constants.NOISE_LVL),
                                   Constants.MOTOR_TO_MOTOR_TAU_INH,
                                   Constants.MOTOR_TO_MOTOR_DELAY_STEP,
                                   Constants.MOTOR_TO_MOTOR_SYNAPTIC_GAIN));

                }

            }

            _ringToInput = new LinkedList<SynapseSTDP>();
            inputlayer = l;

            


        }

        public void addPath(NeuronMorrisLecar destNeuron)
        {
            Random rnd = new Random();
            count_id++;//ogni volta che aggiungero' un neurone dovro' incrementare questo valore
            AltNeuron n = new AltNeuron();
            n.setCoord(0, count_id, -1, destNeuron.COLUMN, LayerNumbers.CONTEXT); //nomenclatura coordinate per questi neuroni anello,id_univoco,id_padre,id_target
            n.inserted = true;

            //aggiungo il neurone nel primo cerchio
            neurons[0].AddLast(n);
            // aggiungo il collegamento tra morris layer e il neurone del primo cerchio
            Synapse syn = new Synapse(destNeuron, n, Constants.MORRIS_TO_CONTEX_W_EXC * (1 + (rnd.NextDouble() - 0.5) * Constants.NOISE_LVL), Constants.THIRD_TO_THIRD_WSEC,
                          Constants.MORRIS_TO_CONTEX_TAU / outlayer.getMatrixf(destNeuron.COLUMN),
                          Constants.THIRD_TO_THIRD_DELAY_STEP,
                          Constants.THIRD_TO_THIRD_SYNAPTIC_GAIN);
            _morrisToRing.AddLast(syn);
            // aggiungo le sinapsy tra il neurone di contesto e quello di endsequence
            _contextToEndSequence.AddLast(new SynapseSTDP(n, _endSequenceNeurons[0],
                                         Constants.THIRD_TO_END_SYNAPTIC_GAIN_STDP,
                                         Constants.THIRD_TO_THIRD_SYNAPTIC_GAIN_END,
                                         Constants.STDP_CONTEXT_END_INIT_W,
                                         Constants.STDP_W_HI,
                                         Constants.STDP_CONTEXT_END_INIT_W));
            for (int i = 0; i < Constants.MOTOR; i++)
            {
                _contextToMotor.AddLast(new SynapseSTDP(n, _motorNeurons[i],
                                             Constants.CONTEXT_TO_MOTOR_SYNAPTIC_GAIN_STDP,
                                             Constants.CONTEXT_TO_MOTOR_SYNAPTIC_GAIN,
                                             Constants.STDP_CONTEXT_MOTOR_INIT_W,
                                             Constants.STDP_W_HI,
                                             Constants.STDP_CONTEXT_MOTOR_INIT_W));
            }
            //aggiungo le synapsi tra i neuroni del context e i morris Lecar
            foreach (AltNeuron m in neurons[0])
            {
                if (m.inserted)
                {
                    for (int i = 0; i <= destNeuron.COLUMN; i++)
                        _ringToMorris.AddLast(new SynapseSTDP(m, outlayer.getNeuronMorris(i),
                                          Constants.CONTEXT_TO_MORRIS_GAIN_STDP,
                                          Constants.CONTEXT_TO_MORRIS_GAIN, Constants.CONTEXT_TO_MORRIS_Weight));

                    for (int i = 0; i < Constants.FIRST_LAYER_DIMENSION_I; i++)
                        for (int j = 0; j < Constants.FIRST_LAYER_DIMENSION_J; j++)
                            _ringToInput.AddLast(new SynapseSTDP(m, inputlayer.getFirstLayerNeuron1(i, j), 
                                Constants.CONTEXT_TO_MORRIS_GAIN_STDP,
                                Constants.CONTEXT_TO_INPUT_GAIN, 
                                Constants.CONTEXT_TO_MORRIS_Weight));
                }
                else
                    _ringToMorris.AddLast(new SynapseSTDP(m, destNeuron,
                                         Constants.CONTEXT_TO_MORRIS_GAIN_STDP,
                                         Constants.CONTEXT_TO_MORRIS_GAIN, Constants.CONTEXT_TO_MORRIS_Weight));
            }


            // procedo ad inserire i neuroni in tutti gli anelli rimanenti
            //AltNeuron nprev = new AltNeuron();
            //nprev = n;
            //sembra che inserted=true ce l'hanno solo i neuroni inseriti ora.
            //quelli già inseriti ce l'hanno false.
            for (int i = 1; i < Constants.RINGS; i++)
            {
                foreach (AltNeuron n1 in neurons[i - 1])
                {
                    if (n1.inserted) //se n1 l'ho inserito solo ora devo creare TUTTO l'alber
                    {
                        //faccio questo ciclo for perchè ogni volta che aggiungo un neurone nel layer di output 
                        //bisogna aggiungere i neuroni di contesto riferiti a quest'utimo elemento anche per tutti i neuroni associati agli elementi precedenti
                        for (int j = 0; j <= destNeuron.COLUMN; j++)
                        {
                            count_id++;
                            AltNeuron ns = new AltNeuron();
                            ns.setCoord(i, count_id, n1.COLUMN, j, LayerNumbers.CONTEXT);
                            ns.inserted = true;
                            neurons[i].AddLast(ns);
                            //creo il collegamento tra il neurone  e quello di EndSequence
                            _contextToEndSequence.AddLast(new SynapseSTDP(ns, _endSequenceNeurons[i],
                                         Constants.THIRD_TO_END_SYNAPTIC_GAIN_STDP,
                                         Constants.THIRD_TO_THIRD_SYNAPTIC_GAIN_END,
                                         Constants.STDP_CONTEXT_END_INIT_W,
                                         Constants.STDP_W_HI,
                                         Constants.STDP_CONTEXT_END_INIT_W));

                            for (int z = 0; z < Constants.MOTOR; z++)
                            {
                                _contextToMotor.AddLast(new SynapseSTDP(ns, _motorNeurons[z],
                                                             Constants.CONTEXT_TO_MOTOR_SYNAPTIC_GAIN_STDP,
                                                             Constants.CONTEXT_TO_MOTOR_SYNAPTIC_GAIN,
                                                             Constants.STDP_CONTEXT_MOTOR_INIT_W,
                                                             Constants.STDP_W_HI,
                                                             Constants.STDP_CONTEXT_MOTOR_INIT_W));
                            }

                            // creo il collegamento dei neuroni nell'anello con il morrisLecar corrispettivo
                            _morrisToRing.AddLast(new Synapse(outlayer.getNeuronMorris(j), ns, Constants.MORRIS_TO_CONTEX_W_EXC * (1 + (rnd.NextDouble() - 0.5) * Constants.NOISE_LVL), Constants.THIRD_TO_THIRD_WSEC,
                                  Constants.MORRIS_TO_CONTEX_TAU / outlayer.getMatrixf(j),
                                  Constants.THIRD_TO_THIRD_DELAY_STEP,
                                  Constants.THIRD_TO_THIRD_SYNAPTIC_GAIN));

                            // creo il collegamento dei neuroni in un anello con quello precedente
                            _ringToOtherRingSTDP.AddLast(new SynapseSTDP(n1, ns,
                                        Constants.THIRD_TO_THIRD_SYNAPTIC_GAIN_STDP,
                                        Constants.THIRD_TO_THIRD_SYNAPTIC_GAIN));
                            _ringToOtherRing.AddLast(new Synapse(n1, ns, Constants.THIRD_TO_THIRD_W_EXC * (1 + (rnd.NextDouble() - 0.5) * Constants.NOISE_LVL), Constants.THIRD_TO_THIRD_WSEC,
                                  Constants.THIRD_TO_THIRD_TAU,
                                  Constants.THIRD_TO_THIRD_DELAY_STEP,
                                  Constants.THIRD_TO_THIRD_SYNAPTIC_GAIN));
                        }
                        n1.inserted = false;
                    }
                    else
                    {
                        count_id++;
                        AltNeuron ns = new AltNeuron();
                        ns.setCoord(i, count_id, n1.COLUMN, destNeuron.COLUMN, LayerNumbers.CONTEXT);
                        ns.inserted = true;
                        neurons[i].AddLast(ns);
                        //creo il collegamento tra il neurone  e quello di EndSequence
                        _contextToEndSequence.AddLast(new SynapseSTDP(ns, _endSequenceNeurons[i],
                                     Constants.THIRD_TO_END_SYNAPTIC_GAIN_STDP,
                                     Constants.THIRD_TO_THIRD_SYNAPTIC_GAIN_END,
                                     Constants.STDP_CONTEXT_END_INIT_W,
                                     Constants.STDP_W_HI,
                                     Constants.STDP_CONTEXT_END_INIT_W));

                        for (int z = 0; z < Constants.MOTOR; z++)
                        {
                            _contextToMotor.AddLast(new SynapseSTDP(ns, _motorNeurons[z],
                                                         Constants.CONTEXT_TO_MOTOR_SYNAPTIC_GAIN_STDP,
                                                         Constants.CONTEXT_TO_MOTOR_SYNAPTIC_GAIN,
                                                         Constants.STDP_CONTEXT_MOTOR_INIT_W,
                                                         Constants.STDP_W_HI,
                                                         Constants.STDP_CONTEXT_MOTOR_INIT_W));
                        }
                        // creo il collegamento dei neuroni nell'anello con il morrisLecar corrispettivo
                        _morrisToRing.AddLast(new Synapse(destNeuron, ns, Constants.MORRIS_TO_CONTEX_W_EXC * (1 + (rnd.NextDouble() - 0.5) * Constants.NOISE_LVL), Constants.THIRD_TO_THIRD_WSEC,
                              Constants.MORRIS_TO_CONTEX_TAU / outlayer.getMatrixf(destNeuron.COLUMN),
                              Constants.THIRD_TO_THIRD_DELAY_STEP,
                              Constants.THIRD_TO_THIRD_SYNAPTIC_GAIN));

                        // creo il collegamento dei neuroni in un anello con quello precedente
                        _ringToOtherRingSTDP.AddLast(new SynapseSTDP(n1, ns,
                                    Constants.THIRD_TO_THIRD_SYNAPTIC_GAIN_STDP,
                                    Constants.THIRD_TO_THIRD_SYNAPTIC_GAIN));
                        _ringToOtherRing.AddLast(new Synapse(n1, ns, Constants.THIRD_TO_THIRD_W_EXC * (1 + (rnd.NextDouble() - 0.5) * Constants.NOISE_LVL), Constants.THIRD_TO_THIRD_WSEC,
                              Constants.THIRD_TO_THIRD_TAU,
                              Constants.THIRD_TO_THIRD_DELAY_STEP,
                              Constants.THIRD_TO_THIRD_SYNAPTIC_GAIN));
                    }
                }


                //aggiungo le synapsi tra i neuroni del context e i morris Lecar
                foreach (AltNeuron m in neurons[i])
                {
                    if (m.inserted) //solo gli ultimi neuroni aggiunti del ring in questione
                    {
                        for (int k = 0; k <= destNeuron.COLUMN; k++)
                            _ringToMorris.AddLast(new SynapseSTDP(m, outlayer.getNeuronMorris(k),
                                              Constants.CONTEXT_TO_MORRIS_GAIN_STDP,
                                              Constants.CONTEXT_TO_MORRIS_GAIN, Constants.CONTEXT_TO_MORRIS_Weight));

                        for (int k = 0;k  < Constants.FIRST_LAYER_DIMENSION_I; k++)
                            for (int j = 0; j < Constants.FIRST_LAYER_DIMENSION_J; j++)
                                _ringToInput.AddLast(new SynapseSTDP(m, inputlayer.getFirstLayerNeuron1(k, j),
                                    Constants.CONTEXT_TO_MORRIS_GAIN_STDP,
                                    Constants.CONTEXT_TO_INPUT_GAIN,
                                    Constants.CONTEXT_TO_MORRIS_Weight));
                    }
                    else
                        _ringToMorris.AddLast(new SynapseSTDP(m, destNeuron,
                                             Constants.CONTEXT_TO_MORRIS_GAIN_STDP,
                                             Constants.CONTEXT_TO_MORRIS_GAIN, Constants.CONTEXT_TO_MORRIS_Weight));

                }
            }

            // _ringToring = new LinkedList<Synapse>();  //cancello tutte le sinapsi e le ricreo per ragioni di facilità di codice altrimenti ogni volta 
            //bisognerebbe aggiungere solo le sinapsi dai neuroni esistenti a questo aggiunto e poi le sinapsy da questo neurone a tutti gli altri esistenti
            //creo le sinapsi inibitorie all'interno dei neuroni dello stesso anello
            /*for (int i = 0; i < Constants.RINGS; i++)
            {
                foreach (AltNeuron n1 in neurons[i])
                {
                    foreach (AltNeuron n2 in neurons[i])
                    {
                        if (n1.COLUMN != n2.COLUMN)
                        {
                            _ringToring.AddLast(new Synapse(n1, n2, Constants.THIRD_TO_THIRD_W_INH * (1 + (rnd.NextDouble() - 0.5) * Constants.NOISE_LVL),
                          Constants.THIRD_TO_THIRD_TAU_INH,
                          Constants.THIRD_TO_THIRD_DELAY_STEP,
                          Constants.THIRD_TO_THIRD_SYNAPTIC_GAIN));
                        }
                    }

                }

            }*/

            for (int i = 0; i < Constants.MOTOR; i++)
            {
                _morrisToMotor.AddLast(new SynapseSTDP(destNeuron, _motorNeurons[i], Constants.MORRIS_TO_MOTOR_SYNAPTIC_GAIN_STDP, Constants.MORRIS_TO_MOTOR_SYNAPTIC_GAIN,
                              Constants.MORRIS_MOTOR_INIT_W_STDP,
                              Constants.MORRIS_MOTOR_MAX_W_STDP,
                              Constants.MORRIS_MOTOR_INIT_W_STDP,
                              Constants.MORRIS_TO_MOTOR_TAU / outlayer.getMatrixf(destNeuron.COLUMN)));
            }

        }


        /// <summary>
        /// Retrieves the winner neuron of the contex layer 
        /// </summary>
        /// <param name="lastTimestamp">Last timestamp (i.e. integration step) on which
        /// calculate the frequency</param>
        /// <returns>The winner neurons for each ring of the context layer</returns>
        internal AltNeuron[] getWinnerContext(int lastTimestamp)
        {
            double winnerFreq = 0;
            AltNeuron[] nWin = new AltNeuron[Constants.RINGS];
            for (int i = 0; i < Constants.RINGS; i++)
            {
                winnerFreq = 0;
                foreach (AltNeuron n1 in neurons[i])
                {
                    double freq = n1.NSpikes;
                    if (freq > winnerFreq)
                    {
                        nWin[i] = n1;
                        winnerFreq = freq;
                        valid[i] = n1.COLUMN;
                    }
                }
                if (winnerFreq == 0)
                {
                    nWin[i] = null;
                    valid[i] = -1;
                }
            }

            // ritorno un array in cui in ogni posizione c'è il neurone vincente del corrispettivo anello
            return nWin;
        }

        //come la funzione di sopra restituisce una lista di neuroni vincitori.
        //in aggiunta l'opzione permette di settare i campi is_winner_old/now di
        //tutti i neuoroni del contesto. (option 1 e 2).
        internal AltNeuron[] getWinnerContext(int lastTimestamp, int option)
        {
            double winnerFreq = 0;
            AltNeuron[] nWin = new AltNeuron[Constants.RINGS];
            for (int i = 0; i < Constants.RINGS; i++)
            {
                winnerFreq = 0;
                foreach (AltNeuron n1 in neurons[i])
                {
                    if (option == 1)
                        n1.is_winner_old = false;
                    if (option == 2)
                        n1.is_winner_now = false;
                    double freq = n1.NSpikes;
                    if (freq > winnerFreq)
                    {
                        nWin[i] = n1;
                        winnerFreq = freq;
                        valid[i] = n1.COLUMN;
                    }
                }
                if (winnerFreq == 0)
                {
                    nWin[i] = null;
                    valid[i] = -1;
                }
            }


            if (option == 1)
                for (int j = 0; j < Constants.RINGS; j++)
                    if (nWin[j] != null)
                        nWin[j].is_winner_old = true;
            if (option == 2)
                for (int j = 0; j < Constants.RINGS; j++)
                    if (nWin[j] != null)
                        nWin[j].is_winner_now = true;

            return nWin;
        }

        internal int getWinnerMotor(int lastTimestamp)
        {
            double winnerFreq = 0;
            int nWin = -1;
            for (int i = 0; i < Constants.MOTOR; i++)
            {
                double freq = _motorNeurons[i].NSpikes;
                if (freq > winnerFreq)
                {
                    nWin = _motorNeurons[i].COLUMN;
                    winnerFreq = freq;

                }
            }

            if (winnerFreq == 0)
            {
                nWin = -1;

            }


            return nWin;
        }


        public void simulate(int step, StateLogger log, StateLogger logSTDP, int sim_number, int simNumberInternal, bool end, bool test, int level, int motor)
        {
            //Modifiche da fare:
            //0) Il logging non deve essere fatto all'interno dei Parallel per i conflitti.
            //  Si potrebbero fare 3 cose: 
            //      -si possono fare in modo sequenziale dopo i parallel;
            //      -si potrebbe modificare la classe dei Log in modo tale da avere
            //      una struttura che contenga un log per ogni sinapsi/neurone invece che
            //      avere un log per tutti gli elementi. In questo modo non si dovrebbero
            //      avere conflitti nella parallelizzazione.
            //      -utilizzo di una ConcurrentBag, da usare nei Parallel. Cioè nella classe
            //      StateLogger dovrà essere usata questa struttara invece della LinkedList.
            //      Questa forse è la modifica meno radicale, ma anche la più efficente perchè conserva il parallelismo
            //2) Utilizzare la Gpu.
            //3) Approfondire la libreria intel mkl
            //AVANZAMENTO t (varia tra 0 e RINGS-1)
            if (step == Constants.SIMULATION_STEPS_FEEDFORWARD && simNumberInternal == -1)
            {
                //inizialmente t=-1
                t = (t + 1) % Constants.RINGS;
                if (t == 0)
                {
                    endSequence();
                    t = 0;
                }
            }

            //CORRENTE DI BIAS (vincitori epoca precedente)
            for (int i = 0; i < t; i++)
                if (winContextOld[i] != null)
                    winContextOld[i].I = 10 * Math.Exp((Constants.SIMULATION_STEPS_FEEDFORWARD - step) * Constants.INTEGRATION_STEP / (400));
            //La Corrente di Bias non deve essere troppo grande, altrimenti questi neuroni prevarranno sempre, anche 
            //sul nuovo input dell'epoca corrente, risultando così eternamente vincitori.

            //La parallelizzazione potrebbe essere inefficente perché tra tutti i neuroni di un ring ne 
            //verrà simulato solo uno, gli altri si gireranno i pollici.


            //END e MOTOR REWARDS
            if (!test)
            {
                if (end && step > Constants.SIMULATION_STEPS_FEEDFORWARD + 500)
                {
                    for (int i = 0; i <= t; i++)
                    {
                        switch (level)
                        {
                            case 1:
                                _endSequenceNeurons[i].I = 18;
                                break;
                            case 2:
                                _endSequenceNeurons[i].I = 22;
                                break;
                            case 3:
                                _endSequenceNeurons[i].I = 26;
                                break;
                        }
                    }
                }
                else
                    _endSequenceNeurons[t].I = 0;

                if (motor != -1 && step > Constants.SIMULATION_STEPS_FEEDFORWARD + 500)
                {

                    for (int i = 0; i < Constants.MOTOR; i++)
                    {
                        if (i == motor)
                        {
                            _motorNeurons[i].I = 26;

                        }
                        else
                            _motorNeurons[i].I = 0;
                    }

                }
                else
                    for (int i = 0; i < Constants.MOTOR; i++)
                        _motorNeurons[i].I = 0;
            }

            //END NEURONS
            for (int i = 0; i < Constants.RINGS; i++)
            {
                _endSequenceNeurons[i].simulate(step);
                if (log != null)
                    log.logNeuron(step, _endSequenceNeurons[i]);
            }

            //MOTOR NEURONS
            for (int i = 0; i < Constants.MOTOR; i++)
            {
                _motorNeurons[i].simulate(step);
                if (log != null)
                    log.logNeuron(step, _motorNeurons[i]);
            }

            //MORRIS TO RING
            if (Constants.DEBUG == 1 && step == 999)
                Console.WriteLine("Morris to Ring: " + _morrisToRing.Count);
            foreach (Synapse s in _morrisToRing)
            {
                if (s.Dest.ROW <= t)
                    s.simulate_morris_to_ring(step, 1);
                if (log != null)
                    log.logSynapse(s, step);
            }
            //1) Se simulo i neuroni fino al ring t, non ha senso simulare le sinapsi che terminano nei rings > t
            //2) Se abbandoniamo l'idea delle sotto-sequenze si può dare l'input (fornito dai Morris-Lecar) solamente al ring t.


            //CONTEXT NEURONS (fino al ring t)
            for (int i = 0; i <= t; i++)
            {
                foreach (AltNeuron n1 in neurons[i])
                {

                    n1.simulate(step);

                }
                if (Constants.DEBUG == 1 && step == 777 && i <= t)
                    Console.WriteLine("Neuroni ring " + i + ": " + neurons[i].Count);
            }
            for (int i = 0; i < Constants.RINGS; i++)
            {
                foreach (AltNeuron n1 in neurons[i])
                {
                    if (log != null)
                        log.logNeuron(step, n1, n1.idp, n1.idt);
                }
            }

            //RING TO (same) RING (tranne vincitori)
            /*if (Constants.DEBUG == 1 && step == 999)
                Console.WriteLine("Simulo " + _ringToring.Count + " sinapsi Ring to Ring");
            Parallel.ForEach(_ringToring, s => //sono tutte sinapsi INIBITORIE
            {
                
                if (!s.Start.is_winner_old && s.Start.ROW <= t)
                {
                    s.simulate(step);
                    if (log != null)
                        log.logSynapse(s, step);
                }
            });
            if (Constants.DEBUG == 1 && step == 999)
                Console.WriteLine("Finito di simulare");*/



            //RING TO OTHER RING (solo vincitori)
            if (sim_number != 0)
            {
                foreach (Synapse s in _ringToOtherRing)
                {
                    if (s.Start.is_winner_old && s.Dest.ROW <= t)
                    {
                        s.simulate(step);
                        if (log != null)
                            log.logSynapse(s, step);
                    }
                }

                foreach (SynapseSTDP s in _ringToOtherRingSTDP)
                {
                    if (s.Start.is_winner_old && s.Dest.ROW <= t)
                    {
                        s.simulate(step);
                        if (log != null)
                            log.logSynapse(s, step);
                    }
                }
            }

            //CONTEXT TO END
            foreach (SynapseSTDP s in _contextToEndSequence)
            {
                s.simulate_context_to_end(step);
                if (logSTDP != null)
                    logSTDP.logSynapse(s, step);
            }

            //RING TO MORRIS
            if (test && t > 0)
            {
                foreach (SynapseSTDP s in _ringToMorris)
                {
                    if (s.Start == winContextOld[t - 1])
                    {
                        s.simulatecontext(step);
                        if (logSTDP != null)
                            logSTDP.logSynapse(s, step);
                    }
                }

                foreach (SynapseSTDP s in _ringToInput)
                {
                    if (s.Start == winContextOld[t - 1])
                    {
                        s.simulate(step);
                        if (logSTDP != null)
                            logSTDP.logSynapse(s, step);
                    }
                }
            }

            //CONTEXT TO MOTOR
            foreach (SynapseSTDP s in _contextToMotor)
            {
                if (s.Start.ROW == t)
                {
                    s.simulate(step);
                    if (logSTDP != null)
                        logSTDP.logSynapse(s, step);
                }
            }

            //MORRIS TO MOTOR
            foreach (SynapseSTDP s in _morrisToMotor)
            {

                s.simulate_morris_to_motor(step);
                if (logSTDP != null)
                    logSTDP.logSynapse(s, step);

            }
        }

        /// Resets the neurons' state in the context layer
        /// </summary>
        internal void resetNeuronsState()
        {

            for (int i = 0; i < Constants.RINGS; i++)
            {
                foreach (AltNeuron n1 in neurons[i])
                    n1.resetState();

                _endSequenceNeurons[i].resetState();
            }

            for (int i = 0; i < Constants.MOTOR; i++)
            {
                _motorNeurons[i].resetState();
            }

        }


        /// Restart a new sequence in the context layer
        /// </summary>
        internal void endSequence()
        {

            t = -1;
            if (winContextOld != null)
                for (int i = 0; i < Constants.RINGS; i++)
                    winContextOld[i] = null;

        }

        /// Return the frequency of the end sequence in the last ring respect the actual time
        /// </summary>
        internal double getFrequencyEndSequence(int timestamp)
        {
            return _endSequenceNeurons[t].getFrequencyEnd(timestamp);
        }


        /// Make the learning in the context layer
        /// </summary>
        internal void learn(StateLogger logSTDP, int lastTimestamp, bool end, int motor)  //si fa l'apprendimento solo tra le sinapsy che collegno il neurone vincitore all'epoca precedente con quello nell'epoca successiva ( si presuppone che i vincitori siano i neuroni giusti in ogni anello, questo sarà un punto critico!!!)
        {
            AltNeuron[] win = this.getWinnerContext(lastTimestamp, 2);
            /* for (int i = 0; i <= t; i++)
             {
                 if (winContextOld != null && winContextOld[i] != null)
                     Console.WriteLine("Vincitore Old ring " + i + ": id=" + winContextOld[i].COLUMN + ", target= " + winContextOld[i].idt + ", father= " + winContextOld[i].idp);

             }

             for (int i = 0; i <= t; i++)
             {
                 if (win != null && win[i] != null)
                     Console.WriteLine("Vincitore Now ring " + i + ": id=" + win[i].COLUMN + ", target= " + win[i].idt + ", father= " + win[i].idp);
             }*/

            foreach (SynapseSTDP s in _ringToOtherRingSTDP)
            {
                if (s.Start.is_winner_old && s.Dest.is_winner_now)
                {
                    s.learn(0, 0);
                    if (logSTDP != null)
                        logSTDP.logSynapse(s, Constants.SIMULATION_STEPS_LIQUID + Constants.SIMULATION_STEPS_FEEDFORWARD + 1);
                }

            }
            if (end)
            {
                foreach (SynapseSTDP s in _contextToEndSequence)
                {
                    if (s.Start.is_winner_now)
                    {
                        s.learn(0, 0);
                        if (logSTDP != null)
                            logSTDP.logSynapse(s, Constants.SIMULATION_STEPS_LIQUID + Constants.SIMULATION_STEPS_FEEDFORWARD + 1);
                    }

                }

            }

            if (motor != -1)
            {
                foreach (SynapseSTDP s in _contextToMotor)
                {
                    if (s.Start.is_winner_now && s.Dest.COLUMN == motor)
                    {
                        s.learn(0, 0);
                        if (logSTDP != null)
                            logSTDP.logSynapse(s, Constants.SIMULATION_STEPS_LIQUID + Constants.SIMULATION_STEPS_FEEDFORWARD + 1);
                    }
                }
            }

            if (t > 0)
            {
                foreach (SynapseSTDP s in _ringToMorris)
                {
                    if (s.Dest.COLUMN == outlayer.getWinnerNeuron() && s.Start.is_winner_old && s.Start.ROW < t)
                    {


                        s.setW(160, 50);


                        if (logSTDP != null)
                            logSTDP.logSynapse(s, Constants.SIMULATION_STEPS_LIQUID + Constants.SIMULATION_STEPS_FEEDFORWARD + 1);
                    }
                    else
                    {
                        if (s.Start.is_winner_old && s.Start.ROW < t)
                            s.setW(-160, 50);
                    }

                }

                foreach (SynapseSTDP s in _ringToInput)
                {  
                    if (inputlayer._inputs1[s.Dest.ROW, s.Dest.COLUMN] && s.Start.is_winner_old && s.Start.ROW < t)
                    {


                        s.setW(160, 50);


                        if (logSTDP != null)
                            logSTDP.logSynapse(s, Constants.SIMULATION_STEPS_LIQUID + Constants.SIMULATION_STEPS_FEEDFORWARD + 1);
                    }
                    else
                    {
                        if (s.Start.is_winner_old && s.Start.ROW < t)
                            s.setW(-160, 50); 
                    }

                }
            }

            if (motor != -1)
                foreach (SynapseSTDP s in _morrisToMotor)
                {
                    if (s.Start.COLUMN == outlayer.getWinnerNeuron())
                    {
                        if (s.Dest.COLUMN == motor)
                        {
                            //if (s.Dest.COLUMN == 2)
                            s.setW(0.2);  //2
                                          //else s.setW(10.5);

                            if (logSTDP != null)
                                logSTDP.logSynapse(s, Constants.SIMULATION_STEPS_LIQUID + Constants.SIMULATION_STEPS_FEEDFORWARD + 1);
                        }
                        else
                        {

                            s.setW(-0.2);
                            if (logSTDP != null)
                                logSTDP.logSynapse(s, Constants.SIMULATION_STEPS_LIQUID + Constants.SIMULATION_STEPS_FEEDFORWARD + 1);
                        }
                    }

                }
        }


        /// Return true if it's time for the resetting of the context layer
        /// </summary>
        public bool isEndSequence()
        {

            if (_endSequenceNeurons[t].getFrequencyEnd(Constants.SIMULATION_STEPS_FEEDFORWARD + Constants.SIMULATION_STEPS_LIQUID) > Constants.THRESHOLD_END_SEQUENCE)
            {
                valid[valid.Length - 1] = 1;
                return true;
            }
            else
            {
                valid[valid.Length - 1] = 0;
                return false;
            }

        }

        public void nullValid()
        {
            for (int i = 0; i < valid.Length; i++)
            {
                valid[i] = -1;

            }

        }


    }




}
