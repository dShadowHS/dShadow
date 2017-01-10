using System;

namespace Perсeptron_Duo
{
    class LayerD
    {
        protected int inputsCount = 0;
        protected int neuronsCount = 0;
        protected NeuronD[] neurons;
        protected double[] outputs;
        protected double alpha = 2.0;

        public int InputsCount
        {
            get { return inputsCount; }
        }

        public int NeuronsCount
        {
            get { return neuronsCount; }
        }

        public double[] Outputs
        {
            get { return outputs; }
        }

        public NeuronD this[int index]
        {
            get { return neurons[index]; }
        }

        public LayerD(int inputsCount, int neuronsCount, double alpha)
        {
            this.inputsCount = Math.Max(1, inputsCount);
            this.neuronsCount = Math.Max(1, neuronsCount);
            neurons = new NeuronD[this.neuronsCount];
            outputs = new double[this.neuronsCount];
            this.alpha = alpha;
            for (int i = 0; i < neuronsCount; i++)
                neurons[i] = new NeuronD(inputsCount, this.alpha);
        }

        public double[] Compute(double[] inputs)
        {
            for (int i = 0; i < neuronsCount; i++)
                outputs[i] = neurons[i].Compute(inputs);

            return outputs;
        }

    }
}
