using AutoMapper;
using AutoMapper.Extensions.EnumMapping;

namespace Template.Business.UnitTests;

[TestClass]
public class BusinessMapperProfileUnitTests
{
    [TestMethod]
    public void TestMapping()
    {
        //Arrange
        //Act
        var configuration = new MapperConfiguration(cfg =>
        {
            // validates enums
            cfg.EnableEnumMappingValidation();
            cfg.AddProfile(new BusinessMapperProfileUnit());
        });
        
        //Assert
        configuration.AssertConfigurationIsValid();
    }
}