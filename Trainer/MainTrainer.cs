using System;
using System.Collections.Generic;
using System.Linq;
using SimpleNN.NeuralNetwork;

namespace SimpleNN.Trainer
{
    internal class MainTrainer
    {
        public Network network { get; private set; }
        public int epoche { get; private set; }
        public double loss { get; private set; }
        public List<double> lossLogs { get; private set; } = new List<double>();

        private readonly Random rng = new Random(0);

        public MainTrainer(Network nt, int epochs)
        {
            network = nt;
            epoche = epochs;
        }

        public MainTrainer(int numInputs, int numHiddenLayers, int numNeuronsPerLayer,
                           ActivationFunction activationFunction, int numOutputs, int epochs)
        {
            network = new Network(numInputs, numHiddenLayers, numNeuronsPerLayer, activationFunction, numOutputs);
            epoche = epochs;
        }

        // SGD per-campione, con shuffle a ogni epoca. `labels` deve essere gia' standardizzato.
        public void Fit(List<double[]> inputs, List<double> labels)
        {
            lossLogs.Clear();
            int[] order = Enumerable.Range(0, inputs.Count).ToArray();

            for (int e = 0; e < epoche; e++)
            {
                Shuffle(order);
                double sum = 0.0;
                foreach (int n in order)
                {
                    network.Prediction(new List<double>(inputs[n]));
                    sum += network.BackPropagate(new double[] { labels[n] });
                }
                loss = sum / inputs.Count;
                lossLogs.Add(loss);
            }
        }

        // RMSE in unita' originali (dollari). `inputs` gia' normalizzati con le stats del training,
        // `rawLabels` in dollari, e labelMean/labelStd presi dal training per de-scalare la predizione.
        public double EvaluateRmse(List<double[]> inputs, List<double> rawLabels, double labelMean, double labelStd)
        {
            double sum = 0.0;
            for (int i = 0; i < inputs.Count; i++)
            {
                double predScaled = network.Prediction(new List<double>(inputs[i]))[0];
                double pred = predScaled * labelStd + labelMean;
                double d = pred - rawLabels[i];
                sum += d * d;
            }
            return Math.Sqrt(sum / inputs.Count);
        }

        private void Shuffle(int[] a)
        {
            for (int i = a.Length - 1; i > 0; i--)
            {
                int j = rng.Next(i + 1);
                (a[i], a[j]) = (a[j], a[i]);
            }
        }
    }
}
