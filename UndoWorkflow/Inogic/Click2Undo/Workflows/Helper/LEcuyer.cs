using System;

namespace Inogic.Click2Undo.Workflows.Helper
{
    internal class LEcuyer
    {
        private int p;

        public long[] shuffle { get; set; }

        public long gen1 { get; set; }

        public long gen2 { get; set; }

        public long state { get; set; }

        public LEcuyer(long s)
        {
            shuffle = new long[32];
            gen1 = (gen2 = s & Convert.ToInt64(int.MaxValue));
            for (var i = 0; i < 19; i++)
            {
                gen1 = uGen(gen1, 40014, 53668, 12211, 2147483563);
            }

            for (var j = 0; j < 32; j++)
            {
                gen1 = uGen(gen1, 40014, 53668, 12211, 2147483563);
                shuffle[31 - j] = gen1;
            }

            state = Convert.ToInt64(shuffle[0]);
        }

        private long uGen(long old, int a, int q, int r, int m)
        {
            var num = old / q;
            num = a * (old - num * q) - num * r;
            return (num < 0) ? (num + m) : num;
        }

        public long next()
        {
            gen1 = uGen(gen1, 40014, 53668, 12211, 2147483563);
            gen2 = uGen(gen2, 40692, 52774, 3791, 2147483399);
            var num = state / 67108862;
            state = Convert.ToInt32((shuffle[num] + gen2) % 2147483563);
            shuffle[num] = gen1;
            return Convert.ToInt32(state);
        }

        public long nextInt(int n)
        {
            var num = 1;
            while (n >= num)
            {
                num <<= 1;
            }

            num--;
            long num2;
            do
            {
                num2 = next() & num;
            } while (num2 > n);

            return num2;
        }
    }
}