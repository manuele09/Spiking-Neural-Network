using System;
using System.Windows.Forms;

namespace SLN
{
    /// <summary>
    /// A synapse between two neurons
    /// </summary>
    [Serializable]
    public class SynapseSTDP : Synapse
    {

        private double _multFactorSTDP;

        /// <summary>
        /// The min weight of the synapse 
        /// </summary>
        protected double stdp_w_lo;

        /// <summary>
        /// The max weight of the synapse 
        /// </summary>
        protected double stdp_w_hi;

        internal SynapseSTDP(Neuron start, Neuron dest, double multFactorSTDP, double gain, double w, double max_w, double min_w, double tau, int delay)
            : base(start, dest, w, tau, delay, gain)
        {
            _multFactorSTDP = multFactorSTDP;
            stdp_w_lo = min_w;
            stdp_w_hi = max_w;
        }



        /// <summary>
        /// Updates the synaptic weight according to the status of starting and
        /// destination neurons, and to the previous input history
        /// </summary>
        /// <param name="rewardOffset">Difference between the last epoch when a reward
        /// was triggered and the current one</param>
        /// <param name="nOldSpikes">Number of the spikes the premotor neuron fired the last
        /// time a reward was triggered</param>
        internal void learn()
        {
            for (int k = 0; k < Start.NSpikes; k++)
            {
                int tSpkStart = Start.getSpikeAt(k);
                for (int m = 0; m < Dest.NSpikes && tSpkStart != -1; m++)
                {
                    int tSpkDest = Dest.getSpikeAt(m);
                    if (tSpkDest == -1)
                        continue;
                    //double dt = Constants.INTEGRATION_STEP * (tSpkStart - tSpkDest); // SBAGLIATO!!!
                    double dt = Constants.INTEGRATION_STEP * (tSpkDest - tSpkStart);

                    if (dt >= 0)
                        _W += _multFactorSTDP * Constants.STDP_A_P * Math.Exp(-dt / Constants.STDP_TAU_P);
                    else
                        _W -= _multFactorSTDP * Constants.STDP_A_N * Math.Exp(+dt / Constants.STDP_TAU_N);
                }
            }

            // Decay
            _W *= Constants.STDP_DECAY;

            // Saturation values
            _W = _W < stdp_w_lo ? stdp_w_lo : _W;
            _W = _W > stdp_w_hi ? stdp_w_hi : _W;

        }

        internal override double simulate(int step)
        {
            base.simulate(step);
            learn();
            return 0;
        }



        internal override void resetState()
        {
           ;
        }
    }
}