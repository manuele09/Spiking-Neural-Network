﻿using System;
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
                Console.WriteLine("Going Forward");
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
                Console.WriteLine("Stopping");
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
                Console.WriteLine("Turning Right");
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
                Console.WriteLine("Turning Left");
        }
        
        public void Exit(bool output)
        {
            Stop(false);
            serialPort.Close();
            if (output)
                Console.WriteLine("Serial comunication killed");
        }
        public static void PrintPortAvaibles()
        {
            string[] serialPorts = SerialPort.GetPortNames();
            foreach (string p in serialPorts)
                Console.WriteLine(p);
        }
    }



}
