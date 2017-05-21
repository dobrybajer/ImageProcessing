using System;
using System.Collections.Generic;
using System.Drawing;
using ImageProcessing.Model;

namespace ImageProcessing.Algorithms
{
    internal class Canny : BaseAlgorithm
    {
        public Canny() : base(AlgorithmType.Canny)
        {
        }

        public int[,] GaussianKernel { get; set; }

        public override Bitmap ProcessImage(Bitmap image)
        {
            //// TODO by Łukasz

            var grayimage = ToGrayScale(image);

            var gx = new[,]
            {
                {-1, 0, 1},
                {-2, 0, 2},
                {-1, 0, 1}
            };
            var gy = new[,]
            {
                {1, 2, 1},
                {0, 0, 0},
                {-1, -2, -1}
            };


            Image grayImage = grayimage;
            grayImage.Save(
                "C:\\Users\\Lukasz\\Documents\\GitHub\\ImageProcessing\\ImageProcessing\\ImageProcessing\\obj\\Debug\\TempPE\\tmp.bmp");

            var valuesOfGray = new int[grayimage.Width, grayimage.Height];


            for (var i = 0; i < grayimage.Width; i++)
                for (var j = 0; j < grayimage.Height; j++)
                    valuesOfGray[i, j] = grayimage.GetPixel(i, j).G;


            var imageWithGauss = GaussianFilter(valuesOfGray);
            var imageWithDifferentiatex = Differentiate(imageWithGauss, gx);
            var imageWithDifferentiatey = Differentiate(imageWithGauss, gy);

            var gradientImage = new int[imageWithDifferentiatex.GetLength(0), imageWithDifferentiatex.GetLength(1)];
            var angleImage = new int[imageWithDifferentiatex.GetLength(0), imageWithDifferentiatex.GetLength(1)];


            for (var i = 0; i < imageWithDifferentiatex.GetLength(0); i++)
            {
                for (var j = 0; j < imageWithDifferentiatex.GetLength(1); j++)
                {
                    var gxVal = imageWithDifferentiatex[i, j];
                    var gyVal = imageWithDifferentiatey[i, j];

                    gradientImage[i, j] = (int) Math.Sqrt(gxVal*gxVal + gyVal*gyVal);
                    var radians = Math.Atan2(gxVal, gyVal);
                    angleImage[i, j] = (int) (radians*(180/Math.PI));
                }
            }


            //TODO: czy potrzebne rozszerzenie 
            var grad_border = new int[imageWithDifferentiatex.GetLength(0), imageWithDifferentiatex.GetLength(1)];
            var rows = gradientImage.GetLength(0);
            var cols = gradientImage.GetLength(1);
            var edgeList = new List<Point>();


            for (var i = 0; i < grad_border.GetLength(0); i++)
                for (var j = 0; j < grad_border.GetLength(1); j++)
                    grad_border[i, j] = gradientImage[i, j];


            float highT = 200, lowT = 50;

            for (var row = 1; row < rows - 1; row++)
            {
                for (var col = 1; col < cols - 1; col++)
                {
                    var angle = angleImage[row - 1, col - 1];
                    var act = new Point(row - 1, col - 1);
                    Point p1, p2;
                    var w = neighbours(angle, out p1, out p2);

                    var g = grad_border[row, col];

                    var gpn1 = grad_border[addPoint(act, p1).X, addPoint(act, p1).Y];
                    var gpn2 = grad_border[addPoint(act, p2).X, addPoint(act, p2).Y];
                    var g1 = gpn2*w + gpn1*(1 - w);
                    var gmp1 = grad_border[subtractPoint(act, p1).X, subtractPoint(act, p1).Y];
                    var gmp2 = grad_border[subtractPoint(act, p2).X, subtractPoint(act, p2).Y];
                    var g2 = gmp2*w + gmp1*(1 - w);
                    if (g < g1 || g < g2)
                    {
                        grad_border[row, col] = 0;
                    }
                    else if (g > highT)
                    {
                        edgeList.Add(new Point(col, row));
                    }
                }
            }
            //TODO: Dodać Double thresholding?


            var tmp = new Bitmap(grayimage);

            for (var i = 0; i < grayimage.Width; i++)
                for (var j = 0; j < grayimage.Height; j++)
                {
                    tmp.SetPixel(i, j, Color.Black);
                }

            foreach (var point in edgeList)
            {
                tmp.SetPixel(point.Y, point.X, Color.White);
            }


            tmp.Save(
                "C:\\Users\\Lukasz\\Documents\\GitHub\\ImageProcessing\\ImageProcessing\\ImageProcessing\\obj\\Debug\\TempPE\\tmp2.bmp");


            return base.ProcessImage(image);
        }


