using System;
using System.Collections.Generic;

namespace SLN
{
    /// <summary>
    /// A "fake" implementation of a neuron, where all spikes have already
    /// been calculated for a certain number of simulation steps
    /// </summary>
    [Serializable]
    internal class FakeNeuron : Neuron
	{
		/// <summary>
		/// Constructor (with definition of the spikelist and starting time)
		/// </summary>
		/// <param name="spkList">The list of spikes of each simulation step</param>
		/// <param name="startingTime">The first simulation step we want
		/// to consider in the spikelist</param>
		internal FakeNeuron(SpikeList spkList, int startingTime)
		{
			_spikeList = spkList;
			int ind = _spikeList.FindLastIndex(
				delegate(int spk)
				{
					return spk < startingTime;
				}
			);

			_spikeList.RemoveRange(0, ind + 1);
		}

		/// <summary>
		/// Constructor (with empty spikelist)
		/// </summary>
		internal FakeNeuron()
			: base()
		{ }

		/// <summary>
		/// Prints the spikelist of the neuron
		/// </summary>
		/// <returns>A string representation of the spikelist</returns>
		public override string ToString()
		{
			return _spikeList.ToString();
		}

		/// <summary>
		/// Creates an array of "fake" neurons from an array of spike lists
		/// </summary>
		/// <param name="spikeArray">An array of <code>SpikeList</code> objects</param>
		/// <param name="startingTime">The first simulation step we want
		/// to consider in the spikelist</param>
		/// <returns>An array of <code>FakeNeuron</code> objects with the same dimensions
		/// of the input spikelist array</returns>
		internal static void updateFakeLayer(FakeNeuron[,] fakeLayer, SpikeList[,] spikeArray)
		{
			int iDim = fakeLayer.GetLength(0);
			int jDim = fakeLayer.GetLength(1);

			for (int i = 0; i < iDim; i++)
				for (int j = 0; j < jDim; j++)
					fakeLayer[i, j].setSpikeList(spikeArray[i, j]);
		}

		/// <summary>
		/// Sets the spikelist of the neuron
		/// </summary>
		/// <param name="spikeList">The spikelist</param>
		private void setSpikeList(SpikeList spikeList)
		{
			_spikeList = spikeList;
		}
	}
}
