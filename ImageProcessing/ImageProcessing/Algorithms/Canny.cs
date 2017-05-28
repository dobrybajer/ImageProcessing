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
            var bb = Canny2(image);
            //var bb = Canny2(image);

            return bb;
        }

        private Bitmap Canny2(Bitmap image)
        {
            var b = image;
            var width = b.Width;
            var height = b.Height;


            var grayimage = ToGrayScale(image);
            //pictureBox2.Image = n;//////////////////////////////////////////////////////here onward use n///////////////////////////////////////////////
            var allPixRn = new int[width, height];


            for (var i = 0; i < width; i++)
            {
                for (var j = 0; j < height; j++)
                {
                    allPixRn[i, j] = grayimage.GetPixel(i, j).R;
                }
            }


            int[,] gx = {{-1, 0, 1}, {-2, 0, 2}, {-1, 0, 1}};
            int[,] gy = {{1, 2, 1}, {0, 0, 0}, {-1, -2, -1}};
            int new_rx = 0, new_ry = 0;
            int new_gx = 0, new_gy = 0;
            int new_bx = 0, new_by = 0;
            int rc;
            int gradR;

            var graidientR = new int[width, height];


            int atanR;

            var tanR = new int[width, height];


            for (var i = 1; i < b.Width - 1; i++)
            {
                for (var j = 1; j < b.Height - 1; j++)
                {
                    new_rx = 0;
                    new_ry = 0;


                    for (var wi = -1; wi < 2; wi++)
                    {
                        for (var hw = -1; hw < 2; hw++)
                        {
                            rc = allPixRn[i + hw, j + wi];
                            new_rx += gx[wi + 1, hw + 1]*rc;
                            new_ry += gy[wi + 1, hw + 1]*rc;
                        }
                    }

                    //find gradieant
                    gradR = (int) Math.Sqrt(new_rx*new_rx + new_ry*new_ry);
                    graidientR[i, j] = gradR;


                    //find tans
                    ////////////////tan red//////////////////////////////////
                    atanR = (int) (Math.Atan((double) new_ry/new_rx)*(180/Math.PI));
                    if ((atanR > 0 && atanR < 22.5) || (atanR > 157.5 && atanR < 180))
                    {
                        atanR = 0;
                    }
                    else if (atanR > 22.5 && atanR < 67.5)
                    {
                        atanR = 45;
                    }
                    else if (atanR > 67.5 && atanR < 112.5)
                    {
                        atanR = 90;
                    }
                    else if (atanR > 112.5 && atanR < 157.5)
                    {
                        atanR = 135;
                    }

                    if (atanR == 0)
                    {
                        tanR[i, j] = 0;
                    }
                    else if (atanR == 45)
                    {
                        tanR[i, j] = 1;
                    }
                    else if (atanR == 90)
                    {
                        tanR[i, j] = 2;
                    }
                    else if (atanR == 135)
                    {
                        tanR[i, j] = 3;
                    }
                }
            }

            var allPixRs = new int[width, height];


            for (var i = 2; i < width - 2; i++)
            {
                for (var j = 2; j < height - 2; j++)
                {
                    if (tanR[i, j] == 0)
                    {
                        if (graidientR[i - 1, j] < graidientR[i, j] && graidientR[i + 1, j] < graidientR[i, j])
                        {
                            allPixRs[i, j] = graidientR[i, j];
                        }
                        else
                        {
                            allPixRs[i, j] = 0;
                        }
                    }
                    if (tanR[i, j] == 1)
                    {
                        if (graidientR[i - 1, j + 1] < graidientR[i, j] && graidientR[i + 1, j - 1] < graidientR[i, j])
                        {
                            allPixRs[i, j] = graidientR[i, j];
                        }
                        else
                        {
                            allPixRs[i, j] = 0;
                        }
                    }
                    if (tanR[i, j] == 2)
                    {
                        if (graidientR[i, j - 1] < graidientR[i, j] && graidientR[i, j + 1] < graidientR[i, j])
                        {
                            allPixRs[i, j] = graidientR[i, j];
                        }
                        else
                        {
                            allPixRs[i, j] = 0;
                        }
                    }
                    if (tanR[i, j] == 3)
                    {
                        if (graidientR[i - 1, j - 1] < graidientR[i, j] && graidientR[i + 1, j + 1] < graidientR[i, j])
                        {
                            allPixRs[i, j] = graidientR[i, j];
                        }
                        else
                        {
                            allPixRs[i, j] = 0;
                        }
                    }
                }
            }

            var threshold = 80;
            var allPixRf = new int[width, height];

            // Bitmap bb = new Bitmap (pictureBox1.Image);
            var bb = new Bitmap(width, height);

            for (var i = 2; i < width - 2; i++)
            {
                for (var j = 2; j < height - 2; j++)
                {
                    if (allPixRs[i, j] > threshold)
                    {
                        allPixRf[i, j] = 1;
                    }
                    else
                    {
                        allPixRf[i, j] = 0;
                    }

                    if (allPixRf[i, j] == 1)
                    {
                        bb.SetPixel(i, j, Color.White);
                        _numberOfEdgePixels++;
                    }
                    else
                        bb.SetPixel(i, j, Color.Black);
                }
            }
            return bb;
        }

        private Bitmap CannyAlgo(Bitmap image, out Bitmap tmp)
        {
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

            var grayimage = ToGrayScale(image);


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


            var grad_border = new int[imageWithDifferentiatex.GetLength(0), imageWithDifferentiatex.GetLength(1)];
            var rows = gradientImage.GetLength(0);
            var cols = gradientImage.GetLength(1);
            var edgeList = new Stack<Point>();
            var edgeList2 = new List<Point>();

            for (var i = 0; i < grad_border.GetLength(0); i++)
                for (var j = 0; j < grad_border.GetLength(1); j++)
                    grad_border[i, j] = gradientImage[i, j];


            float highT = 200, lowT = 20;
            //wycinanie krawedzi 
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
                        grad_border[row, col] = g;
                    }
                    else if (g > highT)
                    {
                        edgeList.Push(new Point(row, col));
                        edgeList2.Add(new Point(row, col));
                    }
                }
            }

            tmp = new Bitmap(grayimage);


            //while (edgeList.Count != 0)
            //{
            //    var act = edgeList.Pop();
            //    var angle = angleImage[act.X, act.Y];
            //    //tmp.SetPixel(act.X, act.Y, Color.White);
            //    Point p1, p2;
            //    var w = neighbours(angle, out p1, out p2);

            //    var newPoint1 = new Point(act.X + p1.X, act.Y + p1.Y);
            //    var newPoint2 = new Point(act.X + p2.X, act.Y + p2.Y);
            //    var newPoint3 = new Point(act.X - p1.X, act.Y - p1.Y);
            //    var newPoint4 = new Point(act.X - p2.X, act.Y - p2.Y);

            //    if (grad_border[newPoint1.X, newPoint1.Y] > lowT)
            //        edgeList2.Add(newPoint1);

            //    if (grad_border[newPoint2.X, newPoint2.Y] > lowT)
            //        edgeList2.Add(newPoint2);

            //    if (grad_border[newPoint3.X, newPoint3.Y] > lowT)
            //        edgeList2.Add(newPoint3);

            //    if (grad_border[newPoint4.X, newPoint4.Y] > lowT)
            //        edgeList2.Add(newPoint4);
            //}          

            for (var i = 0; i < grayimage.Width; i++)
                for (var j = 0; j < grayimage.Height; j++)
                {
                    tmp.SetPixel(i, j, Color.Black);
                }

            foreach (var point in edgeList2)
            {
                tmp.SetPixel(point.X, point.Y, Color.White);
            }

            _numberOfEdgePixels = edgeList2.Count;

            return tmp;
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