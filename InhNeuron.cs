using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections;

namespace SLN
{
    /// <summary>
    /// An Izhikevich's neuron with an Inhibition-Induced Spiking model
    /// </summary>
    [Serializable]
    internal class InhNeuron : Neuron
    {
        internal InhNeuron()
            : base()
        {
            A = Constants.A_IZH_INH;
            B = Constants.B_IZH_INH;
            C = Constants.C_IZH_INH;
            D = Constants.D_IZH_INH;
        }
    }
}
