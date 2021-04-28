using Microsoft.AspNetCore.Http;

namespace VigenereDecryptor.Services
{
    public interface IFileService
    {
        bool CreateFiles(string text, string webRootPath);

        string ParseFile(IFormFile inputFile, string webRootPath);
    }
}
