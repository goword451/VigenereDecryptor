using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Text;
using System;

namespace VigenereDecryptor.Services
{
    public class FileService : IFileService
    {
        private ILogger<FileService> Logger { get; }

        public FileService(ILogger<FileService> logger)
        {
            Logger = logger;
        }

        public string ParseFile(IFormFile inputFile, string webRootPath)
        {
            var extension = inputFile.FileName.Split('.')[1];
            var uploadFolder = Path.Combine(webRootPath, Constants.File.filesFolder);
            var filePath = Path.Combine(uploadFolder, Constants.File.inputFileName + extension);

            using (var fs = new FileStream(filePath, FileMode.Create))
            {
                inputFile.CopyTo(fs);
            }
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            if (extension == Constants.File.txtFormat)
            {
                var input = File.ReadAllText(filePath);
                if (input.Contains(Constants.File.specialSymbol))
                {
                    return File.ReadAllText(filePath, Encoding.GetEncoding(1251));
                }

                return input;
            }
            else
            {
                try
                {
                    using (var document = WordprocessingDocument.Open(filePath, false))
                    {
                        return document.MainDocumentPart.Document.Body.InnerText;
                    }
                }
                catch
                {
                    Logger.LogError(Constants.Errors.inputError);
                }
            }

            return string.Empty;
        }

        public bool CreateFiles(string text, string webRootPath)
        {
            var uploadFolder = Path.Combine(webRootPath, Constants.File.filesFolder);

            var filePathTxt = Path.Combine(uploadFolder, Constants.File.outputFileTxt);

            var filePathDocx = Path.Combine(uploadFolder, Constants.File.outputFileDoc);

            try
            {
                using (var fs = new FileStream(filePathTxt, FileMode.Create))
                {
                    var array = Encoding.Default.GetBytes(text);
                    fs.Write(array, 0, array.Length);
                }

                using (var document = WordprocessingDocument.Create(filePathDocx, WordprocessingDocumentType.Document))
                {
                    document.AddMainDocumentPart();
                    document.MainDocumentPart.Document = new Document(new Body(new Paragraph(new Run(new Text(text)))));
                }

                return true;
            }
            catch(Exception ex)
            {
                Logger.LogError(ex.Message);

                return false;
            }
        }
    }
}
