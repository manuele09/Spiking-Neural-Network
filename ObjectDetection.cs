﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;

namespace SLN
{

    [Serializable]
    public class ObjectDetection
    {
        string script_path;
        string input_path;
        string center_path;
        string distance_path;

        public int[] inputs = new int[2]; //di default massimo 2 input
        public float[] centers = new float[2]; //di default massimo 2 input
        public float[] distances = new float[2]; //di default massimo 2 input
        public int n_inputs;

        public ObjectDetection(string script_path, string input_path, string center_path, string distance_path)
        {
            ResetVars();
            this.script_path = script_path;
            this.input_path = input_path;
            this.center_path = center_path;
            this.distance_path = distance_path;
        }



        //esegue uno script python (python 3.8)
        public void Run_Python(bool stdout)
        {

            Process p = new Process();
            p.StartInfo = new ProcessStartInfo(@"C:\Users\Emanuele\AppData\Local\Programs\Python\Python38-32\python.exe", script_path)
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            p.Start();

            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();

            if (stdout)
                Console.WriteLine(output);
        }

        //avvia lo script per accedere all'input visuale.
        //estra tutte le informazioni utili da esso.
        public void GetInput()
        {
            int number;
            string datasample;

            ResetVars();

            Run_Python(false);

            StreamReader dataStream = new StreamReader(input_path);

            for (int i = 0; ((datasample = dataStream.ReadLine()) != null) && i < 2; i++)
                if (int.TryParse(datasample, out number))
                    inputs[n_inputs++] = number;

            dataStream.Close();

            float number2;

            dataStream = new StreamReader(center_path);


            for (int i = 0; ((datasample = dataStream.ReadLine()) != null) && i < 2; i++)
                if (float.TryParse(datasample, out number2))
                    centers[i] = number2;

            dataStream.Close();

            dataStream = new StreamReader(distance_path);
            datasample = dataStream.ReadLine();
            float.TryParse(datasample, NumberStyles.Any, CultureInfo.InvariantCulture, out number2);
            dataStream.Close();

            for (int i = 0; i < n_inputs; i++)
                distances[i] = number2;

        }

        public void ResetVars()
        {
            n_inputs = 0;

            inputs[0] = -1;
            inputs[1] = -1;

            centers[0] = -1;
            centers[1] = -1;

            distances[0] = -1;
            distances[1] = -1;
        }
        public static int[] FromIdToFeatures(int id)
        {
            int[] features = new int[2]; //colore e forma
            features[0] = -1;
            features[1] = -1;

            if (id == 0) //rettangolo rosso
            {
                features[0] = 1;
                features[1] = 1;
            }
            if (id == 1) //cerchio rosso
            {
                features[0] = 1;
                features[1] = 3;
            }
            if (id == 2) //triangolo rosso
            {
                features[0] = 1;
                features[1] = 2;
            }

            if (id == 3) //rettangolo blu
            {
                features[0] = 2;
                features[1] = 1;
            }
            if (id == 4) //cerchio blu
            {
                features[0] = 2;
                features[1] = 3;
            }
            if (id == 5) //triangolo blu
            {
                features[0] = 2;
                features[1] = 2;
            }

            if (id == 6) //rettangolo giallo
            {
                features[0] = 0;
                features[1] = 1;
            }
            if (id == 7) //cerchio giallo
            {
                features[0] = 0;
                features[1] = 3;
            }
            if (id == 8) //triangolo giallo
            {
                features[0] = 0;
                features[1] = 2;
            }

            return features;

        }

        public static NetworkInput GenerateInput(int id, int motor, int reward_level)
        {
            int[] features = FromIdToFeatures(id);

            //prima feature: colore;
            //seconda feature: forma;

            //motor 0 o 1;
            //reward level da 1 a 3 (compresi).

            //valori più piccoli di quelli elencati implicano zero reward.

            return new NetworkInput(features[0], features[1], -1, -1, motor, reward_level);



        }

    }



}
