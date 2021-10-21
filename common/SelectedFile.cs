namespace OpSy_Cryptor.common
{
    public class SelectedFile
    {
        public SelectedFile(string path, byte[] contents)
        {
            Path = path;
            Name = Path[Path.LastIndexOf('\\')..];
            Contents = contents;
        }

        public string Name { get; }
        public string Path { get; }
        public byte[] Contents { get; }
        public string SHA256Hash { get; set; } = "";
        public string EncryptedContent { get; set; } = "";
    }
}
