namespace CutOutWiz.Core
{
    public class DriveImageModel
    {
        public string Name { get; set; }
        public long Size { get; set; }
        public string Path { get; set; }
        public string? Data { get; set; }
        public string FolderName { get; set; }
        public int PathType { get; set; } //1=Actual Image Source Url, 2 = FTP file Path
        public string? RawImagePath { get; set; }
        public string FullPath { get; set; }
        public string LastModifiedDate { get; set; }
        public string DisplaySize => BytesToString(Size);

        public DriveImageModel(string folderName, string fileName, string filePath, long size, DateTime lastModified, int pathType = 1)
        {
            this.FolderName = folderName;
            this.Name = fileName;
            this.Path = filePath;
            this.Size = size;
            this.PathType = pathType;
            this.LastModifiedDate = $"{lastModified.ToShortDateString()} {lastModified.ToShortTimeString()}";
        }

        private string BytesToString(long byteCount)
        {
            string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" }; //Longs run out around EB
            if (byteCount == 0)
                return "0" + suf[0];
            long bytes = Math.Abs(byteCount);
            int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            double num = Math.Round(bytes / Math.Pow(1024, place), 1);
            return (Math.Sign(byteCount) * num).ToString() + suf[place];
        }
    }
}