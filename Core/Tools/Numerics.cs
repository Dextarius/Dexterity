using System;

namespace Core.Tools
{
    public static class Numerics
    {
        public const double DoubleEqualityTolerance = 0.0000000001;
        //- TODO : Consider designing a better method of determining tolerance, or 
        //         make it a property perhaps?  This value won't work for comparing
        //         very large numbers
        
        public static bool DoublesAreNotEqual(double firstNumber, double secondNumber) => 
            Math.Abs(firstNumber - secondNumber) > DoubleEqualityTolerance;
    }
}