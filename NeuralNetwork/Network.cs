using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleNN.NeuralNetwork
{
    /*
     * Rete feed-forward per REGRESSIONE.
     *  - Gli hidden layer usano l'attivazione passata nel costruttore (ReLU consigliata).
     *  - Il layer di output e' LINEARE con `numOutputs` neuroni (per il prezzo: 1).
     *  - Gli input entrano direttamente nel primo hidden layer: niente "input layer" pesato.
     * Uso: Prediction(input) per il forward, poi BackPropagate(target) per un passo di training.
     */
    internal class Network
    {
        public List<Layer> Layers { get; private set; } = new List<Layer>();
        public int NumLayers => Layers.Count;
        public int NumInputs { get; private set; }
        public int NumOutputs => Layers[NumLayers - 1].NumNeurons;
        public double LearningRate { get; set; } = 0.01;
        public double CurrentLoss { get; private set; }

        private double[] lastInputs = Array.Empty<double>();

        public Network(int numInputs, int numHiddenLayers, int numNeuronsPerLayer,
                       ActivationFunction activationFunction, int numOutputs)
        {
            if (numHiddenLayers < 1) throw new ArgumentException("Number of hidden layers must be at least 1.");
            if (numNeuronsPerLayer < 1) throw new ArgumentException("Number of neurons per layer must be at least 1.");
            if (numOutputs < 1) throw new ArgumentException("Number of outputs must be at least 1.");

            NumInputs = numInputs;

            // Primo hidden layer: prende direttamente i `numInputs` valori grezzi.
            Layers.Add(new Layer(numNeuronsPerLayer, numInputs, activationFunction));
            for (int i = 1; i < numHiddenLayers; i++)
                Layers.Add(new Layer(numNeuronsPerLayer, Layers[i - 1].NumNeurons, activationFunction));

            // Output di regressione: neurone/i LINEARE/i.
            Layers.Add(new Layer(numOutputs, Layers[Layers.Count - 1].NumNeurons, ActivationFunction.Linear));
        }

        public List<double> Prediction(List<double> inputs)
        {
            lastInputs = inputs.ToArray();
            double[] outputs = lastInputs;
            foreach (Layer l in Layers)
                outputs = l.Activate(outputs);
            return new List<double>(outputs);
        }

        // Mean Squared Error: la loss corretta per la regressione.
        public double CalculateLoss(double[] target)
        {
            if (target.Length != NumOutputs)
                throw new ArgumentException("Target length does not match number of outputs.");

            Layer outLayer = Layers[NumLayers - 1];
            double sum = 0.0;
            for (int i = 0; i < NumOutputs; i++)
            {
                double diff = outLayer.Neurons[i].Output - target[i];
                sum += diff * diff;
            }
            CurrentLoss = sum / NumOutputs;
            return CurrentLoss;
        }

        // Backpropagation standard. Chiamare prima Prediction(...) per popolare Output/lastInputs.
        public double BackPropagate(double[] target)
        {
            if (target.Length != NumOutputs)
                throw new ArgumentException("Target length does not match number of outputs.");

            // 1) Delta del layer di output: (pred - target) * f'(z). Per Linear f' = 1.
            Layer outLayer = Layers[NumLayers - 1];
            for (int i = 0; i < NumOutputs; i++)
            {
                Neuron neu = outLayer.Neurons[i];
                neu.Delta = (neu.Output - target[i]) * neu.Derivate();
            }

            // 2) Delta dei layer nascosti, propagati all'indietro dal layer successivo.
            for (int l = NumLayers - 2; l >= 0; l--)
            {
                Layer layer = Layers[l];
                Layer next = Layers[l + 1];
                for (int j = 0; j < layer.NumNeurons; j++)
                {
                    double sum = 0.0;
                    for (int k = 0; k < next.NumNeurons; k++)
                        sum += next.Neurons[k].Weights[j] * next.Neurons[k].Delta;
                    layer.Neurons[j].Delta = sum * layer.Neurons[j].Derivate();
                }
            }

            // 3) Discesa del gradiente. Le "attivazioni precedenti" del layer 0 sono gli input grezzi.
            for (int l = 0; l < NumLayers; l++)
            {
                Layer layer = Layers[l];
                double[] prevOut = (l == 0)
                    ? lastInputs
                    : Layers[l - 1].Neurons.Select(n => n.Output).ToArray();

                for (int j = 0; j < layer.NumNeurons; j++)
                {
                    Neuron neu = layer.Neurons[j];
                    for (int w = 0; w < neu.Weights.Count; w++)
                        neu.Weights[w] -= LearningRate * neu.Delta * prevOut[w];
                    neu.Bias -= LearningRate * neu.Delta;
                }
            }

            return CalculateLoss(target);
        }
    }
}
