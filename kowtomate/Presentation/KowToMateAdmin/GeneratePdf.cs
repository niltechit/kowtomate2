using System.Diagnostics;

namespace KowToMateAdmin
{
    public class GeneratePdf
    {
        public string url { get; set; }

        public GeneratePdf(string url)
        {
            this.url = url;
        }

        public byte[] GetPdf()
        {
            var switches = $"-q {url} -";
            string rotativaPath = Path.Combine(Directory.GetCurrentDirectory(), "rotativa", "wkhtmltopdf.exe");
            using(var proc = new Process())
            {
                try
                {
                    proc.StartInfo = new ProcessStartInfo
                    {
                        FileName = rotativaPath,
                        Arguments = switches,
                        UseShellExecute = false,
                        RedirectStandardError = true,
                        RedirectStandardOutput = true,
                        RedirectStandardInput = true,
                        CreateNoWindow = true,

                    };
                    proc.Start();
                }
                catch
                {

                }

                using (var ms = new MemoryStream())
                {
                    proc.StandardOutput.BaseStream.CopyTo(ms);
                    return ms.ToArray();
                }
            }

        }
    }
}
