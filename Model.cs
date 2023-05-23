using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord;
using Accord.Math;



namespace SLN
{
    public class Model
    {
        public int input_size;
        public int output_size;

        double[,] X_training;
        double[,] Y_training;
        double[,] X_testing;
        double[,] Y_testing;
        double[,] W;

        private int X_row_training;
        private int Y_row_training;
        private int X_row_testing;
        private int Y_row_testing;
        public int n_samples_training;

        public Model(int input_size, int output_size, int n_samples_training)
        {
            this.input_size = input_size;
            this.output_size = output_size;
            this.n_samples_training = n_samples_training;

            SetRandomW();

            X_training = new double[n_samples_training, input_size];
            Y_training = new double[n_samples_training, output_size];

            X_testing = new double[n_samples_training, input_size];
            Y_testing = new double[n_samples_training, output_size];

            X_row_training = 0;
            X_row_testing = 0;
            Y_row_training = 0;
            Y_row_testing = 0;
        }

        public void AddToXTraining(double[] input)
        {
            if (input.Length != input_size)
            {
                Console.WriteLine("Dimensione input non valida.");
                System.Windows.Forms.Application.Exit();
            }

            if (X_row_training >= n_samples_training)
            {
                Console.WriteLine("Numero massimo di input inseriti.");
                System.Windows.Forms.Application.Exit();
            }

            for (int i = 0; i < input.Length; i++)
                X_training[X_row_training, i] = input[i];
            X_row_training++;
        }

        public void AddToYTraining(double[] target)
        {
            if (target.Length != output_size)
            {
                Console.WriteLine("Dimensione di output non valida.");
                System.Windows.Forms.Application.Exit();
            }

            if (Y_row_training >= n_samples_training)
            {
                Console.WriteLine("Numero massimo di target inseriti.");
                System.Windows.Forms.Application.Exit();
            }

            for (int i = 0; i < target.Length; i++)
                Y_training[Y_row_training, i] = target[i];
            Y_row_training++;
        }

        public void AddToXTesting(double[] input)
        {
            for (int i = 0; i < input.Length; i++)
                X_testing[X_row_testing, i] = input[i];
            X_row_testing++;
        }

        public void AddToYTesting(double[] target)
        {
            for (int i = 0; i < target.Length; i++)
                Y_testing[Y_row_testing, i] = target[i];
            Y_row_testing++;
        }

        public void LearnW()
        {
            double[,] X_bias = AddBias(X_training);
            W = X_bias.PseudoInverse().Dot(Y_training); //input_size+1 x output_size
        }

        public void SetRandomW()
        {
            Random random = new Random();
            W = new double[input_size + 1, output_size];
            for (int i = 0; i < input_size + 1; i++)
                for (int j = 0; j < output_size; j++)
                    W[i, j] = random.NextDouble();
        }

        public double[] ComputePrediction(double[] input)
        {
            //fare controllo su X_row e Y_row, se sono uguali, o in generale se sono pieni
            if (input.Length != input_size)
            {
                Console.WriteLine("Dimensione input non valida.");
                System.Windows.Forms.Application.Exit();
            }

            double[,] prediction = AddBias(input).Dot(W); //1 x outputsize

            double[] prediction_vec = new double[output_size];

            for (int i = 0; i < output_size; i++)
                prediction_vec[i] = prediction[0, i];

            return prediction_vec;
        }
        public double ComputeLoss(double[] prediction, double[] target, bool Console_Output)
        {
            double error = 0;
            double mean_value = 0;
            for (int i = 0; i < prediction.Length; i++)
            {
                error += prediction[i] - target[i];
                mean_value += prediction[i]; 
            }
            if (Console_Output)
                Console.WriteLine("Loss: " + error / prediction.Length + "; Mean Value Ratio: " + error / mean_value + "; Mean Value: " + mean_value / prediction.Length);
            return error / prediction.Length;
        }
        public double ComputeTrainLoss()
        {
            double[,] prediction = AddBias(X_training).Dot(W);
            double error = 0;
            for (int i = 0; i < n_samples_training; i++)
            {
                for (int j = 0; j < output_size; j++)
                    error += (prediction[i, j] - Y_training[i, j]) / n_samples_training;
            }
            return error;
        }

        public double ComputeTestLoss()
        {
            double[,] prediction = AddBias(X_testing).Dot(W);
            double error = 0;
            for (int i = 0; i < n_samples_training; i++)
            {
                for (int j = 0; j < output_size; j++)
                    error += (prediction[i, j] - Y_testing[i, j]) / n_samples_training;
            }
            return error;
        }

        public double[,] AddBias(double[,] matrix)
        {
            double[,] matrixBias = new double[matrix.GetLength(0), 1 + matrix.GetLength(1)];
            for (int i = 0; i < matrix.GetLength(0); i++)
                matrixBias[i, 0] = 1;
            for (int i = 0; i < matrix.GetLength(0); i++)
                for (int j = 0; j < matrix.GetLength(1); j++)
                    matrixBias[i, j + 1] = matrix[i, j];
            return matrixBias;
        }

        public double[,] AddBias(double[] vector)
        {
            double[,] vectorBias = new double[1, 1 + vector.Length];
            vectorBias[0, 0] = 1;
            for (int j = 0; j < vector.Length; j++)
                vectorBias[0, j + 1] = vector[j];
            return vectorBias;
        }
    }
}
