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
    internal class AltNeuron : Neuron
	{
        public int idp;
        public int idt;
        public bool inserted;
		internal AltNeuron()
			: base()
		{
            A = Constants.A_IZH_SPK;
            B = Constants.B_IZH_SPK;
            C = Constants.C_IZH_SPK;
            D = Constants.D_IZH_SPK;
		}

        public void setCoord(int i, int j, int z1, int z2, LayerNumbers layer)
        {
            this.setCoord(i,j,layer);
            idp = z1;
            idt = z2;
            inserted = false;
        }
	}
}
