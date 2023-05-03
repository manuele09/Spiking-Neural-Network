using System;
using System.Collections.Generic;
using System.IO;

namespace SLN
{
    /// <summary>
    /// Logs the network activity
    /// </summary>
    [Serializable]
    public class StateLogger
	{
		private LinkedList<NeuronLoggerStruct> _neuronLog;
		private LinkedList<SynapseLoggerStruct> _synapseLog;
        private LinkedList<NeuronMorrisLecarLoggerStruct> _neuronMorrisLecarLog;

		private StreamWriter _neuronSw;
		private StreamWriter _synapseSw;
        private StreamWriter _neuronSwMorrisLecar;

		private bool _isEnabledN;
		private bool _isEnabledS;
        private bool _isEnabledNMorrisLecar;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="neuronPath">The path of the neuron log file</param>
		/// <param name="synapsePath">The path of the synapse log file</param>
		/// <param name="enableNeuron">Set to <i>true</i> enables the logging of the neurons</param>
		/// <param name="enableSynapse">Set to <i>true</i> enables the logging of the synapses</param>
		public StateLogger(string neuronPath, string synapsePath, bool enableNeuron, bool enableSynapse)
		{
			_isEnabledN = enableNeuron;
			_isEnabledS = enableSynapse;
            _isEnabledNMorrisLecar = false;

			if (_isEnabledN)
			{
				_neuronSw = new StreamWriter(neuronPath);
				_neuronLog = new LinkedList<NeuronLoggerStruct>();

				_neuronSw.WriteLine("#Log started at " + DateTime.Now);

				_neuronSw.WriteLine(NeuronLoggerStruct.printHeader());
			}

			if (_isEnabledS)
			{
				_synapseSw = new StreamWriter(synapsePath);
				_synapseLog = new LinkedList<SynapseLoggerStruct>();

				_synapseSw.WriteLine("#Log started at " + DateTime.Now);

				_synapseSw.WriteLine(SynapseLoggerStruct.printHeader());
			}

			if (Constants.ENABLE_OUTPUT)
				Console.WriteLine("Sim started at " + DateTime.Now);
		}

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="neuronPath">The path of the neuron log file</param>
        /// <param name="synapsePath">The path of the synapse log file</param>
        /// <param name="neuronMorrisPath">The path of the MorrisLecar neuron log file</param>
        /// <param name="enableNeuron">Set to <i>true</i> enables the logging of the neurons</param>
        /// <param name="enableSynapse">Set to <i>true</i> enables the logging of the synapses</param>
        /// <param name="enableNeuronMorris">Set to <i>true</i> enables the logging of the Morris-Lecar neurons</param>
        public StateLogger(string neuronPath, string synapsePath, string neuronMorrisPath, bool enableNeuron, bool enableSynapse, bool enableNeuronMorris)
        {
            _isEnabledN = enableNeuron;
            _isEnabledS = enableSynapse;
            _isEnabledNMorrisLecar = enableNeuronMorris;

            if (_isEnabledN)
            {
                _neuronSw = new StreamWriter(neuronPath);
                _neuronLog = new LinkedList<NeuronLoggerStruct>();

                _neuronSw.WriteLine("#Log started at " + DateTime.Now);

                _neuronSw.WriteLine(NeuronLoggerStruct.printHeader());
            }

            if (_isEnabledS)
            {
                _synapseSw = new StreamWriter(synapsePath);
                _synapseLog = new LinkedList<SynapseLoggerStruct>();

                _synapseSw.WriteLine("#Log started at " + DateTime.Now);

                _synapseSw.WriteLine(SynapseLoggerStruct.printHeader());
            }

            if (_isEnabledNMorrisLecar)
            {
                _neuronSwMorrisLecar = new StreamWriter(neuronMorrisPath);
                _neuronMorrisLecarLog = new LinkedList<NeuronMorrisLecarLoggerStruct>();

                _neuronSwMorrisLecar.WriteLine("#Log started at " + DateTime.Now);

                _neuronSwMorrisLecar.WriteLine(NeuronMorrisLecarLoggerStruct.printHeader());
            }

            if (Constants.ENABLE_OUTPUT)
                Console.WriteLine("Sim started at " + DateTime.Now);
        }

