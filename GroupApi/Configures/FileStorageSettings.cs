namespace GroupApi.Configures
{
    public class FileStorageSettings
    {
        public string UploadDirectory { get; set; }
        public long MaxFileSizeBytes { get; set; }
        public string[] AllowedExtensions { get; set; }
    }
}
