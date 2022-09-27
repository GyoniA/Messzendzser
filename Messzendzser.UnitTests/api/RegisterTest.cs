using Messzendzser.Controllers;
using Messzendzser.Model.Managers.User;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messzendzser.UnitTests.api
{
    internal class RegisterTest
    {
        IUserManager userManager;
        RegisterController controller;
        [SetUp]
        public void Setup()
        {
            controller = new RegisterController();
            userManager = new TestUserManager();
        }

        [Test]
        public void UsernameMissingTest()
        {
            string response = controller.Register("test@asd.com", null, "Abasd123", userManager);
            string pattern = ApiTestUtils.ResponseRegex(1, "Invalid parameters", new Dictionary<string, string>() { { "username", "Username cannot be empty" } });
            Assert.That(response, Does.Match(pattern));                
        }
        [Test]
        public void EmailMissingTest()
        {
            string response = controller.Register(null, "username", "Abasd123", userManager);
            string pattern = ApiTestUtils.ResponseRegex(1, "Invalid parameters", new Dictionary<string, string>() { { "email", "Email cannot be empty" } });
            Assert.That(response, Does.Match(pattern));
        }

        [Test]
        public void PasswordMissingTest()
        {
            string response = controller.Register("test@asd.com", "username", null, userManager);
            string pattern = ApiTestUtils.ResponseRegex(1, "Invalid parameters", new Dictionary<string, string>() { { "password", "Password cannot be empty" } });
            Assert.That(response, Does.Match(pattern));
        }

        [Test]
        public void PasswordWeakTest()
        {
            string response = controller.Register("test@asd.com", "username", "asd123", userManager);
            string pattern = ApiTestUtils.ResponseRegex(1, "Invalid parameters", new Dictionary<string, string>() { { "password", "Password is not strong enough" } });
            Assert.That(response, Does.Match(pattern));
        }
        [Test]
        public void EmailTakenTest()
        {
            string response = controller.Register("taken@test.com", "username", "Abasd123", userManager);
            string pattern = ApiTestUtils.ResponseRegex(1, "Invalid parameters", new Dictionary<string, string>() { { "email", "Email is already taken" } });
            Assert.That(response, Does.Match(pattern));
        }

        [Test]
        public void UsernameTakenTest()
        {
            string response = controller.Register("test@asd.com", "takenusername", "Abasd123", userManager);
            string pattern = ApiTestUtils.ResponseRegex(1, "Invalid parameters", new Dictionary<string, string>() { { "username", "Username is already taken" } });
            Assert.That(response, Does.Match(pattern));
        }

        [Test]
        public void SuccessfulTakenTest()
        {
            string response = controller.Register("test@asd.com", "username", "Abasd123", userManager);
            string pattern = ApiTestUtils.ResponseRegex(200, "Ok", null);
            Assert.That(response, Does.Match(pattern));
        }
    }
}
