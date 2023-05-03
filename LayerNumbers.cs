namespace SLN
{
	/// <summary>
	/// Code number of the layer in the log files
	/// </summary>
	public enum LayerNumbers
	{
		/// <summary>
		/// The input layer (to SOSL #1)
		/// </summary>
		FirstLayer_1 = 1,

		/// <summary>
		/// The SOSL #1 (time t)
		/// </summary>
		SOSL_1,

		/// <summary>
		/// The SOSL #2 (time t-1)
		/// </summary>
		SOSL_2,

		/// <summary>
		/// The premotor neuron
		/// </summary>
		Premotor,

		/// <summary>
		/// The sameness neuron
		/// </summary>
		Sameness,

		/// <summary>
		/// The reward neuron
		/// </summary>
		Reward,

		/// <summary>
		/// The "fake" SOSL used in the simulation
		/// of conditioning neurons
		/// </summary>
		Fake,

		/// <summary>
		/// The input layer (to SOSL #2)
		/// </summary>
		FirstLayer_2,

		/// <summary>
		/// The "fake" SOSL used in the simulation
		/// of feedback synapses between SOSL #1 and 
		/// the related input layer at the <i>t+1</i>-th
		/// step of simulation
		/// </summary>
		Fake_2_1,

		/// <summary>
		/// The "fake" SOSL used in the simulation
		/// of feedback synapses between SOSL #2 and 
		/// the related input layer at the <i>t+1</i>-th
		/// step of simulation
		/// </summary>
		Fake_2_2,

		/// <summary>
		/// The difference neuron
		/// </summary>
		Difference,

        /// <summary>
        /// The SOSL #3 (time t-n)
        /// </summary>
        SOSL_3,

        /// <summary>
        /// The End Sequence Layer
        /// </summary>
        END_SEQUENCE,

         /// <summary>
        /// The Liquid State layer
        /// </summary>
        LIQUID_STATE,

          /// <summary>
        /// The Output layer - 15
        /// </summary>
        OUTPUT_LAYER,

        /// <summary>
        /// The Output layer - 16
        /// </summary>
        CONTEXT

	};
}
