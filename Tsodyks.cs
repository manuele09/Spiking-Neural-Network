using System;
using System.Windows.Forms;

namespace SLN
{
    /// <summary>
    /// A synapse between two neurons
    /// </summary>
    [Serializable]
    public class Tsodyks : Synapse
    {

        protected double tau_psc_;
        protected double tau_fac_;
        protected double tau_rec_;
        protected double U_;
        protected double x_;
        protected double y_;
        protected double u_;
        protected double z_;

        /// <summary>
        /// Constructor (with different excitation weights specified)
        /// </summary>
        /// <param name="start">The starting neuron</param>
        /// <param name="dest">The destination neuron</param>
        internal Tsodyks(Neuron start, Neuron dest, double w, int delay, double tau_psc, double tau_fac, double tau_rec, double U, double x, double u) : base(start, dest, w, 0, delay, 1)
        {
            tau_psc_ = tau_psc;
            tau_fac_ = tau_fac;
            tau_rec_ = tau_rec;
            U_ = U;
            x_ = x;
            u_ = u;
            y_ = 0;
            z_ = 0;
        }


        /// <summary>
        /// Simulates the synapse
        /// </summary>
        /// <param name="step">The current simulation step</param>
        internal override double simulate(int step)
        {
            //t_1 = t_psc
            double h = Math.Exp(step - Start._spikeList.getLastSpike(step) + Delay);

            double dx = z_ / tau_rec_ - u_ * x_ * h;
            x_ = x_ + dx * Constants.INTEGRATION_STEP;

            double dy = -y_ / tau_psc_ + u_ * x_ * h;
            y_ = y_ + dy * Constants.INTEGRATION_STEP;

            double dz = y_ / tau_psc_ - z_ / tau_rec_;
            z_ = z_ + dz * Constants.INTEGRATION_STEP;

            double du = -u_ / tau_fac_ + U_ * (1 - u_) * h;
            u_ = u_ + du * Constants.INTEGRATION_STEP;

            //Dest.updateI(y_ * W);
            current = y_ * W;
           /* double Puu = (tau_fac_ == 0.0) ? 0.0 : Math.Exp(-h / tau_fac_);
            double Pyy = Math.Exp(-h / tau_psc_);
            double Pzz = Math.Exp(-h / tau_rec_);

            double Pxy = ((Pzz - 1.0) * tau_rec_ - (Pyy - 1.0) * tau_psc_) / (tau_psc_ - tau_rec_);
            double Pxz = 1.0 - Pzz;

            double z = 1.0 - x_ - y_;

            u_ *= Puu;
            x_ += Pxy * y_ + Pxz * z;
            y_ *= Pyy;

            u_ += U_ * (1.0 - u_);

            double delta_y_tsp = u_ * x_ * Constants.INTEGRATION_STEP;

            // delta function x, y
            x_ -= delta_y_tsp;
            y_ += delta_y_tsp;
            Dest.updateI(y_ * W);
           */
            //if (step > 90)
            //  Console.WriteLine("I = " + y_ + ", w = " + W);
            //Dest.updateI(delta_y_tsp);
            return 0;
        }
        internal override void resetState()
        {
            y_ = 0;
        }
    }
}