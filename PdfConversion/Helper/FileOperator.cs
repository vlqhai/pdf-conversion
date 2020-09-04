using System.IO;

namespace PdfConversion.Helper
{
    public static class FileOperator
    {
        public static void Delete(string filePath)
        {
            try
            {
                if (File.Exists(filePath)) File.Delete(filePath);
            }
            catch (IOException ex)
            {
            }
        }
    }
}