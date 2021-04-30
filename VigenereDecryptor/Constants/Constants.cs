namespace VigenereDecryptor
{
    public class Constants
    {
        public class Errors
        {
            public const string ParsingError = "Не удалось прочитать файл";

            public const string FilesCreationError = "Внутренняя ошибка. Не удалось создать файлы для загрузки";

            public const string ProcessingError = "Не удалось произвести вычисления";
        }

        public class File
        {
            public const string FilesFolder = "Files";

            public const string OutputFileDoc = "output.docx";

            public const string OutputFileTxt = "output.txt";

            public const string InputFileName = "input.";

            public const string TxtFormat = "txt";

            public const char SpecialSymbol = '�';
        }
    }
}


