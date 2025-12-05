using BLL.Helpers;
using BLL.Services;
using Contracts.DTOs;
using Contracts.Enums;
using Contracts.Interfaces;
using FluentAssertions;
using Moq;
using System.Data;
using Xunit;

namespace BLL.Tests
{
    public class UserServiceTests
    {
        private readonly Mock<IUserAccess> _userAccessMock = new();
        private readonly UserService _service;

        public UserServiceTests()
        {
            _service = new UserService(_userAccessMock.Object);
        }

        [Fact]
        public void UpdateProfile_ShouldHashPassword_AndCallUpdateUser()
        {
            var user = new UserDto { Id = 1, Email = "test@example.com", PasswordHash = "NewPassword123" };

            _service.UpdateProfile(user);

            user.PasswordHash.Should().NotBe("NewPassword123");
            _userAccessMock.Verify(u => u.UpdateUser(It.Is<UserDto>(x => x.Id == 1)), Times.Once);
        }

        [Fact]
        public void DeleteAccount_ShouldCallDeleteUser()
        {
            _service.DeleteAccount(42);

            _userAccessMock.Verify(u => u.DeleteUser(42), Times.Once);
        }

        [Fact]
        public void GetAllRoles_ShouldReturnRolesFromUserAccess()
        {
            var roles = new List<(int, string)> { (1, "Student"), (2, "Teacher") };
            _userAccessMock.Setup(u => u.GetAllRoles()).Returns(roles);

            var result = _service.GetAllRoles();

            result.Should().BeEquivalentTo(roles);
        }

        [Fact]
        public void Register_ShouldHashPassword_AndCallCreateUser()
        {
            var user = new UserDto { Name = "Test", Email = "test@example.com", PasswordHash = "Password123", Role = Role.Student };

            _service.Register(user);

            user.PasswordHash.Should().NotBe("Password123");
            _userAccessMock.Verify(u => u.CreateUser(It.Is<UserDto>(x => x.Email == "test@example.com")), Times.Once);
        }

        [Fact]
        public void Login_ShouldReturnNull_WhenUserNotFound()
        {
            _userAccessMock.Setup(u => u.GetUserByEmail("missing@example.com")).Returns((UserDto?)null);

            var result = _service.Login("missing@example.com", "Password123");

            result.Should().BeNull();
        }

        [Fact]
        public void Login_ShouldReturnNull_WhenPasswordDoesNotMatch()
        {
            var user = new UserDto { Email = "test@example.com", PasswordHash = PasswordHasher.Hash("Password123") };
            _userAccessMock.Setup(u => u.GetUserByEmail("test@example.com")).Returns(user);

            var result = _service.Login("test@example.com", "WrongPassword");

            result.Should().BeNull();
        }

        [Fact]
        public void Login_ShouldReturnUser_WhenPasswordMatches()
        {
            var user = new UserDto { Email = "test@example.com", PasswordHash = PasswordHasher.Hash("Password123"), Role = Role.Student };
            _userAccessMock.Setup(u => u.GetUserByEmail("test@example.com")).Returns(user);

            var result = _service.Login("test@example.com", "Password123");

            result.Should().NotBeNull();
            result!.Email.Should().Be("test@example.com");
        }
    }

}
