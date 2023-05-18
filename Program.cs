/*
 * Compile ALWAYS with x86 option, NEVER with AnyCPU.
 * Put the DLL in the Debug/Release directory of the project
 * In the Property Pages of each C++ project set:
 * - C/C++ --> General --> Addtional Include Directories: add the DLL file directory
 * - C/C++ --> General --> Resolve #using references: add the DLL file directory
 * - C/C++ --> Optimization --> Whole program optimization: set to "No"
 * - Linker --> General --> Additional library directories: add the DLL file directory
 */
//Strumenti -> Gestione Pacchetti Nuget -> Console Gestione di Pacchetti -> PM> Install-Package System.Threading.dll



using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using System.Linq;
using System.Runtime.CompilerServices;
using Accord.Statistics.Models;
using System.Windows.Input;
using System.Globalization;
using Accord.Math;
using CSML;

namespace SLN
{
    /// <summary>
    /// Class containing the Main method
    /// </summary>
	public class Program
    {
        //stim_interval = 300;
        //stim_lenght = 50;
        //readout_delay = 50;

        /// <summary>
        /// The Main method
        /// </summary>
        /// <param name="args">Currently not used</param>
        public static void Main()
        {

            String prediction_frame_path = "../../../PyScript/Prediction/";
            String prediction_frame_name = "_prediction.txt";

            String train_frame_path = "../../../PyScript/dati/video_6/train_target/";
            String train_frame_name = "_train_target.txt";
            int n_train = 23;

            String test_frame_path = "../../../PyScript/dati/video_6/test_target/";
            String test_frame_name = "_test_target.txt";

            // test_frame_path = train_frame_path;
            //test_frame_name = train_frame_name;
            int n_test = 23;


            double stim_interval = 120; //ms
            double stim_lenght = 50; //ms
            double readout_delay = 30; //ms


            int stim_interval_steps = (int)(stim_interval / Constants.INTEGRATION_STEP);
            int stim_lenght_steps = (int)(stim_lenght / Constants.INTEGRATION_STEP);
            int readout_delay_steps = (int)(readout_delay / Constants.INTEGRATION_STEP);

            int input_size = 100;
            int output_size = 64 * 64;

            int n_exc = input_size;
            int n_inh = 80;
            int n_rec = n_exc;

            double[] input_vector; //input_size 
            double[] prediction_vector;

            LSM liquid = new LSM(n_exc, n_inh, n_rec, output_size);

            Model model = new Model(input_size, output_size, n_train * 5);

            #region Training
            for (int t = 6; t <= 6; t++)
            {

                training(liquid, model, stim_lenght_steps, readout_delay_steps, stim_interval_steps, "../../../PyScript/dati/video_" + t + "/train_target/", train_frame_name, n_train);
                liquid.resetState();
            }

            //Calcolo i Pesi
            model.LearnW();
            Console.WriteLine(model.ComputeTrainLoss());

            //BinarySerialization.WriteToBinaryFile("net.bin", liquid);
            #endregion
            #region Testing

            // Network net = BinarySerialization.ReadFromBinaryFile<Network>("net.bin");
            for (int i = 0; i < n_test; i++)
            {
                Console.WriteLine("Simulando" + i);

                //Leggo il frame da file, e determino l'input current
                input_vector = ReadVectorFromFile(test_frame_path + i + test_frame_name);

                //Lo do in input come corrente
                liquid.SetLiquidCurrent(input_vector, 2);

                //simulo il liquido per stim_lenght ms
                liquid.simulateLiquid(stim_lenght_steps, true);
                liquid.ResetLiquidBiasCurrent();

                //simulo per read_out_steps
                liquid.simulateLiquid(readout_delay_steps, true);

                //calcolo gli stati
                prediction_vector = model.ComputePrediction(liquid.GetLiquidStates(liquid.current_step, 3));
                WriteMatrixToFile(VectorToMatrix(prediction_vector, 64, 64), prediction_frame_path + i + prediction_frame_name, 0.3);

                //simulo i rimanenti steps
                liquid.simulateLiquid(stim_interval_steps - stim_lenght_steps - readout_delay_steps, true);
            }


            #endregion

        }


        public static void training(LSM liquid, Model model, int stim_lenght_steps, int readout_delay_steps, int stim_interval_steps, String train_frame_path, String train_frame_name, int n_samples)
        {
            double[] input_vector;
            for (int i = 0; i < n_samples; i++)
            {
                Console.WriteLine("Simulando" + i);

                //Leggo il frame da file, e determino l'input current
                input_vector = ReadVectorFromFile(train_frame_path + i + train_frame_name);
                if (i != 0)
                    model.AddToYTraining(input_vector);

                //Lo do in input come corrente
                liquid.SetLiquidCurrent(input_vector, 2);

                //simulo l'input
                liquid.simulateLiquid(stim_lenght_steps, true);
                liquid.ResetLiquidBiasCurrent();

                //simulo per read_out_steps
                liquid.simulateLiquid(readout_delay_steps, true);

                //calcolo gli stati
                if (i != (n_samples - 1))
                    model.AddToXTraining(liquid.GetLiquidStates(liquid.current_step, 3));

                //simulo i rimanenti steps
                liquid.simulateLiquid(stim_interval_steps - stim_lenght_steps - readout_delay_steps, true);
            }

        }

