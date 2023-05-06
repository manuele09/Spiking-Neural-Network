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
            String net_path = pathPc + @"\Saved_Networks\net-";
            #region ABC
            NetworkInput inputAReward = new NetworkInput(1, 1, 1, 1, true, false);
            NetworkInput inputANoRewardRight = new NetworkInput(1, 1, 1, 1, 0, false);
            NetworkInput inputANoRewardRightEnd = new NetworkInput(1, 1, 1, 1, 0, true, 1);
            NetworkInput inputANoRewardLeft = new NetworkInput(1, 1, 1, 1, 1, false);
            NetworkInput inputANoRewardLeftEnd = new NetworkInput(1, 1, 1, 1, 1, true, 1);
            NetworkInput inputANoRewardLeftEnd3 = new NetworkInput(1, 1, 1, 1, 1, true, 3);
            NetworkInput inputANoReward = new NetworkInput(1, 1, 1, 1, false, false);
            NetworkInput inputANoRewardInc = new NetworkInput(1, 1, -1, 1, false, false);
            NetworkInput inputANoRewardEnd = new NetworkInput(1, 1, 1, 1, false, true);
            NetworkInput inputANoRewardEnd2 = new NetworkInput(1, 1, 1, 1, false, true, 2);
            NetworkInput inputANoRewardEnd3 = new NetworkInput(1, 1, 1, 1, false, true, 3);
            NetworkInput inputBReward = new NetworkInput(2, 2, 2, 2, true, false);
            NetworkInput inputBNoReward = new NetworkInput(0, 1, 2, 3, false, false);
            NetworkInput inputBNoRewardInc = new NetworkInput(0, 1, -1, 3, false, false);
            NetworkInput inputBNoRewardRight = new NetworkInput(0, 1, 2, 3, 0, false);
            NetworkInput inputBNoRewardEnd = new NetworkInput(0, 1, 2, 3, false, true);
            NetworkInput inputBNoRewardEnd2 = new NetworkInput(0, 1, 2, 3, false, true, 2);
            NetworkInput inputBNoRewardEnd3 = new NetworkInput(0, 1, 2, 3, false, true, 3);
            NetworkInput inputBNoRewardLeftEnd3 = new NetworkInput(0, 1, 2, 3, 1, true, 3);
            NetworkInput inputBRewardEnd = new NetworkInput(2, 2, 2, 2, true, true);
            NetworkInput inputCNoReward = new NetworkInput(3, 2, 1, 0, false, false);
            NetworkInput inputCNoRewardRightEnd = new NetworkInput(3, 2, 1, 0, 0, true);
            NetworkInput inputCNoRewardLeftEnd = new NetworkInput(3, 2, 1, 0, 1, true);
            NetworkInput inputCNoRewardEnd = new NetworkInput(3, 2, 1, 0, false, true);
            NetworkInput inputCNoRewardEndInc = new NetworkInput(3, -1, 1, 0, false, true);
            NetworkInput inputCNoRewardEnd2 = new NetworkInput(3, 2, 1, 0, false, true, 2);
            NetworkInput inputCNoRewardEnd3 = new NetworkInput(3, 2, 1, 0, false, true, 3);
            NetworkInput inputCReward = new NetworkInput(3, 3, 3, 3, true, false);
            NetworkInput inputCNoRewardRight = new NetworkInput(3, 2, 1, 0, 0, false);
            NetworkInput inputDNoReward = new NetworkInput(2, 2, 0, 1, false, false);
            NetworkInput inputDNoRewardEnd = new NetworkInput(2, 2, 0, 1, false, true);
            NetworkInput inputNull = new NetworkInput(-1, -1, -1, -1);
            NetworkInput inputNullRewardLevel1 = new NetworkInput(-1, -1, -1, -1, false, true, 1);
            NetworkInput inputNullRewardLevel2 = new NetworkInput(-1, -1, -1, -1, false, true, 2);
            NetworkInput inputNullRewardLevel3 = new NetworkInput(-1, -1, -1, -1, false, true, 3);
            NetworkInput inputNullRewardSeq = new NetworkInput(-1, -1, -1, -1, false, true);
            NetworkInput inputNNoReward = new NetworkInput(1, 1, 1, -1, false, false);

            NetworkInput inputANoRewardIncomplete = new NetworkInput(1, 1, 1, -1, false, false);

            NetworkInput inputANoRewardLowColor1 = new NetworkInput(1, 1, 1, 1, false, false, -2.85, 0, 0, 0);
            NetworkInput inputANoRewardLowColor2 = new NetworkInput(1, 1, 1, 1, false, false, -5.71, 0, 0, 0);
            NetworkInput inputANoRewardLowVdistr1 = new NetworkInput(1, 1, 1, 1, false, false, 0, 0, 0, -10);
            NetworkInput inputANoRewardLowVdistr2 = new NetworkInput(1, 1, 1, 1, false, false, 0, 0, 0, -20);
            NetworkInput inputANoRewardLowVdistr3 = new NetworkInput(1, 1, 1, 1, false, false, 0, 0, 0, -30);
            NetworkInput inputANoRewardLowVdistr4 = new NetworkInput(1, 1, 1, 1, false, false, 0, 0, 0, -40);
            NetworkInput inputANoRewardLowVdistr5 = new NetworkInput(1, 1, 1, 1, false, false, 0, 0, 0, -50);
            NetworkInput inputANoRewardLowVdistr6 = new NetworkInput(1, 1, 1, 1, false, false, 0, 0, 0, -60);
            NetworkInput inputANoRewardLowVdistr7 = new NetworkInput(1, 1, 1, 1, false, false, 0, 0, 0, -70);
            NetworkInput inputANoRewardLowVdistr8 = new NetworkInput(1, 1, 1, 1, false, false, 0, 0, 0, -80);
            NetworkInput inputANoRewardLowVdistr9 = new NetworkInput(1, 1, 1, 1, false, false, 0, 0, 0, -90);
            NetworkInput inputANoRewardLowVdistr10 = new NetworkInput(1, 1, 1, 1, false, false, 0, 0, 0, -100);
            NetworkInput inputANoRewardHighVdistr1 = new NetworkInput(1, 1, 1, 1, false, false, 0, 0, 0, 50);
            NetworkInput inputANoRewardHighVdistr2 = new NetworkInput(1, 1, 1, 1, false, false, 0, 0, 0, 60);

            NetworkInput inputBNoRewardLowColor1 = new NetworkInput(2, 2, 2, 2, false, false, -2.85, 0, 0, 0);
            NetworkInput inputBNoRewardLowColor2 = new NetworkInput(2, 2, 2, 2, false, false, -5.71, 0, 0, 0);
            NetworkInput inputBNoRewardLowVdistr1 = new NetworkInput(2, 2, 2, 2, false, false, -5, -5, -5, -5);
            NetworkInput inputBNoRewardLowVdistr2 = new NetworkInput(2, 2, 2, 2, false, false, 0, 0, 0, -6);
            NetworkInput inputBNoRewardHighVdistr1 = new NetworkInput(2, 2, 2, 2, false, false, 1, 1, 1, 1);
            NetworkInput inputBNoRewardHighVdistr2 = new NetworkInput(2, 2, 2, 2, false, false, 0, 0, 0, 2);

            NetworkInput inputDNoRewardLowColor1 = new NetworkInput(0, 0, 0, 0, false, false, -2.85, 0, 0, 0);
            NetworkInput inputDNoRewardLowColor2 = new NetworkInput(0, 0, 0, 0, false, false, -5.71, 0, 0, 0);
            NetworkInput inputDNoRewardLowVdistr1 = new NetworkInput(0, 0, 0, 0, false, false, 0, 0, 0, -2);
            NetworkInput inputDNoRewardLowVdistr2 = new NetworkInput(0, 0, 0, 0, false, false, 0, 0, 0, -3);
            NetworkInput inputDNoRewardHighVdistr1 = new NetworkInput(0, 0, 0, 0, false, false, 0, 0, 0, 3);
            NetworkInput inputDNoRewardHighVdistr2 = new NetworkInput(0, 0, 0, 0, false, false, 0, 0, 0, 4);
            #endregion

            #region Figure Input
            NetworkInput YellowSquare = new NetworkInput(0, 0, -1, -1, false, false);
            NetworkInput YellowRectangle = new NetworkInput(0, 1, -1, -1, false, false);
            NetworkInput YellowTriangle = new NetworkInput(0, 2, -1, -1, false, false);
            NetworkInput YellowCircle = new NetworkInput(0, 3, -1, -1, false, false);
            NetworkInput YellowCircleSx = new NetworkInput(0, 3, -1, -1, 1, false, -1);

            NetworkInput YellowRectangleDx = new NetworkInput(0, 1, -1, -1, 0, false, -1);
            NetworkInput YellowRectangleSx = new NetworkInput(0, 1, -1, -1, 1, false, -1);
            NetworkInput YellowRectangleDxEnd = new NetworkInput(0, 1, -1, -1, 0, true, 2);
            NetworkInput YellowRectangleSxEnd = new NetworkInput(0, 1, -1, -1, 1, true, 1);
            NetworkInput YellowCircleDx = new NetworkInput(0, 3, -1, -1, 0, false, -1);

            NetworkInput RedSquare = new NetworkInput(1, 0, -1, -1, false, false);
            NetworkInput RedRectangle = new NetworkInput(1, 1, -1, -1, false, false);
            NetworkInput RedTriangle = new NetworkInput(1, 2, -1, -1, false, false);
            NetworkInput RedCircle = new NetworkInput(1, 3, -1, -1, false, false);
            NetworkInput RedCircleSxEnd = new NetworkInput(1, 3, -1, -1, 1, true, 1);
            NetworkInput RedCircleSxEnd3 = new NetworkInput(1, 3, -1, -1, 1, true, 3);
            NetworkInput RedCircleDxEnd = new NetworkInput(1, 3, -1, -1, 0, true, 1);
            NetworkInput RedCircleDx = new NetworkInput(1, 3, -1, -1, 0, false, -1);
            NetworkInput RedCircleSx = new NetworkInput(1, 3, -1, -1, 1, false, -1);

            NetworkInput RedCircleDxEnd3 = new NetworkInput(1, 3, -1, -1, 0, true, 3);


            NetworkInput RedRectangleSx = new NetworkInput(1, 1, -1, -1, 1, false, -1);
            NetworkInput RedRectangleDx = new NetworkInput(1, 1, -1, -1, 0, false, -1);
            NetworkInput RedRectangleSxEnd = new NetworkInput(1, 1, -1, -1, 1, true, 1);
            NetworkInput RedRectangleDxEnd = new NetworkInput(1, 1, -1, -1, 0, true, 2);



            NetworkInput BlueSquare = new NetworkInput(2, 0, -1, -1, false, false);
            NetworkInput BlueRectangle = new NetworkInput(2, 1, -1, -1, false, false);
            NetworkInput BlueTriangle = new NetworkInput(2, 2, -1, -1, false, false);
            NetworkInput BlueCircle = new NetworkInput(2, 3, -1, -1, false, false);

            NetworkInput BlueCircleSx = new NetworkInput(2, 3, -1, -1, 1, false, -1);
            NetworkInput BlueCircleDxEnd = new NetworkInput(2, 3, -1, -1, 0, true, 1);

            NetworkInput BlueRectangleDx = new NetworkInput(2, 1, -1, -1, 0, false, -1);
            NetworkInput BlueRectangleDxEnd3 = new NetworkInput(2, 1, -1, -1, 0, true, 3);
            NetworkInput BlueRectangleSx = new NetworkInput(2, 1, -1, -1, 1, false, -1);
            NetworkInput BlueTriangleSx = new NetworkInput(2, 2, -1, -1, 1, false, -1);

            #endregion

            Network net = Network.generateNetwork();
           // net = BinarySerialization.ReadFromBinaryFile<Network>(net_path + 0 + ".bin");

            #region learning
            System.Console.WriteLine("*** *** ****** *** *** *** *** *** *** *** Learning *** *** *** ****** *** *** ****** *** ***");

            #region prima_sequenza
            List<NetworkInput> prima_sequenza = new List<NetworkInput>();
            List<string> str_prima_sequenza = new List<string>();
            List<int> prima_targets = new List<int>();

            prima_sequenza.Add(BlueRectangleDx);
            prima_sequenza.Add(RedRectangleSx);
            prima_sequenza.Add(BlueCircleSx);
            prima_sequenza.Add(RedCircleSxEnd3);

            str_prima_sequenza.Add("Blue Rectangle Dx");
            str_prima_sequenza.Add("Red Rectangle Sx");
            str_prima_sequenza.Add("Blue Circle Sx");
            str_prima_sequenza.Add("Red Circle Sx End3");

            prima_targets.Add(0);
            prima_targets.Add(1);
            prima_targets.Add(2);
            prima_targets.Add(3);
            #endregion
            #region seconda_sequenza
            List<NetworkInput> seconda_sequenza = new List<NetworkInput>();
            List<string> str_seconda_sequenza = new List<string>();
            List<int> seconda_targets = new List<int>();

            seconda_sequenza.Add(YellowCircleSx);
            seconda_sequenza.Add(YellowRectangleDxEnd);

            str_seconda_sequenza.Add("Yellow Circle Sx");
            str_seconda_sequenza.Add("Yellow Rectangle Dx End");

            seconda_targets.Add(4);
            seconda_targets.Add(5);
            #endregion
            #region terza_sequenza
            List<NetworkInput> terza_sequenza = new List<NetworkInput>();
            List<string> str_terza_sequenza = new List<string>();
            List<int> terza_targets = new List<int>();

            terza_sequenza.Add(BlueRectangleSx);
            terza_sequenza.Add(YellowRectangleSxEnd);

            str_terza_sequenza.Add("Blue Rectangle Sx");
            str_terza_sequenza.Add("Yellow Rectangle Sx End");

            terza_targets.Add(0);
            terza_targets.Add(5);
            #endregion
            #region quarta_sequenza
            List<NetworkInput> quarta_sequenza = new List<NetworkInput>();
            List<string> str_quarta_sequenza = new List<string>();
            List<int> quarta_targets = new List<int>();

            quarta_sequenza.Add(YellowCircleDx);
            quarta_sequenza.Add(RedRectangleDxEnd);

            str_quarta_sequenza.Add("Yellow Circle Dx");
            str_quarta_sequenza.Add("Red Rectangle Dx End");

            quarta_targets.Add(4);
            quarta_targets.Add(1);
            #endregion
            for (int l = 0; l < 2; l++)
            {
                net.current_learning++;
                SimulateInputs(net, prima_sequenza, str_prima_sequenza, prima_targets, 1);
                SimulateInputs(net, seconda_sequenza, str_seconda_sequenza, seconda_targets, 1);
                SimulateInputs(net, terza_sequenza, str_terza_sequenza, terza_targets, 1);
                SimulateInputs(net, quarta_sequenza, str_quarta_sequenza, quarta_targets, 1);
                BinarySerialization.WriteToBinaryFile<Network>(net_path + net.current_learning + ".bin", net);
            }
            #endregion

            #region testing

            List<NetworkInput> imagination = new List<NetworkInput>();
            List<string> str_imagination = new List<string>();

            imagination.Add(BlueRectangle);
            imagination.Add(RedRectangle);
            imagination.Add(BlueCircle);
            imagination.Add(RedCircle);
            imagination.Add(YellowCircle);
            imagination.Add(YellowRectangle);
            
            str_imagination.Add("Blue Rectangle");
            str_imagination.Add("Red Rectangle");
            str_imagination.Add("Blue Circle");
            str_imagination.Add("Red Circle");
            str_imagination.Add("Yellow Circle");
            str_imagination.Add("Yellow Rectangle");



            System.Console.WriteLine("*** *** ****** *** *** *** *** *** *** *** Testing *** *** *** ****** *** *** ****** *** ***");
            //net = BinarySerialization.ReadFromBinaryFile<Network>(net_path + 4 + ".bin");


            #region sequenza_test
            List<NetworkInput> test_sequenza = new List<NetworkInput>();
            List<string> str_test_sequenza = new List<string>();
            
            test_sequenza.Add(BlueRectangle);
            test_sequenza.Add(inputNull);
            test_sequenza.Add(inputNull);
            test_sequenza.Add(inputNull);
            test_sequenza.Add(inputNull);
            
            str_test_sequenza.Add("Blue Rectangle");
            str_test_sequenza.Add("Input Null");
            str_test_sequenza.Add("Input Null");
            str_test_sequenza.Add("Input Null");
            str_test_sequenza.Add("Input Null");
            
            #endregion


            //SimulateInputs(net, test_sequenza, str_test_sequenza, null, 0);
            SimulateInputsImagination(net, imagination, str_imagination, null, 0);

         

            #endregion

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

        public static void SimulateInputs(Network net, List<NetworkInput> inputs, List<string> str_inputs, List<int> target_inputs, int learn)
        {
            String pathPc = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string output_message;
            for (int i = 0; i < inputs.Count; i++)
            {
                Console.WriteLine($"*** *** *** *** Simulazione {net.current_epoch + 1} *** *** *** ***");

                StateLogger sl = new StateLogger(pathPc + @"\Dati\Neurons" + net.current_epoch + ".txt", pathPc + @"\Dati\Synapse" + net.current_epoch + ".txt", pathPc + @"\Dati\NeuronMorrisLecar" + net.current_epoch + ".txt", true, false, true);
                StateLogger slStdp = new StateLogger(pathPc + @"\Dati\NeuronsStdp" + net.current_epoch + ".txt", pathPc + @"\Dati\SynapseSTDP" + net.current_epoch + ".txt", false, true);

                net.resetInputs();
                net.setInput(inputs[i]);
                output_message = "\t\t\t\t\t\t\t\t" + (i + 1) + "o Input Dato";
                if (str_inputs != null)
                    output_message += ": " + str_inputs[i];
                if (target_inputs != null)
                    output_message += "; Expected Target: " + target_inputs[i];
                Console.WriteLine(output_message);

                Console.WriteLine("Sim started at " + DateTime.Now);
                if (learn == 1)
                    net.learnLiquid(sl, slStdp, net.current_epoch);
                else
                    net.testLiquid(sl, slStdp, net.current_epoch);
                Console.WriteLine("Sim finished at " + DateTime.Now);

                net.current_epoch++;
            }
        }

        public static void SimulateInputsImagination(Network net, List<NetworkInput> inputs, List<string> str_inputs, List<int> target_inputs, int learn)
        {
            String pathPc = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string output_message;
            int prima = 0;

            for (int i = 0; i < Constants.RINGS; i++)
            {
                Console.WriteLine($"*** *** *** *** Simulazione {net.current_epoch + 1} *** *** *** ***");

                StateLogger sl = new StateLogger(pathPc + @"\Dati\Neurons" + net.current_epoch + ".txt", pathPc + @"\Dati\Synapse" + net.current_epoch + ".txt", pathPc + @"\Dati\NeuronMorrisLecar" + net.current_epoch + ".txt", true, false, true);
                StateLogger slStdp = new StateLogger(pathPc + @"\Dati\NeuronsStdp" + net.current_epoch + ".txt", pathPc + @"\Dati\SynapseSTDP" + net.current_epoch + ".txt", false, true);

                net.resetInputs();
                if (prima == 0)
                {
                    Console.WriteLine("Fase di Imagination.");
                    for (int j = 0; j < inputs.Count; j++)
                    {
                        output_message = "\t\t\t\t\t\t\t\t" + (j + 1) + "o Input Dato";
                        if (str_inputs != null)
                            output_message += ": " + str_inputs[j];
                        if (target_inputs != null)
                            output_message += "; Expected Target: " + target_inputs[j];
                        net.setInput(inputs[j]);
                        Console.WriteLine(output_message);
                    }
                    prima++;
                }
                else
                {
                    Console.WriteLine("Input dato: Null");
                    net.setInput(new NetworkInput(-1, -1, -1, -1));
                }


                Console.WriteLine("Sim started at " + DateTime.Now);
                net.testLiquid(sl, slStdp, net.current_epoch);
                Console.WriteLine("Sim finished at " + DateTime.Now);

                net.current_epoch++;
            }
        }

    }
}