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
            //Console.ReadLine();

            String pathPc = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            String net_path = pathPc + @"\Saved_Networks\net-";
            int n_learnings = 2;
            bool do_imagination = true;


            Network net = Network.generateNetwork();


            #region learning

            #region prima_sequenza
            int[] ids = { 3, 0, 4, 1 };
            int[] motors = { 0, 1, 1, 1 };
            List<NetworkInput> prima_sequenza = NetworkInput.GetNetworkList(ids, motors, 3);
            #endregion

            #region seconda_sequenza
            ids = new int[] { 7, 6 };
            motors = new int[] { 1, 0 };
            List<NetworkInput> seconda_sequenza = NetworkInput.GetNetworkList(ids, motors, 1);
            #endregion

            #region terza_sequenza
            ids = new int[] { 3, 6 };
            motors = new int[] { 1, 1 };
            List<NetworkInput> terza_sequenza = NetworkInput.GetNetworkList(ids, motors, 1);
            #endregion

            #region quarta_sequenza
            ids = new int[] { 7, 1 };
            motors = new int[] { 0, 0 };
            List<NetworkInput> quarta_sequenza = NetworkInput.GetNetworkList(ids, motors, 1);
            #endregion

            for (int l = 0; l < n_learnings; l++)
            {
                net.current_learning++;
                System.Console.WriteLine("*** *** ****** *** *** *** *** *** *** *** Learning First Sequence*** *** *** ****** *** *** ****** *** ***");
                SimulateInputs(net, prima_sequenza, 1);
                System.Console.WriteLine("*** *** ****** *** *** *** *** *** *** *** Learning Second Sequence*** *** *** ****** *** *** ****** *** ***");
                //SimulateInputs(net, seconda_sequenza, 1);
                System.Console.WriteLine("*** *** ****** *** *** *** *** *** *** *** Learning Third Sequence*** *** *** ****** *** *** ****** *** ***");
                //SimulateInputs(net, terza_sequenza, 1);
                System.Console.WriteLine("*** *** ****** *** *** *** *** *** *** *** Learning Fourth Sequence*** *** *** ****** *** *** ****** *** ***");
                //SimulateInputs(net, quarta_sequenza, 1);

                BinarySerialization.WriteToBinaryFile<Network>(net_path + net.current_learning + ".bin", net);
            }


            #endregion

            //net = BinarySerialization.ReadFromBinaryFile<Network>(net_path + "funzionante.bin");

            #region  imagination
            StateLogger sl;
            StateLogger slStdp;
            List<NetworkInput> imagination = new List<NetworkInput>();
            if (do_imagination)
            {
                sl = new StateLogger(pathPc + @"\Dati\Neurons" + net.current_epoch + ".txt", pathPc + @"\Dati\Synapse" + net.current_epoch + ".txt", pathPc + @"\Dati\NeuronMorrisLecar" + net.current_epoch + ".txt", true, false, true);
                slStdp = new StateLogger(pathPc + @"\Dati\NeuronsStdp" + net.current_epoch + ".txt", pathPc + @"\Dati\SynapseSTDP" + net.current_epoch + ".txt", false, true);
                net.resetInputs();
                net.setInput(new NetworkInput(3, -1, -1)); //Blue Rectangle
                Console.WriteLine("Sim started attt " + DateTime.Now);
                net.simulateLiquid(sl, slStdp, net.current_epoch, -1, false, 0, true);
                Console.WriteLine("Sim finished attt " + DateTime.Now);
                net.current_epoch++;

                sl = new StateLogger(pathPc + @"\Dati\Neurons" + net.current_epoch + ".txt", pathPc + @"\Dati\Synapse" + net.current_epoch + ".txt", pathPc + @"\Dati\NeuronMorrisLecar" + net.current_epoch + ".txt", true, false, true);
                slStdp = new StateLogger(pathPc + @"\Dati\NeuronsStdp" + net.current_epoch + ".txt", pathPc + @"\Dati\SynapseSTDP" + net.current_epoch + ".txt", false, true);
                net.resetInputs();
                net.setInput(new NetworkInput(-1, -1, -1, -1));
                Console.WriteLine("Sim started attt " + DateTime.Now);
                net.simulateLiquid(sl, slStdp, net.current_epoch, -1, false, 0, true);
                Console.WriteLine("Sim finished attt " + DateTime.Now);
                net.current_epoch++;

                sl = new StateLogger(pathPc + @"\Dati\Neurons" + net.current_epoch + ".txt", pathPc + @"\Dati\Synapse" + net.current_epoch + ".txt", pathPc + @"\Dati\NeuronMorrisLecar" + net.current_epoch + ".txt", true, false, true);
                slStdp = new StateLogger(pathPc + @"\Dati\NeuronsStdp" + net.current_epoch + ".txt", pathPc + @"\Dati\SynapseSTDP" + net.current_epoch + ".txt", false, true);
                net.resetInputs();
                net.setInput(new NetworkInput(-1, -1, -1, -1));
                Console.WriteLine("Sim started attt " + DateTime.Now);
                net.simulateLiquid(sl, slStdp, net.current_epoch, -1, false, 0, true);
                Console.WriteLine("Sim finished attt " + DateTime.Now);
                net.current_epoch++;

                sl = new StateLogger(pathPc + @"\Dati\Neurons" + net.current_epoch + ".txt", pathPc + @"\Dati\Synapse" + net.current_epoch + ".txt", pathPc + @"\Dati\NeuronMorrisLecar" + net.current_epoch + ".txt", true, false, true);
                slStdp = new StateLogger(pathPc + @"\Dati\NeuronsStdp" + net.current_epoch + ".txt", pathPc + @"\Dati\SynapseSTDP" + net.current_epoch + ".txt", false, true);
                net.resetInputs();
                net.setInput(new NetworkInput(-1, -1, -1, -1));
                Console.WriteLine("Sim started attt " + DateTime.Now);
                net.simulateLiquid(sl, slStdp, net.current_epoch, -1, false, 0, true);
                Console.WriteLine("Sim finished attt " + DateTime.Now);
                net.current_epoch++;


                //SimulateInputsImagination(net, imagination);

            }
            #endregion

            string script_path = "C:\\Users\\Emanuele\\Desktop\\Detection_varie\\Tag_Aruco\\Tag_Aruco_Form_Color_detectionV2.py";
            string input_path = "C:\\Users\\Emanuele\\Desktop\\Detection_varie\\Tag_Aruco\\input.txt";
            string center_path = "C:\\Users\\Emanuele\\Desktop\\Detection_varie\\Tag_Aruco\\centers.txt";
            string distance_path = "C:\\Users\\Emanuele\\Desktop\\Detection_varie\\Tag_Aruco\\distanza.txt";

            ObjectDetection detector = new ObjectDetection(script_path, input_path, center_path, distance_path);

            float dist_tresh = 150;

            int[] lista_vincente = new int[4] { 3, 0, 1, 4 };
            //int[] lista_vincente = new int[1] { 4 };
            //lista_vincente = net.winner_seq_ids;
            //int[] lista_motore = new int[1] {  1 };
            int[] lista_motore = new int[4] { 0, 1, 0, 1 };
            //lista_motore = net.winner_seq_motors;


            int counter = 0;
            int timer = 700; //millisecondi

            bool esci;

            bool min;
            bool max;
            SerialWriter serial = new SerialWriter("COM3");



            /*serial.GoForward(true);
            Thread.Sleep(20000);
            serial.Stop(true);*/

            while (false)
            {


                //1) Vai Avanti finchè non sei abbastanza vicino al target
                serial.GoForward(true, lista_vincente[counter]);

                esci = false;
                while (true)
                {
                    detector.GetInput();
                    for (int i = 0; i < detector.n_inputs; i++)
                    {
                        //Controlla la distanza dal target
                        //Console.WriteLine("Distance from target " + detector.inputs[i] + ": " + detector.distances[i] + " cm");
                        Console.WriteLine("Distance from " + ObjectDetection.FromIdToString(detector.inputs[i]) + "(target " + detector.inputs[i] + "): " + detector.distances[i] + " cm");
                        if (detector.inputs[i] == lista_vincente[counter] && detector.distances[i] < dist_tresh)
                        {
                            esci = true;
                            break;
                        }

                        //Calibra l'assetto in modo da tenere la figura centrata
                        //Console.WriteLine("Center position of target " + detector.inputs[i] + ": " + detector.centers[i]);
                        Console.WriteLine("Center position of " + ObjectDetection.FromIdToString(detector.inputs[i]) + "(target " + detector.inputs[i] + "): " + detector.centers[i] + " pixels");
                        Console.WriteLine("");
                        if (detector.inputs[i] == lista_vincente[counter] && detector.centers[i] < (150))
                        {
                            Console.WriteLine("Centering target position: turning left");
                            serial.GoLeft(true);
                            Thread.Sleep(3000);

                            serial.Stop(true);
                            serial.GoForward(true);
                        }
                        if (detector.inputs[i] == lista_vincente[counter] && detector.centers[i] > (500))
                        {
                            Console.WriteLine("Centering target position: turning right");


                            serial.GoRight(true);
                            Thread.Sleep(3000);

                            serial.Stop(true);
                            serial.GoForward(true);
                        }

                    }
                    if (esci)
                        break;
                    Thread.Sleep(timer);
                }



                //2) Giro verso il target successivo, finché questo non è centrato nell'inquadratura
                esci = false;
                min = false;
                max = false;
                while (true)
                {
                    //Svolta finale nel caso dell'ultimo elemento della sequenza
                    if (counter == (lista_vincente.Length - 1))
                    {
                        for (int i = 0; i < 2; i++)
                        {

                            if (lista_motore[counter] == 0)
                                serial.GoRight(true);
                            else
                                serial.GoLeft(true);
                            Thread.Sleep(3000);
                        }

                        serial.Stop(true);

                        serial.GoForward(true);
                        Thread.Sleep(20000);
                        serial.Stop(true);

                        serial.Exit(true);
                        Environment.Exit(1);
                    }

                    //Giro a destra o a sinistra
                    if (lista_motore[counter] == 0)
                        serial.GoRight(true, lista_vincente[counter + 1]);
                    else
                        serial.GoLeft(true, lista_vincente[counter + 1]);
                    Thread.Sleep(3000);

                    //Per stabilizzare l'immagine mi fermo
                    serial.Stop(true);
                    detector.GetInput();

                    for (int i = 0; i < detector.n_inputs; i++)
                    {
                        Console.WriteLine("Center position of " + ObjectDetection.FromIdToString(detector.inputs[i]) + "(target " + detector.inputs[i] + "): " + detector.centers[i] + " pixels");
                        //Se gira troppo velocemente e oltrepassa il centro interrompe la rotazione
                        if (detector.inputs[i] == lista_vincente[counter + 1] && detector.centers[i] > (320 - 50))
                            min = true;
                        if (detector.inputs[i] == lista_vincente[counter + 1] && detector.centers[i] < (320 + 50))
                            max = true;
                        if (min && max)
                        {
                            Console.WriteLine("Center passed, stopping turning.");
                            esci = true;
                            break;
                        }

                        //Se l'oggetto è centrato nell'inquadratura interrompo la rotazione
                        //Console.WriteLine("Center position of target " + detector.inputs[i] + ": " + detector.centers[i]);
                        if (detector.inputs[i] == lista_vincente[counter + 1] && detector.centers[i] > (320 - 50) && detector.centers[i] < (320 + 50))
                        {
                            esci = true;
                            break;
                        }
                    }
                    if (esci)
                        break;
                }

                //fermati
                serial.Stop(true);
                counter++;
            }


        }


        public static void SimulateInputs(Network net, List<NetworkInput> inputs, int learn)
        {
            String pathPc = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            for (int i = 0; i < inputs.Count; i++)
            {

                StateLogger sl = new StateLogger(pathPc + @"\Dati\Neurons" + net.current_epoch + ".txt", pathPc + @"\Dati\Synapse" + net.current_epoch + ".txt", pathPc + @"\Dati\NeuronMorrisLecar" + net.current_epoch + ".txt", true, false, true);
                StateLogger slStdp = new StateLogger(pathPc + @"\Dati\NeuronsStdp" + net.current_epoch + ".txt", pathPc + @"\Dati\SynapseSTDP" + net.current_epoch + ".txt", false, true);

                net.resetInputs();
                net.setInput(inputs[i]);
                Console.WriteLine("Sim started at " + DateTime.Now);

                if (Constants.OUTPUT)
                    Console.WriteLine("Input given: " + inputs[i]);

                if (learn == 1)
                    net.learnLiquid(sl, slStdp, net.current_epoch);
                else
                    net.testLiquid(sl, slStdp, net.current_epoch);
                Console.WriteLine("Sim finished at " + DateTime.Now);
                Console.WriteLine("*********************************************************************************************");

                net.current_epoch++;
            }
        }

        public static void SimulateInputsImagination(Network net, List<NetworkInput> inputs)
        {
            String pathPc = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string output_message;
            int prima = 0;



            StateLogger sl = new StateLogger(pathPc + @"\Dati\Neurons" + net.current_epoch + ".txt", pathPc + @"\Dati\Synapse" + net.current_epoch + ".txt", pathPc + @"\Dati\NeuronMorrisLecar" + net.current_epoch + ".txt", true, false, true);
            StateLogger slStdp = new StateLogger(pathPc + @"\Dati\NeuronsStdp" + net.current_epoch + ".txt", pathPc + @"\Dati\SynapseSTDP" + net.current_epoch + ".txt", false, true);

            net.resetInputs();
            for (int j = 0; j < inputs.Count; j++)
            {

                net.setInput(inputs[j]);
            }



            Console.WriteLine("Sim started at " + DateTime.Now);
            net.testLiquid(sl, null, net.current_epoch);
            Console.WriteLine("Sim finished at " + DateTime.Now);

            net.current_epoch++;

        }

    }

}


