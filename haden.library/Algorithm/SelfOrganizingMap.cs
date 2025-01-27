//
// This autonomous intelligent system software is the property of Cartheur Research B.V. Copyright 2023, all rights reserved.
//
using System;
using System.Linq;
using Boagaphish.Core.Layers;
using Boagaphish.Core.Networks;
using Boagaphish.Core.Neurons;

namespace Boagaphish.Core.Learning
{
    /// <summary>
    /// Kohonen Self-Organizing Map (SOM) learning algorithm
    /// </summary>
    /// <remarks>This class implements Kohonen's SOM learning algorithm and is widely used in clusterization tasks. The class allows to train <see cref="DistanceNetwork">Distance Networks</see>.
    /// </remarks>
    public class SelfOrganizingMap : IUnsupervised
    {
        // neural network to train
        private readonly DistanceNetwork _network;
        // network's dimension
        private readonly int _width;
        private readonly int _height;
        // learning rate
        private double _learningRate = 0.1;
        // learning radius
        private double _learningRadius = 7;
        // squared learning radius multiplied by 2 (precalculated value to speed up computations)
        private double _squaredRadius2 = 2 * 7 * 7;
        /// <summary>
        /// Learning rate
        /// </summary>
        /// <value>The learning rate.</value>
        /// <remarks>Determines speed of learning. Value range is [0, 1]. Default value equals to 0.1.</remarks>
        public double LearningRate
        {
            get { return _learningRate; }
            set
            {
                _learningRate = Math.Max(0.0, Math.Min(1.0, value));
            }
        }
        /// <summary>
        /// Learning radius
        /// </summary>
        /// <value>The learning radius.</value>
        /// <remarks>Determines the amount of neurons to be updated around
        /// winner neuron. Neurons, which are in the circle of specified radius,
        /// are updated during the learning procedure. Neurons, which are closer
        /// to the winner neuron, get more update.<br/><br/>
        /// Default value equals to 7.</remarks>
        public double LearningRadius
        {
            get { return _learningRadius; }
            set
            {
                _learningRadius = Math.Max(0, value);
                _squaredRadius2 = 2 * _learningRadius * _learningRadius;
            }
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="SelfOrganizingMap"/> class
        /// </summary>
        /// <param name="network">Neural network to train</param>
        /// <remarks>This constructor supposes that a square network will be passed for training - it should be possible to get square root of network's neurons amount.
        /// </remarks>
        public SelfOrganizingMap(DistanceNetwork network)
        {
            // network's dimension was not specified, let's try to guess
            var neuronsCount = network[0].NeuronsCount;
            _width = (int)Math.Sqrt(neuronsCount);

            if (_width * _width != neuronsCount)
            {
                throw new ArgumentException("Invalid network size");
            }

            // ok, we got it
            _network = network;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="SelfOrganizingMap"/> class
        /// </summary>
        /// <param name="network">Neural network to train</param>
        /// <param name="width">Neural network's width</param>
        /// <param name="height">Neural network's height</param>
        /// <remarks>The constructor allows to pass network of arbitrary rectangular shape.
        /// The amount of neurons in the network should be equal to <b>width</b> * <b>height</b>.
        /// </remarks>
        public SelfOrganizingMap(DistanceNetwork network, int width, int height)
        {
            // check network size
            if (network[0].NeuronsCount != width * height)
            {
                throw new ArgumentException("Invalid network size");
            }

            _network = network;
            _width = width;
            _height = height;
        }
        /// <summary>
        /// Runs learning iteration
        /// </summary>
        /// <param name="input">input vector</param>
        /// <returns>
        /// Returns learning error - summary absolute difference between updated
        /// weights and according inputs. The difference is measured according to the neurons
        /// distance to the  winner neuron.
        /// </returns>
        public double Run(double[] input)
        {
            double error = 0.0;

            // compute the network
            _network.Compute(input);
            int winner = _network.GetWinner();

            // get layer of the network
            Layer layer = _network[0];

            // check learning radius
            if (_learningRadius == 0)
            {
                Neuron neuron = layer[winner];

                // update weight of the winner only
                for (int i = 0, n = neuron.InputsCount; i < n; i++)
                {
                    neuron[i] += (input[i] - neuron[i]) * _learningRate;
                }
            }
            else
            {
                // winner's X and Y
                int wx = winner % _width;
                int wy = winner / _width;

                // walk through all neurons of the layer
                for (int j = 0, m = layer.NeuronsCount; j < m; j++)
                {
                    Neuron neuron = layer[j];

                    int dx = (j % _width) - wx;
                    int dy = (j / _width) - wy;

                    // update factor ( Gaussian based )
                    double factor = Math.Exp(-(double)(dx * dx + dy * dy) / _squaredRadius2);

                    // update weight of the neuron
                    for (int i = 0, n = neuron.InputsCount; i < n; i++)
                    {
                        // calculate the error
                        double e = (input[i] - neuron[i]) * factor;
                        error += Math.Abs(e);
                        // update weight
                        neuron[i] += e * _learningRate;
                    }
                }
            }
            return error;
        }
        /// <summary>
        /// Runs learning epoch
        /// </summary>
        /// <param name="input">array of input vectors</param>
        /// <returns>
        /// Returns summary learning error for the epoch. See <see cref="Run"/>
        /// method for details about learning error calculation.
        /// </returns>
        public double RunEpoch(double[][] input)
        {
            // walk through all training samples and return summary error
            return input.Sum(sample => Run(sample));
        }
    }
}
