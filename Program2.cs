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
	public class Program2
    {
        public static void Main()
        {
            String main_dir = "../../../PyScript/C#";

            CreateDatasetsRandom(main_dir, 51, 0.8);
            //CreateDatasets(main_dir, 51, 0.8);
            string[] train_inputs = Directory.GetFiles(main_dir + "/train_inputs");
            string[] train_targets = Directory.GetFiles(main_dir + "/train_targets");
            string[] test_inputs = Directory.GetFiles(main_dir + "/test_inputs");
            string[] test_targets = Directory.GetFiles(main_dir + "/test_targets");

            double stim_interval = 50; //ms
            double stim_lenght = 40; //ms
            double readout_delay = 0; //ms
            double gain = 0.0025;


            int stim_interval_steps = (int)(stim_interval / Constants.INTEGRATION_STEP);
            int stim_lenght_steps = (int)(stim_lenght / Constants.INTEGRATION_STEP);
            int readout_delay_steps = (int)(readout_delay / Constants.INTEGRATION_STEP);

            int output_size = 64 * 64;

            int n_exc = 400; //400
            int n_inh = 100; //100
            int n_rec = n_exc / 2; //200

            double[] input_vector;
            double[] target_vector;
            double[] prediction_vector;
            double[] liquid_states;

            StateLogger logger = new StateLogger("../../../PyScript/Logs/log.txt");

            LSM liquid = new LSM(n_exc, n_inh, n_rec, output_size);

            Model model = new Model(n_rec, output_size, train_inputs.Length);

            #region Training

            for (int i = 0; i < train_inputs.Length; i++)
            {
                Console.WriteLine("\nSimulando" + i);

                //Leggo il frame da file, e determino l'input current
                input_vector = ReadVectorFromFile(train_inputs[i]);

                //Lo do in input come corrente
                liquid.SetLiquidCurrent(input_vector, gain);

                //simulo l'input
                liquid.simulateLiquid(stim_lenght_steps, true, logger);
                liquid.ResetLiquidBiasCurrent();

                //simulo per read_out_steps
                liquid.simulateLiquid(readout_delay_steps, true, logger);

                //faccio la lettura degli stati
                liquid_states = liquid.GetLiquidStates(liquid.current_step, 3);
                model.AddToXTraining(liquid_states);
                target_vector = ReadVectorFromFile(train_targets[i]);
                model.AddToYTraining(target_vector);

                prediction_vector = model.ComputePrediction(liquid_states);
                model.ComputeLoss(prediction_vector, target_vector, true);

                //simulo i rimanenti steps
                liquid.simulateLiquid(stim_interval_steps - stim_lenght_steps - readout_delay_steps, true, logger);
            }

            logger.printLog();
            //Calcolo i Pesi
            model.LearnW();
            Console.WriteLine(model.ComputeTrainLoss());
            #endregion

            Directory.CreateDirectory(main_dir + "/test_predictions");
            Directory.CreateDirectory(main_dir + "/liquid_states");
            String prediction_path = main_dir + "/test_predictions/";
            liquid = new LSM(n_exc, n_inh, n_rec, output_size);
            #region Testing
            for (int i = 0; i < test_inputs.Length; i++)
            {
                Console.WriteLine("\nSimulando" + i);

                //Leggo il frame da file, e determino l'input current
                input_vector = ReadVectorFromFile(test_inputs[i]);

                //Lo do in input come corrente
                liquid.SetLiquidCurrent(input_vector, gain);

                //simulo l'input
                liquid.simulateLiquid(stim_lenght_steps, true);
                liquid.ResetLiquidBiasCurrent();

                //simulo per read_out_steps
                liquid.simulateLiquid(readout_delay_steps, true);

                //faccio la lettura degli stati
                liquid_states = liquid.GetLiquidStates(liquid.current_step, 3);
                prediction_vector = model.ComputePrediction(liquid_states);
                target_vector = ReadVectorFromFile(test_targets[i]);

                WriteMatrixToFile(VectorToMatrix(liquid_states, 10, 10), main_dir + "/liquid_states/" + Path.GetFileName(test_inputs[i]), -1);
                WriteMatrixToFile(VectorToMatrix(prediction_vector, 64, 64), prediction_path + Path.GetFileName(test_inputs[i]), 0.5);

                model.ComputeLoss(prediction_vector, target_vector, true);

                //simulo i rimanenti steps
                liquid.simulateLiquid(stim_interval_steps - stim_lenght_steps - readout_delay_steps, true);
            }
            #endregion

            liquid = new LSM(n_exc, n_inh, n_rec, output_size);
            String first_input = test_inputs[0];
            #region Testing feedback
            prediction_path = main_dir + "/test_feedback/";
            Directory.CreateDirectory(prediction_path);
            String nome;
            input_vector = ReadVectorFromFile(first_input);
            for (int i = 0; i < 10; i++)
            {
                if (i < 10)
                    nome = "0";
                else
                    nome = "";
                Console.WriteLine("\nSimulando" + i);

                //Lo do in input come corrente
                liquid.SetLiquidCurrent(input_vector, gain);

                //simulo l'input
                liquid.simulateLiquid(stim_lenght_steps, true);
                liquid.ResetLiquidBiasCurrent();

                //simulo per read_out_steps
                liquid.simulateLiquid(readout_delay_steps, true);

                //faccio la lettura degli stati
                liquid_states = liquid.GetLiquidStates(liquid.current_step, 3);
                prediction_vector = model.ComputePrediction(liquid_states);
                input_vector = prediction_vector;
                target_vector = ReadVectorFromFile(test_targets[i]);

                WriteMatrixToFile(VectorToMatrix(liquid_states, 10, 10), main_dir + "/liquid_states/" + Path.GetFileName(test_inputs[i]), -1);
                WriteMatrixToFile(VectorToMatrix(prediction_vector, 64, 64), prediction_path + nome + i + "_prediction.txt", 0.5);

                model.ComputeLoss(prediction_vector, target_vector, true);

                //simulo i rimanenti steps
                liquid.simulateLiquid(stim_interval_steps - stim_lenght_steps - readout_delay_steps, true);
            }
            #endregion
        }

        public static void CreateDatasetsRandom(String main_dir, int n_samples, double p)
        {
            int[] train_idx = new int[(int)(n_samples * p)];
            int[] train_target_idx = new int[(int)(n_samples * p)];
            int[] test_idx = new int[n_samples - train_idx.Length - 1];
            int[] test_target_idx = new int[n_samples - train_idx.Length - 1];

            int[] all_idx = GenerateDistinctNumbers(n_samples - 1, 0, n_samples - 2);
            int index = 0;
            for (int i = 0; i < train_idx.Length; i++)
            {
                train_idx[i] = all_idx[index];
                train_target_idx[i] = all_idx[index++] + 1;
            }
            for (int i = 0; i < test_idx.Length; i++)
            {
                test_idx[i] = all_idx[index];
                test_target_idx[i] = all_idx[index++] + 1;
            }

            CopySelectedFiles(main_dir + "/frames_DVS", main_dir + "/train_inputs", train_idx);
            CopySelectedFiles(main_dir + "/frames_DVS", main_dir + "/train_targets", train_target_idx);
            CopySelectedFiles(main_dir + "/frames_DVS", main_dir + "/test_inputs", test_idx);
            CopySelectedFiles(main_dir + "/frames_DVS", main_dir + "/test_targets", test_target_idx);

        }
        public static void CreateDatasets(String main_dir, int n_samples, double p)
        {
            int[] train_idx = new int[(int)(n_samples * p)];
            int[] train_target_idx = new int[(int)(n_samples * p)];
            int[] test_idx = new int[n_samples - train_idx.Length - 1];
            int[] test_target_idx = new int[n_samples - train_idx.Length - 1];

            int index = 0;
            for (int i = 0; i < train_idx.Length; i++)
            {
                train_idx[i] = index;
                train_target_idx[i] = index++ + 1;
            }
            for (int i = 0; i < test_idx.Length; i++)
            {
                test_idx[i] = index;
                test_target_idx[i] = index++ + 1;
            }

            CopySelectedFiles(main_dir + "/frames_DVS", main_dir + "/train_inputs", train_idx);
            CopySelectedFiles(main_dir + "/frames_DVS", main_dir + "/train_targets", train_target_idx);
            CopySelectedFiles(main_dir + "/frames_DVS", main_dir + "/test_inputs", test_idx);
            CopySelectedFiles(main_dir + "/frames_DVS", main_dir + "/test_targets", test_target_idx);

        }
        public static int[] GenerateDistinctNumbers(int count, int min, int max)
        {
            Random rand = new Random();
            HashSet<int> numbers = new HashSet<int>();
            while (numbers.Count < count)
            {
                numbers.Add(rand.Next(min, max + 1));
            }
            return numbers.ToArray();
        }
        public static void CopySelectedFiles(string sourceDir, string destDir, int[] fileIndices)
        {
            // Get all files in the source directory
            string[] files = Directory.GetFiles(sourceDir);
            Array.Sort(files);

            // Ensure the destination directory exists
            //Directory.Delete(destDir, true);
            Directory.CreateDirectory(destDir);

            // Copy the files with the given indices to the destination directory
            foreach (int index in fileIndices)
            {
                if (index >= 0 && index < files.Length)
                {
                    string sourceFile = files[index];
                    string destFile = Path.Combine(destDir, Path.GetFileName(sourceFile));
                    File.Copy(sourceFile, destFile, true);
                }
                else
                {
                    Console.WriteLine("Index out of range: " + index);
                }
            }
        }
        public static void training(LSM liquid, Model model, int stim_lenght_steps, int readout_delay_steps, int stim_interval_steps, String train_frame_path, String train_frame_name, int start, int end)
        {
            double[] input_vector;
            for (int i = start; i <= end; i++)
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
                if (i != end)
                    model.AddToXTraining(liquid.GetLiquidStates(liquid.current_step, 3));

                //simulo i rimanenti steps
                liquid.simulateLiquid(stim_interval_steps - stim_lenght_steps - readout_delay_steps, true);
            }

        }

        public static void training(LSM liquid, Model model, int stim_lenght_steps, int readout_delay_steps, int stim_interval_steps, String train_frame_path, String train_frame_name, int start, int end, StateLogger logger)
        {
            double[] input_vector;
            for (int i = start; i <= end; i++)
            {
                Console.WriteLine("Simulando" + i);

                //Leggo il frame da file, e determino l'input current
                input_vector = ReadVectorFromFile(train_frame_path + i + train_frame_name);
                if (i != 0)
                    model.AddToYTraining(input_vector);

                //Lo do in input come corrente
                liquid.SetLiquidCurrent(input_vector, 2);

                //simulo l'input
                liquid.simulateLiquid(stim_lenght_steps, true, logger);
                liquid.ResetLiquidBiasCurrent();

                //simulo per read_out_steps
                liquid.simulateLiquid(readout_delay_steps, true, logger);

                //calcolo gli stati
                if (i != end)
                    model.AddToXTraining(liquid.GetLiquidStates(liquid.current_step, 3));

                //simulo i rimanenti steps
                liquid.simulateLiquid(stim_interval_steps - stim_lenght_steps - readout_delay_steps, true, logger);
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