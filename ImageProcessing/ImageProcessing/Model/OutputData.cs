namespace ImageProcessing.Model
{
    using System;
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
                log += $"Name: {image.Key}, Size:  {image.Value.Width}x{image.Value.Height}, Execution time:" + Environment.NewLine;
                log += $"- Canny            : {GetExecutionTimeByKey(Data[AlgorithmType.Canny].ExecutionTime, image.Key)}" + Environment.NewLine;
                log += $"- Laplace Operator : {GetExecutionTimeByKey(Data[AlgorithmType.LaplaceOperator].ExecutionTime, image.Key)}" + Environment.NewLine;
                log += $"- Roberts Cross    : {GetExecutionTimeByKey(Data[AlgorithmType.RobertsCross].ExecutionTime, image.Key)}" + Environment.NewLine;
                log += $"- Sobel Operator   : {GetExecutionTimeByKey(Data[AlgorithmType.SobelOperator].ExecutionTime, image.Key)}" + Environment.NewLine;
                log += Environment.NewLine;
            }

            // TODO add avg time for every processed image

            return log;
        }

        private static string GetExecutionTimeByKey(IReadOnlyDictionary<string, long> dict, string key)
        {
            return dict.ContainsKey(key) ? dict[key].ToString(CultureInfo.InvariantCulture) + "ms" : "-";
        }
    }
}
