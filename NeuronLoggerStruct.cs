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
		public int _row;
		public int _col;
		public LayerNumbers _layer;
		public double _v;
		public double _u;
		public double _i;
		public int _nSpikes;
		public int _step;
        public double _iOut;
        public int _simNumber;
        public int _idp;
        public int _idt;
        public double _iprev;
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
            _iprev = neuron.IPrev;
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
            _iprev = neuron.IPrev;

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
			//_v = neuron.V;
            _v = neuron.v_prev;
            _u = neuron.U;
			_i = neuron.IPrev;
			_nSpikes = neuron.NSpikes;
            _iOut = neuron._iOut;
            _simNumber = 0;
            _idp = -1;
            _idt = -1;
            _iprev = neuron.IPrev;
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
            _iprev = neuron.IPrev;
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
            _iprev = neuron.IPrev;
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
                + "\t" + _idp.ToString() + "\t" + _idt.ToString() + "\t" + _iprev.ToString();
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
