using System;
using System.Collections;
using System.Collections.Generic;

namespace NeonBlack.Utilities.ObjectPool
{
    public class ObjectPool<T> : IEnumerable<T>, IDisposable where T : class
    {
        private readonly int maxSize;
        private readonly Stack<T> objects = new();

        public ObjectPool(int maxSize)
        {
            this.maxSize = maxSize;
        }

        public void Dispose()
        {
            objects.Clear();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return objects.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public GetResult Get(out T obj)
        {
            if (objects.Count > 0)
            {
                obj = objects.Pop();
                return GetResult.Found;
            }

            obj = null;
            return GetResult.Missing;
        }

        public ReturnResult Return(T obj)
        {
            if (objects.Count >= maxSize)
            {
                return ReturnResult.Discarded;
            }

            objects.Push(obj);
            return ReturnResult.Returned;
        }
    }
}
