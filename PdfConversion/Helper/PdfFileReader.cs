using System;
using System.IO;
using iText.Kernel.Pdf;

namespace PdfConversion.Helper
{
    public static class PdfFileReader
    {
        public static int GetTotalPages(string filePath)
        {
            var totalPages = 0;

            using (var file = new FileStream(filePath, FileMode.Open))
            {
                using (var pdfReader = new PdfReader(file))
                {
                    pdfReader.SetCloseStream(false);
                    var pdfDocumentLoaded = new PdfDocument(pdfReader);
                    totalPages = pdfDocumentLoaded.GetNumberOfPages();
                    Console.WriteLine($"Total of pages: {totalPages}");
                }
            }

            return totalPages;
        }

        public static FileStream ReadFrom(string filePath)
        {
            return new FileStream(filePath, FileMode.Open);
        }
    }
}