        private void GenerateGaussianKernel(int N, float S, out int Weight)
        {
            var Sigma = S;
            float pi;
            pi = (float) Math.PI;
            int i, j;
            var SizeofKernel = N;

            var Kernel = new float[N, N];
            GaussianKernel = new int[N, N];
            var OP = new float[N, N];
            float D1, D2;


            D1 = 1/(2*pi*Sigma*Sigma);
            D2 = 2*Sigma*Sigma;

            float min = 1000;

            for (i = -SizeofKernel/2; i <= SizeofKernel/2; i++)
            {
                for (j = -SizeofKernel/2; j <= SizeofKernel/2; j++)
                {
                    Kernel[SizeofKernel/2 + i, SizeofKernel/2 + j] = 1/D1*(float) Math.Exp(-(i*i + j*j)/D2);
                    if (Kernel[SizeofKernel/2 + i, SizeofKernel/2 + j] < min)
                        min = Kernel[SizeofKernel/2 + i, SizeofKernel/2 + j];
                }
            }
            var mult = (int) (1/min);
            var sum = 0;
            if ((min > 0) && (min < 1))
            {
                for (i = -SizeofKernel/2; i <= SizeofKernel/2; i++)
                {
                    for (j = -SizeofKernel/2; j <= SizeofKernel/2; j++)
                    {
                        Kernel[SizeofKernel/2 + i, SizeofKernel/2 + j] =
                            (float) Math.Round(Kernel[SizeofKernel/2 + i, SizeofKernel/2 + j]*mult, 0);
                        GaussianKernel[SizeofKernel/2 + i, SizeofKernel/2 + j] =
                            (int) Kernel[SizeofKernel/2 + i, SizeofKernel/2 + j];
                        sum = sum + GaussianKernel[SizeofKernel/2 + i, SizeofKernel/2 + j];
                    }
                }
            }
            else
            {
                sum = 0;
                for (i = -SizeofKernel/2; i <= SizeofKernel/2; i++)
                {
                    for (j = -SizeofKernel/2; j <= SizeofKernel/2; j++)
                    {
                        Kernel[SizeofKernel/2 + i, SizeofKernel/2 + j] =
                            (float) Math.Round(Kernel[SizeofKernel/2 + i, SizeofKernel/2 + j], 0);
                        GaussianKernel[SizeofKernel/2 + i, SizeofKernel/2 + j] =
                            (int) Kernel[SizeofKernel/2 + i, SizeofKernel/2 + j];
                        sum = sum + GaussianKernel[SizeofKernel/2 + i, SizeofKernel/2 + j];
                    }
                }
            }
            //Normalizing kernel Weight
            Weight = sum;
        }

        private int[,] GaussianFilter(int[,] Data)
        {
            var KernelWeight = 0;
            var KernelSize = 5;
            GenerateGaussianKernel(5, 1, out KernelWeight);

            var Output = new int[Data.GetLength(0), Data.GetLength(1)];
            int i, j, k, l;
            var Limit = KernelSize/2;

            float Sum = 0;


            Output = Data; // Removes Unwanted Data Omission due to kernel bias while convolution


            for (i = Limit; i <= Data.GetLength(0) - 1 - Limit; i++)
            {
                for (j = Limit; j <= Data.GetLength(1) - 1 - Limit; j++)
                {
                    Sum = 0;
                    for (k = -Limit; k <= Limit; k++)
                    {
                        for (l = -Limit; l <= Limit; l++)
                        {
                            Sum = Sum + Data[i + k, j + l]*GaussianKernel[Limit + k, Limit + l];
                        }
                    }
                    Output[i, j] = (int) Math.Round(Sum/KernelWeight);
                }
            }


            return Output;
        }


        private float[,] Differentiate(int[,] Data, int[,] Filter)
        {
            int i, j, k, l, Fh, Fw;

            Fw = Filter.GetLength(0);
            Fh = Filter.GetLength(1);
            float sum = 0;
            var Output = new float[Data.GetLength(0), Data.GetLength(1)];

            for (i = Fw/2; i <= Data.GetLength(0) - Fw/2 - 1; i++)
            {
                for (j = Fh/2; j <= Data.GetLength(1) - Fh/2 - 1; j++)
                {
                    sum = 0;
                    for (k = -Fw/2; k <= Fw/2; k++)
                    {
                        for (l = -Fh/2; l <= Fh/2; l++)
                        {
                            sum = sum + Data[i + k, j + l]*Filter[Fw/2 + k, Fh/2 + l];
                        }
                    }
                    Output[i, j] = sum;
                }
            }
            return Output;
        }

        private float neighbours(int angle, out Point n1, out Point n2)
        {
            if (angle >= 180)
                angle -= 180;
            var quarter = angle/45;
            var weight = angle - quarter;
            switch (quarter)
            {
                case 0:
                    n1 = new Point(1, 0);
                    n2 = new Point(1, 1);
                    break;
                case 1:
                    n1 = new Point(1, 1);
                    n2 = new Point(0, 1);
                    break;
                case 2:
                    n1 = new Point(0, 1);
                    n2 = new Point(-1, 1);
                    break;
                default:
                    n1 = new Point(-1, 1);
                    n2 = new Point(-1, 0);
                    break;
            }
            return weight/45.0f;
        }

        private Point addPoint(Point a, Point b)
        {
            return new Point(a.X + b.X + 1, a.Y + b.Y + 1); //1 do sztuczne przesuniecie 
        }

        private Point subtractPoint(Point a, Point b)
        {
            return new Point(a.X - b.X + 1, a.Y - b.Y + 1); //1 do sztuczne przesuniecie 
        }
    }
}