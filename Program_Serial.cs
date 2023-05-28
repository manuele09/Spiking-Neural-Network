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
	public class Program_Serial
    {
        /// <summary>
        /// The Main method
        /// </summary>
        /// <param name="args">Currently not used</param>
        public static void Maina(string[] args)
        {
            string script_path = "C:\\Users\\Emanuele\\Desktop\\Detection_varie\\Tag_Aruco\\Tag_Aruco_Form_Color_detection.py";
            string input_path = "C:\\Users\\Emanuele\\Desktop\\Detection_varie\\Tag_Aruco\\input.txt";
            string center_path = "C:\\Users\\Emanuele\\Desktop\\Detection_varie\\Tag_Aruco\\centers.txt";
            string distance_path = "C:\\Users\\Emanuele\\Desktop\\Detection_varie\\Tag_Aruco\\distanza.txt";

            ObjectDetection detector = new ObjectDetection(script_path, input_path, center_path, distance_path);

            float distance;
            float[] centers = new float[4];
            int[] objects;
            float dist_tresh = 100;
            float center;

            int[] lista_vincente = new int[4] { 3, 0, 1, 4 };
            int[] lista_motore = new int[4] { 0, 1, 0, 1 };

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
                Console.WriteLine("Vado verso l'id " + lista_vincente[counter]);
                esci = false;
                while (true)
                {
                    detector.GetInput();
                    Console.WriteLine("Distanza Rilevata: " + detector.distances[0]);
                    for (int i = 0; i < detector.n_inputs; i++)
                    {
                        if (detector.inputs[i] == lista_vincente[counter] && detector.distances[i] < dist_tresh)
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
                /*.WriteLine("Mi fermo");
                try
                {
                    serialPort.WriteLine("h");
                    serialPort.BaseStream.Flush();

                }
                catch (Exception ex)
                {
                    Console.WriteLine("write error: " + ex.Message);
                    Environment.Exit(1);
                }*/

                if (counter == (lista_vincente.Length - 1)) //vedere condizione contorno
                {
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
                    Console.WriteLine("Esco");
                    serialPort.Close();
                    Environment.Exit(1);
                }

                //gira a lista_motore[counter]
                Console.WriteLine("Inizio svolta");
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
                    detector.GetInput();
                    for (int i = 0; i < detector.n_inputs; i++)
                    {
                        Console.WriteLine("Centri rilevati: " + detector.centers[i]);
                        if (detector.inputs[i] == lista_vincente[counter + 1] && detector.centers[i] > (320 - 30) && detector.centers[i] < (320 + 30))
                        {
                            esci = true;
                            break;
                        }
                        if (detector.inputs[i] == lista_vincente[counter + 1])
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
                /*Console.WriteLine("Mi fermo");
                try
                {
                    serialPort.WriteLine("h");

                    serialPort.BaseStream.Flush();

                }
                catch (Exception ex)
                {
                    Console.WriteLine("write error: " + ex.Message);
                    Environment.Exit(1);
                }*/
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