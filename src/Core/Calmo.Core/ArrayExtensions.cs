using System.Linq;

namespace System
{
    public static class ArrayExtensions
    {
        public static T[] RemoveAt<T>(this T[] array, int index)
        {
            var tempList = array.ToList();
            tempList.RemoveAt(index);

            return tempList.ToArray();
        }
    }
}
