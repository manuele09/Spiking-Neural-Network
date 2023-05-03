using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;

namespace SLN
{
    [Serializable]
    class SynapseProportional : Synapse
    {

        /// <summary>
        /// The weight of the synapse 
        /// </summary>
        protected double _K;

        /// <summary>
        /// The weight of the synapse (with local excitation)
        /// </summary>
        internal double K
        {
            get { return _K; }
            set { _K = value; }
        }

        /// <summary>
		/// Constructor (with weights specified)
		/// </summary>
		/// <param name="gain">Gain value</param>
        internal SynapseProportional(Neuron start, Neuron dest, double gain):base(start, dest, 0, 0, 0, 0){
            
            K=gain;

        }

        
		/// <summary>
		/// Simulates the synapse
		/// </summary>
		/// <param name="step">The current simulation step</param>
        internal new void simulate(int step)
        {
            //if(Start.V>25)
                Dest.updateI(Start.V*K);
        }

    }
}
