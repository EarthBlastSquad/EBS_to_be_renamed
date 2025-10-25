using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    public static class StringUtils
    {
        public static int GetSubstringCount(this string target, string substring)
        {
            if (string.IsNullOrEmpty(substring) || target.Length < substring.Length)
            {
                return 0;
            }

            int cnt = 0;

            int i = 0;
            while(i < target.Length && (Math.Abs(target.Length - i) >= substring.Length))
            {
                if (string.Compare(target, i, substring, 0, substring.Length, System.StringComparison.Ordinal) == 0)
                {
                    i += substring.Length;
                    cnt++;
                }
                else
                {
                    i++;
                }
            }

            return cnt;
        }
    }
}