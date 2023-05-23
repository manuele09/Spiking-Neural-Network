using System;

namespace SLN
{
    /// <summary>
    /// Struct used to save the state of a neuron for logging purposes
    /// </summary>
    [Serializable]
    internal struct NeuronLoggerStruct
    {

        public int index;
        public double _v;
        public double _i;
        public double _step;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="step">The step of simulation</param>
        /// <param name="i">The row of the neuron</param>
        /// <param name="j">The column of the neuron</param>
        /// <param name="layer">The layer the neuron belongs to</param>
        /// <param name="neuron">The neuron to be logged</param>
        internal NeuronLoggerStruct(int step, Neuron neuron)
        {
            _step = step;
            this.index = neuron.index;
            _v = neuron.V;
            _i = neuron.I_prev + neuron.IBias;
        }
        /// <summary>
        /// Prints informations about the neuron's state
        /// </summary>
        /// <returns>A String object with tab-separated elements</returns>
        public override String ToString()
        {

            String str = _step + "\t" + index + "\t" + _v + "\t" + _i + "\t";
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
            return "step\tindex\tV\tI_tot";
        }
    }
}