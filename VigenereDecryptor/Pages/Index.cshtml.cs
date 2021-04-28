using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using VigenereDecryptor.Services;
using System.IO;
using System.Text;

namespace VigenereDecryptor.Pages
{
    public class IndexModel : PageModel
    {
        private IWebHostEnvironment WebHostEnvironment { get; }

        private ILogger<IndexModel> Logger { get; }

        private ICypherService Cypher { get; }

        [BindProperty]
        public string Input { get; set; }

        public string Key { get; set; }

        public string Output { get; set; }

        [BindProperty]
        public IFormFile InputFile { get; set; }

        [BindProperty]
        public string Alphabet { get; set; }

        public IndexModel(ILogger<IndexModel> logger, IWebHostEnvironment webHostEnvironment, ICypherService cypher)
        {
            Logger = logger;
            WebHostEnvironment = webHostEnvironment;
            Cypher = cypher;
        }

        public void DecryptOrEncrypt(string input, string key, string action)
        {
            if (InputFile != null)
            {
                ParseFile();
            }
            else
            {
                Input = input;
            }

            Key = key;
            
            if (string.IsNullOrEmpty(Input) || string.IsNullOrEmpty(Key))
            {
                return;
            }

            try
            {
                Output = action == Constants.Actions.actionEncrypt ? Cypher.Encrypt(Input, Key) : Cypher.Decrypt(Input, Key);
            }
            catch
            {
                Logger.LogError(Constants.Errors.alphabetError);
                Output = Input;
            }

            CreateFiles();
        }

        private void ParseFile()
        {
            var extension = InputFile.FileName.Split('.')[1];
            var uploadFolder = Path.Combine(WebHostEnvironment.WebRootPath, Constants.File.filesFolder);
            var filePath = Path.Combine(uploadFolder, Constants.File.inputFileName + extension);

            using (var fs = new FileStream(filePath, FileMode.Create))
            {
                InputFile.CopyTo(fs);
            }
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            if (extension == Constants.File.txtFormat)
            {
                Input = System.IO.File.ReadAllText(filePath);
                if (Input.Contains('�'))
                {
                    Input = System.IO.File.ReadAllText(filePath, Encoding.GetEncoding(1251));
                }
            }
            else
            {
                try
                {
                    using (var document = WordprocessingDocument.Open(filePath, false))
                    {
                        Input = document.MainDocumentPart.Document.Body.InnerText;
                    }
                }
                catch
                {
                    Logger.LogError(Constants.Errors.inputError);
                }
            }
        }

        private void CreateFiles()
        {
            var uploadFolder = Path.Combine(WebHostEnvironment.WebRootPath, Constants.File.filesFolder);

            var filePathTxt = Path.Combine(uploadFolder, Constants.File.outputFileTxt);

            var filePathDocx = Path.Combine(uploadFolder, Constants.File.outputFileDoc);

            using (var fs = new FileStream(filePathTxt, FileMode.Create))
            {
                var array = Encoding.Default.GetBytes(Output);
                fs.Write(array, 0, array.Length);
            }

            using (var document = WordprocessingDocument.Create(filePathDocx, WordprocessingDocumentType.Document))
            {
                document.AddMainDocumentPart();
                document.MainDocumentPart.Document = new Document(new Body(new Paragraph(new Run(new Text(Output)))));
            }
        }
    }
}
