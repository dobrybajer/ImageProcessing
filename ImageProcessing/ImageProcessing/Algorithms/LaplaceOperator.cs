namespace ImageProcessing.Algorithms
{
    using System.Drawing;

    using Model;

    internal class LaplaceOperator : BaseAlgorithm
    {
        #region Constructors 

        public LaplaceOperator(LaplaceFilterType type = LaplaceFilterType.Lapl1) : base(AlgorithmType.LaplaceOperator)
        {
            Kernel1 = LaplaceFilter.GetLaplaceFilter(type);
        }

        #endregion

        #region Public Methods

        public override Bitmap ProcessImage(Bitmap image)
        {
            return ProcessImageGrayScale(image);
        }

        #endregion
    }
}
