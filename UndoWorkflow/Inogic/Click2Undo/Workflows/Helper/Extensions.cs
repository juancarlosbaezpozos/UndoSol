namespace Inogic.Click2Undo.Workflows.Helper
{
    public static class Extensions
    {
        public static T[] Slice<T>(this T[] source, int start, string value)
        {
            T[] array = null;
            var num = 0;
            num = source.Length;
            array = (value == "") ? new T[num] : new T[source.Length - start];
            if (value != "")
            {
                var num2 = 0;
                while (num2 < array.Length)
                {
                    array[num2] = source[start];
                    num2++;
                    start++;
                }
            }
            else
            {
                for (var i = 0; i < num; i++)
                {
                    array[i] = source[start];
                }
            }

            return array;
        }

        public static T[] Slice<T>(this T[] source, int start, int end)
        {
            if (end < 0)
            {
                end = source.Length + end;
            }

            var num = 0;
            num = end - start;
            var array = new T[num];
            for (var i = 0; i < num; i++)
            {
                array[i] = source[i + start];
            }

            return array;
        }

        public static string SliceString(this string source, int start, int end)
        {
            if (end < 0)
            {
                end = source.Length + end;
            }

            var length = end - start;
            return source.Substring(start, length);
        }
    }
}