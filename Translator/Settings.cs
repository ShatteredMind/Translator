namespace Translator
{
    public static class Settings
    {
        private const string russianPattern = @"[А-Яа-я0-9 .,-=+(){}\[\]\\]";
        private const string englishPattern = @"[A-Za-z0-9 .,-=+(){}\[\]\\]";
        private const string key =
        "key=trnsl.1.1.20160703T202841Z.9970db2a9f8a5e54.a0ad0ae4dc97f541b08bd569d72d63e84e17861f";

        public static string RussianPattern { get { return russianPattern; }  }

        public static string EndlishPattern { get { return englishPattern; } }

        public static string APIKey { get { return key; } }
    }
}
