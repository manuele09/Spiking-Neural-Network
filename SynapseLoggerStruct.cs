using System;

namespace SLN
{
    /// <summary>
    /// Struct used to save the state of a synapse for logging purposes
    /// </summary>
    [Serializable]
    internal struct SynapseLoggerStruct
	{
		#region Logged variables
		private int _rowStart;
		private int _colStart;
		private int _rowDest;
		private int _colDest;
		private LayerNumbers _layerStart;
		private LayerNumbers _layerDest;
		private int _step;
		private double _W;
		private double _I;
        private int _simNumber;
		#endregion

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="synapse">The synapse to be logged</param>
		/// <param name="step">The step of simulation</param>
		internal SynapseLoggerStruct(Synapse synapse, int step)
		{
			_rowStart = synapse.Start.ROW;
			_colStart = synapse.Start.COLUMN;
			_layerStart = synapse.Start.LAYER;

			_rowDest = synapse.Dest.ROW;
			_colDest = synapse.Dest.COLUMN;
			_layerDest = synapse.Dest.LAYER;

			_step = step;
			_W = synapse.W;
			_I = synapse.IPrev;
            _simNumber = 0;
		}

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="synapse">The synapse to be logged</param>
        /// <param name="step">The step of simulation</param>
        internal SynapseLoggerStruct(Synapse synapse, int simNumber, int step)
        {
            _rowStart = synapse.Start.ROW;
            _colStart = synapse.Start.COLUMN;
            _layerStart = synapse.Start.LAYER;

            _rowDest = synapse.Dest.ROW;
            _colDest = synapse.Dest.COLUMN;
            _layerDest = synapse.Dest.LAYER;

            _step = step;
            _W = synapse.W;
            _I = synapse.IPrev;
            _simNumber = simNumber;
        }

		/// <summary>
		/// Prints informations about the synapse state
		/// </summary>
		/// <returns>A String object with tab-separated elements</returns>
		public override String ToString()
		{

			//String str = _step.ToString() + "\t" 
			//    + Enum.Format(typeof(LayerNumbers), _layerStart, "d") + "\t"
			//    + _rowStart.ToString() + "\t" + _colStart.ToString() + "\t"
			//    +Enum.Format(typeof(LayerNumbers), _layerDest, "d") + "\t"
			//    + _rowDest.ToString() + "\t" + _colDest.ToString() + "\t"
			//    + _W.ToString() + "\t" + _I.ToString();

			String str = _step.ToString() + "\t"
				+ (int)_layerStart + "\t"
				+ _rowStart.ToString() + "\t" + _colStart.ToString() + "\t"
				+ (int)_layerDest + "\t"
				+ _rowDest.ToString() + "\t" + _colDest.ToString() + "\t"
				+ _W.ToString() + "\t" + _I.ToString() + "\t" + _simNumber.ToString();

			str = str.Replace(',', '.');
			return str;
		}

		/// <summary>
		/// Print the log's header
		/// </summary>
		/// <returns>A String object with tab-separated elements</returns>
		internal static String printHeader()
		{
			return "#t\tStLayer\tStRow\tStCol\tDstLayer\tDstRow\tDstCol\tW\tI\tSimNumber";
		}
	}
}
