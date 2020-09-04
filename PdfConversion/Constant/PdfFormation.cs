namespace PdfConversion.Constant
{
    public static class PdfFormation
    {
        public const int PAGE_NUMBER_FOOTER_MARGIN_BOTTOM = 20;
        public const int PAGE_NUMBER_FOOTER_HEIGHT = 15;

        public const string PAGE_NUMBER_TEMPLATE = "{pageNumber}";
        public const string TOTAL_PAGE_TEMPLATE = "{totalPages}";

        public static string PageNumberFooterTemplate => $"{PAGE_NUMBER_TEMPLATE}/{TOTAL_PAGE_TEMPLATE}";
    }
}