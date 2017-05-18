namespace ImageProcessing.Data
{
    using System.Drawing;

    using Model;

    internal static class ImageManager
    {
        #region Public Methods

        public static Bitmap GetBitmapFromOutputData(OutputData output, AlgorithmType type, string key, int width, int height)
        {
            if (output.Data.ContainsKey(type) && output.Data[type].Images.ContainsKey(key) && output.Data[type].Images[key] != null)
            {
                return output.Data[type].Images[key];
            }

            return GetEmptyWhiteBitmap(width, height);
        }

        #endregion

        #region Private Methods

        private static Bitmap GetEmptyWhiteBitmap(int x, int y)
        {
            var bmp = new Bitmap(x, y);

            using (var g = Graphics.FromImage(bmp))
            {
                g.FillRectangle(Brushes.White, new Rectangle(0, 0, x, y));
            }

            return bmp;
        }

        #endregion
    }
}
