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
        private const string actionEncrypt = "encrypt";

        private const string alphabetError = "Используйте русский алфавит";

        private const string inputError = "Ошибка ввода docx";

        private const string filesFolder = "Files";

        private const string outputFileDoc = "output.docx";

        private const string outputFileTxt = "output.txt";

        private const string inputFileName = "input.";

        private const string txtFormat = "txt";

        public IWebHostEnvironment WebHostEnvironment { get; private set; }

        public ILogger<IndexModel> Logger { get; private set; }

        public ICypherService Cypher { get; private set; }

        [BindProperty]

        public string Input { get; set; }

        public string Keyword { get; set; }

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

        public void OnPostAsync(string input, string keyword, string radio)
        {
            if (InputFile != null)
            {
                FileParser();
            }
            else
            {
                Input = input;
            }

            Keyword = keyword;
            
            if (string.IsNullOrEmpty(Input) || string.IsNullOrEmpty(Keyword))
            {
                return;
            }

            try
            {
                Output = radio == actionEncrypt ? Cypher.Encrypt(Input, Keyword) : Cypher.Decrypt(Input, Keyword);
            }
            catch
            {
                Logger.LogError(alphabetError);
                Output = Input;
            }

            GenerateFiles();
        }

        private void GenerateFiles()
        {
            var uploadFolder = Path.Combine(WebHostEnvironment.WebRootPath, filesFolder);

            var filePathTxt = Path.Combine(uploadFolder, outputFileTxt);

            var filePathDocx = Path.Combine(uploadFolder, outputFileDoc);

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

        private void FileParser()
        {
            var extension = InputFile.FileName.Split('.')[1];
            var uploadFolder = Path.Combine(WebHostEnvironment.WebRootPath, filesFolder);
            var filePath = Path.Combine(uploadFolder, inputFileName + extension);

            using (var fs = new FileStream(filePath, FileMode.Create))
            {
                InputFile.CopyTo(fs);
            }
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            if (extension == txtFormat)
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
                    Logger.LogError(inputError);
                }
            }
        }
    }
}
