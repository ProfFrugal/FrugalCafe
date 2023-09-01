using System;
using System.Threading;

namespace FrugalCafe
{
    public static class SingletonArrayPool<T>
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

        public static void Release(T[] buffer, int count = -1)
        {
            for (int i = 0; i < count; i++)
            {
                buffer[i] = default(T);
            }

            SingletonArrayPool<T>.reusedBuffer = buffer;
        }
    }
}
