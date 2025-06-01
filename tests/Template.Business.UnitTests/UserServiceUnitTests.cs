using AutoMapper;
using FluentAssertions;
using MockQueryable;
using Moq;
using Template.Business.DTOs;
using Template.Business.ExternalServices;
using Template.Business.Services;
using Template.Domain;
using Template.Domain.Repository;

namespace Template.Business.UnitTests;

[TestClass]
public class UserServiceUnitTests
{
    private readonly Mock<IRepository> _repository = new();

    private readonly IMapper _mapper =
        new Mapper(new MapperConfiguration(cfg => cfg.AddProfile(new BusinessMapperProfileUnit())));

    private readonly Mock<IPaymentService> _paymentService = new();

    private readonly UserService _sut;

    public UserServiceUnitTests()
    {
        _sut = new UserService(_mapper, _repository.Object, _paymentService.Object);
    }

    [TestInitialize]
    public void Initialize()
    {
        _repository.Setup(repo => repo.QueryAsNoTracking<User>()).Returns(() => new List<User>()
        {
            new()
            {
                Id = new Guid("29cbcd3d-9216-409e-a6a2-37f9c9b21fd4"),
                SurName = "Doe",
                Name = "John",
                Email = "john.doe@test.com",
                Password = "password",
            },
            new()
            {
                Name = "Mark",
                SurName = "Cooper",
                Email = "mark.cooper@test.com",
                Password = "mark.password",
                Id = new Guid("29cbcd3d-9216-409e-a6a2-37f9c9b21fd5")
            }
        }.AsQueryable().BuildMock());
    }

    [TestCleanup]
    public void Cleanup()
    {
        _repository.Reset();
    }

    #region GetUser

    [TestMethod]
    public async Task GetUserByUidWhenUserExists()
    {
        //Arrange
        var user = new UserDto()
        {
            UserUid = new Guid("29cbcd3d-9216-409e-a6a2-37f9c9b21fd4"),
            LastName = "Doe",
            Name = "John",
            Email = "john.doe@test.com"
        };

        //Act
        var result = await _sut.GetUser(user.UserUid);

        //Assert

        // With Fluent Assertion library
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(user);

        // Without Fluent Assertion library
        /*Assert.IsNotNull(result);
        Assert.AreEqual(result.UserUid, user.UserUid);
        Assert.AreEqual(result.Email, user.Email);
        Assert.AreEqual(result.Name, user.Name);*/
    }


    [TestMethod]
    public async Task GetUserByUidWhenUserDoesNotExist()
    {
        //Arrange
        var userId = Guid.NewGuid();

        //Act
        var user = await _sut.GetUser(userId);

        //Assert
        user.Should().BeNull();
    }

    [TestMethod]
    public async Task GetUsers()
    {
        //Arrange
        var expectedUser1 = new UserDto()
        {
            UserUid = new Guid("29cbcd3d-9216-409e-a6a2-37f9c9b21fd4"),
            LastName = "Doe",
            Name = "John",
            Email = "john.doe@test.com"
        };
        var expectedUser2 = new UserDto()
        {
            Name = "Mark",
            LastName = "Cooper",
            Email = "mark.cooper@test.com",
            UserUid = new Guid("29cbcd3d-9216-409e-a6a2-37f9c9b21fd5")
        };

        var expectedUsers = new List<UserDto>()
        {
            expectedUser1,
            expectedUser2
        };

        // Act
        var users = await _sut.GetUsers();

        //Assert
        users.Should().NotBeNull();
        users.Should().BeEquivalentTo(expectedUsers);
    }

    #endregion

    #region CreateUser

    [TestMethod]
    public async Task CreateUserWhenUserDoesNotExist()
    {
        // Arrange    
        var createUserDto = new CreateUserDto()
        {
            Email = "brian.doe@test.com",
            Password = "password",
            Name = "Brian",
            LastName = "Doe"
        };

        // Act       
        await _sut.CreateUser(createUserDto);

        // Assert    
        // No exception expected
        _repository.Verify(repo => repo.Add<User>(It.Is<User>(x => x.Email == createUserDto.Email
                                                                   && x.Name == createUserDto.Name
                                                                   && x.SurName == createUserDto.LastName)),
            Times.Once);
        _repository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
    }

    [TestMethod]
    public async Task CreateUserWhenUserExists()
    {
        // Arrange    
        var createUserDto = new CreateUserDto()
        {
            LastName = "Doe",
            Name = "John",
            Email = "john.doe@test.com",
            Password = "password",
        };

        // Act   
        var requestAction = async () => await _sut.CreateUser(createUserDto);

        // Assert 
        await requestAction.Should().ThrowAsync<Exception>()
            .WithMessage($"User with email:{createUserDto.Email} already exists");
        _repository.Verify(repo => repo.Add(It.IsAny<User>()), Times.Never());
        _repository.Verify(repo => repo.SaveChangesAsync(), Times.Never);
    }

    #endregion

    #region DeleteUser

    [TestMethod]
    public async Task DeleteUserHappyPath()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var email = "john.doe@test.com";
        _repository.Setup(repo => repo.Query<User>()).Returns(() => new List<User>()
        {
            new()
            {
                Id = userId,
                Email = email,
            },
        }.AsQueryable().BuildMock());
        _paymentService.Setup(x => x.HasPendingPayments(email)).ReturnsAsync(false);

        //Act
        await _sut.DeleteUser(userId);

        // Assert
        _repository.Verify(repo => repo.Delete(It.Is<User>(u => u.Id == userId
                                                                && u.Email == email)), Times.Once);
        _repository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        _paymentService.Verify(payment => payment.HasPendingPayments(email), Times.Once);
    }


    [TestMethod]
    public async Task DeleteUserWhenUserDoesNotExist()
    {
        // Arrange    
        var userId = Guid.NewGuid();
        _repository.Setup(repo => repo.Query<User>()).Returns(() => new List<User>(){ }.AsQueryable().BuildMock());

        // Act   
        var requestAction = async () => await _sut.DeleteUser(userId);

        // Assert 
        await requestAction.Should().ThrowAsync<Exception>()
            .WithMessage($"User with UserId:{userId} does not exist.");

        _repository.Verify(repo => repo.Delete(It.IsAny<User>()), Times.Never());
        _repository.Verify(repo => repo.SaveChangesAsync(), Times.Never);
        _paymentService.Verify(payment => payment.HasPendingPayments(It.IsAny<string>()), Times.Never);
    }


    [TestMethod]
    public async Task DeleteUserWhenUserHasPendingPayments()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var email = "john.doe@test.com";
        _repository.Setup(repo => repo.Query<User>()).Returns(() => new List<User>()
        {
            new()
            {
                Id = userId,
                Email = email,
            },
        }.AsQueryable().BuildMock());
        _paymentService.Setup(x => x.HasPendingPayments(email)).ReturnsAsync(true);

        //Act
        var requestAction = async () => await _sut.DeleteUser(userId);

        // Assert

        await requestAction.Should().ThrowAsync<Exception>()
            .WithMessage($"User with email:{email} has pending payments");
        _paymentService.Verify(payment => payment.HasPendingPayments(email), Times.Once);
        _repository.Verify(repo => repo.Delete(It.IsAny<User>()), Times.Never());
        _repository.Verify(repo => repo.SaveChangesAsync(), Times.Never);
    }

    #endregion
}