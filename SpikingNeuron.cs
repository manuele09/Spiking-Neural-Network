using System;

namespace SLN
{
    /// <summary>
    /// An Izhikevich's spiking neuron
    /// </summary>
    [Serializable]
    internal class SpikingNeuron : Neuron
	{
		internal SpikingNeuron()
			: base()
		{
			A = Constants.A_IZH_SPK;
			B = Constants.B_IZH_SPK;
			C = Constants.C_IZH_SPK;
			D = Constants.D_IZH_SPK;
		}
	}
}
