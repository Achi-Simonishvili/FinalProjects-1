using Calculator.Services;

namespace Calculator.Tests
{
    public class CalculatorServiceTests
    {
        private readonly CalculatorService _service = new CalculatorService();

        [Fact]
        public void Add_ReturnsSum()
        {
            double result = _service.Add(3.5, 2.5);
            Assert.Equal(6.0, result, 6);
        }

        [Fact]
        public void Subtract_ReturnsDifference()
        {
            double result = _service.Subtract(10, 4.25);
            Assert.Equal(5.75, result, 6);
        }

        [Fact]
        public void Multiply_ReturnsProduct()
        {
            double result = _service.Multiply(3, 2.5);
            Assert.Equal(7.5, result, 6);
        }

        [Fact]
        public void Divide_ReturnsQuotient()
        {
            double result = _service.Divide(10, 2);
            Assert.Equal(5, result, 6);
        }

        [Fact]
        public void Divide_ByZero_ThrowsDivideByZeroException()
        {
            Assert.Throws<DivideByZeroException>(() => _service.Divide(1, 0));
        }
    }
}