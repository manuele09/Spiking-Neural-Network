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

//Le scritture su file impiegano il 30% dell'esecuzione
//verificare che la non simulazione delle sinapsi inibitorie del context
//non influiscano eccessivamente sulla normale esecuzione
//modifica morris con izikevich

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
        public static void Main(string[] args)
        {
            String prediction_frame_path = "../../../PyScript/video_1/SimTestPred/";
            String prediction_frame_name = "_prediction.txt";

            String train_frame_path = "../../../PyScript/video_1/train_target/";
            String train_frame_name = "_train_target.txt";
            int n_train = 25;

            String test_frame_path = "../../../PyScript/video_1/test_target/";
            String test_frame_name = "_test_target.txt";

            //test_frame_path = train_frame_path;
            //test_frame_name = train_frame_name;
            int n_test = 25;

            double sample_time = 10; //ms
            int sample_steps = (int)(sample_time / Constants.INTEGRATION_STEP);

            double[,] input_matrix; //input_i * input_j
            double[,] liquid_states_train = new double[n_train, Constants.LIQUID_DIMENSION_I * Constants.LIQUID_DIMENSION_J];
            double[,] liquid_state_test = new double[1, Constants.LIQUID_DIMENSION_I * Constants.LIQUID_DIMENSION_J];
            double[,] targets_train = new double[50, Constants.FIRST_LAYER_DIMENSION_I * Constants.FIRST_LAYER_DIMENSION_J];

            #region Training
            Network net = Network.generateNetwork();

            for (int i = 0; i < n_train; i++)
            {
                //Leggo il frame da file
                if (i == 0)
                    input_matrix = ReadMatrixFromFile(train_frame_path + i + train_frame_name);
                else //lo salvo come target (predizione futura)
                    input_matrix = ReadMatrixFromFile(train_frame_path + i + train_frame_name, targets_train, i - 1); //stato futuro

                //Lo do in input come corrente
                net.SetLiquidCurrent(input_matrix, 3);

                //simulo il liquido
                net.simulateLiquid(sample_steps * i, sample_steps);

                //Leggo lo stato del liquido e lo aggiungo alla matrice degli stati
                if (i != (n_train - 1))
                    net.AddLiquidStates(liquid_states_train, i, sample_steps * (i + 1) - 1, 3); //stato passato

                Console.WriteLine("Simulato" + i);
            }

            //Calcolo i Pesi e li salvo nel network.
            double[,] inv = addBias(liquid_states_train).PseudoInverse();
            double[,] W = inv.Dot(targets_train);
            net.saveWeights(W);

            // BinarySerialization.WriteToBinaryFile("net.bin", net);
            net.resetLiquid();
            #endregion

            #region Testing
            // Network net = BinarySerialization.ReadFromBinaryFile<Network>("net.bin");
            for (int i = 0; i < n_test - 1; i++)
            {
                input_matrix = ReadMatrixFromFile(test_frame_path + i + test_frame_name);
                net.SetLiquidCurrent(input_matrix, 3);
                net.simulateLiquid(sample_steps * i, sample_steps);
                net.AddLiquidStates(liquid_state_test, 0, sample_steps * (i + 1) - 1, 3);

                WritePreditction(addBias(liquid_state_test).Dot(net.W), prediction_frame_path + i + prediction_frame_name);
                Console.WriteLine("Simulato" + i);
            }

            #endregion

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
                int matrixSize = lines.Length;
                double[,] matrix = new double[matrixSize, matrixSize];

                for (int i = 0; i < matrixSize; i++)
                {
                    var line = lines[i].Split(' ').Select(int.Parse).ToArray();
                    for (int j = 0; j < matrixSize; j++)
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

        public static void WriteMatrixToFile(double[,] matrix, string filePath)
        {
            using (StreamWriter file = new StreamWriter(filePath))
            {
                int rowCount = matrix.GetLength(0);
                int colCount = matrix.GetLength(1);


                for (int i = 0; i < rowCount; i++)
                {
                    for (int j = 0; j < colCount; j++)
                        file.Write(matrix[i, j].ToString(CultureInfo.InvariantCulture) + " ");
                    // Write a newline character after each row except the last one
                    if (i < rowCount - 1)
                        file.WriteLine();
                }
            }
        }

        public static void WritePreditction(double[,] prediction, string filePath)
        {
            using (StreamWriter file = new StreamWriter(filePath))
            {



                for (int i = 0; i < Constants.FIRST_LAYER_DIMENSION_I; i++)
                {
                    for (int j = 0; j < Constants.FIRST_LAYER_DIMENSION_J; j++)
                    {
                        /*if (prediction[0, i * Constants.FIRST_LAYER_DIMENSION_J + j] > 0.25) //0.09
                            file.Write(1.ToString(CultureInfo.InvariantCulture) + " ");
                        else
                            file.Write(0.ToString(CultureInfo.InvariantCulture) + " ");*/
                        file.Write(prediction[0, i * Constants.FIRST_LAYER_DIMENSION_J + j].ToString(CultureInfo.InvariantCulture) + " ");

                    }
                    file.WriteLine();

                }
            }
        }
        public static void PrintMatrix(double[,] matrix1, double[,] matrix2)
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
                    if ((matrix1[i, j] - matrix2[i, j]) > 0.1)
                        Console.WriteLine(matrix1[i, j]);
                }
                //Console.WriteLine();
            }
        }


    }
}