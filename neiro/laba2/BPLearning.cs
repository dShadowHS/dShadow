using System;

namespace Perсeptron_Duo
{
    class BPLearning
    {
        private NeuroNetD network;
        private double learningRate = 0.1;
        protected double alpha = 2.0;

        private double[][] neuronErrors = null;
        private double[][][] weightsUpdates = null;
        private double[][] thresholdsUpdates = null;


        public double LearningRate
        {
            get { return learningRate; }
            set
            {
                learningRate = Math.Max(0.0, Math.Min(1.0, value));
            }
        }


        public BPLearning(NeuroNetD network, double alpha)
        {
            this.network = network;
            this.alpha = alpha;

            neuronErrors = new double[network.LayersCount][];
            weightsUpdates = new double[network.LayersCount][][];
            thresholdsUpdates = new double[network.LayersCount][];

            for (int i = 0, n = network.LayersCount; i < n; i++)
            {
                LayerD layer = network[i];

                neuronErrors[i] = new double[layer.NeuronsCount];
                weightsUpdates[i] = new double[layer.NeuronsCount][];
                thresholdsUpdates[i] = new double[layer.NeuronsCount];

                for (int j = 0; j < layer.NeuronsCount; j++)
                {
                    weightsUpdates[i][j] = new double[layer.InputsCount];
                }
            }
        }

        public double Run(double[] input, double[] output)
        {
            network.Compute(input);
            double error = CalculateError(output);
            CalculateUpdates(input);
            UpdateNetwork();

            return error;
        }

        public double RunEpoch(double[][] input, double[][] output)
        {
            double error = 0.0;

            for (int i = 0, n = input.Length; i < n; i++)
            {
                error += Run(input[i], output[i]);
            }

            return error;
        }


        private double CalculateError(double[] learnOutput)
        {
            LayerD layer, layerNext;
            double[] errors, errorsNext;
            double error = 0, e, sum;
            double output;
            int layersCount = network.LayersCount;

            layer = network[layersCount - 1];       // начинаем с последнего слоя
            errors = neuronErrors[layersCount - 1];

            for (int i = 0, n = layer.NeuronsCount; i < n; i++)
            {
                output = layer[i].Output;
                e = learnOutput[i] - output;

                errors[i] = e * alpha * output * (1 - output);
                error += (e * e);
            }

            for (int j = layersCount - 2; j >= 0; j--)  // для остальных слоев
            {
                layer = network[j];
                layerNext = network[j + 1];
                errors = neuronErrors[j];
                errorsNext = neuronErrors[j + 1];

                for (int i = 0, n = layer.NeuronsCount; i < n; i++)
                {
                    sum = 0.0;
                    for (int k = 0, m = layerNext.NeuronsCount; k < m; k++)
                    {
                        sum += errorsNext[k] * layerNext[k][i];
                    }
                    errors[i] = sum * alpha * layer[i].Output * (1 - layer[i].Output);
                }
            }

            return error / 2.0;
        }

        private void CalculateUpdates(double[] input)
        {
            NeuronD neuron;
            LayerD layer, layerPrev;
            double[][] layerWeightsUpdates;
            double[] layerThresholdUpdates;
            double[] errors;
            double[] neuronWeightUpdates;

            layer = network[0];
            errors = neuronErrors[0];
            layerWeightsUpdates = weightsUpdates[0];
            layerThresholdUpdates = thresholdsUpdates[0];

            for (int i = 0, n = layer.NeuronsCount; i < n; i++)     // начинаем с первого слоя 
            {
                neuron = layer[i];
                neuronWeightUpdates = layerWeightsUpdates[i];

                for (int j = 0, m = neuron.InputsCount; j < m; j++)
                {
                    neuronWeightUpdates[j] = neuronWeightUpdates[j] + learningRate * errors[i] * input[j];
                }

                layerThresholdUpdates[i] = layerThresholdUpdates[i] + learningRate * errors[i];
            }

            for (int k = 1, l = network.LayersCount; k < l; k++)        // для остальных слоев
            {
                layerPrev = network[k - 1];
                layer = network[k];
                errors = neuronErrors[k];
                layerWeightsUpdates = weightsUpdates[k];
                layerThresholdUpdates = thresholdsUpdates[k];

                for (int i = 0, n = layer.NeuronsCount; i < n; i++)
                {
                    neuron = layer[i];
                    neuronWeightUpdates = layerWeightsUpdates[i];

                    for (int j = 0, m = neuron.InputsCount; j < m; j++)
                    {
                        neuronWeightUpdates[j] = neuronWeightUpdates[j] +
                                     learningRate * errors[i] * layerPrev[j].Output;
                    }

                    layerThresholdUpdates[i] = layerThresholdUpdates[i] + learningRate * errors[i];
                }
            }
        }

        private void UpdateNetwork()
        {
            NeuronD neuron;
            LayerD layer;
            double[][] layerWeightsUpdates;
            double[] layerThresholdUpdates;
            double[] neuronWeightUpdates;

            for (int i = 0, n = network.LayersCount; i < n; i++)
            {
                layer = network[i];
                layerWeightsUpdates = weightsUpdates[i];
                layerThresholdUpdates = thresholdsUpdates[i];

                for (int j = 0, m = layer.NeuronsCount; j < m; j++)
                {
                    neuron = layer[j];
                    neuronWeightUpdates = layerWeightsUpdates[j];

                    for (int k = 0, s = neuron.InputsCount; k < s; k++)
                    {
                        neuron[k] += neuronWeightUpdates[k];
                    }
                    neuron.Threshold += layerThresholdUpdates[j];
                }
            }
        }
    }
}
