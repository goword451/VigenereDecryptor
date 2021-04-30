using System;
using System.IO;
using System.Text;

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace VigenereDecryptor.Services
{
    public class FileService : IFileService
    {
        private ILogger<FileService> Logger { get; }

        public FileService(ILogger<FileService> logger)
        {
            Logger = logger;
        }

        public bool ParseFile(IFormFile inputFile, string webRootPath, out string result)
        {
            result = string.Empty;
            var extension = inputFile.FileName.Split('.')[1];
            var uploadFolder = Path.Combine(webRootPath, Constants.File.FilesFolder);
            var filePath = Path.Combine(uploadFolder, Constants.File.InputFileName + extension);

            using (var fs = new FileStream(filePath, FileMode.Create))
            {
                inputFile.CopyTo(fs);
            }
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            try
            {
                if (extension == Constants.File.TxtFormat)
                {
                    result = File.ReadAllText(filePath);
                    if (result.Contains(Constants.File.SpecialSymbol))
                    {
                        result = File.ReadAllText(filePath, Encoding.GetEncoding(1251));
                    }
                }
                else
                {
                    using (var document = WordprocessingDocument.Open(filePath, false))
                    {
                        result = document.MainDocumentPart.Document.Body.InnerText;
                    }

                }

                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
                return false;
            }
        }

        public bool CreateFiles(string text, string webRootPath)
        {
            var uploadFolder = Path.Combine(webRootPath, Constants.File.FilesFolder);

            var filePathTxt = Path.Combine(uploadFolder, Constants.File.OutputFileTxt);

            var filePathDocx = Path.Combine(uploadFolder, Constants.File.OutputFileDoc);

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
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);

                return false;
            }
        }
    }
}
