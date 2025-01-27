using System;
using System.Collections.Generic;
using System.Linq;

namespace Haden.Library.Algorithm
{
    public class ForwardLayer
    {
        public ForwardLayer(object in_features, object out_features, bool bias = true, object device = null, object dtype = null)
            : base(out_features, bias, device, dtype)
        {
            var relu = torch.nn.ReLU();
            var opt = Adam(this.parameters(), lr: 0.03);
            var threshold = 2.0;
            var num_epochs = 1000;
        }

            public virtual object Forward(object x)
        {
            var x_direction = x / (x.norm(2, 1, keepdim: true) + 0.0001);
            return this.relu(torch.mm(x_direction, this.weight.T) + this.bias.unsqueeze(0));
        }

        public static Tuple Train(object x_pos, object x_neg)
        {
            foreach (var i in tqdm(Enumerable.Range(0, this.num_epochs)))
            {
                var g_pos = this.forward(x_pos).pow(2).mean(1);
                var g_neg = this.forward(x_neg).pow(2).mean(1);
                // The following loss pushes pos (neg) samples to
                // values larger (smaller) than the self.threshold.
                var loss = torch.log(1 + torch.exp(torch.cat(new List<object> {
                        -g_pos + this.threshold,
                        g_neg - this.threshold
                    }))).mean();
                this.opt.zero_grad();
                // this backward just compute the derivative and hence
                // is not considered backpropagation.
                loss.backward();
                this.opt.step();
            }
            return Tuple.Create(this.forward(x_pos).detach(), this.forward(x_neg).detach());
        }
    }
    
}
