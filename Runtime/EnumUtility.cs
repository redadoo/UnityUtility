using System;

namespace Utility
{
    public static class EnumUtility
    {
        /// <summary>
        /// Returns the next value of an enum, clamped to the last value if already at the end.
        /// </summary>
        public static T GetNextEnum<T>(T value) where T : struct, Enum
        {
            T[] values = (T[])Enum.GetValues(typeof(T));
            int index = Array.IndexOf(values, value);

            int nextIndex = Math.Min(index + 1, values.Length - 1);
            return values[nextIndex];
        }

        /// <summary>
        /// Returns the next value of an enum, looping back to the first if at the end.
        /// </summary>
        public static T GetNextEnumLooped<T>(T value) where T : struct, Enum
        {
            T[] values = (T[])Enum.GetValues(typeof(T));
            int index = Array.IndexOf(values, value);

            int nextIndex = (index + 1) % values.Length;
            return values[nextIndex];
        }

        /// <summary>
        /// Returns true if the provided enum value is the last one.
        /// </summary>
        public static bool IsLastEnum<T>(T value) where T : struct, Enum
        {
            T[] values = (T[])Enum.GetValues(typeof(T));
            int index = Array.IndexOf(values, value);
            return index == values.Length - 1;
        }
    }
}
