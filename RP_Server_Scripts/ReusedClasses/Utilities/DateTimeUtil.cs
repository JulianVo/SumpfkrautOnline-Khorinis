using System;
using System.Globalization;
using GUC.Utilities;

namespace RP_Server_Scripts.ReusedClasses.Utilities
{
    public class DateTimeUtil : ExtendedObject
    { 

        protected static string dateFormat = "yyyy-MM-dd HH:mm:ss";



        protected DateTimeUtil () { }
             


        /**
         *   Creates a standardized string-representation of the given DateTime.object using a format.
         *   It uses a standard-format by default ("yyyy-MM-dd HH:mm:ss"), however, a custom one can be 
         *   provided as well.
         *   @param format is a pattern-string according to the rules of System.DateTime-class
         *   @see System.Datetime
         */
        public static string DateTimeToString (System.DateTime dt, string format=null)
        {
            if (format == null)
            {
                format = dateFormat;
            }

            return dt.ToString(format);
        }

        public static bool TryDateTimeToString (DateTime dt, out string str, string format=null)
        {
            if (format == null)
            {
                format = dateFormat;
            }

            str = dt.ToString(format);

            if (str == null)
            {
                return false;
            }

            return true;
        }

        //public static DateTime StringToDateTime (string str)
        //{
        //    DateTime dt;

        //    if (DateTime.TryParseExact(str, dateFormat, CultureInfo.InvariantCulture,
        //        DateTimeStyles.None, out dt))
        //    {
        //        return dt;
        //    }

        //    return dt;
        //}

        public static bool TryStringToDateTime (string str, out DateTime dt)
        {
            if (DateTime.TryParseExact(str, dateFormat, CultureInfo.InvariantCulture,
                DateTimeStyles.None, out dt))
            {
                return true;
            }
            // else
            return false;
        }

    }
}
