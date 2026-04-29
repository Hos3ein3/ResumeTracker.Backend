namespace ResumeTracker.Application.Services;

public class MathCalculator
{
    public  double Sum(double a, double b)
    =>  a + b;
    
    
    public  double Minus( double a, double b)
    =>  a - b;
    
    
    public  double Divide( double a, double b)
    =>  b == 0 ? throw new DivideByZeroException() :a / b;

    public  double Multiply(double a, double b)
        => a * b;

}