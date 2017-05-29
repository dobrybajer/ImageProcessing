using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using ImageProcessing.Model;
using OpenCvSharp;
using Point = System.Drawing.Point;



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
            //var bb = Canny2(image);


            Mat src = new Mat("C:\\Users\\Lukasz\\Documents\\GitHub\\ImageProcessing\\ImageProcessing\\Input\\DSC_9509.JPG", ImreadModes.GrayScale);
            Mat dst = new Mat();
            Cv2.Canny(src, dst, 50, 200);
            using (new Window("src image", src))
            using (new Window("dst image", dst))
            {
                Cv2.WaitKey();
            }

            var bb = Canny2(image);

            return bb;
        }

        private Bitmap Canny2(Bitmap image)
        {
            var b = image;
            var width = b.Width;
            var height = b.Height;

            var edgeList = new Stack<Point>();
            var edgeList2 = new List<Point>();
            var grayimage = ToGrayScale(image);

            var allPix = new int[width, height];


            for (var i = 0; i < width; i++)
            {
                for (var j = 0; j < height; j++)
                {
                    allPix[i, j] = grayimage.GetPixel(i, j).R;
                }
            }

            //allPix = GaussianFilter(allPix);

            int[,] gx = {{-1, 0, 1}, {-2, 0, 2}, {-1, 0, 1}};
            int[,] gy = {{1, 2, 1}, {0, 0, 0}, {-1, -2, -1}};
            int new_x = 0, new_y = 0;
            int rc;
            int grad;
            var graidient = new int[width, height];


            int atan;

            var tanR = new int[width, height];


            for (var i = 1; i < b.Width - 1; i++)
            {
                for (var j = 1; j < b.Height - 1; j++)
                {
                    new_x = 0;
                    new_y = 0;


                    for (var wi = -1; wi < 2; wi++)
                    {
                        for (var hw = -1; hw < 2; hw++)
                        {
                            rc = allPix[i + hw, j + wi];
                            new_x += gx[wi + 1, hw + 1]*rc;
                            new_y += gy[wi + 1, hw + 1]*rc;
                        }
                    }


                    grad = (int) Math.Sqrt(new_x*new_x + new_y*new_y);
                    graidient[i, j] = grad;

                    atan = (int) (Math.Atan((double) new_y/new_x)*(180/Math.PI));
                    if ((atan > 0 && atan < 22.5) || (atan > 157.5 && atan < 180))
                    {
                        atan = 0;
                    }
                    else if (atan > 22.5 && atan < 67.5)
                    {
                        atan = 45;
                    }
                    else if (atan > 67.5 && atan < 112.5)
                    {
                        atan = 90;
                    }
                    else if (atan > 112.5 && atan < 157.5)
                    {
                        atan = 135;
                    }

                    if (atan == 0)
                    {
                        tanR[i, j] = 0;
                    }
                    else if (atan == 45)
                    {
                        tanR[i, j] = 1;
                    }
                    else if (atan == 90)
                    {
                        tanR[i, j] = 2;
                    }
                    else if (atan == 135)
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
                        if (graidient[i - 1, j] < graidient[i, j] && graidient[i + 1, j] < graidient[i, j])
                        {
                            allPixRs[i, j] = graidient[i, j];
                        }
                        else
                        {
                            allPixRs[i, j] = 0;
                        }
                    }
                    if (tanR[i, j] == 1)
                    {
                        if (graidient[i - 1, j + 1] < graidient[i, j] && graidient[i + 1, j - 1] < graidient[i, j])
                        {
                            allPixRs[i, j] = graidient[i, j];
                        }
                        else
                        {
                            allPixRs[i, j] = 0;
                        }
                    }
                    if (tanR[i, j] == 2)
                    {
                        if (graidient[i, j - 1] < graidient[i, j] && graidient[i, j + 1] < graidient[i, j])
                        {
                            allPixRs[i, j] = graidient[i, j];
                        }
                        else
                        {
                            allPixRs[i, j] = 0;
                        }
                    }
                    if (tanR[i, j] == 3)
                    {
                        if (graidient[i - 1, j - 1] < graidient[i, j] && graidient[i + 1, j + 1] < graidient[i, j])
                        {
                            allPixRs[i, j] = graidient[i, j];
                        }
                        else
                        {
                            allPixRs[i, j] = 0;
                        }
                    }
                }
            }

            var threshold = 200;
            var lowT = 50;
            var allPixRf = new int[width, height];

            var bb = new Bitmap(width, height);

            for (var i = 2; i < width - 2; i++)
            {
                for (var j = 2; j < height - 2; j++)
                {
                    if (allPixRs[i, j] > threshold)
                    {
                        allPixRf[i, j] = 1;
                        edgeList.Push(new Point(i, j));
                        edgeList2.Add(new Point(i, j));
                    }
                    else
                    {
                        allPixRf[i, j] = 0;
                    }

                    //    if (allPixRf[i, j] == 1)
                    //    {
                    //        bb.SetPixel(i, j, Color.White);
                    //        _numberOfEdgePixels++;
                    //    }
                    //    else
                    //        bb.SetPixel(i, j, Color.Black);
                }
            }

            while (edgeList.Count != 0)
            {
                var act = edgeList.Pop();

                //tmp.SetPixel(act.X, act.Y, Color.White);
                Point p1, p2;
                switch (tanR[act.X, act.Y])
                {
                    case 0:
                        p1 = new Point(1, 0);
                        p2 = new Point(1, 1);
                        break;
                    case 1:
                        p1 = new Point(1, 1);
                        p2 = new Point(0, 1);
                        break;
                    case 2:
                        p1 = new Point(0, 1);
                        p2 = new Point(-1, 1);
                        break;
                    default:
                        p1 = new Point(-1, 1);
                        p2 = new Point(-1, 0);
                        break;
                }

                var newPoint1 = new Point(act.X + p1.X, act.Y + p1.Y);
                var newPoint2 = new Point(act.X + p2.X, act.Y + p2.Y);
                var newPoint3 = new Point(act.X - p1.X, act.Y - p1.Y);
                var newPoint4 = new Point(act.X - p2.X, act.Y - p2.Y);

                if (allPixRs[newPoint1.X, newPoint1.Y] > lowT )
                    edgeList2.Add(newPoint1);

                if (allPixRs[newPoint2.X, newPoint2.Y] > lowT )
                    edgeList2.Add(newPoint2);

                if (allPixRs[newPoint3.X, newPoint3.Y] > lowT )
                    edgeList2.Add(newPoint3);

                if (allPixRs[newPoint4.X, newPoint4.Y] > lowT )
                    edgeList2.Add(newPoint4);
            }

            for (var i = 0; i < grayimage.Width; i++)
                for (var j = 0; j < grayimage.Height; j++)
                {
                    bb.SetPixel(i, j, Color.Black);
                }

            foreach (var point in edgeList2)
            {
                bb.SetPixel(point.X, point.Y, Color.White);
            }

            _numberOfEdgePixels = edgeList2.Count;


            return bb;
        }

        private Bitmap CannyAlgo(Bitmap image)
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


            float highT = 70, lowT = 20;
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

            Bitmap tmp = new Bitmap(grayimage);


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


            Output = Data;


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