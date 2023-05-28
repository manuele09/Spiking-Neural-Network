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
using System.Runtime.ConstrainedExecution;
using Accord.Math;
using System.Globalization;
using System.IO.Ports;

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
            int n_learnings = 1;

            NetworkInput inputNull = new NetworkInput(-1, -1, -1, -1);

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

            for (int l = 0; l < n_learnings; l++)
            {
                System.Console.WriteLine("*** *** ****** *** *** *** *** *** *** *** Learning " + (l + 1) + " *** *** *** ****** *** *** ****** *** ***");
                net.current_learning++;
                SimulateInputs(net, prima_sequenza, str_prima_sequenza, prima_targets, 1);
                SimulateInputs(net, seconda_sequenza, str_seconda_sequenza, seconda_targets, 1);
                SimulateInputs(net, terza_sequenza, str_terza_sequenza, terza_targets, 1);
                SimulateInputs(net, quarta_sequenza, str_quarta_sequenza, quarta_targets, 1);
                BinarySerialization.WriteToBinaryFile<Network>(net_path + net.current_learning + ".bin", net);
            }

            #endregion

            #region input imagination

            List<NetworkInput> imagination = new List<NetworkInput>();
            List<string> str_imagination = new List<string>();

            imagination.Add(BlueRectangle);
            imagination.Add(YellowCircle);

            str_imagination.Add("Blue Rectangle");
            str_imagination.Add("Yellow Circle");

            #endregion

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

            System.Console.WriteLine("*** *** ****** *** *** *** *** *** *** *** Testing *** *** *** ****** *** *** ****** *** ***");
            //net = BinarySerialization.ReadFromBinaryFile<Network>(net_path + 1 + ".bin");
            net = BinarySerialization.ReadFromBinaryFile<Network>(net_path + "prime_due.bin");



            //SimulateInputs(net, test_sequenza, str_test_sequenza, null, 0);
            SimulateInputsImagination(net, imagination, str_imagination, null, 0);

            float distance;
            float[] centers = new float[4];
            int[] objects;
            float dist_tresh = 100;
            float center;

            int[] lista_vincente = new int[4] {0, 1, 2, 3};
            int[] lista_motore = new int[4] {0, 1, 1, 1};

            int counter = 0;
            int timer = 700; //millisecondi

            bool esci;

            string[] serialPorts = SerialPort.GetPortNames();
            foreach (string p in serialPorts)
                Console.WriteLine(p);
            SerialPort serialPort = new SerialPort("COM3", 1000000, Parity.None, 8, StopBits.One);
            try
            {
                serialPort.Open();
            }
            catch (Exception ex)
            {
                Console.WriteLine("open() error: " + ex.Message);
                Environment.Exit(1);
            }
            while (true)
            {
                //vai avanti
                try
                {
                    serialPort.WriteLine("w");
                    serialPort.BaseStream.Flush();

                }
                catch (Exception ex)
                {
                    Console.WriteLine("write error: " + ex.Message);
                    Environment.Exit(1);
                }

                //cammino finchè sono vicino all'item "counter"
                esci = false;
                while (true)
                {
                    Run_cmd();
                    distance = Get_Distance();
                    objects = Get_Features();
                    foreach (int o in objects)
                    {
                        if (o == lista_vincente[counter] && distance < dist_tresh)
                        {
                            esci = true;
                            break;
                        }
                    }
                    if (esci)
                        break;
                    Thread.Sleep(timer);
                }

                //fermati
                try
                {
                    serialPort.WriteLine("h");
                    serialPort.BaseStream.Flush();

                }
                catch (Exception ex)
                {
                    Console.WriteLine("write error: " + ex.Message);
                    Environment.Exit(1);
                }

                if (counter == (lista_vincente.Length - 1)) //vedere condizione contorno
                {
                    serialPort.Close();
                    Environment.Exit(1);
                }

                //gira a lista_motore[counter]
                try
                {
                    if (lista_motore[counter] == 0)
                        serialPort.WriteLine("d");
                    else
                        serialPort.WriteLine("a");
                    serialPort.BaseStream.Flush();

                }
                catch (Exception ex)
                {
                    Console.WriteLine("write error: " + ex.Message);
                    Environment.Exit(1);
                }

                //giro nella direzione lista_motore[counter]
                //finchè l'oggetto non è centrato
                esci = false;
                while (true)
                {
                    Run_cmd();
                    objects = Get_Features();
                    foreach (int o in objects)
                    {
                        center = centers[objects.IndexOf(o)];
                        if (o == lista_vincente[counter + 1] && center > (320 - 30) && center < (320 + 30))
                        {
                            esci = true;
                            break;
                        }
                    }
                    if (esci)
                        break;
                    Thread.Sleep(timer);
                }

                //fermati
                try
                {
                    serialPort.WriteLine("h");

                    serialPort.BaseStream.Flush();

                }
                catch (Exception ex)
                {
                    Console.WriteLine("write error: " + ex.Message);
                    Environment.Exit(1);
                }
                counter++;
            }



        }

        public static void Run_cmd()
        {
            string fileName = @"C:\Users\Emanuele\Desktop\Detection_varie\Tag_Aruco\Tag_Aruco_Form_Color_detection.py";

            Process p = new Process();
            p.StartInfo = new ProcessStartInfo(@"C:\Users\Emanuele\AppData\Local\Programs\Python\Python38-32\python.exe", fileName)
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
            string fileName = @"C:\Users\Emanuele\Desktop\Detection_varie\Tag_Aruco\Tag_Aruco_Form_Color_detection.py";

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

        public static NetworkInput ConvertFromId(int id)
        {
            if (id == 0) //rettangolo rosso
                return new NetworkInput(1, 1, -1, -1);
            if (id == 1) //cerchio rosso
                return new NetworkInput(1, 3, -1, -1);
            if (id == 2) //triangolo rosso
                return new NetworkInput(1, 2, -1, -1);
            if (id == 3) //rettangolo blu
                return new NetworkInput(2, 1, -1, -1);
            if (id == 4) //cerchio blu
                return new NetworkInput(2, 3, -1, -1);
            if (id == 5) //triangolo blu
                return new NetworkInput(2, 2, -1, -1);
            if (id == 6) //rettangolo giallo
                return new NetworkInput(0, 1, -1, -1);
            if (id == 7) //cerchio giallo
                return new NetworkInput(0, 3, -1, -1);
            if (id == 8) //triangolo giallo
                return new NetworkInput(0, 2, -1, -1);
            if (id == -1) //null
                return new NetworkInput(-1, -1, -1, -1);
            return null;

        }

        public static int[] Get_Features()
        {
            int[] inputs = new int[2];
            int number;

            String file = "C:\\Users\\Emanuele\\Desktop\\Detection_varie\\Tag_Aruco\\input.txt";
            StreamReader dataStream = new StreamReader(file);
            string datasample;


            int i = 0;
            while ((datasample = dataStream.ReadLine()) != null)
            {
                if (i > 2)
                    break;
                if (int.TryParse(datasample, out number))
                {
                    inputs[i++] = number;
                }

            }
            dataStream.Close();

            return inputs;


        }

        public static float[] Get_Centers()
        {
            float[] centers = new float[2];
            float number;

            String file = "C:\\Users\\Emanuele\\Desktop\\Detection_varie\\Tag_Aruco\\centers.txt";
            StreamReader dataStream = new StreamReader(file);
            string datasample;


            int i = 0;
            while ((datasample = dataStream.ReadLine()) != null)
            {
                if (i > 2)
                    break;
                if (float.TryParse(datasample, NumberStyles.Any, CultureInfo.InvariantCulture, out number))
                {
                    centers[i++] = number;
                }

            }
            dataStream.Close();
            if (i == 0)
            {
                centers = new float[] { -1, -1 };
                return centers;
            }

            return centers;


        }

        //ritorna -1 se non valido
        public static float Get_Distance()
        {

            String file = "C:\\Users\\Emanuele\\Desktop\\Detection_varie\\Tag_Aruco\\distanza.txt";
            StreamReader dataStream = new StreamReader(file);
            string datasample = dataStream.ReadLine();



            float number;
            float.TryParse(datasample, NumberStyles.Any, CultureInfo.InvariantCulture, out number);
            Console.WriteLine("data: " + number);
            dataStream.Close();

            return number;


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