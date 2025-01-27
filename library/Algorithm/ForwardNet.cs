using System;
using System.Collections.Generic;
using System.Linq;

namespace Haden.Library.Algorithm
{
    public class ForwardNet
    {
        public ForwardNet(int dimensions) 
        {
            var layers = new List<ForwardLayer>();
            foreach (var d in Enumerable.Range(0, dimensions - 1))
            {
                layers.Add(new ForwardLayer(dimensions, dimensions));
            }
        }
        public ForwardNet(int[] dimensions)
        {
            var layers = new List<object>();
            foreach (var d in Enumerable.Range(0, dimensions.Count - 1))
            {
                layers.Add(new ForwardLayer(dimensions[d], dimensions[d + 1]));
            }
        }
        public double Predict(object x)
        {
            var goodness_per_label = new List<object>();
            foreach (var label in Enumerable.Range(0, 10))
            {
                var h = overlay_y_on_x(x, label);
                var goodness = new List<object>();
                foreach (var layer in this.layers)
                {
                    h = layer(h);
                    goodness += new List<object> {
                            h.pow(2).mean(1)
                        };
                }
                goodness_per_label += new List<object> {
                        goodness.Sum().unsqueeze(1)
                    };
            }
            goodness_per_label = torch.cat(goodness_per_label, 1);
            return goodness_per_label.argmax(1);
        }

        public void Train(object x_pos, object x_neg)
        {
            var h_pos = x_pos;
            var h_neg = x_neg;
            foreach (var _tup_1 in this.layers.Select((_p_1, _p_2) => Tuple.Create(_p_2, _p_1)))
            {
                var i = _tup_1.Item1;
                var layer = _tup_1.Item2;
                Console.WriteLine("training layer", i, "...");
                var _tup_2 = layer.train(h_pos, h_neg);
                h_pos = _tup_2.Item1;
                h_neg = _tup_2.Item2;
            }
        }
    }
}
