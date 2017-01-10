using System;

namespace Perсeptron_Duo
{
    class NeuroNetD
    {
        protected int inputsCount;
        protected int layersCount = 2;
        protected LayerD[] layers;
        protected double[] outputs;
        protected double alpha = 2.0;

        public int InputsCount
        {
            get { return inputsCount; }
        }

        public int LayersCount
        {
            get { return layersCount; }
        }

        public double[] Outputs
        {
            get { return outputs; }
        }

        public LayerD this[int index]
        {
            get { return layers[index]; }
        }


        public NeuroNetD(int inputsCount, int layer1size, int layer2size, double alpha)
        {
            this.inputsCount = Math.Max(1, inputsCount);
            layers = new LayerD[this.layersCount];
            this.alpha = alpha;

            layers[0] = new LayerD(inputsCount, layer1size, this.alpha);
            layers[1] = new LayerD(layer1size, layer2size, this.alpha);
        }

        public double[] Compute(double[] inputs)
        {
            outputs = inputs;

            foreach (LayerD layer in layers)
            {
                outputs = layer.Compute(outputs);
            }

            return outputs;
        }
    }
}