		/// <summary>
		/// Clears the logging structure, allowing the logging of a 
		/// new iteration
		/// </summary>
		internal void newIteration()
		{
			if (_isEnabledN)
				_neuronLog.Clear();

			if (_isEnabledS)
				_synapseLog.Clear();

            if (_isEnabledNMorrisLecar)
                _neuronMorrisLecarLog.Clear();

		}

		/// <summary>
		/// Logs the activity of a single neuron
		/// </summary>
		/// <param name="step">The step of simulation</param>
		/// <param name="i">The row of the neuron</param>
		/// <param name="j">The column of the neuron</param>
		/// <param name="layer">The layer the neuron belongs to</param>
		/// <param name="neuron">The neuron to be logged</param>
		internal void logNeuron(int step, int i, int j, LayerNumbers layer, Neuron neuron)
		{
			if (_isEnabledN)
				_neuronLog.AddLast(new NeuronLoggerStruct(step, i, j, layer, neuron));
		}

        /// <summary>
        /// Logs the activity of a single neuron
        /// </summary>
        /// <param name="step">The step of simulation</param>
        /// <param name="simNumber">The number of internal simulation</param>
        /// <param name="i">The row of the neuron</param>
        /// <param name="j">The column of the neuron</param>
        /// <param name="layer">The layer the neuron belongs to</param>
        /// <param name="neuron">The neuron to be logged</param>
        internal void logNeuron(int simNumber, int step, int i, int j, LayerNumbers layer, Neuron neuron)
        {
            if (_isEnabledN)
                _neuronLog.AddLast(new NeuronLoggerStruct(simNumber, step, i, j, layer, neuron));
        }

		/// <summary>
		/// Logs the activity of a single neuron
		/// </summary>
		/// <param name="step">The step of simulation</param>
		/// <param name="neuron">The neuron to be logged</param>
		internal void logNeuron(int step, Neuron neuron)
		{
			if (_isEnabledN)
				_neuronLog.AddLast(new NeuronLoggerStruct(step, neuron));
		}

        /// <summary>
        /// Logs the activity of a single neuron
        /// </summary>
        /// <param name="step">The step of simulation</param>
        /// <param name="neuron">The neuron to be logged</param>
        internal void logNeuron(int step, Neuron neuron, int idp, int idt)
        {
            if (_isEnabledN)
                _neuronLog.AddLast(new NeuronLoggerStruct(step, neuron, idp, idt));
        }


        /// <summary>
        /// Logs the activity of a single neuron
        /// </summary>
        /// <param name="step">The step of simulation</param>
        /// <param name="neuron">The neuron to be logged</param>
        internal void logNeuron(int simNumber, int step, Neuron neuron)
        {
            if (_isEnabledN)
                _neuronLog.AddLast(new NeuronLoggerStruct(simNumber, step, neuron));
        }

        /// <summary>
        /// Logs the activity of a single Morris Lecar neuron
        /// </summary>
        /// <param name="step">The step of simulation</param>
        /// <param name="i">The row of the neuron</param>
        /// <param name="j">The column of the neuron</param>
        /// <param name="layer">The layer the neuron belongs to</param>
        /// <param name="neuron">The neuron to be logged</param>
        internal void logNeuronMorrisLecar(int step, int i, int j, LayerNumbers layer, NeuronMorrisLecar neuron)
        {
            if (_isEnabledNMorrisLecar)
                _neuronMorrisLecarLog.AddLast(new NeuronMorrisLecarLoggerStruct(step, i, j, layer, neuron));
        }

