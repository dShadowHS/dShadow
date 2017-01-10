using System;

namespace Perceptron_Duo
{
    class DataPoint
    {
        private int dim;
        private double[] coords;
        private int cls;

        public double this[int index]
        {
            get { return coords[index]; }
            set { coords[index] = value; }
        }

        public int Dim
        {
            get { return dim; }
            set { dim = value; }
        }

        public int Cls
        {
            get { return cls; }
            set { cls = value; }
        }


        public DataPoint(int dm, int cl)
        {
            dim = dm;
            coords = new double[dim];
            cls = cl;
        }

        public override string ToString()
        {
            string c = "";
            for (int i = 0; i < dim; i++)
            {
                c += string.Format("{0}", coords[i].ToString("f" + 3)) + ";";
            }
            c += string.Format("{0}", cls.ToString());
            return c;
        }

    }
}
