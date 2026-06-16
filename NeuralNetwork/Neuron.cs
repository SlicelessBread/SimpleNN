using System;
using System.Collections.Generic;

namespace SimpleNN.NeuralNetwork
{
    public enum ActivationFunction
    {
        Sigmoid,
        ReLU,
        Tanh,
        SoftMax,
        Linear
    }

    internal class Neuron
    {
        // UNA sola Random condivisa: `new Random()` per neurone, creato nello stesso
        // millisecondo, dava lo stesso seed -> pesi IDENTICI per tutti i neuroni.
        private static readonly Random rand = new Random();

        public double Bias { get; set; }
        public double Output { get; private set; }   // valore dopo l'attivazione
        public double Delta { get; set; }             // usato durante la backprop
        public List<double> Weights { get; private set; } = new List<double>();
        public ActivationFunction ActivationFunction { get; private set; }
        public int NumInput => Weights.Count;

        public Neuron(int numInputs, ActivationFunction activationFunction)
        {
            // Init "He" centrato su 0: essenziale con ReLU + input standardizzati.
            double scale = Math.Sqrt(2.0 / Math.Max(1, numInputs));
            for (int i = 0; i < numInputs; i++)
                Weights.Add((rand.NextDouble() * 2.0 - 1.0) * scale);
            Bias = 0.0;
            ActivationFunction = activationFunction;
        }

        public Neuron(int numInputs) : this(numInputs, ActivationFunction.ReLU) { }

        public void Activate(double input)
        {
            switch (ActivationFunction)
            {
                case ActivationFunction.Sigmoid: Output = 1.0 / (1.0 + Math.Exp(-input)); break;
                case ActivationFunction.ReLU:    Output = Math.Max(0.0, input); break;
                case ActivationFunction.Tanh:    Output = Math.Tanh(input); break;
                case ActivationFunction.Linear:  Output = input; break;
                default:                         Output = input; break;
            }
        }

        public double Derivate()
        {
            switch (ActivationFunction)
            {
                case ActivationFunction.Sigmoid: return Output * (1.0 - Output);
                case ActivationFunction.ReLU:    return Output > 0.0 ? 1.0 : 0.0;
                case ActivationFunction.Tanh:    return 1.0 - Output * Output;
                case ActivationFunction.Linear:  return 1.0;
                default:                         return 1.0;
            }
        }
    }
}
