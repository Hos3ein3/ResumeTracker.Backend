using System.Globalization;

using Moq;

using ResumeTracker.Application.Services;

using Xunit.Abstractions;

namespace ResumeTracker.UnitTests.Learning;

public class MathCalculatorTest
{

    public MathCalculatorTest(ITestOutputHelper testOutputHelper)
    {
        
    }

    [Fact]
    public void Sum_ValidInputs_ReturnsCorrectResult()
    {
        //Arrange
        MathCalculator calculator = new();

        //Act
        var result= calculator.Sum(10, 10);

        //Assert
        Assert.Equal(20,result);
    }
    
    [Fact]
    public void Divide_ZeroDenominator_DivideByZeroException()
    {
        //Arrange
        MathCalculator calculator = new();
        
        //Act + Assert
        
        Assert.Throws<DivideByZeroException>(() => calculator.Divide(10, 0));
    }
    
    [Fact]
    public void Divide_ZeroNumerator_ReturnZero()
    {
        //Arrange
        
        MathCalculator calculator = new();
        //Act 
        var result = calculator.Divide(0, 10);
        
        //Assert
        Assert.Equal(0,result);
    }

    // [Fact]
    // public void Divide_FloatZeroOnFloatZero_ReturnNaN()
    // {
    //     MathCalculator calculator = new();
    //     
    //     var result = calculator.Divide(0.0, 0.0);
    //     
    //     Assert.True(double.IsNaN(result));
    // }
}