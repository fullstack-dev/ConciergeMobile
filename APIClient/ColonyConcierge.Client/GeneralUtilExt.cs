using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.Client
{
    public static class GeneralUtilExt
    {
        public static string ToIDListString(this IEnumerable<int> listOfIDs)
        {
            return string.Join(",", listOfIDs.Select(i => i.ToString()));
        }


    }


    public abstract class EnumWrapper<T> where T : class
    {
        /// <summary>
        /// Enumerates over the bit-field defined flags of an enumerated value.
        /// </summary>
        /// <typeparam name="TE"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IEnumerable<TE> FlagValues<TE>(TE value)  where TE : struct, T
        {
            var values = Enum.GetValues(typeof(TE)).OfType<object>().ToArray();

            int int_value = (int)((object)value);
            foreach(var v in values)
            {
                int int_v = (int)v;
                
                if ((int_v & int_value) == int_v )
                {
                    yield return (TE)v;
                }

            }

        }
    }

    public class EnumUtil : EnumWrapper<System.Enum>
    {

    }
}
