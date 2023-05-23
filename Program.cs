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
        public static void Maina()
        {
            int[] indexs = { 0, 3, 6 };
            //CopySelectedFiles("../../../PyScript/C#/frames_DVS", "../../../PyScript/C#/train", indexs);
            CreateDatasets();
            Console.WriteLine("FAtto!");

            String prediction_frame_path = "../../../PyScript/Prediction/";
            String prediction_frame_name = "_prediction.txt";

            String train_frame_path = "../../../PyScript/dati/video_5/frames_DVS/";
            String train_frame_name = "_frame_DVS.txt";
            int n_train = 23;
            int start = 0;
            int end = 100;

            String test_frame_path = "../../../PyScript/dati/video_6/test_target/";
            String test_frame_name = "_test_target.txt";

            test_frame_path = train_frame_path;
            test_frame_name = train_frame_name;
            int n_test = 23;


            double stim_interval = 180; //ms
            double stim_lenght = 100; //ms
            double readout_delay = 80; //ms
            int gain = 2;


            int stim_interval_steps = (int)(stim_interval / Constants.INTEGRATION_STEP);
            int stim_lenght_steps = (int)(stim_lenght / Constants.INTEGRATION_STEP);
            int readout_delay_steps = (int)(readout_delay / Constants.INTEGRATION_STEP);

            int output_size = 64 * 64;

            int n_exc = 400; //400
            int n_inh = 100; //100
            int n_rec = n_exc / 2; //200

            double[] input_vector; //input_size 
            double[] prediction_vector;

            StateLogger logger = new StateLogger("../../../PyScript/Logs/log.txt");

            LSM liquid = new LSM(n_exc, n_inh, n_rec, output_size);

            Model model = new Model(n_rec, output_size, 210);

            #region Training Video 5


            training(liquid, model, stim_lenght_steps, readout_delay_steps, stim_interval_steps, train_frame_path, train_frame_name, 0, 100, logger);
            liquid = new LSM(n_exc, n_inh, n_rec, output_size);

            training(liquid, model, stim_lenght_steps, readout_delay_steps, stim_interval_steps, train_frame_path, train_frame_name, 200, 300);
            liquid = new LSM(n_exc, n_inh, n_rec, output_size);


            logger.printLog();
            //Calcolo i Pesi
            model.LearnW();
            Console.WriteLine(model.ComputeTrainLoss());

            //BinarySerialization.WriteToBinaryFile("net.bin", liquid);
            #endregion


            #region Testing Video 5

            // Network net = BinarySerialization.ReadFromBinaryFile<Network>("net.bin");
            for (int i = 0; i < 100; i++)
            {
                Console.WriteLine("Simulando" + i);

                //Leggo il frame da file, e determino l'input current
                input_vector = ReadVectorFromFile(test_frame_path + i + test_frame_name);

                //Lo do in input come corrente
                liquid.SetLiquidCurrent(input_vector, gain);

                //simulo il liquido per stim_lenght ms
                liquid.simulateLiquid(stim_lenght_steps, true);
                liquid.ResetLiquidBiasCurrent();

                //simulo per read_out_steps
                liquid.simulateLiquid(readout_delay_steps, true);

                //calcolo gli stati
                prediction_vector = model.ComputePrediction(liquid.GetLiquidStates(liquid.current_step, 3));
                WriteMatrixToFile(VectorToMatrix(prediction_vector, 64, 64), prediction_frame_path + i + "_prima.txt", 0.5);

                //simulo i rimanenti steps
                liquid.simulateLiquid(stim_interval_steps - stim_lenght_steps - readout_delay_steps, true);
            }
            liquid = new LSM(n_exc, n_inh, n_rec, output_size);

            for (int i = 100; i < 200; i++)
            {
                Console.WriteLine("Simulando" + i);

                //Leggo il frame da file, e determino l'input current
                input_vector = ReadVectorFromFile(test_frame_path + i + test_frame_name);

                //Lo do in input come corrente
                liquid.SetLiquidCurrent(input_vector, gain);

                //simulo il liquido per stim_lenght ms
                liquid.simulateLiquid(stim_lenght_steps, true);
                liquid.ResetLiquidBiasCurrent();

                //simulo per read_out_steps
                liquid.simulateLiquid(readout_delay_steps, true);

                //calcolo gli stati
                prediction_vector = model.ComputePrediction(liquid.GetLiquidStates(liquid.current_step, 3));
                WriteMatrixToFile(VectorToMatrix(prediction_vector, 64, 64), prediction_frame_path + i + prediction_frame_name, 0.5);

                //simulo i rimanenti steps
                liquid.simulateLiquid(stim_interval_steps - stim_lenght_steps - readout_delay_steps, true);
            }
            liquid = new LSM(n_exc, n_inh, n_rec, output_size);

            for (int i = 200; i < 300; i++)
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
                WriteMatrixToFile(VectorToMatrix(prediction_vector, 64, 64), prediction_frame_path + i + "_seconda.txt", 0.5);

                //simulo i rimanenti steps
                liquid.simulateLiquid(stim_interval_steps - stim_lenght_steps - readout_delay_steps, true);
            }


            #endregion

            
            /*
            LSM liquid = new LSM(n_exc, n_inh, n_rec, output_size);

            Model model = new Model(input_size, output_size, 30);
            #region Training Video 2
            train_frame_path = "../../../PyScript/dati/video_2/frames_DVS/";

            training(liquid, model, stim_lenght_steps, readout_delay_steps, stim_interval_steps, train_frame_path, train_frame_name, 0, 30);
            liquid.resetState();
            //training(liquid, model, stim_lenght_steps, readout_delay_steps, stim_interval_steps, train_frame_path, train_frame_name, 200, 300);
            //liquid.resetState();


            //Calcolo i Pesi
            model.LearnW();
            Console.WriteLine(model.ComputeTrainLoss());

            //BinarySerialization.WriteToBinaryFile("net.bin", liquid);
            #endregion


            #region Testing Video 2

            // Network net = BinarySerialization.ReadFromBinaryFile<Network>("net.bin");
            for (int i = 0; i < 30; i++)
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
                WriteMatrixToFile(VectorToMatrix(prediction_vector, 64, 64), prediction_frame_path + i + "_prima.txt", 0.5);

                //simulo i rimanenti steps
                liquid.simulateLiquid(stim_interval_steps - stim_lenght_steps - readout_delay_steps, true);
            }
            liquid.resetState();
            for (int i = 30; i < 37; i++)
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
                WriteMatrixToFile(VectorToMatrix(prediction_vector, 64, 64), prediction_frame_path + i + prediction_frame_name, 0.5);

                //simulo i rimanenti steps
                liquid.simulateLiquid(stim_interval_steps - stim_lenght_steps - readout_delay_steps, true);
            }
            liquid.resetState();
            
            #endregion
            */
        }

        public static void CreateDatasets()
        {
            int n_samples = 51; //l'ultimo non potrà mai essere fornito in ingresso
            int[] train_idx = new int[(int) (n_samples * 0.8)];
            int[] train_target_idx = new int[(int) (n_samples * 0.8)];
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

            CopySelectedFiles("../../../PyScript/C#/frames_DVS", "../../../PyScript/C#/train", train_idx);
            CopySelectedFiles("../../../PyScript/C#/frames_DVS", "../../../PyScript/C#/train_target", train_target_idx);
            CopySelectedFiles("../../../PyScript/C#/frames_DVS", "../../../PyScript/C#/test", test_idx);
            CopySelectedFiles("../../../PyScript/C#/frames_DVS", "../../../PyScript/C#/test_target", test_target_idx);

        }
        public static int[] GenerateDistinctNumbers(int count, int min, int max)
        {
            Random rand = new Random(123);
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
                        if (treshold > -100)
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