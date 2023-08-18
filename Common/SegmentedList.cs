using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace FrugalCafe
{
    public class SegmentedList<T> : IEnumerable<T>, IReadOnlyList<T>
    { 
        private readonly int _segmentShift;
        private readonly int _segmentMask;

        private T[][] _segments;
        private T[] _current;

        private int _count;

        public SegmentedList(int segmentSize = -1)
        {
            if (segmentSize < 0)
            {
                segmentSize = 1024;
            }

            _segmentShift = 4;

            while ((1 << _segmentShift) < segmentSize)
            {
                _segmentShift++;
            }

            _segmentMask = (1 << _segmentShift) - 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(T item)
        {
            int offset = _count & _segmentMask;

            if (offset == 0)
            {
                this.EnsureSegment();
            }

            _current[offset] = item;
            _count++;
        }

        public T this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (((uint)index) >= _count)
                {
                    ThrowHelper.ThrowIndexOutOfRangeException();
                }

                return _segments[index >> _segmentShift][index & _segmentMask];
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                if (((uint)index) >= _count)
                {
                    ThrowHelper.ThrowIndexOutOfRangeException();
                }

                _segments[index >> _segmentShift][index & _segmentMask] = value;
            }
        }

        public int Count => _count;

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return new Enumerator(this);
        }

        private void EnsureSegment()
        {
            int segment = _count >> _segmentShift;

            if (_segments == null)
            {
                _segments = new T[1][];
            }
            else if (segment == _segments.Length)
            {
                Array.Resize(ref _segments, _segments.Length * 2);
            }

            _current = _segments[segment];

            if (_current == null)
            {
                _current = new T[_segmentMask + 1];
                _segments[segment] = _current;
            }
        }

        public struct Enumerator : IEnumerator<T>, IDisposable, IEnumerator
        {
            private readonly SegmentedList<T> _list;

            private int _index;

            private T _current;

            public T Current => _current;

            object IEnumerator.Current
            {
                get
                {
                    if (_index == 0 || _index == _list._count + 1)
                    {
                        ThrowHelper.ThrowInvalidOperationException_InvalidOperation_EnumOpCantHappen();
                    }

                    return _current;
                }
            }

            internal Enumerator(SegmentedList<T> list)
            {
                _list = list;
                _index = 0;
                _current = default(T);
            }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                SegmentedList<T> list = _list;

                if ((uint)_index < (uint)list._count)
                {
                    _current = list._segments[_index >> list._segmentShift][_index & list._segmentMask];
                    _index++;
                    
                    return true;
                }

                _index = _list._count + 1;
                _current = default(T);

                return false;
            }

            void IEnumerator.Reset()
            {
                _index = 0;
                _current = default(T);
            }
        }
    }

}
