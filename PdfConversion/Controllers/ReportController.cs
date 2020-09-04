using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PdfConversion.Contracts;
using PdfConversion.Dto;
using PdfConversion.Helper;

namespace PdfConversion.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReportController : ControllerBase
    {
        private readonly ILogger<ReportController> _logger;
        private readonly IReportService _reportService;

        public ReportController(ILogger<ReportController> logger,
            IReportService reportService)
        {
            _logger = logger;
            _reportService = reportService;
        }

        [HttpGet]
        public async Task<IActionResult> ConvertHtmlToPdf()
        {
            _logger.LogInformation("Serving convert html to pdf");
            var htmlContent = await HtmlReader.ReadHtmlFile();
            return await _reportService.ExportToPdf(htmlContent);
        }

        [HttpPost]
        public async Task<IActionResult> ExportToPdf([FromBody] ReportExportingRequest request)
        {
            return await _reportService.ExportToPdf(request.HtmlContent);
        }
    }
}