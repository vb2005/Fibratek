using OpenCvSharp;

namespace Fibratek.Calculation
{
    public static class Calc
    {
        public static int GetMaxLine(Mat img, int minValue)
        {
            img = img.Reduce(ReduceDimension.Row, ReduceTypes.Avg, MatType.CV_32FC1);

            img.MinMaxLoc(out double min, out double max, out OpenCvSharp.Point p_min, out OpenCvSharp.Point p_max);
            
            // Если картинка по яркости ниже порога
            if (max < minValue) 
                return -1;

            // Иначе возврат координаты
            return p_max.X;
        }

        public static double GetTrueValue(int pixValue)
        {
            if (pixValue < 0) return -1;
            // TODO: тут должна быть формула для приведения значения в пикслеях в рельный результат
            return pixValue * 2;
        }
    }
}
