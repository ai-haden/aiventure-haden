using System;
using System.Collections.Generic;
using System.Linq;

namespace Haden.Library.Algorithm
{
    /// <summary>
    /// The back propagation learning algorithm, which is widely used for training multi-layer neural networks with ReLU and Boltzmann machines.
    /// </summary>
    public class ForwardForward
    {
        public static int X { get; set; }
        public static int Y { get; set; }
        public static int x_pos = overlay_y_on_x(X, Y);

        public static object rnd = torch.randperm(x.size(0));

        public static int x_neg = overlay_y_on_x(X, Y[rnd]);


        public ForwardForward() 
        {
            var network = new ForwardNet(2);
            var seed = 1234;
            //torch.manual_seed(1234);
            network.Train(x_pos, x_neg);
        }
        // Todo: Convert and implement with Boagaphish the following:
        //public static Tuple MNISTLoaders(int train_batch_size = 50000, int test_batch_size = 10000)
        //{
        //    var transform = Compose(new List<object> {
        //        ToTensor(),
        //        Normalize(ValueTuple.Create(0.1307), ValueTuple.Create(0.3081)),
        //        Lambda(x => torch.flatten(x))
        //    });
        //    var train_loader = DataLoader(MNIST("./data/", train: true, download: true, transform: transform), batch_size: train_batch_size, shuffle: true);
        //    var test_loader = DataLoader(MNIST("./data/", train: false, download: true, transform: transform), batch_size: test_batch_size, shuffle: false);

        //    return Tuple.Create(train_loader, test_loader);
        //}
        // FOr image manipulation in the mnist setting, don't need this.
        public static int overlay_y_on_x(int x, int y)
        {
            var x_ = x.Clone();
            
            x_[":",::10] *= 0.0;
            x_[Enumerable.Range(0, x.shape[0]), y] = x.max();
            return x_;
        }

        public class Net //: torch.nn.Module
        {
                        
        }

        public class Layer //: nn.Linear
        {

           
        }

        public static object net = new ForwardNet(new [] {
            784,
            500,
            500
        });

        
    }
}
