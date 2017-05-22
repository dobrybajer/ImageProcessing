namespace ImageProcessing.Model
{
    using System;

    internal enum LaplaceFilterType
    {
        Lapl1,
        Lapl2,
        Lapl3,
        LaplSkew,
        LaplHorizontal,
        LaplVertical
    }

    internal static class LaplaceFilter
    {
        public static double[,] GetLaplaceFilter(LaplaceFilterType type)
        {
            switch (type)
            {
                case LaplaceFilterType.Lapl1:
                    return new double[,]
                    {
                        {0, -1, 0},
                        {-1, 4, -1},
                        {0, -1, 0}
                    };
                case LaplaceFilterType.Lapl2:
                    return new double[,]
                    {
                        {-1, -1, -1},
                        {-1, 8, -1},
                        {-1, -1, -1}
                    };
                case LaplaceFilterType.Lapl3:
                    return new double[,]
                    {
                        {1, -2, 1},
                        {-2, 4, -2},
                        {1, -2, 1}
                    };
                case LaplaceFilterType.LaplSkew:
                    return new double[,]
                    {
                        {-1, 0, -1},
                        {0, 4, 0},
                        {-1, 0, -1}
                    };
                case LaplaceFilterType.LaplHorizontal:
                    return new double[,]
                    {
                        {0, -1, 0},
                        {0, 2, 0},
                        {0, -1, 0}
                    };
                case LaplaceFilterType.LaplVertical:
                    return new double[,]
                    {
                        {0, 0, 0},
                        {-1, 2, -1},
                        {0, 0, 0}
                    };
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}
