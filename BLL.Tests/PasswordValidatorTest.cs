using Xunit;
using FluentAssertions;
using BLL.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Tests
{
    public class PasswordValidatorTests
    {
        [Theory]
        [InlineData("Password1", true)]
        [InlineData("password1", false)]
        [InlineData("Password", false)]
        [InlineData("Pass1", false)]
        public void IsValidPassword_ShouldWork(string password, bool expected)
        {
            PasswordValidator.IsValidPassword(password).Should().Be(expected);
        }
    }
}
