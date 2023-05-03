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
    internal class Class1Neuron : Neuron
    {
        internal Class1Neuron()
            : base()
        {
            A = Constants.A_IZH_CLASS1;
            B = Constants.B_IZH_CLASS1;
            C = Constants.C_IZH_CLASS1;
            D = Constants.D_IZH_CLASS1;

           // IOut_array = new double[Constants.SIMULATION_STEPS_LIQUID];
        }

        private bool _active;
        /// <summary>
        /// Bool value used for setting the input current in the liquid
        /// </summary>
        internal bool Active
        {
            get { return _active; }
            set { _active = value; }
        }

        private double _inputCurrent;
        /// <summary>
        /// Bool value used for setting the input current in the liquid
        /// </summary>
        internal double INPUTCURRENT
        {
            get { return _inputCurrent; }
            set { _inputCurrent = value; }
        }

        //public double[] IOut_array;

        //internal override bool simulate(int step){
        
        //    bool spike = base.simulate(step);
        //    if(step >= Constants.SIMULATION_STEPS_FEEDFORWARD)
        //       IOut_array[step-Constants.SIMULATION_STEPS_FEEDFORWARD]=this.IOut;
        //    return spike;
        //}

    }
}