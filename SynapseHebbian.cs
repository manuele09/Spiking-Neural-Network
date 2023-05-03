using System.Collections.Generic;

namespace SLN
{
    /// <summary>
    /// A synapse with Hebbian learning
    /// </summary>
    [Serializable]
    internal class SynapseHebbian : Synapse
	{
		// <summary>
		// The weight of the synapse, with upper and lower saturation
		// values defined respectively in <code>Constants.HEB_W_HI</code>
		// and <code>Constants.HEB_W_LO</code>
		// </summary>

		/// <summary>
		/// The weight of the synapse
		/// </summary>
		internal new double W
		{
			get { return _W; }

			set
			{
				_W = value;
				//_W = _W > Constants.HEB_W_HI ? Constants.HEB_W_HI : _W;
				//_W = _W < Constants.HEB_W_LO ? Constants.HEB_W_LO : _W;
				_Wsec = _W;
			}
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="start">The starting neuron</param>
		/// <param name="dest">The destination neuron</param>
		/// <param name="w">The synaptic weight</param>
		/// <param name="tau">The synapse time constant</param>
		/// <param name="delay">The synaptic delay (in steps of simulation)</param>
		/// <param name="gain">Gain in the calculation of the current</param>
		internal SynapseHebbian(Neuron start, Neuron dest, double w, double tau, int delay, double gain)
			: base(start, dest, w, tau, delay, gain)
		{ }

		/// <summary>
		/// Prints the weight of the synapse
		/// </summary>
		/// <returns>A string with the value of the synaptic weight</returns>
		public override string ToString()
		{
			string str = Start.ROW + "," + Start.COLUMN + " (" + Start.LAYER.ToString() + ")->" +
			Dest.ROW + "," + Dest.COLUMN + " (" + Dest.LAYER.ToString() + ") W = " + _W;
			return str;
		}

		/// <summary>
		/// Resets the synapse weight to initial value
		/// </summary>
		internal void resetWeight()
		{
			_W = Constants.SECOND_TO_FIRST_W;
			_Wsec = Constants.SECOND_TO_FIRST_W;
		}
	}
}
