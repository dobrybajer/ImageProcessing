using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessing.Data
{
    internal static class FileManager
    {
        public static void ReadInput()
        {
            var input = ConfigurationManager.AppSettings["InputFolder"];
        }

        public static void WriteOutput()
        {
            var output = ConfigurationManager.AppSettings["OutputFolder"];
        }
    }
}
