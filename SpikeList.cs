using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace SLN
{
    /// <summary>
    /// Represents the list of the timestamps of the neuron's spikes
    /// </summary>
    [Serializable]
    public class SpikeList
    {
        public const int MaxSpikes = 200;
        public int[] _spikelist;
        //L'indice sarà la timestamp (modulata)
        //Il contenuto saranno le timestamps in cui si sono verificati
        //gli spikes, -1 se non si è verificato.

        //
        /// <summary>
        /// Constructor
        /// </summary>
        public SpikeList()
        {
            _spikelist = new int[MaxSpikes];
            for (int i = 0; i < MaxSpikes; i++)
                _spikelist[i] = -1;
        }

        /// <summary>
        /// Adds a spike to the list
        /// </summary>
        /// <param name="tstamp">The timestamp (i.e. the simulation step)
        /// in which the spike was triggered</param>
        internal void addSpike(int step, bool is_spike)
        {
            if (is_spike)
                _spikelist[step % MaxSpikes] = step;
            else
                _spikelist[step % MaxSpikes] = -1;

        }

        /// <summary>
        /// The number of the spikes the neuron fired
        /// </summary>
        internal int NSpikes
        {
            get { return MaxSpikes; }
        }

        /// <summary>
        /// Returns the <i>n</i>-th element of the list (beginning from 0)
        /// </summary>
        /// <param name="n">The zero-based index of the element to be retrieved</param>
        /// <returns>The element at the <i>n</i>-th position (beginning from 0) in the list</returns>
        internal int getSpikeAt(int n)
        {//da togliere
            return _spikelist[n];
        }

        internal int getLastSpike(int step)
        {
            int last_spike = -1;
            for (int i = 0; i < MaxSpikes; i++)
            {
                if (_spikelist[i] > last_spike && _spikelist[i] <= step)
                {

                    last_spike = _spikelist[i];
                   // Console.WriteLine(last_spike);
                }
                //if (_spikelist[i] > -1)
                  // Console.Write(_spikelist[i] + ", step=" + step + " | ");
            }
           // Console.WriteLine();
            return last_spike;
        }

    }
}
