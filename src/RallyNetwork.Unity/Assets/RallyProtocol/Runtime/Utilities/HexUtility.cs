using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace RallyProtocol.Utilities
{

    public static class HexUtility
    {

        public static string HexZeroPad(this string value, int length)
        {
            while (value.Length < 2 * length + 2)
            {
                value = "0x0" + value.Substring(2);
            }

            return value;
        }

    }

}