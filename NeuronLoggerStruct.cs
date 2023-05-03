using System;

namespace SLN
{
    /// <summary>
    /// Struct used to save the state of a neuron for logging purposes
    /// </summary>
    [Serializable]
    internal struct NeuronLoggerStruct
	{
		#region Logged variables
		private int _row;
		private int _col;
		private LayerNumbers _layer;
		private double _v;
		private double _u;
		private double _i;
		private int _nSpikes;
		private int _step;
        private double _iOut;
        private int _simNumber;
        private int _idp;
        private int _idt;
		#endregion

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="step">The step of simulation</param>
		/// <param name="i">The row of the neuron</param>
		/// <param name="j">The column of the neuron</param>
		/// <param name="layer">The layer the neuron belongs to</param>
		/// <param name="neuron">The neuron to be logged</param>
		internal NeuronLoggerStruct(int step, int i, int j, LayerNumbers layer, Neuron neuron)
		{
			_step = step;
			_row = i;
			_col = j;
			_layer = layer;
			_v = neuron.V;
			_u = neuron.U;
			_i = neuron.IPrev;
			_nSpikes = neuron.NSpikes;
            _iOut = neuron._iOut;
            _simNumber = 0;
            _idp = -1;
            _idt = -1;
		}

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="step">The step of simulation</param>
        /// <param name="i">The row of the neuron</param>
        /// <param name="j">The column of the neuron</param>
        /// <param name="layer">The layer the neuron belongs to</param>
        /// <param name="neuron">The neuron to be logged</param>
        internal NeuronLoggerStruct(int simNumber, int step, int i, int j, LayerNumbers layer, Neuron neuron)
        {
            _step = step;
            _row = i;
            _col = j;
            _layer = layer;
            _v = neuron.V;
            _u = neuron.U;
            _i = neuron.IPrev;
            _nSpikes = neuron.NSpikes;
            _iOut = neuron._iOut;
            _simNumber = simNumber;
            _idp = -1;
            _idt = -1;
        }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="step">The step of simulation</param>
		/// <param name="neuron">The neuron to be logged</param>
		internal NeuronLoggerStruct(int step, Neuron neuron)
		{
			_step = step;
			_row = neuron.ROW;
			_col = neuron.COLUMN;
			_layer = neuron.LAYER;
			_v = neuron.V;
			_u = neuron.U;
			_i = neuron.IPrev;
			_nSpikes = neuron.NSpikes;
            _iOut = neuron._iOut;
            _simNumber = 0;
            _idp = -1;
            _idt = -1;
		}

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="step">The step of simulation</param>
        /// <param name="neuron">The neuron to be logged</param>
        internal NeuronLoggerStruct(int step, Neuron neuron, int idp, int idt)
        {
            _step = step;
            _row = neuron.ROW;
            _col = neuron.COLUMN;
            _layer = neuron.LAYER;
            _v = neuron.V;
            _u = neuron.U;
            _i = neuron.IPrev;
            _nSpikes = neuron.NSpikes;
            _iOut = neuron._iOut;
            _simNumber = 0;
            _idp = idp;
            _idt = idt;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="step">The step of simulation</param>
        /// <param name="neuron">The neuron to be logged</param>
        internal NeuronLoggerStruct(int simNumber, int step, Neuron neuron)
        {
            _step = step;
            _row = neuron.ROW;
            _col = neuron.COLUMN;
            _layer = neuron.LAYER;
            _v = neuron.V;
            _u = neuron.U;
            _i = neuron.IPrev;
            _nSpikes = neuron.NSpikes;
            _iOut = neuron._iOut;
            _simNumber = simNumber;
            _idp = -1;
            _idt = -1;
        }



        /// <summary>
        /// Prints informations about the neuron's state
        /// </summary>
        /// <returns>A String object with tab-separated elements</returns>
        public override String ToString()
        {

            String str = _step.ToString() + "\t"
                + Enum.Format(typeof(LayerNumbers), _layer, "d") + "\t"
                + _row.ToString() + "\t" + _col.ToString() +
                "\t" + _v.ToString() + "\t" + _u.ToString() +
                "\t" + _nSpikes.ToString() + "\t" + _i.ToString() + "\t" + _iOut.ToString() + "\t" + _simNumber.ToString()
                + "\t" + _idp.ToString() + "\t" + _idt.ToString();
            str = str.Replace(',', '.');
            str = str.Replace("Non un numero reale", "0");
            return str;
        }


		/// <summary>
		/// Print the log's header
		/// </summary>
		/// <returns>A String object with tab-separated elements</returns>
		public static String printHeader()
		{
			return "#t\tlayer\tRow\tColumn\tV\tU\tNSpikes\tI\tIOut\tSimNumber\tIDFather\tIDTarget";
		}
	}
}
