using Messzendzser.Controllers;
using Messzendzser.Model.Managers.User;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messzendzser.UnitTests.api
{
    internal class LoginTest
    {
        LoginController controller;
        IUserManager userManager;
        [SetUp]
        public void SetUp()
        {
            controller = new LoginController();
             userManager = new TestUserManager();
        }

        [Test]
        public void UsernameMissingTest()
        {
            string response = controller.Login(null, "Abasd123", userManager);
            string pattern = ApiTestUtils.RersponseRegex(1, "Invalid parameters", new Dictionary<string, string>() { { "username", "Username cannot be empty" } });
            Assert.That(response, Does.Match(pattern));
        }
        [Test]
        public void PasswordMissingTest()
        {
            string response = controller.Login("test@asd.com", null, userManager);
            string pattern = ApiTestUtils.RersponseRegex(1, "Invalid parameters", new Dictionary<string, string>() { { "password", "Password cannot be empty" } });
            Assert.That(response, Does.Match(pattern));
        }

        [Test]
        public void WrogCredentialsTest()
        {
            string response = controller.Login("wrongusername", "wrongpassword", userManager);
            string pattern = ApiTestUtils.RersponseRegex(1, "Invalid parameters", new Dictionary<string, string>() { { "username", "Given credentials do not match any record" } });
            Assert.That(response, Does.Match(pattern));
        }

        [Test]
        public void SuccesfulTest()
        {
            string response = controller.Login("test@asd.com", "password", userManager);
            string pattern = ApiTestUtils.RersponseRegex(200, "Ok", new Dictionary<string, string>() { { "token", "SuccessfulToken" } });
            Assert.That(response, Does.Match(pattern));
        }
    }
}
