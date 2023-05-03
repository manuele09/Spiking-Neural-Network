using System.IO;
using System;
using System.Collections;
using System.Collections.Generic;
namespace SLN
{
    /// <summary>
    /// An Izhikevich's neuron vith an "alternative" representation
    /// </summary>
    [Serializable]
    internal class SquareNeuron : Neuron
    {
        /// <summary>
        /// The decay parameter of th integration
        /// </summary>
        protected double decay;

        /// <summary>
        /// The decay parameter of th integration
        /// </summary>
        protected double gain;

        internal SquareNeuron()
            : base()
        {
            A = double.NaN;
            B = double.NaN;
            C = double.NaN;
            D = double.NaN;
            decay = 0.996;
            gain = 0;
            V = 0;
        }

        internal SquareNeuron(double d)
            : base()
        {
            A = double.NaN;
            B = double.NaN;
            C = double.NaN;
            D = double.NaN;
            decay = d;
            gain = 0;
            V = 0;
        }

        internal SquareNeuron(double d, double g)
            : base()
        {
            A = double.NaN;
            B = double.NaN;
            C = double.NaN;
            D = double.NaN;
            decay = d;
            gain = g;
            V = 0;
        }

        /// <summary>
        /// Returns the membrane potential of the neuron
        /// </summary>
        /// <returns>The membrane potential of the neuron</returns>
        internal double getV()
        {
            return V;
        }

        /// <summary>
        /// Neset the neuron's state
        /// </summary>
        internal new void resetState()
        {

            resetI();

        }


        /// <summary>
        /// Simulates the neuron behavior
        /// </summary>
        /// <param name="step">The current simulation step</param>
        /// <returns><i>true</i> if the neuron fired a spike, <i>false</i> otherwise</returns>
        internal new bool simulate(int step)
        {

            IPrev = I;
            V = I;
            resetI();
            return false;

        }

        /// <summary>
        /// Simulates the neuron behavior
        /// </summary>
        /// <param name="step">The current simulation step</param>
        /// <returns><i>true</i> if the neuron fired a spike, <i>false</i> otherwise</returns>
        internal bool simulateSameness(int step, bool integration)
        {

            IPrev = I;
            if (integration)
            {
                V += I;
                V += V * gain;
            }

            //if(step > Constants.SIMULATION_STEPS_FEEDFORWARD + Constants.SIMULATION_STEPS_LIQUID - 100)
            if (0.001 * V > 0.01)
                V = V - 0.01;
            else
                V = decay * V;

            resetI();
            return false;

        }



    }
}
