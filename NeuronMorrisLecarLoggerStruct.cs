using System;

namespace SLN
{
    /// <summary>
    /// Struct used to save the state of a neuron for logging purposes
    /// </summary>
    [Serializable]
    internal struct NeuronMorrisLecarLoggerStruct
    {
        #region Logged variables
        private int _row;
        private int _col;
        private LayerNumbers _layer;
        private double _v;
        private double _w;
        private double _i;
        private double _iPrev;
        private int _nSpikes;
        private int _step;
        private double _iOut;
        private double _kf;
        private int _simNumber;
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="step">The step of simulation</param>
        /// <param name="i">The row of the neuron</param>
        /// <param name="j">The column of the neuron</param>
        /// <param name="layer">The layer the neuron belongs to</param>
        /// <param name="neuron">The neuron to be logged</param>
        internal NeuronMorrisLecarLoggerStruct(int step, int i, int j, LayerNumbers layer, NeuronMorrisLecar neuron)
        {
            _step = step;
            _row = i;
            _col = j;
            _layer = layer;
            _v = neuron.V;
            _w = neuron.W;
            _i = neuron.I;
            _nSpikes = neuron.NSpikes;
            _iOut = neuron._iOut;
            _iPrev = neuron.IPrev;
            _kf = neuron.KF;
            _simNumber = 0;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="step">The step of simulation</param>
        /// <param name="neuron">The neuron to be logged</param>
        internal NeuronMorrisLecarLoggerStruct(int step, NeuronMorrisLecar neuron)
        {
            _step = step;
            _row = neuron.ROW;
            _col = neuron.COLUMN;
            _layer = neuron.LAYER;
            _v = neuron.V;
            _w = neuron.W;
            _i = neuron.I;
            _nSpikes = neuron.NSpikes;
            _iOut = neuron._iOut;
            _iPrev = neuron.IPrev;
            _kf = neuron.KF;
            _simNumber = 0;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="step">The step of simulation</param>
        /// <param name="neuron">The neuron to be logged</param>
        internal NeuronMorrisLecarLoggerStruct(int simNumber, int step, NeuronMorrisLecar neuron)
        {
            _step = step;
            _row = neuron.ROW;
            _col = neuron.COLUMN;
            _layer = neuron.LAYER;
            _v = neuron.V;
            _w = neuron.W;
            _i = neuron.I;
            _nSpikes = neuron.NSpikes;
            _iOut = neuron._iOut;
            _iPrev = neuron.IPrev;
            _kf = neuron.KF;
            _simNumber = simNumber;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="step">The step of simulation</param>
        /// <param name="i">The row of the neuron</param>
        /// <param name="j">The column of the neuron</param>
        /// <param name="layer">The layer the neuron belongs to</param>
        /// <param name="neuron">The neuron to be logged</param>
        internal NeuronMorrisLecarLoggerStruct(int simNumber, int step, int i, int j, LayerNumbers layer, NeuronMorrisLecar neuron)
        {
            _step = step;
            _row = i;
            _col = j;
            _layer = layer;
            _v = neuron.V;
            _w = neuron.W;
            _i = neuron.I;
            _nSpikes = neuron.NSpikes;
            _iOut = neuron._iOut;
            _iPrev = neuron.IPrev;
            _kf = neuron.KF;
            _simNumber = simNumber;
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
                "\t" + _v.ToString() + "\t" + _w.ToString() +
                "\t" + _nSpikes.ToString() + "\t" + _i.ToString() + "\t" + _iOut.ToString()
                + "\t" + _iPrev.ToString() + "\t" + _kf.ToString() + "\t" + _simNumber.ToString();
            str = str.Replace(',', '.');
            return str;
        }

        /// <summary>
        /// Print the log's header
        /// </summary>
        /// <returns>A String object with tab-separated elements</returns>
        public static String printHeader()
        {
            return "#t\tlayer\tRow\tColumn\tV\tU\tNSpikes\tI_freq\tIOut\tI_tot\tKf\tSimNumber";
        }
    }
}

