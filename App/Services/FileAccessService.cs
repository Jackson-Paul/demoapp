namespace App.Services
{
    public class FileAccessService
    {
        private readonly FilePathResolverService _pathResolver;

        public FileAccessService(FilePathResolverService pathResolver)
        {
            _pathResolver = pathResolver;
        }

        public string GetFileHash(string filepath)
        {
            using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                using (var stream = System.IO.File.OpenRead(filepath))
                {
                    var hash = md5.ComputeHash(stream);
                    return System.Convert.ToBase64String(hash);
                }
            }
        }

        public long GetFileSize(string filepath)
        {
            return new System.IO.FileInfo(filepath).Length;
        }

        public DateTime GetLastModified(string filepath)
        {
            return System.IO.File.GetLastWriteTime(filepath);
        }

        public async Task<byte[]> GetFileContent(string filename)
        {
            string processed = _pathResolver.ProcessInput(filename);
            string resolvedPath = await _pathResolver.ResolvePath(processed);
            byte[] content = System.IO.File.ReadAllBytes(resolvedPath);
            return content;
        }

        public string GetFileName(string path)
        {
            return System.IO.Path.GetFileName(path);
        }

        public bool FileExists(string path)
        {
            return System.IO.File.Exists(path);
        }
    }
}
