using System.Collections.Generic;
using GUC.Utilities;

namespace RP_Server_Scripts.ReusedClasses.Utilities
{

    public class ListUtil : ExtendedObject
    {

        protected ListUtil () { }



        public static List<T> Populate<T> (List<T> list, T value, bool createNew = false)
        {
            if (createNew)
            {
                var newList = new List<T>(list.Capacity);
                for (int i = 0; i < list.Count; i++)
                {
                    newList.Add(value);
                }
                return newList;
            }
            else
            {
                for (int i = 0; i < list.Count; i++)
                {
                    list[i] = value;
                }
                return list;
            }
        }

    }

}
