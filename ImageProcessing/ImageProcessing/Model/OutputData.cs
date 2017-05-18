namespace ImageProcessing.Model
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Globalization;

    internal class OutputData
    {
        public Dictionary<string, Bitmap> Images { get; set; }

        public Dictionary<AlgorithmType, AlgorithmOutputData> Data;

        public OutputData(Dictionary<string, Bitmap> images)
        {
            Images = images;
            Data = new Dictionary<AlgorithmType, AlgorithmOutputData>
            {
                {AlgorithmType.Canny, new AlgorithmOutputData()},
                {AlgorithmType.LaplaceOperator, new AlgorithmOutputData()},
                {AlgorithmType.RobertsCross, new AlgorithmOutputData()},
                {AlgorithmType.SobelOperator, new AlgorithmOutputData()}
            };
        }

        public string GetOutputLog()
        {
            var log = "";

            foreach (var image in Images)
            {
                log += $"Name: {image.Key}, Size:  {image.Value.Width}x{image.Value.Height}, Percentage of edge pixels / Execution time:" + Environment.NewLine;
                log += $"- Canny            : {GetPercentageEdgePixelsByKey(Data[AlgorithmType.Canny].EdgePixelsPercentage, image.Key)} / " +
                       $"{GetExecutionTimeByKey(Data[AlgorithmType.Canny].ExecutionTime, image.Key)}" + Environment.NewLine;
                log += $"- Laplace Operator : {GetPercentageEdgePixelsByKey(Data[AlgorithmType.LaplaceOperator].EdgePixelsPercentage, image.Key)} / " +
                       $"{GetExecutionTimeByKey(Data[AlgorithmType.LaplaceOperator].ExecutionTime, image.Key)}" + Environment.NewLine;
                log += $"- Roberts Cross    : {GetPercentageEdgePixelsByKey(Data[AlgorithmType.RobertsCross].EdgePixelsPercentage, image.Key)} / " +
                       $"{GetExecutionTimeByKey(Data[AlgorithmType.RobertsCross].ExecutionTime, image.Key)}" + Environment.NewLine;
                log += $"- Sobel Operator   : {GetPercentageEdgePixelsByKey(Data[AlgorithmType.SobelOperator].EdgePixelsPercentage, image.Key)} / " +
                       $"{GetExecutionTimeByKey(Data[AlgorithmType.SobelOperator].ExecutionTime, image.Key)}" + Environment.NewLine;
                log += Environment.NewLine;
            }

            log += "Avarage execution time per algorithm:" + Environment.NewLine;
            log += $"- Canny            : {Data[AlgorithmType.Canny].ExecutionTime.Values.Average()}ms" + Environment.NewLine;
            log += $"- Laplace Operator : {Data[AlgorithmType.LaplaceOperator].ExecutionTime.Values.Average()}ms" + Environment.NewLine;
            log += $"- Roberts Cross    : {Data[AlgorithmType.RobertsCross].ExecutionTime.Values.Average()}ms" + Environment.NewLine;
            log += $"- Sobel Operator   : {Data[AlgorithmType.SobelOperator].ExecutionTime.Values.Average()}ms" + Environment.NewLine;
            
            return log;
        }

        private static string GetExecutionTimeByKey(IReadOnlyDictionary<string, long> dict, string key)
        {
            return dict.ContainsKey(key) ? dict[key].ToString(CultureInfo.InvariantCulture) + "ms" : "-";
        }

        private static string GetPercentageEdgePixelsByKey(IReadOnlyDictionary<string, double> dict, string key)
        {
            return dict.ContainsKey(key) ? dict[key].ToString("P") : "-";
        }
    }
}
