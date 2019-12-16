namespace Chat.FileServer
{
    public class ChatFileServerServiceConfiguration
    {
        public const string SECTION_NAME = "FileInteraction";

        public string OutputFileName { get; set; }
        public string OutputDirectoryName { get; set; }
    }
}
