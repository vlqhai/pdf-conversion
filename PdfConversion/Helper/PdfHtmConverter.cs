using System;
using System.IO;
using System.Text;
using iText.Html2pdf;
using iText.Html2pdf.Resolver.Font;
using iText.IO.Util;
using iText.Kernel.Events;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Layout;
using iText.Layout.Element;
using PdfConversion.Constant;
using PdfConversion.Dto;

namespace PdfConversion.Helper
{
    public class PdfHtmConverter
    {
        public const string FONT_DIRECTORY = "./Fonts/";
        public const string FONT_TIMES_NEW_ROMAN = "./Fonts/times.ttf";
        public const float REGULAR_FONT_SIZE = 12;

        protected readonly ConvertRequest _convertRequest;

        public PdfHtmConverter(ConvertRequest convertRequest)
        {
            _convertRequest = convertRequest;
            var registeredFontCounts = PdfFontFactory.RegisterDirectory(FONT_DIRECTORY);
            Console.WriteLine($"Number of the font registered: {registeredFontCounts}");
        }

        public MemoryStream ManipulatePdf(PageSize pageSize)
        {
            var memoryStream = new MemoryStream();

            var properties = new ConverterProperties();
            var defaultFontProvider = new DefaultFontProvider(true, true, false);
            defaultFontProvider.AddDirectory(FONT_DIRECTORY);
            properties.SetFontProvider(defaultFontProvider);

            using (var pdfWriter = new PdfWriter(memoryStream))
            {
                pdfWriter.SetCloseStream(false);
                var pdfDocument = new PdfDocument(pdfWriter);
                pdfDocument.SetDefaultPageSize(pageSize);

                if (!string.IsNullOrEmpty(_convertRequest.Header))
                {
                    var headerHandler = new ReportHeaderCreator(_convertRequest.Header);
                    pdfDocument.AddEventHandler(PdfDocumentEvent.START_PAGE, headerHandler);
                }

                if (!string.IsNullOrEmpty(_convertRequest.Footer))
                {
                    var footerHandler = new ReportFooterCreator(_convertRequest.Footer);
                    pdfDocument.AddEventHandler(PdfDocumentEvent.END_PAGE, footerHandler);
                }

                var document = HtmlConverter.ConvertToDocument(_convertRequest.Content, pdfDocument, properties);
                document.Close();
            }

            memoryStream.Position = 0;
            return memoryStream;
        }

        public void DecoratePdf(string inputFile, string outputFile)
        {
            using var pdfReader = new PdfReader(inputFile);
            using var pdfWriter = new PdfWriter(outputFile);
            var pdfDocument = new PdfDocument(pdfReader, pdfWriter);
            var totalPages = pdfDocument.GetNumberOfPages();
            Console.WriteLine($"Total number of pages: {pdfDocument.GetNumberOfPages()}");

            for (var i = 1; i <= totalPages; i++)
            {
                var page = pdfDocument.GetPage(i);
                var dictionary = page.GetPdfObject();
                var pdfObject = dictionary.Get(PdfName.Contents);
                if (pdfObject.GetType().Equals(typeof(PdfStream)))
                {
                    var pdfStream = (PdfStream) pdfObject;
                    var data = pdfStream.GetBytes();
                    var replacedData = JavaUtil.GetStringForBytes(data)
                        .Replace(PdfFormation.TOTAL_PAGE_TEMPLATE, totalPages.ToString());
                    pdfStream.SetData(Encoding.UTF8.GetBytes(replacedData));
                }
            }

            pdfDocument.Close();
        }

        // Header event handler
        protected class ReportHeaderCreator : IEventHandler
        {
            protected readonly string _header;

            public ReportHeaderCreator(string header)
            {
                _header = header;
            }

            public virtual void HandleEvent(Event @event)
            {
                var docEvent = (PdfDocumentEvent) @event;

                var page = docEvent.GetPage();
                var pageSize = page.GetPageSize();

                var canvas = new Canvas(new PdfCanvas(page), pageSize);

                // Write text at position
                foreach (var element in HtmlConverter.ConvertToElements(_header)) canvas.Add((IBlockElement) element);
                canvas.Close();
            }
        }

        // Footer event handler
        protected class ReportFooterCreator : IEventHandler
        {
            protected readonly string _footer;

            public ReportFooterCreator(string footer)
            {
                _footer = footer;
            }

            public virtual void HandleEvent(Event @event)
            {
                var docEvent = (PdfDocumentEvent) @event;
                var pdf = docEvent.GetDocument();

                var page = docEvent.GetPage();
                var pageSize = page.GetPageSize();


                var coordX = pageSize.GetWidth() / 2;
                float coordY = PdfFormation.PAGE_NUMBER_FOOTER_MARGIN_BOTTOM;
                var canvasWidth = pageSize.GetWidth();
                float canvasHeight = PdfFormation.PAGE_NUMBER_FOOTER_HEIGHT;

                var canvas = new Canvas(new PdfCanvas(page), new Rectangle(coordX, coordY, canvasWidth, canvasHeight));
                canvas.SetFontProvider(new DefaultFontProvider(true, true, true));
                canvas.SetFontSize(REGULAR_FONT_SIZE);
                canvas.SetFont(PdfFontFactory.CreateFont(FONT_TIMES_NEW_ROMAN));

                var pageFooter = _footer.Replace(PdfFormation.PAGE_NUMBER_TEMPLATE, pdf.GetPageNumber(page).ToString());

                var properties = new ConverterProperties();
                var defaultFontProvider = new DefaultFontProvider(true, true, true);
                defaultFontProvider.AddDirectory(FONT_DIRECTORY);
                properties.SetFontProvider(defaultFontProvider);

                foreach (var element in HtmlConverter.ConvertToElements(pageFooter, properties))
                    canvas.Add((IBlockElement) element);

                canvas.Close();
            }
        }
    }
}