using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace PdfConversion.Contracts
{
    public interface IReportService
    {
        Task<FileStreamResult> ExportToPdf(string htmlContent);
    }
}