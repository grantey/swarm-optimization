using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading;

namespace SwarmOptimization
{
    public abstract class SO
    {
        protected int I;
        protected int D;
        protected double[,] goodX;  //лучший набор координат среди всех итераций
        protected double[,] V;  //набор скоростей

        public double[,] bestX; //лучший набор координат среди всех частиц и итераций
        public double[,] currX;  //текущий набор координат
        public double bestF;

        int _Function;
        double[] Args;  //набор параметров


        public SO(int n, int d, int func, double[] args)
        {
            this.I = n;
            this.D = d;
            this._Function = func;
            Random r = new Random();

            this.bestX = new double[1, D];
            this.currX = new double[I, D];
            this.goodX = new double[I, D];
            this.V = new double[I, D];
            this.Args = new double[args.Length];

            for (int i = 0; i < I; i++)
                for (int j = 0; j < D; j++)
                {
                    currX[i, j] = r.NextDouble() * 5 + 5;
                    goodX[i, j] = currX[i, j];
                    V[i, j] = 0;// r.NextDouble() * 2 - 1;
                }
            for (int j = 0; j < args.Length; j++) Args[j] = args[j];

            bestF = f(bestX, 0);
            int k = 0;
            for (int i = 1; i < I; i++)
                if (f(currX, i) < bestF)
                {
                    k = i;
                    bestF = f(currX, i);
                }
            for (int j = 0; j < D; j++) bestX[0, j] = currX[k, j];
            bestF = f(bestX, 0);
        }

        public void SetX(double x, double v, int i, int j)
        {
            currX[i, j] = x;
            goodX[i, j] = x;
            V[i, j] = v;
        }


        protected double f(double[,] arg, int n)
        {
            double s = 0;
            switch (_Function)
            {
                case 1:
                        for (int i = 0; i < D; i++) s += arg[n, i] * arg[n, i];
                        return s;
                case 2:
                        for (int i = 0; i < D; i++) s += arg[n, i] * arg[n, i] - 10 * Math.Cos(2 * Math.PI * arg[n, i]);
                        return 10 * D + s;
                case 3:
                        for (int i = 0; i < D - 1; i++) s += 100 * Math.Pow(arg[n, i + 1] - arg[n, i] * arg[n, i], 2) + Math.Pow(arg[n, i] - 1, 2);
                        return s;
            }
            return 0;
        }

        public abstract void Swap();
        public abstract void Move();
    }

    class Classic : SO
    {
        double C1;
        double C2;

        public Classic(int n, int d, int func, double[] args) : base(n, d, func, args)
        {
            this.C1 = args[0];
            this.C2 = args[1];
        }

        public override void Swap()
        {
            double t;
            for (int i = 0; i < I; i++)
            {
                if ((t = f(currX, i)) < f(goodX, i)) for (int j = 0; j < D; j++) goodX[i, j] = currX[i, j];
                if (t < bestF)
                {
                    for (int j = 0; j < D; j++) bestX[0, j] = currX[i, j];
                    bestF = t;
                }
            }
        }

        public override void Move()
        {
            Random r = new Random();

            for (int i = 0; i < I; i++)
                for (int j = 0; j < D; j++)
                {
                    V[i, j] = 0.7298*(V[i, j] + C1 * r.NextDouble() * (goodX[i, j] - currX[i, j]) + C2 * r.NextDouble() * (bestX[0, j] - currX[i, j]));
                    currX[i, j] += V[i, j];
                }
        }
    }

    class FIPS : SO
    {
        double C;
        int K;
        double q;
        double[,] Neighbors;

        public FIPS(int n, int d, int func, double[] args) : base(n, d, func, args)
        {
            this.C = args[0];
            this.K = (int)args[1];
            q = 2 / Math.Abs(2 - C - Math.Sqrt(C * C - 4 * C));
            Neighbors = new double[2, I];
        }

