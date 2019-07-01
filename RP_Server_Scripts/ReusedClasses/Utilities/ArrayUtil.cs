using GUC.Utilities;

namespace RP_Server_Scripts.ReusedClasses.Utilities
{

    public class ArrayUtil : ExtendedObject
    {

        protected ArrayUtil () { }



        public static T[] Populate<T> (T[] arr, T value, bool createNew = false)
        {
            T[] a;
            if (createNew) { a = new T[arr.Length]; }
            else { a = arr; }
            for (int i = 0; i < a.Length;i++)
            {
                a[i] = value;
            }
            return a;
        }

    }

}
