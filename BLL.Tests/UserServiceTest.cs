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
        public void Register_ShouldHashPassword_AndCallCreateUser()
        {
            var user = new UserDto { Name = "Test", Email = "test@example.com", PasswordHash = "Password123", Role = Role.Student };

            _service.Register(user);

            user.PasswordHash.Should().NotBe("Password123");
            _userAccessMock.Verify(u => u.CreateUser(It.Is<UserDto>(x => x.Email == "test@example.com")), Times.Once);
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
