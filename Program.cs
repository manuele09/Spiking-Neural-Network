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

namespace SLN
{
    /// <summary>
    /// Class containing the Main method
    /// </summary>
	public class Program
    {


        /// <summary>
        /// The Main method
        /// </summary>
        /// <param name="args">Currently not used</param>
        public static void Main(string[] args)
        {

            String pathPc = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            String frame_path = "../../../PyScript/video_1/frames_DVS/";
            String frame_name = "_frame_DVS.txt";
            double[,] input_matrix;
            Network net = Network.generateNetwork();
            double[,] liquid_state = new double[50, Constants.LIQUID_DIMENSION_I * Constants.LIQUID_DIMENSION_J];
            double[,] targets = new double[50, Constants.FIRST_LAYER_DIMENSION_I * Constants.FIRST_LAYER_DIMENSION_J];
            double sample_time = 10; //ms
            int sample_steps = (int)(sample_time / Constants.INTEGRATION_STEP);

            for (int i = 0; i < 51; i++)
            {
                if (i == 0)
                    input_matrix = ReadMatrixFromFile(frame_path + i + frame_name); 
                else
                    input_matrix = ReadMatrixFromFile(frame_path + i + frame_name, targets, i - 1); //stato futuro
                net.SetLiquidCurrent(input_matrix, 3);
                net.simulateLiquid(sample_steps);
                if (i != 50)
                    net.AddLiquidStates(liquid_state, i, sample_steps, 3); //stato passato
                Console.WriteLine("Simulato" + i);
            }

            // S  * W = T;
            //N*d  d*y N*y 
            double[,] W = liquid_state.PseudoInverse().Multiply(targets);
            net.saveWeights(W);
            //WriteMatrixToFile(liquid_state, "../../../PyScript/video_1/LiquidStates/" + i + "_liquid.txt");
            //PrintMatrix(liquid_state);





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
        public static void PrintMatrix(double[,] matrix)
        {
            if (matrix == null)
            {
                Console.WriteLine("Matrix is null");
                return;
            }

            int rowCount = matrix.GetLength(0);
            int colCount = matrix.GetLength(1);

            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < colCount; j++)
                {
                    Console.Write(matrix[i, j] + " ");
                }
                Console.WriteLine();
            }
        }

        public static void Run_cmd()
        {
            string fileName = @"C:\Users\Emanuele\Desktop\Detection_varie\Form_color_detection.py";

            Process p = new Process();
            p.StartInfo = new ProcessStartInfo(@"C:\Users\Emanuele\AppData\Local\Programs\Python\Python311\python.exe", fileName)
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            p.Start();

            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();


            Console.WriteLine(output);

            //Console.ReadLine();
        }
        public static void Run_cmd_2()
        {
            string fileName = @"C:\Users\Emanuele\Desktop\Detection_varie\Form_color_detection_Double.py";

            Process p = new Process();
            p.StartInfo = new ProcessStartInfo(@"C:\Users\Emanuele\AppData\Local\Programs\Python\Python311\python.exe", fileName)
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            p.Start();

            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();


            Console.WriteLine(output);

            //Console.ReadLine();
        }

