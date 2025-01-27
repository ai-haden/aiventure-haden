//
// This autonomous intelligent system software is the property of Cartheur Research B.V. Copyright 2023, all rights reserved.
//
using System;
using System.Collections.Generic;
using Boagaphish.Core.Layers;
using Boagaphish.Core.Networks;
using Boagaphish.Core.Neurons;

namespace Boagaphish.Core.Learning
{
    /// <summary>
    /// The back propagation learning algorithm, which is widely used for training multi-layer neural networks with continuous activation functions.
    /// </summary>
    public class BackPropagation : ISupervised
    {
        // The network to teach.
        private readonly ActivationNetwork _network;
        // The learning rate.
        private double _learningRate;
        // The momentum.
        private double _momentum;
        // The neurons' errors.
        private readonly double[][] _neuronErrors;
        // The weights' updates.
        private readonly double[][][] _weightsUpdates;
        // The thresholds' updates.
        private readonly double[][] _thresholdUpdates;
        /// <summary>
        /// Initializes a new instance of the <see cref="BackPropagation" /> class, of the original algorithm.
        /// </summary>
        /// <param name="network">The network to teach.</param>
        /// <param name="learningRate">The learning rate.</param>
        public BackPropagation(ActivationNetwork network, double learningRate = 0.1)
        {
            _learningRate = learningRate;
            _network = network;
            // Create error and deltas arrays.
            _neuronErrors = new double[network.LayersCount][];
            _weightsUpdates = new double[network.LayersCount][][];
            _thresholdUpdates = new double[network.LayersCount][];
            // Initialize errors and deltas arrays for each layer.
            for (int i = 0, n = network.LayersCount; i < n; i++)
            {
                Layer layer = network[i];
                _neuronErrors[i] = new double[layer.NeuronsCount];
                _weightsUpdates[i] = new double[layer.NeuronsCount][];
                _thresholdUpdates[i] = new double[layer.NeuronsCount];
                // For each neuron.
                for (var j = 0; j < layer.NeuronsCount; j++)
                {
                    _weightsUpdates[i][j] = new double[layer.InputsCount];
                }
            }
        }
        /// <summary>
        /// The learning rate. The value determines speed of learning. Default value equals to 0.1.
        /// </summary>
        public double LearningRate
        {
            get { return _learningRate; }
            set
            {
                _learningRate = Math.Max(0.0, Math.Min(1.0, value));
            }
        }
        /// <summary>
        /// The momentum. The value determines the portion of previous weight's update to use on current iteration. Weight's update values are calculated on each iteration depending on neuron's error. The momentum specifies the amount of update to use from previous iteration and the amount of update to use from current iteration. If the value is equal to 0.1, for example, then 0.1 portion of previous update and 0.9 portion of current update are used to update weight's value.<br/><br/> Default value equals 0.0.
        /// </summary>
        public double Momentum
        {
            get { return _momentum; }
            set
            {
                _momentum = Math.Max(0.0, Math.Min(1.0, value));
            }
        }
        /// <summary>
        /// Runs a learning iteration. Runs one learning iteration and updates neuron's weights. Returns squared error of the last layer divided by two.
        /// </summary>
        /// <param name="input">The input vector.</param>
        /// <param name="desired">The desired output vector.</param>
        /// <returns>
        /// Returns the learning error.
        /// </returns>
        public double Run(double[] input, double[] desired)
        {
            // Compute the network's output.
            _network.Compute(input);
            // Calculate the network error.
            var error = CalculateError(desired);
            // Calculate weights updates.
            CalculateUpdates(input);
            // Update the network.
            UpdateNetwork();

            return error;
        }
        /// <summary>
        /// Runs a learning epoch. Runs series of learning iterations - one iteration for each input sample. Updates neuron's weights after each sample
        /// presented. Returns sum of squared errors of the last layer divided by two.
        /// </summary>
        /// <param name="input">array of input vectors</param>
        /// <param name="desired">array of output vectors</param>
        /// <returns>
        /// Returns a sum of learning errors.
        /// </returns>
        public double RunEpoch(double[][] input, double[][] desired)
        {
            var error = 0.0;
            // Run learning procedure for all samples.
            for (int i = 0, n = input.Length; i < n; i++)
            {
                error += Run(input[i], desired[i]);
            }
            // Return summary error.
            return error;
        }
        /// <summary>
        /// Calculates error values for all neurons of the network.
        /// </summary>
        /// <param name="desired">The desired output vector.</param>
        /// <returns>
        /// Returns summary squared error of the last layer divided by two.
        /// </returns>
        private double CalculateError(IList<double> desired)
        {
            // The initial error value.
            double error = 0;
            // The layers count.
            var layersCount = _network.LayersCount;
            // Assume that all neurons of the network have the same activation function.
            var function = _network[0][0].ActivationFunction;
            // Calculate error values for the last layer first.
            var layer = _network[layersCount - 1];
            var errors = _neuronErrors[layersCount - 1];

            for (int i = 0, n = layer.NeuronsCount; i < n; i++)
            {
                // The unit’s output value.
                var output = layer[i].Output;
                // The error of the unit.
                var e = desired[i] - output;
                // The error multiplied with activation function's derivative.
                errors[i] = e * function.Derivative2(output);
                // Square the error and sum it.
                error += (e * e);
            }
            // Calculate error values for other layers.
            for (var j = layersCount - 2; j >= 0; j--)
            {
                layer = _network[j];
                var layerNext = _network[j + 1];
                // The current and the next layers.
                errors = _neuronErrors[j];
                // The current and the next error arrays.
                var errorsNext = _neuronErrors[j + 1];
                // For all units of the layer.
                for (int i = 0, n = layer.NeuronsCount; i < n; i++)
                {
                    var sum = 0.0;
                    // For all units of the next layer.
                    for (int k = 0, m = layerNext.NeuronsCount; k < m; k++)
                    {
                        sum += errorsNext[k] * layerNext[k][i];
                    }
                    errors[i] = sum * function.Derivative2(layer[i].Output);
                }
            }
            // Return the squared error of the last layer divided by 2.
            return error / 2.0;
        }
        /// <summary>
        /// Calculate weights updates.
        /// </summary>
        /// <param name="input">The network's input vector.</param>
        private void CalculateUpdates(double[] input)
        {
            // The current neuron.
            ActivationNeuron neuron;
            // The neuron's weights updates.
            double[] neuronWeightUpdates;
            // The error value.
            double error;
            // 1 - Calculate the updates for the last layer first.
            // The current layer.
            ActivationLayer layer = _network[0];
            // The layer's error.
            double[] errors = _neuronErrors[0];
            // The layer's weights updates.
            double[][] layerWeightsUpdates = _weightsUpdates[0];
            // The layer's thresholds updates.
            double[] layerThresholdUpdates = _thresholdUpdates[0];
            // Compute for each neuron of the layer.
            for (int i = 0, n = layer.NeuronsCount; i < n; i++)
            {
                neuron = layer[i];
                error = errors[i];
                neuronWeightUpdates = layerWeightsUpdates[i];
                // Computer for each weight of the neuron.
                for (int j = 0, m = neuron.InputsCount; j < m; j++)
                {
                    // Calculate the weight update value.
                    neuronWeightUpdates[j] = _learningRate * (_momentum * neuronWeightUpdates[j] + (1.0 - _momentum) * error * input[j]);
                }
                // Calculate the threshold update.
                layerThresholdUpdates[i] = _learningRate * (_momentum * layerThresholdUpdates[i] + (1.0 - _momentum) * error);
            }
            // Post the update.

            // 2 - Calculate the update for all other layers.
            for (int k = 1, l = _network.LayersCount; k < l; k++)
            {
                // The previous layer.
                ActivationLayer layerPrev = _network[k - 1];
                layer = _network[k];
                errors = _neuronErrors[k];
                layerWeightsUpdates = _weightsUpdates[k];
                layerThresholdUpdates = _thresholdUpdates[k];
                // Compute for each neuron of the layer.
                for (int i = 0, n = layer.NeuronsCount; i < n; i++)
                {
                    neuron = layer[i];
                    error = errors[i];
                    neuronWeightUpdates = layerWeightsUpdates[i];
                    // Compute for each synapse of the neuron.
                    for (int j = 0, m = neuron.InputsCount; j < m; j++)
                    {
                        // Calculate the weight update value.
                        neuronWeightUpdates[j] = _learningRate * (
                            _momentum * neuronWeightUpdates[j] +
                            (1.0 - _momentum) * error * layerPrev[j].Output
                            );
                    }
                    // Calculate the threshold update.
                    layerThresholdUpdates[i] = _learningRate * (
                        _momentum * layerThresholdUpdates[i] +
                        (1.0 - _momentum) * error
                        );
                }
            }
            // Post the update.

        }
        /// <summary>
        /// Update the network's weights.
        /// </summary>
        private void UpdateNetwork()
        {
            // Compute for each layer of the network.
            for (int i = 0, n = _network.LayersCount; i < n; i++)
            {
                // The current layer.
                ActivationLayer layer = _network[i];
                // The layer's weights updates.
                double[][] layerWeightsUpdates = _weightsUpdates[i];
                // The layer's threshold updates.
                double[] layerThresholdUpdates = _thresholdUpdates[i];
                // Compute for each neuron of the layer.
                for (int j = 0, m = layer.NeuronsCount; j < m; j++)
                {
                    // The current neuron.
                    ActivationNeuron neuron = layer[j];
                    // The neuron's weights updates.
                    double[] neuronWeightUpdates = layerWeightsUpdates[j];
                    // Compute for each weight of the neuron.
                    for (int k = 0, s = neuron.InputsCount; k < s; k++)
                    {
                        // Update weight value.
                        neuron[k] += neuronWeightUpdates[k];
                    }
                    // Update threshold value.
                    neuron.Threshold += layerThresholdUpdates[j];
                }
            }
        }
    }
}
