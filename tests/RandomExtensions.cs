using System;

namespace HeaplessUtility.Tests
{
    public static class RandomExtensions
    {
        public static ref T ChooseFrom<T>(this Random random, T[] collection)
        {
            return ref collection[random.Next(0, collection.Length)];
        }
    }
}