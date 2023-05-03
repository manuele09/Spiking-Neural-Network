using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SLN
{
    /// <summary>
    /// Saves the configuration of the network, i.e. the coordinates
    /// of the synapses between the SOSLs and the input layers and the weight
    /// of STDP synapses
    /// </summary>
    [Serializable]
    public class SynapseSaver
	{
		private bool _isEnabled;
		private string _filePath;
		private StreamWriter _sw;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="filePath">The path of the configuration file that will be written</param>
		/// <param name="enable">Set to <i>true</i> enables the saving of the configuration</param>
		public SynapseSaver(string filePath, bool enable)
		{
			_isEnabled = enable;
			_filePath = filePath;
			if (_isEnabled)
				_sw = new StreamWriter(filePath);
		}

		/// <summary>
		/// Saves the configuration of the synapses in the given list
		/// </summary>
		/// <param name="lst">The list of synapses</param>
		internal void saveSynapseConfig(IEnumerable<Synapse> lst)
		{
			if (_isEnabled)
				foreach (Synapse syn in lst)
				{
					int layer = (int)syn.Start.LAYER;
					int stRow = syn.Start.ROW;
					int stCol = syn.Start.COLUMN;
					int deRow = syn.Dest.ROW;
					int deCol = syn.Dest.COLUMN;
					double wt = syn.W;

					_sw.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}",
						layer, stRow, stCol, deRow, deCol, wt);
				}
		}

		/// <summary>
		/// Saves the configuration of the given synapse
		/// </summary>
		internal void saveSynapseConfig(Synapse syn)
		{
			if (_isEnabled)
			{
				int layer = (int)syn.Start.LAYER;
				int stRow = syn.Start.ROW;
				int stCol = syn.Start.COLUMN;
				int deRow = syn.Dest.ROW;
				int deCol = syn.Dest.COLUMN;
				double wt = syn.W;

				_sw.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}",
					layer, stRow, stCol, deRow, deCol, wt);
			}
		}

		/// <summary>
		/// Closes the saver writing and closing the configuration file
		/// </summary>
		public void close()
		{
			if (_isEnabled)
			{
				_sw.Flush();
				_sw.Close();
			}
		}

		/// <summary>
		/// Closes the existing file and recreates it
		/// </summary>
		internal void reset()
		{
			_sw.Close();
			_sw = new StreamWriter(_filePath);
		}
	}
}
