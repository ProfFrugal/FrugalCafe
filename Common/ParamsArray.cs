using System;
using System.Collections.Generic;

namespace FrugalCafe.Common
{
    public struct ParamsArray<T>
    {
        private static readonly T[] oneArgArray = new T[1];
        private static readonly T[] twoArgArray = new T[2];
        private static readonly T[] threeArgArray = new T[3];

        private readonly T arg0;
        private readonly T arg1;
        private readonly T arg2;
        private readonly T[] args;

        public ParamsArray(T arg0)
        {
            this.arg0 = arg0;
            this.arg1 = default;
            this.arg2 = default;
            this.args = oneArgArray;
        }

        public ParamsArray(T arg0, T arg1)
        {
            this.arg0 = arg0;
            this.arg1 = arg1;
            this.arg2 = default;
            this.args = twoArgArray;
        }

        public ParamsArray(T arg0, T arg1, T arg2)
        {
            this.arg0 = arg0;
            this.arg1 = arg1;
            this.arg2 = arg2;
            this.args = threeArgArray;
        }

        public ParamsArray(T[] args)
        {
            int len = args.Length;
            this.arg0 = len > 0 ? args[0] : default;
            this.arg1 = len > 1 ? args[1] : default;
            this.arg2 = len > 2 ? args[2] : default;
            this.args = args;
        }

        public int Length => this.args.Length;

        public T this[int index] => index == 0 ? this.arg0 : GetAtSlow(index);

        private T GetAtSlow(int index)
        {
            if (index == 1)
                return this.arg1;

            if (index == 2)
                return this.arg2;

            return this.args[index];
        }
    }
}
