using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
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
            while (i < target.Length && (Math.Abs(target.Length - i) >= substring.Length))
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

        public static int GetIndexOf(this StringBuilder target, string substring , int startIdx)
        {
            if (string.IsNullOrEmpty(substring) || target.Length < substring.Length || startIdx < 0 || startIdx >= target.Length)
            {
                return -1;
            }
            
            for (int i = startIdx; i <= target.Length - substring.Length; i++)
            {
                int index = i;
                bool compare = true;
                for(int subIndex = 0; subIndex < substring.Length; subIndex++)
                {
                    if(target[i + subIndex] != substring[subIndex])
                    {
                        compare = false;
                        break;
                    }
                }

                if(compare)
                {
                    return index;
                }
            }

            return -1;
        }
    }
}