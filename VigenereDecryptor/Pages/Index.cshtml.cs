﻿using Microsoft.AspNetCore.Hosting;
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

        public string Key { get; set; }

        public string Output { get; set; }

        [BindProperty]
        public IFormFile InputFile { get; set; }

        [BindProperty]
        public string Alphabet { get; set; }

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

        public void DecryptOrEncrypt(string input, string key, string action)
        {
            if (InputFile != null)
            {
                Input = FileService.ParseFile(InputFile, WebHostEnvironment.WebRootPath);
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
                Output = action == Constants.Actions.actionEncrypt 
                    ? Cypher.Encrypt(Input, Key) 
                    : Cypher.Decrypt(Input, Key);
            }
            catch
            {
                Logger.LogError(Constants.Errors.alphabetError);
                Output = Input;
            }

            FileService.CreateFiles(Output, WebHostEnvironment.WebRootPath);
        }
    }
}
