using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Ports;

namespace SLN
{

    [Serializable]
    public class SerialWriter
    {
        SerialPort serialPort;
        public SerialWriter(string port_name)
        {
            serialPort = new SerialPort(port_name, 1000000, Parity.None, 8, StopBits.One);

            try
            {
                serialPort.Open();
            }
            catch (Exception ex)
            {
                Console.WriteLine("open() error: " + ex.Message);
                Environment.Exit(1);
            }

            Console.CancelKeyPress += (sender, e) =>
            {
                Console.WriteLine("CTRL+C detected!\n");

                // Impedisce che il programma si chiuda immediatamente.
                e.Cancel = true;

                // Chiama la tua funzione qui
                this.Exit(true);

                e.Cancel = false;
            };
        }

        public void GoForward(bool output, int id)
        {
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
            if (output)
                Console.WriteLine("\t\t\t\t\t\tGoing Towards " + ObjectDetection.FromIdToString(id));
        }

        public void GoForward(bool output)
        {
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
            if (output)
                Console.WriteLine("\t\t\t\t\t\tGoing Forward "); ;
        }

        public void Stop(bool output)
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

            if (output)
                Console.WriteLine("\t\t\t\t\t\tStopping");
        }

        public void GoRight(bool output)
        {
            try
            {
                serialPort.WriteLine("d");
                serialPort.BaseStream.Flush();

            }
            catch (Exception ex)
            {
                Console.WriteLine("write error: " + ex.Message);
                Environment.Exit(1);
            }

            if (output)
                Console.WriteLine("\t\t\t\t\t\tTurning Right");
        }

        public void GoRight(bool output, int id)
        {
            try
            {
                serialPort.WriteLine("d");
                serialPort.BaseStream.Flush();

            }
            catch (Exception ex)
            {
                Console.WriteLine("write error: " + ex.Message);
                Environment.Exit(1);
            }

            if (output)
                Console.WriteLine("\t\t\t\t\t\tTurning Right Towards " + ObjectDetection.FromIdToString(id));
        }

        public void GoLeft(bool output)
        {
            try
            {
                serialPort.WriteLine("a");
                serialPort.BaseStream.Flush();

            }
            catch (Exception ex)
            {
                Console.WriteLine("write error: " + ex.Message);
                Environment.Exit(1);
            }

            if (output)
                Console.WriteLine("\t\t\t\t\t\tTurning Left");
        }

        public void GoLeft(bool output, int id)
        {
            try
            {
                serialPort.WriteLine("a");
                serialPort.BaseStream.Flush();

            }
            catch (Exception ex)
            {
                Console.WriteLine("write error: " + ex.Message);
                Environment.Exit(1);
            }

            if (output)
                Console.WriteLine("\t\t\t\t\t\tTurning Left Towards " + ObjectDetection.FromIdToString(id));
        }

        public void Exit(bool output)
        {
            Stop(false);
            serialPort.Close();
            if (output)
                Console.WriteLine("\t\t\t\t\t\tSerial comunication killed");
        }
        public static void PrintPortAvaibles()
        {
            string[] serialPorts = SerialPort.GetPortNames();
            foreach (string p in serialPorts)
                Console.WriteLine(p);
        }
    }



}
