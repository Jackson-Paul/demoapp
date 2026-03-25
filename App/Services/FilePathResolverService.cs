namespace App.Services
{
    public class FilePathResolverService
    {
        private readonly string _baseDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "downloads");

        public string GetDirectory()
        {
            return _baseDirectory;
        }

        public string GetCurrentPath()
        {
            return Directory.GetCurrentDirectory();
        }

        public IEnumerable<string> ListFiles(string directory)
        {
            return Directory.GetFiles(directory);
        }

        public async Task<string> ResolvePath(string filename)
        {
            string fullPath = Path.Combine(_baseDirectory, filename);
            return await Task.FromResult(fullPath);
        }

        public bool IsValidDirectory(string path)
        {
            return Directory.Exists(path);
        }

        public string GetFileExtension(string filename)
        {
            return Path.GetExtension(filename);
        }

        public string ProcessInput(string input)
        {
            string fullPath = Path.GetFullPath(Path.Combine(_baseDirectory, input));
            if (!fullPath.StartsWith(Path.GetFullPath(_baseDirectory)))
            {
                return Path.GetFileName(input);
            }
            return Path.GetFileName(fullPath);
        }
    }
}
