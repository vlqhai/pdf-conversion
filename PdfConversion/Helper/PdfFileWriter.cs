using System.IO;
using System.Threading.Tasks;

namespace PdfConversion.Helper
{
    public static class PdfFileWriter
    {
        public static async Task SaveToFile(MemoryStream outputStream, string outputFilePath)
        {
            using (var file = new FileStream(outputFilePath, FileMode.OpenOrCreate))
            {
                await outputStream.CopyToAsync(file);
                await file.FlushAsync();
            }
        }
    }
}