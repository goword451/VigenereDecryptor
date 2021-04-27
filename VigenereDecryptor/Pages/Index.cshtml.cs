using DocumentFormat.OpenXml.Packaging;
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
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly ILogger<IndexModel> _logger;
        private readonly ICypher cypher;

        [BindProperty]
        public string Input { get; set; }
        public string Keyword { get; set; }
        public string Output { get; set; }
        [BindProperty]
        public IFormFile InputFile { get; set; }
        [BindProperty]
        public string Alphabet { get; set; }


        public IndexModel(ILogger<IndexModel> logger, IWebHostEnvironment webHostEnvironment, ICypher cypher)
        {
            _logger = logger;
            this.webHostEnvironment = webHostEnvironment;
            this.cypher = cypher;
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
                Output = radio == "encrypt" ? cypher.Encryptor(Input, Keyword) : cypher.Decryptor(Input, Keyword);
            }
            catch
            {
                _logger.LogError("Используйте русский алфавит");
                Output = Input;
            }

            GenerateFiles();
        }

        private void GenerateFiles()
        {
            var uploadFolder = Path.Combine(webHostEnvironment.WebRootPath, "Files");
            var filePathTxt = Path.Combine(uploadFolder, "output.txt");
            var filePathDocx = Path.Combine(uploadFolder, "output.docx");

            using (var fs = new FileStream(filePathTxt, FileMode.Create))
            {
                var array = Encoding.Default.GetBytes(Output);
                fs.Write(array, 0, array.Length);
            }

            using (var fs = new FileStream(filePathDocx, FileMode.Create))
            {
                var array = Encoding.Default.GetBytes(Output);
                fs.Write(array, 0, array.Length);
            }

        }

        private void FileParser()
        {
            var extension = InputFile.FileName.Split('.')[1];
            var uploadFolder = Path.Combine(webHostEnvironment.WebRootPath, "Files");
            var filePath = Path.Combine(uploadFolder, "input." + extension);

            using (var fs = new FileStream(filePath, FileMode.Create))
            {
                InputFile.CopyTo(fs);
            }
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            if (extension == "txt")
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
                    _logger.LogError("Ошибка ввода docx");
                }
            }
        }
    }
}
