using Messzendzser.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messzendzser.UnitTests.api.Utils
{
    internal class UserTokenTest
    {
        [SetUp]
        public void SetUp()
        {

        }


        [Test]
        public void TokenTest() {
            UserToken fromToken = new UserToken(1, "username", "email");
            string token = fromToken.ToToken();
            UserToken toToken = new UserToken(token);
            Assert.AreEqual(toToken.Id, 1);
            Assert.AreEqual(toToken.Username,"username");
            Assert.AreEqual(toToken.Email, "email");
        }

        [Test]
        public void InvalidTokenTest()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                UserToken token = new UserToken("invalidToken");
            });
        }
    }
}
