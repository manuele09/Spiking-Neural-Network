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
        private StreamWriter _neuronSw;


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="neuronPath">The path of the neuron log file</param>
        /// <param name="synapsePath">The path of the synapse log file</param>
        /// <param name="enableNeuron">Set to <i>true</i> enables the logging of the neurons</param>
        /// <param name="enableSynapse">Set to <i>true</i> enables the logging of the synapses</param>
        public StateLogger(string neuronPath)
        {
            _neuronSw = new StreamWriter(neuronPath);
            _neuronLog = new LinkedList<NeuronLoggerStruct>();
            _neuronSw.WriteLine(NeuronLoggerStruct.printHeader());
        }


        /// <summary>
        /// Clears the logging structure, allowing the logging of a 
        /// new iteration
        /// </summary>
        internal void newIteration()
        {
            _neuronLog.Clear();
        }

        /// <summary>
        /// Logs the activity of a single neuron
        /// </summary>
        /// <param name="step">The step of simulation</param>
        /// <param name="i">The row of the neuron</param>
        /// <param name="j">The column of the neuron</param>
        /// <param name="layer">The layer the neuron belongs to</param>
        /// <param name="neuron">The neuron to be logged</param>
        internal void logNeuron(int step, Neuron neuron)
        {
            _neuronLog.AddLast(new NeuronLoggerStruct(step, neuron));
        }



        /// <summary>
        /// Saves the log on a file
        /// </summary>
        internal void printLog()
        {
            foreach (NeuronLoggerStruct nls in _neuronLog)
                _neuronSw.WriteLine(nls.ToString());
            _neuronSw.Flush();
        }

        /// <summary>
        /// Closes the log
        /// </summary>
        internal void closeLog()
        {
            _neuronSw.WriteLine("#Log finished at " + DateTime.Now);
            _neuronSw.Close();
        }
    }
}