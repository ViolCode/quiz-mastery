using BLL.Helpers;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Tests
{
    public class PasswordHasherTests
    {
        [Fact]
        public void Hash_ShouldThrow_WhenPasswordIsEmpty()
        {
            Action act = () => PasswordHasher.Hash("");
            act.Should().Throw<Exception>().WithMessage("Password is null or empty");
        }

        [Fact]
        public void Verify_ShouldReturnTrue_WhenPasswordMatchesHash()
        {
            var pwd = "Password123";
            var hash = PasswordHasher.Hash(pwd);
            PasswordHasher.Verify(pwd, hash).Should().BeTrue();
        }
    }

}
