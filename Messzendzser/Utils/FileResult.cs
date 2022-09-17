using Microsoft.AspNetCore.Mvc;

namespace Messzendzser.Utils
{
    public class FileResult
    {
        public bool Success { get; private set; }
        public string? ErrorJson { get; private set; }

        public FileContentResult? FileContentResult { get; private set; }

        public FileResult(FileContentResult result)
        {
            Success = true;
            FileContentResult = result;
        }
        public FileResult(string error)
        {
            Success = false;
            ErrorJson = error;
        }
    }
}
