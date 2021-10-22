namespace OpSy_Cryptor.common
{
    public class SelectedFile
    {
        public SelectedFile(string path, byte[] contents)
        {
            Path = path;
            Name = Path[(Path.LastIndexOf('\\') + 1)..];
            Contents = contents;
        }

        public string Name { get; }
        public string Path { get; }
        public byte[] Contents { get; }
    }
}
