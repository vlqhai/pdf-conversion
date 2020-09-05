using System.IO;
using System.Threading.Tasks;

namespace PdfConversion.Helper
{
    public static class HtmlReader
    {
        public static async Task<string> ReadHtmlFile()
        {
            var body = string.Empty;
            //var htmlFileName = "testing_report.html";
            var htmlFileName = "print_2.html";
            using (var reader = new StreamReader(Path.Combine("Template", htmlFileName)))
            {
                body = await reader.ReadToEndAsync();
            }

            return body;
        }
    }
}