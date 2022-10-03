using Messzendzser.Controllers;
using Messzendzser.Model.Managers.Media;
using Messzendzser.Controllers;
using Messzendzser.Utils;
using Microsoft.Extensions.FileSystemGlobbing.Internal.PatternContexts;
using MySqlX.XDevAPI.Common;

namespace Messzendzser.UnitTests.api
{
    internal class GetSoundTest { 
        GetVoiceController controller;
        IMediaManager mediaManager;

        /// <summary>
        /// Sets up the environment for each test
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            controller = new GetVoiceController();
            mediaManager = new TestMediaManager();
        }

        /// <summary>
        /// Tests the result if the user token is invalid
        /// </summary>
        [Test]
        public void InvalidUserToken()
        {
            FileResult result = controller.LoadSound("valid", "invalid", mediaManager);
            string pattern = ApiTestUtils.ResponseRegex(3, "Invalid user token");
            Assert.That(result.Success == false);
            Assert.That(result.ErrorJson, Does.Match(pattern));
        }
        /// <summary>
        /// Tests the result is a correct error message if the image is not found by the IMediaSource
        /// </summary>
        [Test]
        public void SoundNotFound()
        {
            UserToken token = new UserToken(1, "testUser", "testuser@localhost.com");
            string pattern = ApiTestUtils.ResponseRegex(1, "Invalid parameters", new Dictionary<string, string>() { {"error", "Sound not found" } });
            FileResult result = controller.LoadSound("notfound", token.ToToken(), mediaManager);
            Assert.That(result.Success == false);
            Assert.That(result.ErrorJson, Does.Match(pattern));
        }

        /// <summary>
        /// Tests if the result is a correct error message if the img argument is null
        /// </summary>
        [Test]
        public void SoundIsNullFound()
        {
            UserToken token = new UserToken(1, "testUser", "testuser@localhost.com");
            string pattern = ApiTestUtils.ResponseRegex(1, "Invalid parameters",new Dictionary<string, string>() { { "voice", "Voice cannot be empty" } });
            FileResult result = controller.LoadSound(null, token.ToToken(), mediaManager);
            Assert.That(result.Success == false);
            Assert.That(result.ErrorJson, Does.Match(pattern));
        }

        [Test]
        public void Success()
        {
            UserToken token = new UserToken(1, "testUser", "testuser@localhost.com");            
            FileResult result = controller.LoadSound("valid", token.ToToken(), mediaManager);
            Assert.That(result.Success == true);
            CollectionAssert.AreEqual(result.FileContentResult.FileContents, new byte[] { 1, 2, 3, 4 });
        }
    }
}
