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
using MathWorks.MATLAB.Engine;
using MathWorks.MATLAB.Types;
using System.Threading.Tasks;

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
        public static async Task Main(string[] args)
        {
            string pathPc = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string net_path = pathPc + @"\Saved_Networks\net-";
            int n_learnings = 1;
            bool do_test = true;
            bool do_imagination = false;

            bool simulate_also = true;


            Network net = Network.generateNetwork();


            #region learning
            //Define the four sequences
            #region prima_sequenza
            int[] ids = { 3, 0, 1, 4 };
            int[] motors = { 0, 1, 0, 1 };
            List<NetworkInput> prima_sequenza = NetworkInput.CreateInputList(ids, motors, 3);
            #endregion

            #region seconda_sequenza
            ids = new int[] { 7, 6 };
            motors = new int[] { 1, 0 };
            List<NetworkInput> seconda_sequenza = NetworkInput.CreateInputList(ids, motors, 1);

            #endregion

            #region terza_sequenza
            ids = new int[] { 3, 6 };
            motors = new int[] { 1, 1 };
            List<NetworkInput> terza_sequenza = NetworkInput.CreateInputList(ids, motors, 1);
            #endregion

            #region quarta_sequenza
            ids = new int[] { 7, 1 };
            motors = new int[] { 0, 0 };
            List<NetworkInput> quarta_sequenza = NetworkInput.CreateInputList(ids, motors, 1);
            #endregion

            //Learn the four sequences
            for (int l = 0; l < n_learnings; l++)
            {
                net.current_learning++;
                System.Console.WriteLine("*** *** ****** *** *** *** *** *** *** *** Learning First Sequence*** *** *** ****** *** *** ****** *** ***");
                SimulateInputs(net, prima_sequenza, 1);
                //System.Console.WriteLine("*** *** ****** *** *** *** *** *** *** *** Learning Second Sequence*** *** *** ****** *** *** ****** *** ***");
                //SimulateInputs(net, seconda_sequenza, 1);
                //System.Console.WriteLine("*** *** ****** *** *** *** *** *** *** *** Learning Third Sequence*** *** *** ****** *** *** ****** *** ***");
                //SimulateInputs(net, terza_sequenza, 1);
                //System.Console.WriteLine("*** *** ****** *** *** *** *** *** *** *** Learning Fourth Sequence*** *** *** ****** *** *** ****** *** ***");
                //SimulateInputs(net, quarta_sequenza, 1);

                //BinarySerialization.WriteToBinaryFile<Network>(net_path + net.current_learning + ".bin", net);
            }

            #endregion

            //net = BinarySerialization.ReadFromBinaryFile<Network>(net_path + "funzionante.bin");
            //net = BinarySerialization.ReadFromBinaryFile<Network>(net_path + "2.bin");

            StateLogger sl;
            StateLogger slStdp;
            Task task_result = null;

            #region  testing
            List<NetworkInput> test_inputs = new List<NetworkInput>();
            test_inputs.Add(new NetworkInput(3, -1, -1)); //Blue Rectangle, id 3 (next id 7)
            test_inputs.Add(new NetworkInput(0, -1, -1));
            test_inputs.Add(new NetworkInput(1, -1, -1));
            test_inputs.Add(new NetworkInput(4, -1, -1));

            MLApp.MLApp matlab = new MLApp.MLApp();
            matlab.Execute(@"cd 'C:\Users\Emanuele\Desktop\Calì\liquid22-Motor Neuron\liquid1_new-Motor\liquid1\bin\x86\Release\Matlab-SNN'");
            if (do_test)
            {
                Console.WriteLine("TESTING\n");
                foreach (NetworkInput input in test_inputs)
                {
                    sl = new StateLogger(pathPc + @"\Dati\Neurons" + net.current_epoch + ".txt", pathPc + @"\Dati\Synapse" + net.current_epoch + ".txt", pathPc + @"\Dati\NeuronMorrisLecar" + net.current_epoch + ".txt", true, false, true);
                    slStdp = new StateLogger(pathPc + @"\Dati\NeuronsStdp" + net.current_epoch + ".txt", pathPc + @"\Dati\SynapseSTDP" + net.current_epoch + ".txt", false, true);

                    net.resetInputs();
                    net.setInput(input);

                    Console.WriteLine("Sim started at " + DateTime.Now);
                    net.simulateLiquid(sl, slStdp, net.current_epoch, -1, false, 0, true);
                    if (task_result != null)
                        await task_result;

                    Console.WriteLine("Sim finished at " + DateTime.Now);

                    Console.WriteLine("Generating Matlab Plot:");
                    //task_result = PlotInput(matlab, net.current_epoch, "");

                    net.current_epoch++;
                }


            }

            #endregion


            Environment.Exit(1);
            List<NetworkInput> imagination_inputs = new List<NetworkInput>();
            #region imagination
            if (do_imagination)
            {
                imagination_inputs.Add(new NetworkInput(3, -1, -1));
                imagination_inputs.Add(new NetworkInput(7, -1, -1));
                SimulateInputsImagination(net, imagination_inputs);
            }
            #endregion

            string script_path = "C:\\Users\\Emanuele\\Desktop\\Detection_varie\\Tag_Aruco\\Tag_Aruco_Form_Color_detectionV2.py";
            string input_path = "C:\\Users\\Emanuele\\Desktop\\Detection_varie\\Tag_Aruco\\input.txt";
            string center_path = "C:\\Users\\Emanuele\\Desktop\\Detection_varie\\Tag_Aruco\\centers.txt";
            string distance_path = "C:\\Users\\Emanuele\\Desktop\\Detection_varie\\Tag_Aruco\\distanza.txt";

            ObjectDetection detector = new ObjectDetection(script_path, input_path, center_path, distance_path);

            float dist_tresh = 150;

            int[] lista_vincente = new int[4] { 3, 0, 4, 10};
            //int[] lista_vincente = new int[3] { 3 };
            int[] lista_shapes = new int[4] { 1, 1, 3, 3 };
            //int[] lista_vincente = new int[1] { 4 };
            //lista_vincente = net.winner_seq_ids;
            //int[] lista_motore = new int[1] {  1 };
            int[] lista_motore = new int[4] { 0, 1, 0, 1 };
            //int[] lista_motore = new int[3] { 1, 0, 1 };
            //lista_motore = net.winner_seq_motors;

            sl = new StateLogger(pathPc + @"\Dati\Neurons" + net.current_epoch + ".txt", pathPc + @"\Dati\Synapse" + net.current_epoch + ".txt", pathPc + @"\Dati\NeuronMorrisLecar" + net.current_epoch + ".txt", true, false, true);
            slStdp = new StateLogger(pathPc + @"\Dati\NeuronsStdp" + net.current_epoch + ".txt", pathPc + @"\Dati\SynapseSTDP" + net.current_epoch + ".txt", false, true);

            net.resetInputs();
            net.setInput(new NetworkInput(lista_vincente[3], -1, -1));

            Console.WriteLine("Sim started at " + DateTime.Now);
            net.simulateLiquid(sl, slStdp, net.current_epoch, -1, false, 0, true);

            Console.WriteLine("Sim finished at " + DateTime.Now);

            Console.WriteLine("Generating Matlab Plot:");



            task_result = PlotInput(matlab, net.current_epoch, ObjectDetection.FromIdToString(lista_vincente[3]));
            //4 zoom indietro
            int counter = 0;
            int timer = 700; //millisecondi

            bool esci;

            bool min;
            bool max;
            SerialWriter serial = new SerialWriter("COM3");

            Console.ReadLine();
            detector.GetInput();
            for (int i = 0; i < detector.n_inputs; i++)
                imagination_inputs.Add(new NetworkInput(detector.inputs[i], -1, -1));
            SimulateInputsImagination(net, imagination_inputs);

            /*for (int i = 0; i < net.winner_seq_ids.Length && net.winner_seq_ids[i] != -1; i++)
                Console.WriteLine("\t\t\t" + ObjectDetection.FromIdToString(net.winner_seq_ids[i]));*/


            sl = new StateLogger(pathPc + @"\Dati\Neurons" + net.current_epoch + ".txt", pathPc + @"\Dati\Synapse" + net.current_epoch + ".txt", pathPc + @"\Dati\NeuronMorrisLecar" + net.current_epoch + ".txt", true, false, true);
            slStdp = new StateLogger(pathPc + @"\Dati\NeuronsStdp" + net.current_epoch + ".txt", pathPc + @"\Dati\SynapseSTDP" + net.current_epoch + ".txt", false, true);

            net.resetInputs();
            net.setInput(new NetworkInput(net.winner_seq_ids[0], -1, -1));

            Console.WriteLine("Sim started at " + DateTime.Now);
            net.simulateLiquid(sl, slStdp, net.current_epoch, -1, false, 0, true);

            Console.WriteLine("Sim finished at " + DateTime.Now);

            Console.WriteLine("Generating Matlab Plot:");

            task_result = PlotInput(matlab, net.current_epoch, ObjectDetection.FromIdToString(net.winner_seq_ids[0]));
            net.current_epoch++;
           // Console.ReadLine();

            while (true)
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
                        //if ((detector.inputs[i] == lista_vincente[counter] || ObjectDetection.FromIdToFeatures(detector.inputs[i])[1] == lista_shapes[counter]) && detector.distances[i] < dist_tresh)
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
                serial.Stop(true);
                if (task_result != null)
                    await task_result;
                //fermati
                if (simulate_also)
                {
                    sl = new StateLogger(pathPc + @"\Dati\Neurons" + net.current_epoch + ".txt", pathPc + @"\Dati\Synapse" + net.current_epoch + ".txt", pathPc + @"\Dati\NeuronMorrisLecar" + net.current_epoch + ".txt", true, false, true);
                    slStdp = new StateLogger(pathPc + @"\Dati\NeuronsStdp" + net.current_epoch + ".txt", pathPc + @"\Dati\SynapseSTDP" + net.current_epoch + ".txt", false, true);

                    net.resetInputs();
                    net.setInput(new NetworkInput(lista_vincente[counter + 1], -1, -1));

                    Console.WriteLine("Sim started at " + DateTime.Now);
                    net.simulateLiquid(sl, slStdp, net.current_epoch, -1, false, 0, true);

                    Console.WriteLine("Sim finished at " + DateTime.Now);

                    Console.WriteLine("Generating Matlab Plot:");



                    task_result = PlotInput(matlab, net.current_epoch, ObjectDetection.FromIdToString(lista_vincente[counter + 1]));
                    net.current_epoch++;

                }
                counter++;
            }


        }

        public static async Task PlotInput(MLApp.MLApp matlab, int epoch, string title)
        {
            await Task.Run(() =>
            {

                object result = null;
                matlab.Feval("PlotInputLayer", 0, out result, epoch, title);

            });
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