        /// <summary>
        /// Logs the activity of a single Morris Lecar neuron
        /// </summary>
        /// <param name="step">The step of simulation</param>
        /// <param name="i">The row of the neuron</param>
        /// <param name="j">The column of the neuron</param>
        /// <param name="layer">The layer the neuron belongs to</param>
        /// <param name="neuron">The neuron to be logged</param>
        internal void logNeuronMorrisLecar(int simNumber, int step, int i, int j, LayerNumbers layer, NeuronMorrisLecar neuron)
        {
            if (_isEnabledNMorrisLecar)
                _neuronMorrisLecarLog.AddLast(new NeuronMorrisLecarLoggerStruct(simNumber, step, i, j, layer, neuron));
        }

        /// <summary>
        /// Logs the activity of a single Morris Lecar neuron
        /// </summary>
        /// <param name="step">The step of simulation</param>
        /// <param name="neuron">The neuron to be logged</param>
        internal void logNeuronMorrisLecar(int step, NeuronMorrisLecar neuron)
        {
            if (_isEnabledNMorrisLecar)
                _neuronMorrisLecarLog.AddLast(new NeuronMorrisLecarLoggerStruct(step, neuron));
        }

        /// <summary>
        /// Logs the activity of a single Morris Lecar neuron
        /// </summary>
        /// <param name="step">The step of simulation</param>
        /// <param name="neuron">The neuron to be logged</param>
        internal void logNeuronMorrisLecar(int simNumber, int step, NeuronMorrisLecar neuron)
        {
            if (_isEnabledNMorrisLecar)
                _neuronMorrisLecarLog.AddLast(new NeuronMorrisLecarLoggerStruct(simNumber, step, neuron));
        }

		/// <summary>
		/// Logs the activity of a single synapse
		/// </summary>
		/// <param name="synapse">The synapse to be logged</param>
		/// <param name="step">The step of simulation</param>
		internal void logSynapse(Synapse synapse, int step)
		{
			if (_isEnabledS)
				_synapseLog.AddLast(new SynapseLoggerStruct(synapse, step));
		}

        /// <summary>
        /// Logs the activity of a single synapse
        /// </summary>
        /// <param name="synapse">The synapse to be logged</param>
        /// <param name="step">The step of simulation</param>
        internal void logSynapse(Synapse synapse, int simNumber, int step)
        {
            if (_isEnabledS)
                _synapseLog.AddLast(new SynapseLoggerStruct(synapse, simNumber, step));
        }

		/// <summary>
		/// Saves the log on a file
		/// </summary>
		internal void printLog()
		{
			if (_isEnabledN)
			{
				foreach (NeuronLoggerStruct nls in _neuronLog)
					_neuronSw.WriteLine(nls.ToString());

				_neuronSw.Flush();
			}

			if (_isEnabledS)
			{
				foreach (SynapseLoggerStruct sls in _synapseLog)
					_synapseSw.WriteLine(sls.ToString());

				_synapseSw.Flush();
			}

            if (_isEnabledNMorrisLecar) {

                foreach (NeuronMorrisLecarLoggerStruct nls in _neuronMorrisLecarLog)
                    _neuronSwMorrisLecar.WriteLine(nls.ToString());
            }

		}

		/// <summary>
		/// Closes the log
		/// </summary>
		internal void closeLog()
		{
			if (_isEnabledN)
			{
				_neuronSw.WriteLine("#Log finished at " + DateTime.Now);
				_neuronSw.Close();
			}

			if (_isEnabledS)
			{
				_synapseSw.WriteLine("#Log finished at " + DateTime.Now);
				_synapseSw.Close();
			}

            if (_isEnabledNMorrisLecar) {
                _neuronSwMorrisLecar.WriteLine("#Log finished at " + DateTime.Now);
                _neuronSwMorrisLecar.Close();
            }

			if (Constants.ENABLE_OUTPUT)
			{
				Console.WriteLine("Sim finished at " + DateTime.Now);
				Console.WriteLine();
			}
		}
	}
}
