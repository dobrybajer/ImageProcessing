namespace ImageProcessing.Algorithms
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using Point = System.Drawing.Point;

    using Model;
    
    internal class Canny : BaseAlgorithm
    {
        public Canny() : base(AlgorithmType.Canny) { }

        public int[,] GaussianKernel { get; set; }

        public override Bitmap ProcessImage(Bitmap image)
        {          
            return Canny2(image);
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

            int[,] gx = {{-1, 0, 1}, {-2, 0, 2}, {-1, 0, 1}};
            int[,] gy = {{1, 2, 1}, {0, 0, 0}, {-1, -2, -1}};
            var graidient = new int[width, height];
            
            var tanR = new int[width, height];
            
            for (var i = 1; i < b.Width - 1; i++)
            {
                for (var j = 1; j < b.Height - 1; j++)
                {
                    var newX = 0;
                    var newY = 0;


                    for (var wi = -1; wi < 2; wi++)
                    {
                        for (var hw = -1; hw < 2; hw++)
                        {
                            var rc = allPix[i + hw, j + wi];
                            newX += gx[wi + 1, hw + 1]*rc;
                            newY += gy[wi + 1, hw + 1]*rc;
                        }
                    }


                    var grad = (int) Math.Sqrt(newX*newX + newY*newY);
                    graidient[i, j] = grad;

                    var atan = (int) (Math.Atan((double) newY/newX)*(180/Math.PI));
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

            const int lowT = 50;
            var allPixRf = new int[width, height];

            var bb = new Bitmap(width, height);

            for (var i = 2; i < width - 2; i++)
            {
                for (var j = 2; j < height - 2; j++)
                {
                    if (allPixRs[i, j] > Threshold)
                    {
                        allPixRf[i, j] = 1;
                        edgeList.Push(new Point(i, j));
                        edgeList2.Add(new Point(i, j));
                    }
                    else
                    {
                        allPixRf[i, j] = 0;
                    }
                }
            }

            while (edgeList.Count != 0)
            {
                var act = edgeList.Pop();

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

            NumberOfEdgePixels = edgeList2.Count;


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

            var gradBorder = new int[imageWithDifferentiatex.GetLength(0), imageWithDifferentiatex.GetLength(1)];
            var rows = gradientImage.GetLength(0);
            var cols = gradientImage.GetLength(1);
            var edgeList = new Stack<Point>();
            var edgeList2 = new List<Point>();

            for (var i = 0; i < gradBorder.GetLength(0); i++)
                for (var j = 0; j < gradBorder.GetLength(1); j++)
                    gradBorder[i, j] = gradientImage[i, j];


            const float highT = 70;

            //wycinanie krawedzi 
            for (var row = 1; row < rows - 1; row++)
            {
                for (var col = 1; col < cols - 1; col++)
                {
                    var angle = angleImage[row - 1, col - 1];
                    var act = new Point(row - 1, col - 1);
                    Point p1, p2;
                    var w = Neighbours(angle, out p1, out p2);

                    var g = gradBorder[row, col];

                    var gpn1 = gradBorder[AddPoint(act, p1).X, AddPoint(act, p1).Y];
                    var gpn2 = gradBorder[AddPoint(act, p2).X, AddPoint(act, p2).Y];
                    var g1 = gpn2*w + gpn1*(1 - w);
                    var gmp1 = gradBorder[SubtractPoint(act, p1).X, SubtractPoint(act, p1).Y];
                    var gmp2 = gradBorder[SubtractPoint(act, p2).X, SubtractPoint(act, p2).Y];
                    var g2 = gmp2*w + gmp1*(1 - w);
                    if (g < g1 || g < g2)
                    {
                        gradBorder[row, col] = g;
                    }
                    else if (g > highT)
                    {
                        edgeList.Push(new Point(row, col));
                        edgeList2.Add(new Point(row, col));
                    }
                }
            }

            var tmp = new Bitmap(grayimage);

            for (var i = 0; i < grayimage.Width; i++)
                for (var j = 0; j < grayimage.Height; j++)
                {
                    tmp.SetPixel(i, j, Color.Black);
                }

            foreach (var point in edgeList2)
            {
                tmp.SetPixel(point.X, point.Y, Color.White);
            }

            NumberOfEdgePixels = edgeList2.Count;

            return tmp;
        }

        private void GenerateGaussianKernel(int n, float s, out int weight)
        {
            var sigma = s;
            const float pi = (float) Math.PI;
            int i, j;
            var sizeofKernel = n;

            var kernel = new float[n, n];
            GaussianKernel = new int[n, n];

            var d1 = 1/(2*pi*sigma*sigma);
            var d2 = 2*sigma*sigma;

            float min = 1000;

            for (i = -sizeofKernel/2; i <= sizeofKernel/2; i++)
            {
                for (j = -sizeofKernel/2; j <= sizeofKernel/2; j++)
                {
                    kernel[sizeofKernel/2 + i, sizeofKernel/2 + j] = 1/d1*(float) Math.Exp(-(i*i + j*j)/d2);
                    if (kernel[sizeofKernel/2 + i, sizeofKernel/2 + j] < min)
                        min = kernel[sizeofKernel/2 + i, sizeofKernel/2 + j];
                }
            }

            var mult = (int) (1/min);
            var sum = 0;
            if ((min > 0) && (min < 1))
            {
                for (i = -sizeofKernel/2; i <= sizeofKernel/2; i++)
                {
                    for (j = -sizeofKernel/2; j <= sizeofKernel/2; j++)
                    {
                        kernel[sizeofKernel/2 + i, sizeofKernel/2 + j] =
                            (float) Math.Round(kernel[sizeofKernel/2 + i, sizeofKernel/2 + j]*mult, 0);
                        GaussianKernel[sizeofKernel/2 + i, sizeofKernel/2 + j] =
                            (int) kernel[sizeofKernel/2 + i, sizeofKernel/2 + j];
                        sum = sum + GaussianKernel[sizeofKernel/2 + i, sizeofKernel/2 + j];
                    }
                }
            }
            else
            {
                sum = 0;
                for (i = -sizeofKernel/2; i <= sizeofKernel/2; i++)
                {
                    for (j = -sizeofKernel/2; j <= sizeofKernel/2; j++)
                    {
                        kernel[sizeofKernel/2 + i, sizeofKernel/2 + j] =
                            (float) Math.Round(kernel[sizeofKernel/2 + i, sizeofKernel/2 + j], 0);
                        GaussianKernel[sizeofKernel/2 + i, sizeofKernel/2 + j] =
                            (int) kernel[sizeofKernel/2 + i, sizeofKernel/2 + j];
                        sum = sum + GaussianKernel[sizeofKernel/2 + i, sizeofKernel/2 + j];
                    }
                }
            }
            //Normalizing kernel Weight
            weight = sum;
        }

        private int[,] GaussianFilter(int[,] data)
        {
            int kernelWeight;
            const int kernelSize = 5;
            GenerateGaussianKernel(5, 1, out kernelWeight);

            int i;
            const int limit = kernelSize/2;

            var output = data;
            
            for (i = limit; i <= data.GetLength(0) - 1 - limit; i++)
            {
                int j;
                for (j = limit; j <= data.GetLength(1) - 1 - limit; j++)
                {
                    float sum = 0;
                    int k;
                    for (k = -limit; k <= limit; k++)
                    {
                        int l;
                        for (l = -limit; l <= limit; l++)
                        {
                            sum = sum + data[i + k, j + l]*GaussianKernel[limit + k, limit + l];
                        }
                    }
                    output[i, j] = (int) Math.Round(sum/kernelWeight);
                }
            }
            
            return output;
        }

        private static float[,] Differentiate(int[,] data, int[,] filter)
        {
            int i;

            var fw = filter.GetLength(0);
            var fh = filter.GetLength(1);
            var output = new float[data.GetLength(0), data.GetLength(1)];

            for (i = fw/2; i <= data.GetLength(0) - fw/2 - 1; i++)
            {
                int j;
                for (j = fh/2; j <= data.GetLength(1) - fh/2 - 1; j++)
                {
                    float sum = 0;
                    int k;
                    for (k = -fw/2; k <= fw/2; k++)
                    {
                        int l;
                        for (l = -fh/2; l <= fh/2; l++)
                        {
                            sum = sum + data[i + k, j + l]*filter[fw/2 + k, fh/2 + l];
                        }
                    }
                    output[i, j] = sum;
                }
            }

            return output;
        }

        private static float Neighbours(int angle, out Point n1, out Point n2)
        {
            if (angle >= 180)
            {
                angle -= 180;
            }

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

        private static Point AddPoint(Point a, Point b)
        {
            return new Point(a.X + b.X + 1, a.Y + b.Y + 1); // 1 do sztuczne przesuniecie 
        }

        private static Point SubtractPoint(Point a, Point b)
        {
            return new Point(a.X - b.X + 1, a.Y - b.Y + 1); // 1 do sztuczne przesuniecie 
        }
    }
}