using System;
using System.Collections.Generic;

namespace SLN
{
    /// <summary>
    /// Represents the list of the timestamps of the neuron's spikes
    /// </summary>
    [Serializable]
    public class SpikeListOld : List<int>
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public SpikeListOld()
			: base()
		{ }

		/// <summary>
		/// Adds a spike to the list
		/// </summary>
		/// <param name="tstamp">The timestamp (i.e. the simulation step)
		/// in which the spike was triggered</param>
		internal void addSpike(int tstamp)
		{
			this.Add(tstamp);
		}

		/// <summary>
		/// The number of the spikes the neuron fired
		/// </summary>
		internal int NSpikes
		{
			get { return this.Count; }
		}

		/// <summary>
		/// Returns the <i>n</i>-th element of the list (beginning from 0)
		/// </summary>
		/// <param name="n">The zero-based index of the element to be retrieved</param>
		/// <returns>The element at the <i>n</i>-th position (beginning from 0) in the list</returns>
		internal int getSpikeAt(int n)
		{
			return this[n];
		}

		/// <summary>
		/// Prints a representation of the spikelist
		/// </summary>
		/// <returns>A <code>string</code> object containing a space-separated
		/// list of the values</returns>
		public override string ToString()
		{
			string str = "";

			foreach (int spk in this)
				str += spk + " ";

			return str;
		}
	}
}
