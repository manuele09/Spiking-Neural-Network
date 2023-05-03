using System.Collections.Generic;
using System;
using System.IO;

namespace SLN
{
    /// <summary>
    /// The winner cluster of a SOSL
    /// </summary>
    [Serializable]
    public class WinnerCluster : List<Neuron>
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public WinnerCluster()
			: base()
		{ }

		/// <summary>
		/// Checks if this cluster overlaps with another one
		/// </summary>
		/// <param name="wc">The cluster to confront with</param>
		/// <returns><i>true</i> if there's an overlapping, <i>false</i> otherwise</returns>
		public override bool Equals(Object wc)
		{
            WinnerCluster wc1 = wc as WinnerCluster;
            foreach (Neuron n1 in this)
				foreach (Neuron n2 in wc1)
					if ((n1.ROW == n2.ROW) && (n1.COLUMN == n2.COLUMN))
						return true;

			return false;
		}

        public override string ToString(){
            try
            {
                string t = "row: " + this[0].ROW + "\t column: " + this[0].COLUMN + "\t";
                return t;
            }
            catch (Exception e) 
            {
                //Console.WriteLine(" Nessun WinnerCluster");
                return null;
            }
        }
	}
}
