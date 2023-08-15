using System;

namespace FrugalCafe
{
    public static class HashHelpers
    {
        public static int Combine(this int h1, int h2)
        {
            uint num = (uint)(h1 << 5) | ((uint)h1 >> 27);
            return ((int)num + h1) ^ h2;
        }
    }
}
