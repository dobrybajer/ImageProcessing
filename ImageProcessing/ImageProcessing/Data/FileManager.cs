namespace ImageProcessing.Data
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics;
    using System.Drawing;
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

                foreach (var image in output.Images)
                {
                    foreach (var processedImages in output.Data)
                    {
                        var values = processedImages.Value;
                        var processedImage = values.Images[image.Key];
                        processedImage.Save(outputFolderPath + $"\\{processedImages.Key}_{image.Key}");
                    }
                }
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
