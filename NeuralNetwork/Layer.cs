using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleNN.NeuralNetwork
{
    internal class Layer
    {
        public List<Neuron> Neurons { get; private set; } = new List<Neuron>();
        public int NumNeurons => Neurons.Count;

        public Layer(int numNeurons, int numInputs, ActivationFunction activationFunction)
        {
            for (int i = 0; i < numNeurons; i++)
                Neurons.Add(new Neuron(numInputs, activationFunction));
        }

        public Layer(int numNeurons, int numInputs)
            : this(numNeurons, numInputs, ActivationFunction.ReLU) { }

        // Calcola la somma pesata + bias per ogni neurone, applica l'attivazione,
        // e restituisce gli output del layer (da passare al layer successivo).
        public double[] Activate(double[] inputs)
        {
            if (inputs.Length != Neurons[0].NumInput)
                throw new ArgumentException("Input length does not match number of inputs for neurons in this layer.");

            double[] outputs = new double[NumNeurons];
            for (int i = 0; i < Neurons.Count; i++)
            {
                double weightedSum = Neurons[i].Bias;
                for (int j = 0; j < Neurons[i].NumInput; j++)
                    weightedSum += Neurons[i].Weights[j] * inputs[j];
                Neurons[i].Activate(weightedSum);
                outputs[i] = Neurons[i].Output;
            }
            return outputs;
        }

        public double[] GetOutputs() => Neurons.Select(n => n.Output).ToArray();
    }
}