        public static void Get_input(Network net)
        {

            int[] features = new int[4];
            int i = 0;
            //Thread.Sleep(1500);
            Run_cmd();

            String file = "C:\\Users\\Emanuele\\Desktop\\Detection_varie\\input.txt";
            StreamReader dataStream = new StreamReader(file);
            string datasample;
            //while ((datasample = dataStream.ReadLine()) != null)
            datasample = dataStream.ReadLine();
            string[] numberArray = datasample.Split(new char[] { '(', ',', ' ', ')' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string numberString in numberArray)
            {
                int number;
                if (int.TryParse(numberString, out number))
                {
                    features[i] = number;
                }
                i += 1;
            }
            dataStream.Close();


            Console.WriteLine(String.Format("Input Letto: {0}, {1}, {2}, {3}", features[0], features[1], features[2], features[3]));
            if (features[0] == 0 && features[1] == 0 && features[2] == -1 && features[3] == -1)
                Console.WriteLine("             Yellow Square");
            if (features[0] == 0 && features[1] == 1 && features[2] == -1 && features[3] == -1)
                Console.WriteLine("             Yellow Rectangle");
            if (features[0] == 0 && features[1] == 2 && features[2] == -1 && features[3] == -1)
                Console.WriteLine("             Yellow Triangle");
            if (features[0] == 0 && features[1] == 3 && features[2] == -1 && features[3] == -1)
                Console.WriteLine("             Yellow Circle");

            if (features[0] == 1 && features[1] == 0 && features[2] == -1 && features[3] == -1)
                Console.WriteLine("             Red Square");
            if (features[0] == 1 && features[1] == 1 && features[2] == -1 && features[3] == -1)
                Console.WriteLine("             Red Rectangle");
            if (features[0] == 1 && features[1] == 2 && features[2] == -1 && features[3] == -1)
                Console.WriteLine("             Red Triangle");
            if (features[0] == 1 && features[1] == 3 && features[2] == -1 && features[3] == -1)
                Console.WriteLine("             Red Circle");

            if (features[0] == 2 && features[1] == 0 && features[2] == -1 && features[3] == -1)
                Console.WriteLine("             Blu Square");
            if (features[0] == 2 && features[1] == 1 && features[2] == -1 && features[3] == -1)
                Console.WriteLine("             Blu Rectangle");
            if (features[0] == 2 && features[1] == 2 && features[2] == -1 && features[3] == -1)
                Console.WriteLine("Blu Triangle");
            if (features[0] == 2 && features[1] == 3 && features[2] == -1 && features[3] == -1)
                Console.WriteLine("             Blu Circle");
            Console.WriteLine("Scegliere: niente (-1), destra (0), sinistra (1)");
            //int scelta = Convert.ToInt32(Console.ReadLine());
            // Console.WriteLine("Scegliere: niente (-1), destra (0), sinistra (1)");
            // int reward = Convert.ToInt32(Console.ReadLine());

            net.setInput(new NetworkInput(features[0], features[1], features[2], features[3], false, false));
            net.setInput(new NetworkInput(-1, -1, -1, -1, false, true, 1));
            //NetworkInput inputNullRewardLevel1 = new NetworkInput(-1, -1, -1, -1, false, true, 1);ù
            //NetworkInput inputANoRewardLeft = new NetworkInput(1, 1, 1, 1, 1, false);
            //ret blu dx
            //ret rosso sx
            //tr blu sinistra
            //red circle dx reward 3

            //yellow circle dx
            //yellow rectangle sx
            //

        }

        public static void Get_input_2(Network net)
        {

            int[] features = new int[4];
            int i = 0;
            //Thread.Sleep(1500);
            Run_cmd_2();

            String file = "C:\\Users\\Emanuele\\Desktop\\Detection_varie\\input.txt";
            StreamReader dataStream = new StreamReader(file);
            string datasample;
            //while ((datasample = dataStream.ReadLine()) != null)
            datasample = dataStream.ReadLine();
            string[] numberArray = datasample.Split(new char[] { '(', ',', ' ', ')' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string numberString in numberArray)
            {
                int number;
                if (int.TryParse(numberString, out number))
                {
                    features[i] = number;
                }
                i += 1;
            }


            Console.WriteLine(String.Format("Input Letto: {0}, {1}, {2}, {3}", features[0], features[1], features[2], features[3]));
            if (features[0] == 0 && features[1] == 0 && features[2] == -1 && features[3] == -1)
                Console.WriteLine("             Yellow Square");
            if (features[0] == 0 && features[1] == 1 && features[2] == -1 && features[3] == -1)
                Console.WriteLine("             Yellow Rectangle");
            if (features[0] == 0 && features[1] == 2 && features[2] == -1 && features[3] == -1)
                Console.WriteLine("             Yellow Triangle");
            if (features[0] == 0 && features[1] == 3 && features[2] == -1 && features[3] == -1)
                Console.WriteLine("             Yellow Circle");

            if (features[0] == 1 && features[1] == 0 && features[2] == -1 && features[3] == -1)
                Console.WriteLine("             Red Square");
            if (features[0] == 1 && features[1] == 1 && features[2] == -1 && features[3] == -1)
                Console.WriteLine("             Red Rectangle");
            if (features[0] == 1 && features[1] == 2 && features[2] == -1 && features[3] == -1)
                Console.WriteLine("             Red Triangle");
            if (features[0] == 1 && features[1] == 3 && features[2] == -1 && features[3] == -1)
                Console.WriteLine("             Red Circle");

            if (features[0] == 2 && features[1] == 0 && features[2] == -1 && features[3] == -1)
                Console.WriteLine("             Blu Square");
            if (features[0] == 2 && features[1] == 1 && features[2] == -1 && features[3] == -1)
                Console.WriteLine("             Blu Rectangle");
            if (features[0] == 2 && features[1] == 2 && features[2] == -1 && features[3] == -1)
                Console.WriteLine("Blu Triangle");
            if (features[0] == 2 && features[1] == 3 && features[2] == -1 && features[3] == -1)
                Console.WriteLine("             Blu Circle");
            //int scelta = Convert.ToInt32(Console.ReadLine());
            // Console.WriteLine("Scegliere: niente (-1), destra (0), sinistra (1)");
            // int reward = Convert.ToInt32(Console.ReadLine());

            net.setInput(new NetworkInput(features[0], features[1], features[2], features[3], false, false));
            /////////////////////////////////////////
            datasample = dataStream.ReadLine();
            numberArray = datasample.Split(new char[] { '(', ',', ' ', ')' }, StringSplitOptions.RemoveEmptyEntries);
            i = 0;
            foreach (string numberString in numberArray)
            {
                int number;
                if (int.TryParse(numberString, out number))
                {
                    features[i] = number;
                }
                i += 1;
            }
            dataStream.Close();


            Console.WriteLine(String.Format("Input Letto: {0}, {1}, {2}, {3}", features[0], features[1], features[2], features[3]));
            if (features[0] == 0 && features[1] == 0 && features[2] == -1 && features[3] == -1)
                Console.WriteLine("             Yellow Square");
            if (features[0] == 0 && features[1] == 1 && features[2] == -1 && features[3] == -1)
                Console.WriteLine("             Yellow Rectangle");
            if (features[0] == 0 && features[1] == 2 && features[2] == -1 && features[3] == -1)
                Console.WriteLine("             Yellow Triangle");
            if (features[0] == 0 && features[1] == 3 && features[2] == -1 && features[3] == -1)
                Console.WriteLine("             Yellow Circle");

            if (features[0] == 1 && features[1] == 0 && features[2] == -1 && features[3] == -1)
                Console.WriteLine("             Red Square");
            if (features[0] == 1 && features[1] == 1 && features[2] == -1 && features[3] == -1)
                Console.WriteLine("             Red Rectangle");
            if (features[0] == 1 && features[1] == 2 && features[2] == -1 && features[3] == -1)
                Console.WriteLine("             Red Triangle");
            if (features[0] == 1 && features[1] == 3 && features[2] == -1 && features[3] == -1)
                Console.WriteLine("             Red Circle");

            if (features[0] == 2 && features[1] == 0 && features[2] == -1 && features[3] == -1)
                Console.WriteLine("             Blu Square");
            if (features[0] == 2 && features[1] == 1 && features[2] == -1 && features[3] == -1)
                Console.WriteLine("             Blu Rectangle");
            if (features[0] == 2 && features[1] == 2 && features[2] == -1 && features[3] == -1)
                Console.WriteLine("Blu Triangle");
            if (features[0] == 2 && features[1] == 3 && features[2] == -1 && features[3] == -1)
                Console.WriteLine("             Blu Circle");


            net.setInput(new NetworkInput(features[0], features[1], features[2], features[3], false, false));


        }


    }
}