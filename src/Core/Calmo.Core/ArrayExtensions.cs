using System.Linq;

namespace System
{
    public static class ArrayExtensions
    {
		/// <summary>
		/// Creates a new array without the removed item
		/// </summary>
		/// <typeparam name="T">Array type</typeparam>
		/// <param name="array">Array</param>
		/// <param name="index">Position to remove</param>
		/// <returns></returns>
        public static T[] RemoveAt<T>(this T[] array, int index)
        {
            var tempList = array.ToList();
            tempList.RemoveAt(index);

            return tempList.ToArray();
        }
    }
}
