namespace ImageProcessing.Data
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.IO;
    using System.Linq;

    using Model;

    internal static class FileManager
    {
        private const string SolutionName = "ImageProcessing";
        private static readonly string[] ValidImageExtension = { "jpg", "jpeg", "png", "gif", "bmp" };

        public static InputData ReadInput()
        {
            var inputData = new InputData();
            try
            {
                var solutionName = Path.GetFileName(Process.GetCurrentProcess().MainModule.FileName).Split('.').FirstOrDefault() ?? SolutionName;
                var inputFolderName = ConfigurationManager.AppSettings["InputFolderName"];
                var rootFolder = AppDomain.CurrentDomain.BaseDirectory.Split(new[] { solutionName }, StringSplitOptions.None);
                var inputFolderPath = Directory.GetDirectories(rootFolder[0], inputFolderName, SearchOption.AllDirectories).First();

                var files = GetFilteredFiles(inputFolderPath);
                foreach (var image in files)
                {
                    inputData.Images.Add(GetFileNameFromPath(image), new Bitmap(image));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error occured while reading input data: {e.Message}");
            }

            return inputData;
        }

        public static void WriteOutput(OutputData output)
        {
            try
            {
                var solutionName = Path.GetFileName(Process.GetCurrentProcess().MainModule.FileName).Split('.').FirstOrDefault() ?? SolutionName;
                var outputFolderName = ConfigurationManager.AppSettings["OutputFolderName"];
                var rootFolder = AppDomain.CurrentDomain.BaseDirectory.Split(new[] { solutionName }, StringSplitOptions.None);
                var outputFolderPath = Directory.GetDirectories(rootFolder[0] + $"{solutionName}\\", outputFolderName, SearchOption.AllDirectories).First();

                var log = output.GetOutputLog();
                var logFilePath = outputFolderPath + $"\\{DateTime.Now.ToString("yyyy.MM.dd_HH.mm.ss")}_log.txt";
                File.WriteAllText(logFilePath, log);
                Console.WriteLine(log);

                Console.WriteLine("\rSaving processed images... please wait...");
                foreach (var image in output.Images)
                {
                    var width = image.Value.Width;
                    var height = image.Value.Height;

                    var finalImage = new Bitmap(width*2, height*2);
               
                    using (var g = Graphics.FromImage(finalImage))
                    {
                        var cannyImage = ImageManager.GetBitmapFromOutputData(output, AlgorithmType.Canny, image.Key, width, height);
                        var laplaceImage = ImageManager.GetBitmapFromOutputData(output, AlgorithmType.LaplaceOperator, image.Key, width, height);
                        var robertsImage = ImageManager.GetBitmapFromOutputData(output, AlgorithmType.RobertsCross, image.Key, width, height);
                        var sobelImage = ImageManager.GetBitmapFromOutputData(output, AlgorithmType.SobelOperator, image.Key, width, height);

                        g.DrawImage(cannyImage, 0, 0);
                        g.DrawImage(laplaceImage, width, 0);
                        g.DrawImage(robertsImage, 0, height);
                        g.DrawImage(sobelImage, width, height);

                        var rectLu = new Rectangle(0, 0, width / 4, height / 8);
                        var rectRu = new Rectangle(width, 0, width / 4, height / 8);
                        var rectLd = new Rectangle(0, height, width / 4, height / 8);
                        var rectRd = new Rectangle(width, height, width / 4, height / 8);

                        g.SmoothingMode = SmoothingMode.AntiAlias;
                        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                        var sf = new StringFormat
                        {
                            Alignment = StringAlignment.Center,
                            LineAlignment = StringAlignment.Center
                        };

                        g.DrawString("Canny", new Font("Tahoma", 24, FontStyle.Bold), Brushes.Red, rectLu, sf);
                        g.DrawString("Laplace Operator", new Font("Tahoma", 24, FontStyle.Bold), Brushes.Red, rectRu, sf);
                        g.DrawString("Roberts Cross", new Font("Tahoma", 24, FontStyle.Bold), Brushes.Red, rectLd, sf);
                        g.DrawString("Sobel Operator", new Font("Tahoma", 24, FontStyle.Bold), Brushes.Red, rectRd, sf);
                    }

                    finalImage.Save(outputFolderPath + $"\\processed_{image.Key}");
                }
                Console.WriteLine("\rAll processed images saved properly.");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error occured while writing output data: {e.Message}");
            }
        }

        private static string GetFileNameFromPath(string path)
        {
            return path.Split('\\').Last();
        }

        private static IEnumerable<string> GetFilteredFiles(string path)
        {
            return Directory.EnumerateFiles(path, "*.*").Where(file => ValidImageExtension.Any(x => file.EndsWith(x, StringComparison.OrdinalIgnoreCase)));
        }
    }
}
