using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace base64
{
    class Program
    {
        public static void Main(string[] args)
        {
            var encoded = ToBase64("http://www.github.com/frodehus");
            if (encoded != "aHR0cDovL3d3dy5naXRodWIuY29tL2Zyb2RlaHVz")
                throw new Exception("Maximum failure");

            Console.WriteLine("Great success!");
        }

        private static string ToBase64(string s)
        {
            //convert character into binary and pad into octet (8-bits) and string it all together
            //divide bit-string into 24-bit groups
            //divide each group into sextets - if last group has less than 6 bytes, fill with zero
            //convert each sextet into octet - prepend with 00
            //convert to integer and look up base64 table
            var base64LookupTable = GenerateBase64LookupTable();

            var bits = s.Select(c => (int)c)
                        .Aggregate(string.Empty, (current, next) => current + Convert.ToString(next, 2).PadLeft(8, '0'));

            return bits
                .Select(
                    (_, i) => i)
                        .Where(i => i % 24 == 0)
                        .Select(i => bits.Substring(i, bits.Length - i >= 24 ? 24 : bits.Length - i)
                        .Aggregate(string.Empty, (current, next) => current + Convert.ToString(next))
                )
                .SelectMany(group => group.Select(
                    (_, i) => i)
                        .Where(i => i % 6 == 0)
                        .Select(i => group.Substring(i, group.Length - i >= 6 ? 6 : group.Length - i)
                        .Aggregate(string.Empty, (current, next) => current + Convert.ToString(next)))
                    )
                .Select(sextet => Convert.ToInt32(sextet.PadRight(6, '0'), 2))
                .Select(index => base64LookupTable[index])
                .Aggregate(string.Empty, (current, next) => current + next);
        }

        /// <summary>Generate array with valid Base64 values (A-Z, a-z, 0-9 and +/)</summary>
        private static string[] GenerateBase64LookupTable()
        {
            //generate array with A-Z, a-z, 0-9 and +/
            var uppercase = Enumerable.Range(65, 26).Select(c => Convert.ToString((char)c));
            var lowercase = Enumerable.Range(97, 26).Select(c => Convert.ToString((char)c));
            var numbers = Enumerable.Range(48, 10).Select(c => Convert.ToString((char)c));
            var special = new[] { "+", "/" };
            return uppercase.Concat(lowercase).Concat(numbers).Concat(special).ToArray();
        }
    }
}
