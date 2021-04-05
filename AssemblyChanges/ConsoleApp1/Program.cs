using System;

namespace ConsoleApp1
{
    class Program
    {
        private static int x = 1;

        static void Main(string[] args)
        {
            TestInt();
            TestDouble();
            TestDecimal();

            Nested.NestedTestInt();
            Nested.NestedTestDouble();
            Nested.NestedTestDecimal();
        }

        private static void TestInt()
        {
            var a = 2;
            var b = x * 2;

            Console.WriteLine(a + b);
        }

        private static void TestDouble()
        {
            var a = 5.0;
            double b = x * 3;

            Console.WriteLine(a + b);
        }

        private static void TestDecimal()
        {
            var a = 30m;
            decimal b = x * 4;

            Console.WriteLine(a + b);
        }

        private class Nested
        {
            public static void NestedTestInt()
            {
                var a = 2;
                var b = x * 2;

                Console.WriteLine(a + b);
            }

            public static void NestedTestDouble()
            {
                var a = 5.0;
                double b = x * 3;

                Console.WriteLine(a + b);
            }

            public static void NestedTestDecimal()
            {
                var a = 30m;
                decimal b = x * 4;

                Console.WriteLine(a + b);
            }
        }
    }
}