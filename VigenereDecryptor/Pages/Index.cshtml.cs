using System;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

using VigenereDecryptor.Services;

namespace VigenereDecryptor.Pages
{
    public class IndexModel : PageModel
    {
        private IWebHostEnvironment WebHostEnvironment { get; }

        private ILogger<IndexModel> Logger { get; }

        private ICypherService Cypher { get; }

        private IFileService FileService { get; }

        [BindProperty]
        public string Input { get; set; }

        [BindProperty]
        public string Output { get; set; }

        [BindProperty]
        public string ErrorMessage { get; set; }

        [BindProperty]
        public string Key { get; set; }

        [BindProperty]
        public IFormFile InputFile { get; set; }

        [BindProperty]
        public bool EncryptMode { get; set; } = true;

        public IndexModel(
            ILogger<IndexModel> logger,
            IWebHostEnvironment webHostEnvironment,
            ICypherService cypher,
            IFileService fileService)
        {
            Logger = logger;
            WebHostEnvironment = webHostEnvironment;
            Cypher = cypher;
            FileService = fileService;
        }

        public void OnPostAsync()
        {
            Output = string.Empty;

            if (InputFile != null)
            {
                var isParsed = FileService.ParseFile(InputFile, WebHostEnvironment.WebRootPath, out string parsingResult);
                if (!isParsed)
                {
                    ErrorMessage = Constants.Errors.ParsingError;
                    return;
                }

                Input = parsingResult;
            }

            if (string.IsNullOrEmpty(Input) || string.IsNullOrEmpty(Key))
            {
                return;
            }

            try
            {
                Output = EncryptMode
                    ? Cypher.Encrypt(Input, Key)
                    : Cypher.Decrypt(Input, Key);
            }
            catch (Exception ex)
            {
                ErrorMessage = Constants.Errors.ProcessingError;
                Logger.LogError(ex.Message);
            }

            var isCreated = FileService.CreateFiles(Output, WebHostEnvironment.WebRootPath);
            if (!isCreated)
            {
                ErrorMessage = Constants.Errors.FilesCreationError;
                return;
            }
        }
    }
}