        public static double[] MatrixToVector(double[,] matrix)
        {
            double[] vector = new double[matrix.GetLength(0) * matrix.GetLength(1)];
            for (int i = 0; i < matrix.GetLength(0); i++)
                for (int j = 0; j < matrix.GetLength(1); j++)
                    vector[i * matrix.GetLength(1) + j] = matrix[i, j];
            return vector;
        }
        public static double[,] addBias(double[,] matrix)
        {
            int rowCount = matrix.GetLength(0);
            int colCount = matrix.GetLength(1);

            double[,] bias = new double[rowCount, colCount + 1];
            for (int i = 0; i < rowCount; i++)
                for (int j = 0; j < colCount + 1; j++)
                    if (j == 0)
                        bias[i, j] = 1;
                    else
                        bias[i, j] = matrix[i, j - 1];

            return bias;
        }

        public static double[,] ReadMatrixFromFile(string filename)
        {
            try
            {
                var lines = File.ReadAllLines(filename);
                double[,] matrix = new double[lines.Length, lines.Length];

                for (int i = 0; i < lines.Length; i++)
                {
                    var line = lines[i].Split(' ').Select(int.Parse).ToArray();
                    for (int j = 0; j < lines.Length; j++)
                    {
                        matrix[i, j] = line[j];
                    }
                }

                return matrix;
            }
            catch (IOException e)
            {
                Console.WriteLine("An error occurred while reading the file:");
                Console.WriteLine(e.Message);
                return null;
            }
            catch (FormatException e)
            {
                Console.WriteLine("An error occurred while parsing the matrix:");
                Console.WriteLine(e.Message);
                return null;
            }
        }
        public static double[] ReadVectorFromFile(string filename)
        {
            try
            {
                var lines = File.ReadAllLines(filename);
                double[] vector = new double[lines.Length * lines.Length];
                int index = 0;


                for (int i = 0; i < lines.Length; i++)
                {
                    var line = lines[i].Split(' ').Select(int.Parse).ToArray();
                    for (int j = 0; j < line.Length; j++)
                    {
                        vector[index++] = line[j];
                    }
                }

                return vector;
            }
            catch (IOException e)
            {
                Console.WriteLine("An error occurred while reading the file:");
                Console.WriteLine(e.Message);
                return null;
            }
            catch (FormatException e)
            {
                Console.WriteLine("An error occurred while parsing the matrix:");
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public static double[,] ReadMatrixFromFile(string filename, double[,] targets, int row)
        {
            try
            {
                var lines = File.ReadAllLines(filename);
                int matrixSize = lines.Length;
                double[,] matrix = new double[matrixSize, matrixSize];

                for (int i = 0; i < matrixSize; i++)
                {
                    var line = lines[i].Split(' ').Select(int.Parse).ToArray();
                    for (int j = 0; j < matrixSize; j++)
                    {
                        targets[row, i * Constants.FIRST_LAYER_DIMENSION_J + j] = line[j];
                        matrix[i, j] = line[j];
                    }
                }

                return matrix;
            }
            catch (IOException e)
            {
                Console.WriteLine("An error occurred while reading the file:");
                Console.WriteLine(e.Message);
                return null;
            }
            catch (FormatException e)
            {
                Console.WriteLine("An error occurred while parsing the matrix:");
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public static double[,] VectorToMatrix(double[] vector, int dim1, int dim2)
        {
            double[,] matrix = new double[dim1, dim2];
            for (int i = 0; i < dim1; i++)
                for (int j = 0; j < dim2; j++)
                    matrix[i, j] = vector[i * dim2 + j];
            return matrix;
        }
        public static void WriteMatrixToFile(double[,] matrix, string filePath, double treshold)
        {
            using (StreamWriter file = new StreamWriter(filePath))
            {

                for (int i = 0; i < matrix.GetLength(0); i++)
                {
                    for (int j = 0; j < matrix.GetLength(1); j++)
                    {
                        if (treshold > 0)
                        {
                            if (matrix[i, j] > 0.5)
                                file.Write(1.ToString(CultureInfo.InvariantCulture) + " ");
                            else
                                file.Write(0.ToString(CultureInfo.InvariantCulture) + " ");
                        }
                        else
                            file.Write(matrix[i, j].ToString(CultureInfo.InvariantCulture) + " ");

                    }
                    file.WriteLine();

                }
            }
        }
        public static void PrintMatrix(double[,] matrix1)
        {
            if (matrix1 == null)
            {
                Console.WriteLine("Matrix is null");
                return;
            }

            int rowCount = matrix1.GetLength(0);
            int colCount = matrix1.GetLength(1);

            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < colCount; j++)
                {
                    // Console.Write(matrix1[i, j]- matrix2[i, j] + " ");

                    Console.Write(matrix1[i, j]);
                }
                Console.WriteLine();
            }
        }


    }
}