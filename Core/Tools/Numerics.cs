using System;

namespace Core.Tools
{
    public static class Numerics
    {
        public const double DoubleEqualityTolerance = 0.0000000001;
        //- TODO : Decide on a better method of determining tolerance.
        //         This value won't work for comparing very large numbers
        
        public static bool DoublesAreNotEqual(double firstNumber, double secondNumber) => 
            Math.Abs(firstNumber - secondNumber) > DoubleEqualityTolerance;
    }
}