using System;
using System.Collections.Generic;
using System.Text;

namespace SLN
{
    /// <summary>
    /// The output layer of the network, including:
    /// <list type="table">
    /// <listheader>
    /// <term>Type</term>
    /// <description>Description</description>
    /// </listheader>
    /// <item>
    /// <term>reward</term>
    /// <description>the reward neuron;</description>
    /// </item>
    /// <item>
    /// <term>sameness</term>
    /// <description>the sameness neuron;</description>
    /// </item>
    /// <item>
    /// <term>end sequence</term>
    /// <description>the end sequence neuron.</description>
    /// </item>
    /// </list>
    /// </summary>
    [Serializable]
    internal class OutputLayer
    {
        private Neuron _reward;
        private Neuron _sameness;
        private Neuron _rewardSequence;

        private bool _isReward;
        private bool _isSameness;
        private bool _isRewardSequence;

        /// <summary>
		/// Constructor
		/// </summary>
		internal OutputLayer()
		{
            _reward = new AltNeuron();
            _sameness = new AltNeuron();
            _rewardSequence = new AltNeuron();

            _reward.setCoord(1, 1, LayerNumbers.Reward);
            _sameness.setCoord(1, 1, LayerNumbers.Sameness);
            _rewardSequence.setCoord(2, 2, LayerNumbers.END_SEQUENCE);
        }

        /// <summary>
		/// Simulates the reward neuron
		/// </summary>
		/// <param name="step">The current step of simulation</param>
		/// <param name="log">The logger object</param>
        public void simulateReward(int step, StateLogger log)
        {
            double iReward = _isReward ? Constants.I_REWARD_ON : Constants.I_REWARD_OFF;            
            _reward.updateI(iReward);
            
            _reward.simulate(step);
            if (log != null)
                log.logNeuron(step,_reward);

        }

        /// <summary>
        /// Simulates the reward neuron of a sequence
        /// </summary>
        /// <param name="step">The current step of simulation</param>
        /// <param name="log">The logger object</param>
        public void simulateRewardSequence(int step, StateLogger log, int level)
        {
            double iRewardSequence = _isRewardSequence ? Constants.I_REWARD_ON*level : Constants.I_REWARD_OFF;
            _rewardSequence.updateI(iRewardSequence);

            _rewardSequence.simulate(step);
            if (log != null)
                log.logNeuron(step, _rewardSequence);

        }

        /// <summary>
        /// Simulates the sameness neuron
        /// </summary>
        /// <param name="step">The current step of simulation</param>
        /// <param name="log">The logger object</param>
        public void simulateSameness(int step, StateLogger log)
        {
            double iSameness = _isSameness ? Constants.I_SAMENESS_ON : Constants.I_SAMENESS_OFF;
            _sameness.updateI(iSameness);

            _sameness.simulate(step);
            if (log != null)
                log.logNeuron(step, _sameness);

        }

        /// <summary>
        /// Sets the reward condition
        /// </summary>
        /// <param name="reward"><i>True</i> if reward is on, <i>false</i> otherwise</param>
        internal void setReward(bool reward)
        {
            _isReward = reward;
        }

        /// <summary>
        /// Sets the reward condition for a sequence
        /// </summary>
        /// <param name="reward"><i>True</i> if reward is on, <i>false</i> otherwise</param>
        internal void setRewardSequence(bool reward)
        {
            _isRewardSequence = reward;
        }

        /// <summary>
        /// Sets the sameness condition
        /// </summary>
        /// <param name="sameness"><i>True</i> if sameness is on, <i>false</i> otherwise</param>
        internal void setSameness(bool sameness)
        {
            _isSameness = sameness;
        }

        /// <summary>
        /// Return the reward neuron
        /// </summary>
        /// <returns> The reward neuron</returns>
        internal Neuron getReward()
        {
            return _reward;
        }

        /// <summary>
        /// Return the reward neuron of a sequence
        /// </summary>
        /// <returns> The reward neuron</returns>
        internal Neuron getRewardSequence()
        {
            return _rewardSequence;
        }

        /// <summary>
        /// Return the sameness neuron
        /// </summary>
        /// <returns> The sameness neuron</returns>
        internal Neuron getSameness()
        {
            return _sameness;
        }

        /// <summary>
        /// Resets the state of the neurons
        /// </summary>
        internal void resetNeuronsState()
        {
            _reward.resetState();
            _sameness.resetState();
            _rewardSequence.resetState();
        }

    }
}
