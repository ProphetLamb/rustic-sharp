using System;
using System.Collections.Generic;
using System.Text;

using HeaplessUtility.IO;
using HeaplessUtility.Text;

namespace HeaplessUtility.Extensions
{
    public static class SplitIterExtensions
    {
        public static string[] ToArray(in this SplitIter<char> self)
        {
            using PoolBufWriter<string> buf = new();
            var en = self.GetEnumerator();
            while (en.MoveNext())
            {
                buf.Add(en.Current.ToString());
            }
            return buf.ToArray();
        }

        public static O[] ToArray<T, O>(in this SplitIter<T> self, Func<O, T, O> aggregate)
            where O : new()
        {
            using PoolBufWriter<O> buf = new();
            foreach (var seg in self)
            {
                O cur = new();
                foreach (var itm in seg)
                {
                    cur = aggregate(cur, itm);
                }
            }
            return buf.ToArray();
        }

        public static O[] ToArray<T, O>(in this SplitIter<T> self, Func<O, T, O> aggregate, Func<O> seed)
        {
            using PoolBufWriter<O> buf = new();
            foreach (var seg in self)
            {
                O cur = seed();
                foreach (var itm in seg)
                {
                    cur = aggregate(cur, itm);
                }
            }
            return buf.ToArray();
        }
    }
}
