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
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using MathWorks.MATLAB.Engine;
using MathWorks.MATLAB.Types;



namespace SLN
{
    /// <summary>
    /// Class containing the Main method
    /// </summary>
	public class ProgramMatlab
    {
        /// <summary>
        /// The Main method
        /// </summary>
        /// <param name="args">Currently not used</param>
        public static void Maina(string[] args)
        {

      
 
            MLApp.MLApp matlab = new MLApp.MLApp();
            matlab.Execute(@"cd 'C:\Users\Emanuele\Desktop\Calì\liquid22-Motor Neuron\liquid1_new-Motor\liquid1\bin\x86\Release\Matlab-SNN'");

            object result = null;

            matlab.Feval("Plot_Single_Neuron", 0, out result , 0, 1, 0, 0);
            //matlab.Quit();

            //Thread.Sleep(3000);
            result = null;
            matlab.Feval("Plot_Single_Neuron", 0, out result, 0, 1, 0, 0);

            result = null;

            matlab.Feval("Plot_Single_Neuron", 0, out result, 0, 1, 0, 0);
            //matlab.Quit();

            result = null;
            matlab.Feval("Plot_Single_Neuron", 0, out result, 0, 1, 0, 0);
            //matlab.Quit();

            //matlab.Feval("myfunc", 2, out result, 3.14, 42.0, "world");

        }

    }



}


