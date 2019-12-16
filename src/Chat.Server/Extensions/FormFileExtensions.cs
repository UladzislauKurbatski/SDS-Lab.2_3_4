using Microsoft.AspNetCore.Http;

namespace Chat.Server.Extensions
{
    public static class FormFileExtensions
    {
        public static byte[] GetFileBuffer(this IFormFile formFile)
        {
            var formFileStream = formFile.OpenReadStream();
            var buffer = new byte[formFileStream.Length];
            formFileStream.Read(buffer, 0, (int)formFileStream.Length);
            return buffer;
        }
    }
}
