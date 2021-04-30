using Microsoft.AspNetCore.Http;

namespace VigenereDecryptor.Services
{
    public interface IFileService
    {
        bool CreateFiles(string text, string webRootPath);

        bool ParseFile(IFormFile inputFile, string webRootPath, out string result);
    }
}
