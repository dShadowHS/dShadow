using System;

namespace Perсeptron_Duo
{
    class NeuronD
    {
        protected int inputsCount = 0;
        protected double[] weights = null;
        protected double threshold = 0.0f;
        protected double output = 0;
        protected double alpha = 2.0;

        protected static Random rand = new Random((int)DateTime.Now.Ticks);

        public int InputsCount
        {
            get { return inputsCount; }
        }

        public double Threshold
        {
            get { return threshold; }
            set { threshold = value; }
        }

        public double Output
        {
            get { return output; }
        }

        public double this[int index]
        {
            get { return weights[index]; }
            set { weights[index] = value; }
        }


        public NeuronD(int inputsNum, double alpha)
        {
            inputsCount = Math.Max(1, inputsNum);
            weights = new double[inputsCount];
            this.alpha = alpha;

            for (int i = 0; i < inputsCount; i++)
                weights[i] = rand.NextDouble();

            threshold = rand.NextDouble();
        }

	public double Sigmoid( double s )
	{
		return ( 1.0 / ( 1.0 + Math.Exp( -alpha * s ) ) );
	}

        public double Compute(double[] inputs)
        {
            if (inputs.Length != inputsCount)
                throw new ArgumentException();

            double sum = 0.0;

            for (int i = 0; i < inputsCount; i++)
            {
                sum += weights[i] * inputs[i];
            }

            sum += threshold;

            output = Sigmoid(sum);

            return output;
        }
    }
}
