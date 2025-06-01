using System.Data;
using FluentAssertions;

namespace Template.Business.UnitTests;

public class OrderService
{
    public decimal CalculateTotal(decimal price, int quantity, decimal discountPercent)
    {
        var total = price * quantity;
        var discount = total * discountPercent / 100;
        return total - discount;
    }
}


[TestClass]
public class OrderServiceUnitTests
{
    [TestMethod]
    public void CalculateTotal_ShouldApplyDiscountCorrectly()
    {
        // Arrange
        var service = new OrderService();

        // Act
        var result = service.CalculateTotal(price: 100, quantity: 2, discountPercent: 10);

        // Assert
        Assert.AreEqual(180, result); // 200 - 10% = 180
    }

    [TestMethod]
    [DataRow(100.0, 2, 10.0, 180.0)]
    [DataRow(100.0, 3, 20.0, 240.0)]
    public void CalculateTotal_ShouldApplyDiscountCorrectlyMultiple(double price, int quantity,
        double discountPercent, double expectedTotal)
    {
        // Arrange    
        var service = new OrderService();
        
        // Act       
        var totalPrice = service.CalculateTotal(price: (decimal)price, quantity: quantity,
                                                        discountPercent: (decimal)discountPercent);

        // Assert    
        Assert.AreEqual((decimal)expectedTotal, totalPrice);
    }
}