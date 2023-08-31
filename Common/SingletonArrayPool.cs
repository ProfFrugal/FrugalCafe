using System;
using System.Threading;

namespace FrugalCafe.Common
{
    internal static class SingletonArrayPool<T>
    {
        private static T[] reusedBuffer;

        public static T[] Rent(int capacity)
        {
            var buffer = Interlocked.Exchange(ref SingletonArrayPool<T>.reusedBuffer, null);

            if ((buffer == null) || (buffer.Length < capacity))
            {
                buffer = new T[capacity];
            }

            return buffer;
        }

        public static void Release(T[] buffer)
        {
            SingletonArrayPool<T>.reusedBuffer = buffer;
        }
    }
}
