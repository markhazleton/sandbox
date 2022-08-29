namespace FuncsExamples;

public class SimpleFuncsInputParameters
{
    public static void Runner()
    {
        SomeMath((a, b) => a + b);
        SomeMath((x, y) =>
        {
            int temp = x * y; // you can make your lambdas multi-line
            int calculatedNumber = temp * temp;
            return calculatedNumber;
        });
        Func<int, int, int> myFuncAsLambda = (a, b) => a * b;
        SomeMath(myFuncAsLambda);
        SomeMath(Subtraction);
        Func<int, int, int> myFuncAsMethod = Subtraction;
        SomeMath(myFuncAsMethod);
    }

    public static void SomeMath(Func<int, int, int> mathFunction)
    {
        int result = mathFunction(7, 5);
        Console.WriteLine($"The mathFunction returned {result}");
    }

    public static int Subtraction(int number1, int number2)
    {
        return number1 - number2;
    }
}