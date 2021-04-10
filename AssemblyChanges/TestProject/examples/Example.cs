namespace TestProject.examples
{
    public class Example
    {
        public static int TestInt(int a, int b)
        {
            return a + b;
        }

        public static double TestDouble(double a, double b)
        {
            return a + b;
        }

        public static decimal TestDecimal(decimal a, decimal b)
        {
            return a + b;
        }

        public class Nested
        {
            public static int TestNestedInt(int a, int b)
            {
                return a + b;
            }

            public static double TestNestedDouble(double a, double b)
            {
                return a + b;
            }

            public static decimal TestNestedDecimal(decimal a, decimal b)
            {
                return a + b;
            }
        }
    }
}