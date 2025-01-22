using System;

namespace Party.Utilities
{
    public static class Utility
    {
        #region Random
        private static readonly Random _random = new();

        public static int RandomInt(int minValue, int maxValue)
        {
            return _random.Next(minValue, maxValue);
        }
        public static int RandomInt(int maxValue)
        {
            return _random.Next(0, maxValue);
        }
    
        public static float RandomFloat()
        {
            return (float)_random.NextDouble();
        }
        public static float RandomFloat(float minValue, float maxValue)
        {
            return (float)_random.NextDouble() * (maxValue - minValue) + minValue;
        }

        public static double RandomDouble()
        {
            return _random.NextDouble();
        }
        public static double RandomDouble(double minValue, double maxValue)
        {
            return _random.NextDouble() * (maxValue - minValue) + minValue;
        }
        
        #endregion
        
        public static float Remap (float value, float inputFrom, float inputTo, float outputFrom, float outputTo) {
            return (value - inputFrom) / (inputTo - inputFrom) * (outputTo - outputFrom) + outputFrom;
        }
    }
}