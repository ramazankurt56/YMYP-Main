using FluentAssertions;
using FluentValidation;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NSubstitute.ReturnsExtensions;
using RealWorld.WebAPI.Dtos;
using RealWorld.WebAPI.Logging;
using RealWorld.WebAPI.Models;
using RealWorld.WebAPI.Repositories;
using RealWorld.WebAPI.Services;

namespace Users.API.Tests.Unit;

public class UserServiceTests
{
    private readonly UserService _sut;
    private readonly IUserRepository userRepository = Substitute.For<IUserRepository>();
    private readonly ILoggerAdapter<UserService> logger = Substitute.For<ILoggerAdapter<UserService>>();
    private readonly CreateUserDto createUserDto = new("Taner Saydam", 34, new(1989, 09, 03));
    private readonly UpdateUserDto updateUserDto = new(1, "Toprak Saydam", 34, new DateOnly(1989, 09, 03));
    private User user = new()
    {
        Id = 1,
        Name = "Taner Saydam",
        Age = 34,
        DateOfBirth = new(1989, 09, 03)
    };
    public UserServiceTests()
    {
        _sut = new(userRepository, logger);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnEmptyList_WhenNoUsersExist()
    {
        //Arrange
        userRepository.GetAllAsync().Returns(Enumerable.Empty<User>().ToList());

        //Act
        var result = await _sut.GetAllAsync();

        //Asset
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllAsync_ShouldRetunsUsers_WhenSomeUserExist()
    {
        //Arrange
        var tanerUser = new User
        {
            Id = 1,
            Age = 34,
            Name = "Taner Saydam",
            DateOfBirth = new(1989, 09, 03)
        };

        var enesUser = new User
        {
            Id = 2,
            DateOfBirth = new(1996, 04, 25),
            Name = "Enes Demirta�",
            Age = 28
        };

        var users = new List<User>() { tanerUser, enesUser };

        userRepository.GetAllAsync().Returns(users);

        //Act
        var result = await _sut.GetAllAsync();

        //Assert
        result.Should().BeEquivalentTo(users);
        result.Should().HaveCount(2);
        result.Should().NotHaveCount(3);
    }

    [Fact]
    public async Task GetAllAsync_ShouldLogMessages_WhenInvoked()
    {
        //Arrange
        userRepository.GetAllAsync().Returns(Enumerable.Empty<User>().ToList());

        //Act
        await _sut.GetAllAsync();

        //Assert
        logger.Received(1).LogInformation(Arg.Is("T�m userlar getiriliyor"));
        logger.Received(1).LogInformation(Arg.Is("T�m user listesi �ekildi"));
    }

    [Fact]
    public async Task GetAllAsync_ShouldLogMessageAnException_WhenExceptionIsThrown()
    {
        //Arrange
        var exception = new ArgumentException("User listesi ge�erken bir hatayla kar��la�t�k");
        userRepository.GetAllAsync().Throws(exception);

        //Act
        var requestAction = async () => await _sut.GetAllAsync();

        await requestAction.Should()
            .ThrowAsync<ArgumentException>();

        logger.Received(1).LogError(Arg.Is(exception), Arg.Is("User listesi ge�erken bir hatayla kar��la�t�k"));
    }

    [Fact]
    public async Task CreateAsync_ShouldThrownAnError_WhenUserCreateDetailAreNotValid()
    {
        //Arrange
        CreateUserDto request = new("",0,new(2007,01,01));

        //Act
        var action = async() => await _sut.CreateAsync(request);

        //Assert
        await action.Should().ThrowAsync<ValidationException>();

    }

    [Fact]
    public async Task CreateAsync_ShouldThrownAnError_WhenUserNameExist()
    {
        //Arrange
        userRepository.NameIsExists(Arg.Any<string>()).Returns(true);

        //Act
        var action = async () => await _sut.CreateAsync(new("Taner Saydam",34,new DateOnly(1989,09,03)));

        //Assert
        await action.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public void CreateAsync_ShouldCreateUserDtoToUserObject()
    {
        //Act
        var user = _sut.CreateUserDtoToUserObject(createUserDto);

        //Assert
        user.Name.Should().Be(createUserDto.Name);
        user.Age.Should().Be(createUserDto.Age);
        user.DateOfBirth.Should().Be(createUserDto.DateOfBirth);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateUser_WhenDetailsAreValidAndUnique()
    {
        //Arrange        
        userRepository.NameIsExists(createUserDto.Name).Returns(false);
        userRepository.CreateAsync(Arg.Any<User>()).Returns(true);

        //Act
        var result = await _sut.CreateAsync(createUserDto);

        //Asert
        result.Should().Be(true);
    }

    [Fact]
    public async Task CreateAsync_ShouldLogMessages_WhenInvoked()
    {
        //Arrage
        userRepository.NameIsExists(createUserDto.Name).Returns(false);
        userRepository.CreateAsync(Arg.Any<User>()).Returns(true);

        //Act
        await _sut.CreateAsync(createUserDto);

        //Asert
        logger.Received(1).LogInformation(
            Arg.Is("Kullan�c� ad�: {0} bu olan kullan�c� kayd� yap�lmaya ba�land�."),
            Arg.Any<string>());

        logger.Received(1).LogInformation(
            Arg.Is("User Id: {0} olan kullan�c� {1}ms de olu�turuldu"),
            Arg.Any<int>(),
            Arg.Any<long>());
    }

    [Fact]
    public async Task CreateAsync_ShouldLogMessagesAndException_WhenExceptionIsThrown()
    {
        //Arrange
        var exception = new ArgumentException("Kullan�c� kayd� esnas�nda bir hatayla kar��la�t�m");
        userRepository.CreateAsync(Arg.Any<User>()).Throws(exception);

        //Act
        var action = async () => await _sut.CreateAsync(createUserDto);

        //Assert
        await action.Should()
            .ThrowAsync<ArgumentException>();

        logger.Received(1).LogError(Arg.Is(exception), Arg.Is("Kullan�c� kayd� esnas�nda bir hatayla kar��la�t�m"));
    }

    [Fact]
    public async Task DeleteByIdAsync_ShouldThrownAnError_WhenUserNotExist()
    {
        //Arrange
        int userId = 1;
        userRepository.GetByIdAsync(userId).ReturnsNull();

        //Act
        var action = async ()=> await _sut.DeleteByIdAsync(userId);

        //Assert
        await action.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task DeleteByIdAsync_ShouldDeleteUser_WhenUserExist()
    {
        //Arrange
        int userId = 1;
        User user = new()
        {
            Id = userId,
            Name = "Taner Saydam",
            Age = 34,
            DateOfBirth = new(1989, 09, 03)
        };
        userRepository.GetByIdAsync(userId).Returns(user);
        userRepository.DeleteAsync(user).Returns(true);

        //Act
        var result = await _sut.DeleteByIdAsync(userId);

        //Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteByIdAsync_ShouldLogMessages_WhenInvoked()
    {
        //Arrange
        int userId = 1;
        var user = new User()
        {
            Id = userId,
            DateOfBirth = new(1989, 09, 03),
            Name = "Taner Saydam",
            Age = 34
        };
        userRepository.GetByIdAsync(userId).Returns(user);
        userRepository.DeleteAsync(user).Returns(true);

        //Act
        await _sut.DeleteByIdAsync(userId);

        //Assert
        logger.Received(1).LogInformation(
            Arg.Is("{0} id numaras�na sahip kullan�c� siliniyor..."),
            Arg.Is(userId));

        logger.Received(1).LogInformation(
            Arg.Is("Kullan�c� id'si {0} olan kullan�c� kayd� {1}ms de silindi"),
            Arg.Is(userId),
            Arg.Any<long>());
    }

    [Fact]
    public async Task DeleteByIdAsync_ShouldLogMessagesAndException_WhenExceptionIsThrown()
    {
        //Arrange
        int userId = 1;
        var user = new User()
        {
            Id = userId,
            Name = "Taner Saydam",
            Age = 34,
            DateOfBirth = new(1989, 09, 03)
        };
        userRepository.GetByIdAsync(userId).Returns(user);
        var exception = new ArgumentException("Kullan�c� kayd� silinirken bir hatayla kar��la�t�k");
        userRepository.DeleteAsync(user).Throws(exception);

        //Act
        var action = async()=> await _sut.DeleteByIdAsync(userId);

        //Assert
        await action.Should().ThrowAsync<ArgumentException>();

        logger.Received(1).LogError(Arg.Is(exception), Arg.Is("Kullan�c� kayd� silinirken bir hatayla kar��la�t�k"));
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowAndError_WhenUserNotExist()
    {
        //Arrange        
        userRepository.GetByIdAsync(updateUserDto.Id).ReturnsNull();

        //Act
        var action = async () => await _sut.UpdateAsync(updateUserDto);

        //Assert
        await action.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrownAnError_WhenUserUpdateDetailAreNotValid()
    {
        //Arrange
        UpdateUserDto updateUserDto = new(1, "", 18, new DateOnly(1989, 09, 03));
        userRepository.GetByIdAsync(updateUserDto.Id).Returns(user);

        //Act
        var action = async() => await _sut.UpdateAsync(updateUserDto);

        //Assert
        await action.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrownError_WhenUserNameExist()
    {
        //Arrange
        userRepository.NameIsExists(Arg.Any<string>()).Returns(true);
        userRepository.GetByIdAsync(updateUserDto.Id).Returns(user);

        //Act
        var action = async ()=> await _sut.UpdateAsync(updateUserDto);

        //Assert
        await action.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public void UpdateAsync_ShouldCreateUpdateUserDtoToUserObject()
    {
        //Act
        _sut.CreateUpdateUserObject(ref user, updateUserDto);

        //Asset
        user.Name.Should().Be(updateUserDto.Name);
        user.Age.Should().Be(updateUserDto.Age);
        user.DateOfBirth.Should().Be(updateUserDto.DateOfBirth);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateUser_WhenDeteilsAreValidAndUnique()
    {
        //Arrange
        userRepository.GetByIdAsync(updateUserDto.Id).Returns(user);
        userRepository.NameIsExists(updateUserDto.Name).Returns(false);
        userRepository.UpdateAsync(user).Returns(true);

        //Act
        var result = await _sut.UpdateAsync(updateUserDto);

        //Assert
        result.Should().Be(true);
    }

    [Fact]
    public async Task UpdateAsync_ShouldLogMessages_WhenInvoked()
    {
        //Arrange
        userRepository.GetByIdAsync(updateUserDto.Id).Returns(user);
        userRepository.NameIsExists(updateUserDto.Name).Returns(false);
        userRepository.UpdateAsync(user).Returns(true);

        //Act
        await _sut.UpdateAsync(updateUserDto);

        //Assert
        logger.Received(1).LogInformation(
            Arg.Is("{0} kullan�c�n g�ncelleme i�lemi yap�lmaya ba�land�"),
            Arg.Any<string>());

        logger.Received(1).LogInformation(
            Arg.Is("{0} Id'li kullan�c�n�n g�ncelleme i�lemi {1}ms de ba�ar�yla tamamland�"),
            Arg.Any<int>(),
            Arg.Any<long>());
    }

    [Fact]
    public async Task UpdateAsync_ShouldLogMessagesAndException_WhenExceptionIsThrown()
    {
        //Arrange
        var exception = new ArgumentException("Kullan�c� g�ncelleme esnas�nda bir hatayla kar��la�t�m");
        userRepository.GetByIdAsync(updateUserDto.Id).Returns(user);
        userRepository.NameIsExists(updateUserDto.Name).Returns(false);
        userRepository.UpdateAsync(Arg.Any<User>()).Throws(exception);

        //Act
        var action = async () => await _sut.UpdateAsync(updateUserDto);

        //Assert
        await action.Should()
            .ThrowAsync<ArgumentException>();

        logger.Received(1).LogError(
            Arg.Is(exception), 
            Arg.Is("Kullan�c� g�ncelleme esnas�nda bir hatayla kar��la�t�m"));
    }
}