        private void QuickSort(ref double[,] A, int x, int y)  //массив, индексы начала и конца
        {
            int i = x, j = y;
            double temp_dist, temp_num, aver_dist;

            aver_dist = A[1, x + (y - x >> 1)];
            
            do
            {
                while (A[1, i] < aver_dist) i++;
                while (A[1, j] > aver_dist) j--;

                if (i <= j)
                {
                    temp_num = A[0, i];
                    temp_dist = A[1, i];
                    A[0, i] = A[0, j];
                    A[1, i] = A[1, j];
                    A[0, j] = temp_num;
                    A[1, j] = temp_dist;
                    i++;
                    j--;
                }
            }
            while (i <= j);

            if (j > x) QuickSort(ref A, x, j);
            if (y > i) QuickSort(ref A, i, y);
        }

        public override void Swap()
        {
            double t;
            for (int i = 0; i < I; i++)
            {
                if ((t = f(currX, i)) < f(goodX, i)) for (int j = 0; j < D; j++) goodX[i, j] = currX[i, j];
                if (t < bestF)
                {
                    for (int j = 0; j < D; j++) bestX[0, j] = currX[i, j];
                    bestF = t;
                }
            }
        }

        public override void Move()
        {
            double d;
            Random r = new Random();

            for (int i = 0; i < I; i++)
            {
                for (int k = 0; k < I; k++)
                {
                    Neighbors[0, k] = k;
                    d = 0;
                    for (int j = 0; j < D; j++) d += Math.Pow(currX[i, j] - currX[k, j], 2);
                    Neighbors[1, k] = Math.Sqrt(d);
                }

                QuickSort(ref Neighbors, 0, I-1);

                for (int j = 0; j < D; j++)
                {
                    d = 0;
                    for (int k = 0; k < K; k++)
                    {
                        //Neighbors[0,k] - индекс k-го соседа
                        d += r.NextDouble() * C * (goodX[(int)Neighbors[0, k], j] - currX[i, j]);
                    }
                    V[i, j] = q * (V[i, j] + d / K);
                    currX[i, j] += V[i, j];
                }
            }
        }
    }

    class DEMPS : SO
    {
        double W;
        double Sigma;
        double R;
        int P;
        int Q;
        double dt;
        double a;
        double b;

        public DEMPS(int n, int d, int func, double[] args) : base(n, d, func, args)
        {
            this.W = args[0];
            this.Sigma = args[1];
            this.R = args[2];
            this.P = 3;
            this.Q = 5;
            this.dt = args[3];
            this.a = 1 / Math.Pow(4, 10 - args[4]);
            this.b = 1 / Math.Pow(2, 10 - args[5]);


        }

        private double Gradient(int n, int j)
        {
            double step = 0.001, grad;
            double[,] vect = new double[1, D];
            for (int i = 0; i < D; i++) vect[0, i] = currX[n, i];

            //трехточечный шаблон
            //grad = -3 * f(vect, 0);
            //vect[0, j] += step;
            //grad += 4 * f(vect, 0);
            //vect[0, j] += step;
            //grad -= f(vect, 0);
            //двухточечный шаблон
            grad = -f(vect, 0);
            vect[0, j] += step;
            grad += f(vect, 0);

            //return grad / (2 * step);
            return grad / step;            
        }

        public override void Swap()
        {
            for (int i = 0; i < I; i++)
                if (f(currX, i) < bestF)
                {
                    for (int j = 0; j < D; j++) bestX[0, j] = currX[i, j];
                    bestF = f(currX, i);
                }
        }

        public override void Move()
        {
            double s, t;
            Random r = new Random();

            for (int i = 0; i < I; i++)
                for (int j = 0; j < D; j++)
                {
                    s = 0;
                    for (int k = 0; k < I; k++)
                    {
                        if (k == i) continue;
                        t = R / Math.Abs(currX[i, j] - currX[k, j]);
                        s += (Math.Pow(t, P) - Math.Pow(t, Q)) * (currX[i, j] - currX[k, j]);
                    }
                    V[i, j] = W * V[i, j] - (a * s + b * Gradient(i, j)) * dt; //(bestX[0,j] - currX[i,j])) * dt;
                    currX[i, j] += V[i, j] + Sigma * (r.NextDouble() * dt - dt / 2);
                }
        }
    }
}
