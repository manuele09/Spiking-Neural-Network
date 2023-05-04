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


            //create the network
            Network net = Network.generateNetwork();
            /*Console.WriteLine("Importazione rete: " + DateTime.Now);
            Network net = BinarySerialization.ReadFromBinaryFile<Network>(net_path);
            Console.WriteLine("Rete importata: " + DateTime.Now);*/

            System.Console.WriteLine("*** *** *** *** *** *** *** *** Learning *** *** *** ****** *** *** ***");
            int ep = 0;
            for (int l = 0; l < 0; l++)
            {

                
                for (int i = 0; i < 0; i++, ep++) //seconda sequenza
                {
                    //  if (Constants.ENABLE_OUTPUT)
                    System.Console.WriteLine("*** *** *** *** Simulazione {0} *** *** *** ***", ep + 1);

                    StateLogger sl = new StateLogger(pathPc + @"\Dati\Neurons" + ep + ".txt", pathPc + @"\Dati\Synapse" + ep + ".txt", pathPc + @"\Dati\NeuronMorrisLecar" + ep + ".txt", true, false, true);
                    StateLogger slStdp = new StateLogger(pathPc + @"\Dati\NeuronsStdp" + ep + ".txt", pathPc + @"\Dati\SynapseSTDP" + ep + ".txt", false, true);

                    switch (i % 2)
                    {

                        case 0:
                            net.resetInputs();
                            Console.WriteLine("                                                         Input Dato: Cerchio Giallo Sx (2a sequenza)");
                            net.setInput(YellowCircleSx);
                            break;

                        case 1:
                            net.resetInputs();
                            Console.WriteLine("                                                         Input Dato: Rettangolo Giallo Dx End2 (2a sequenza)");
                            net.setInput(YellowRectangleDxEnd);
                            break;
                    }
                    Console.WriteLine("Sim started at " + DateTime.Now);
                    net.learnLiquid(sl, slStdp, ep);
                    Console.WriteLine("Sim finished at " + DateTime.Now);
                }

                for (int i = 0; i < 0; i++, ep++) //seconda sequenza
                {
                    //  if (Constants.ENABLE_OUTPUT)
                    System.Console.WriteLine("*** *** *** *** Simulazione {0} *** *** *** ***", ep + 1);

                    StateLogger sl = new StateLogger(pathPc + @"\Dati\Neurons" + ep + ".txt", pathPc + @"\Dati\Synapse" + ep + ".txt", pathPc + @"\Dati\NeuronMorrisLecar" + ep + ".txt", true, false, true);
                    StateLogger slStdp = new StateLogger(pathPc + @"\Dati\NeuronsStdp" + ep + ".txt", pathPc + @"\Dati\SynapseSTDP" + ep + ".txt", false, true);

                    switch (i % 2)
                    {

                        case 0:
                            net.resetInputs();
                            Console.WriteLine("                                                         Input Dato: Rettangolo Blu Sx (3a sequenza)");
                            net.setInput(BlueRectangleSx);
                            break;

                        case 1:
                            net.resetInputs();
                            Console.WriteLine("                                                         Input Dato: Rettangolo Giallo Sx End1 (3a sequenza)");
                            net.setInput(YellowRectangleSxEnd);
                            break;
                    }
                    Console.WriteLine("Sim started at " + DateTime.Now);
                    net.learnLiquid(sl, slStdp, ep);
                    Console.WriteLine("Sim finished at " + DateTime.Now);
                }

                for (int i = 0; i < 0; i++, ep++) //seconda sequenza
                {
                    //  if (Constants.ENABLE_OUTPUT)
                    System.Console.WriteLine("*** *** *** *** Simulazione {0} *** *** *** ***", ep + 1);

                    StateLogger sl = new StateLogger(pathPc + @"\Dati\Neurons" + ep + ".txt", pathPc + @"\Dati\Synapse" + ep + ".txt", pathPc + @"\Dati\NeuronMorrisLecar" + ep + ".txt", true, false, true);
                    StateLogger slStdp = new StateLogger(pathPc + @"\Dati\NeuronsStdp" + ep + ".txt", pathPc + @"\Dati\SynapseSTDP" + ep + ".txt", false, true);

                    switch (i % 2)
                    {

                        case 0:
                            net.resetInputs();
                            Console.WriteLine("                                                         Input Dato: Cerchio Giallo Dx (4a sequenza)");
                            net.setInput(YellowCircleDx);
                            break;

                        case 1:
                            net.resetInputs();
                            Console.WriteLine("                                                         Input Dato: Rettangolo Rosso Dx End2 (4a sequenza)");
                            net.setInput(RedRectangleDxEnd);
                            break;
                    }
                    Console.WriteLine("Sim started at " + DateTime.Now);
                    net.learnLiquid(sl, slStdp, ep);
                    Console.WriteLine("Sim finished at " + DateTime.Now);
                }
                
                for (int i = 0; i < 4; i++, ep++) //prima sequenza
                {
                    //  if (Constants.ENABLE_OUTPUT)
                    System.Console.WriteLine("*** *** *** *** Simulazione {0} *** *** *** ***", ep + 1);

                    StateLogger sl = new StateLogger(pathPc + @"\Dati\Neurons" + ep + ".txt", pathPc + @"\Dati\Synapse" + ep + ".txt", pathPc + @"\Dati\NeuronMorrisLecar" + ep + ".txt", true, false, true);
                    StateLogger slStdp = new StateLogger(pathPc + @"\Dati\NeuronsStdp" + ep + ".txt", pathPc + @"\Dati\SynapseSTDP" + ep + ".txt", false, true);

                    switch (i % 4)
                    {

                        case 0:
                            net.resetInputs();
                            Console.WriteLine("                                                         Input Dato: Rettangolo Blu Dx(1a sequenza)");
                            net.setInput(BlueRectangleDx);
                            break;

                        case 1:
                            net.resetInputs();
                            Console.WriteLine("                                                         Input Dato: Rettangolo Rosso Sx (1a sequenza)");
                            net.setInput(RedRectangleSx);
                            break;
                        case 2:
                            net.resetInputs();
                            Console.WriteLine("                                                         Input Dato: Cerchio Blu Sx (1a sequenza)");

                            net.setInput(BlueCircleSx);
                            break;

                        case 3:
                            net.resetInputs();
                            Console.WriteLine("                                                         Input Dato: Cerchio Rosso Sx End3 (1a sequenza)");
                            net.setInput(RedCircleSxEnd);
                            break;
                    }
                    Console.WriteLine("Sim started at " + DateTime.Now);
                    net.learnLiquid(sl, slStdp, ep);
                    Console.WriteLine("Sim finished at " + DateTime.Now);
                }

                BinarySerialization.WriteToBinaryFile<Network>(net_path+l+".bin", net);

            }

            for (int l = 0; l < 2; l++)
            {

                for (int i = 0; i < 5; i++, ep++) //seconda sequenza
                {
                    //  if (Constants.ENABLE_OUTPUT)
                    System.Console.WriteLine("*** *** *** *** Simulazione {0} *** *** *** ***", ep + 1);

                    StateLogger sl = new StateLogger(pathPc + @"\Dati\Neurons" + ep + ".txt", pathPc + @"\Dati\Synapse" + ep + ".txt", pathPc + @"\Dati\NeuronMorrisLecar" + ep + ".txt", true, false, true);
                    StateLogger slStdp = new StateLogger(pathPc + @"\Dati\NeuronsStdp" + ep + ".txt", pathPc + @"\Dati\SynapseSTDP" + ep + ".txt", false, true);
                    switch (i % 5)
                    {

                        case 0:
                            net.resetInputs();
                            Console.WriteLine("                                                         Input Dato: Rettangolo Blu Dx(1a sequenza)");
                            net.setInput(BlueRectangleDx);
                            break;

                        case 1:
                            net.resetInputs();
                            Console.WriteLine("                                                         Input Dato: Rettangolo Rosso Sx (1a sequenza)");
                            net.setInput(RedRectangleSx);
                            break;
                        case 2:
                            net.resetInputs();
                            Console.WriteLine("                                                         Input Dato: Cerchio Blu Sx (1a sequenza)");

                            net.setInput(BlueCircleSx);
                            break;
                        case 3:
                            net.resetInputs();
                            Console.WriteLine("                                                         Input Dato: Cerchio Blu Sx (1a sequenza)");

                            net.setInput(BlueCircleSx);
                            break;
                        case 4:
                            net.resetInputs();
                            Console.WriteLine("                                                         Input Dato: Cerchio Rosso Sx End3 (1a sequenza)");
                            net.setInput(RedCircleSxEnd);
                            break;
                    }

                    Console.WriteLine("Sim started at " + DateTime.Now);
                    net.learnLiquid(sl, slStdp, ep);
                    Console.WriteLine("Sim finished at " + DateTime.Now);
                }

                for (int i = 0; i < 0; i++, ep++) //seconda sequenza
                {
                    //  if (Constants.ENABLE_OUTPUT)
                    System.Console.WriteLine("*** *** *** *** Simulazione {0} *** *** *** ***", ep + 1);

                    StateLogger sl = new StateLogger(pathPc + @"\Dati\Neurons" + ep + ".txt", pathPc + @"\Dati\Synapse" + ep + ".txt", pathPc + @"\Dati\NeuronMorrisLecar" + ep + ".txt", true, false, true);
                    StateLogger slStdp = new StateLogger(pathPc + @"\Dati\NeuronsStdp" + ep + ".txt", pathPc + @"\Dati\SynapseSTDP" + ep + ".txt", false, true);

                    switch (i % 2)
                    {

                        case 0:
                            net.resetInputs();
                            Console.WriteLine("                                                         Input Dato: Rettangolo Rosso Sx (2a sequenza)");
                            net.setInput(RedRectangleSx);
                            break;

                        case 1:
                            net.resetInputs();
                            Console.WriteLine("                                                         Input Dato: Cerchio Blu Dx End1 (2a sequenza)");
                            net.setInput(BlueCircleDxEnd);
                            break;
                    }
                    Console.WriteLine("Sim started at " + DateTime.Now);
                    net.learnLiquid(sl, slStdp, ep);
                    Console.WriteLine("Sim finished at " + DateTime.Now);
                }

                for (int i = 0; i < 0; i++, ep++) //seconda sequenza
                {
                    //  if (Constants.ENABLE_OUTPUT)
                    System.Console.WriteLine("*** *** *** *** Simulazione {0} *** *** *** ***", ep + 1);

                    StateLogger sl = new StateLogger(pathPc + @"\Dati\Neurons" + ep + ".txt", pathPc + @"\Dati\Synapse" + ep + ".txt", pathPc + @"\Dati\NeuronMorrisLecar" + ep + ".txt", true, false, true);
                    StateLogger slStdp = new StateLogger(pathPc + @"\Dati\NeuronsStdp" + ep + ".txt", pathPc + @"\Dati\SynapseSTDP" + ep + ".txt", false, true);

                    switch (i % 2)
                    {

                        case 0:
                            net.resetInputs();
                            Console.WriteLine("                                                         Input Dato: Cerchio Rosso Sx (3a sequenza)");
                            net.setInput(RedCircleSx);
                            break;

                        case 1:
                            net.resetInputs();
                            Console.WriteLine("                                                         Input Dato: Rettangolo Rosso Sx End (3a sequenza)");
                            net.setInput(RedRectangleSxEnd);
                            break;
                    }
                    Console.WriteLine("Sim started at " + DateTime.Now);
                    net.learnLiquid(sl, slStdp, ep);
                    Console.WriteLine("Sim finished at " + DateTime.Now);
                }

                BinarySerialization.WriteToBinaryFile<Network>(net_path + l + ".bin", net);

            }

            //BinarySerialization.WriteToBinaryFile<Network>(net_path + l + ".bin", net);            //Console.WriteLine("Rete Salvata");
            //System.Console.WriteLine("*** *** *** *** *** *** *** *** test *** *** *** ****** *** *** ***");

             Console.WriteLine("Importazione rete: " + DateTime.Now);
             net = BinarySerialization.ReadFromBinaryFile<Network>(net_path + 1 + ".bin");
            Console.WriteLine("Rete importata: " + DateTime.Now);
            for (int epo = 0; epo < 5; epo++, ep++)
            {
                //if (Constants.ENABLE_OUTPUT)
                System.Console.WriteLine("*** *** *** *** Simulazione {0} *** *** *** ***", epo + 1);


                StateLogger sl = new StateLogger(pathPc + @"\Dati\Neurons" + (Constants.LEARNING_NUMBER + epo) + ".txt", pathPc + @"\Dati\Synapse" + (Constants.LEARNING_NUMBER + epo) + ".txt", pathPc + @"\Dati\NeuronMorrisLecar" + (Constants.LEARNING_NUMBER + epo) + ".txt", true, false, true);
                StateLogger slStdp = new StateLogger(pathPc + @"\Dati\NeuronsStdp" + (Constants.LEARNING_NUMBER + epo) + ".txt", pathPc + @"\Dati\SynapseSTDP" + (Constants.LEARNING_NUMBER + epo) + ".txt", pathPc + @"\Dati\NeuronMorrisLecar" + (Constants.LEARNING_NUMBER + epo) + ".txt", false, true, false);
                /*while(true)
                {
                    Get_input_2(net);
                    Console.ReadLine();
                }*/
                switch (epo + 1)
                {

                    case 1:
                        net.resetInputs();
                        Console.WriteLine("                                                         Input Dati: Rettangolo Blu");
                        net.setInput(BlueRectangle);
                        /*net.setInput(RedRectangle);
                        net.setInput(BlueCircle);
                        net.setInput(RedCircleSx);*/
                        break;
               
                        
                    case 2:
                        net.resetInputs();
                        Console.WriteLine("                                                         Input Dati: Null1");
     
                        net.setInput(inputNull);
                        break;
                    case 3:
                        net.resetInputs();
                        Console.WriteLine("                                                         Input Dati: Null2");
                        //net.setInput(inputCNoReward);

                        net.setInput(inputNull);
                        break;
                    case 4:
                        net.resetInputs();
                        Console.WriteLine("                                                         Input Dati: Null2");
                        //net.setInput(inputCNoReward);

                        net.setInput(inputNull);
                        break;
                    case 5:
                        net.resetInputs();
                        Console.WriteLine("                                                         Input Dati: Null2");
                        //net.setInput(inputCNoReward);

                        net.setInput(inputNull);
                        break;


                }
                Console.WriteLine("Sim Started at " + DateTime.Now);
                net.testLiquid(sl, slStdp, ep);
                Console.WriteLine("Sim finished at " + DateTime.Now);
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
