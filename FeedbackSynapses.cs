using System;
using System.Collections.Generic;

namespace SLN
{
    /// <summary>
    /// Feedback synapses created between the two SOSLs.
    /// Used only in phase B and from the 2nd simulation.
    /// </summary>


    [Serializable]
    internal class FeedbackSynapses : List<Synapse>
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="win1old">The winner of SOSL #1 at t-1</param>
		/// <param name="win1curr">The winner of SOSL #1 at t</param>
		/// <param name="win2">The winner of SOSL #2</param>
		internal FeedbackSynapses(WinnerCluster win1old, WinnerCluster win1curr, WinnerCluster win2)
			: base()
		{
			#region SOSL #1 -> SOSL #2
			foreach (Neuron start in win1old)
				foreach (Neuron dest in win2)
				{
					Synapse s = new Synapse(
						start,
						dest,
						Constants.FEEDBACK_W,
						Constants.FEEDBACK_TAU,
						Constants.FEEDBACK_DELAY_STEP,
						Constants.FEEDBACK_SYNAPTIC_GAIN);

					this.Add(s);
				}
			#endregion

			#region SOSL #2 -> SOSL #1
			foreach (Neuron start in win2)
				foreach (Neuron dest in win1curr)
				{
					Synapse s = new Synapse(
						start,
						dest,
						Constants.FEEDBACK_W,
						Constants.FEEDBACK_TAU,
						Constants.FEEDBACK_DELAY_STEP,
						Constants.FEEDBACK_SYNAPTIC_GAIN);

					this.Add(s);
				}
			#endregion
		}

		/// <summary>
		/// Simulate the feedback synapses
		/// </summary>
		/// <param name="step">the simulation step</param>
		internal void simulate(int step)
		{
			foreach (Synapse s in this)
				s.simulate(step);
		}
	}
}
