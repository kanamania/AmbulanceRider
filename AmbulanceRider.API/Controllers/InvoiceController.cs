using AmbulanceRider.API.DTOs;
using AmbulanceRider.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AmbulanceRider.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class InvoiceController : ControllerBase
{
    private readonly InvoiceService _invoiceService;

    public InvoiceController(InvoiceService invoiceService)
    {
        _invoiceService = invoiceService;
    }

    [HttpGet]
    public async Task<IActionResult> GetInvoices([FromQuery] InvoiceFilterDto filter)
    {
        try
        {
            var invoices = await _invoiceService.GetInvoicesAsync(filter);
            return Ok(invoices);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetInvoiceById(int id)
    {
        try
        {
            var invoice = await _invoiceService.GetInvoiceByIdAsync(id);
            if (invoice == null)
                return NotFound(new { message = "Invoice not found" });

            return Ok(invoice);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("preview")]
    public async Task<IActionResult> PreviewInvoice([FromBody] CreateInvoiceDto dto)
    {
        try
        {
            var preview = await _invoiceService.GetInvoicePreviewAsync(dto.CompanyId, dto.PeriodStart, dto.PeriodEnd);
            return Ok(preview);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateInvoice([FromBody] CreateInvoiceDto dto)
    {
        try
        {
            var invoice = await _invoiceService.CreateInvoiceAsync(dto);
            return Ok(invoice);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{id}/mark-paid")]
    public async Task<IActionResult> MarkInvoiceAsPaid(int id, [FromBody] MarkInvoicePaidDto dto)
    {
        try
        {
            var invoice = await _invoiceService.MarkInvoiceAsPaidAsync(id, dto);
            return Ok(invoice);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{id}/generate")]
    public async Task<IActionResult> GenerateInvoiceFiles(int id)
    {
        try
        {
            var (pdfBytes, excelBytes) = await _invoiceService.GenerateInvoiceFilesAsync(id);
            
            return Ok(new
            {
                message = "Invoice files generated successfully",
                pdfSize = pdfBytes.Length,
                excelSize = excelBytes.Length
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{id}/download/pdf")]
    public async Task<IActionResult> DownloadPdf(int id)
    {
        try
        {
            var (pdfBytes, _) = await _invoiceService.GenerateInvoiceFilesAsync(id);
            var invoice = await _invoiceService.GetInvoiceByIdAsync(id);
            
            return File(pdfBytes, "application/pdf", $"{invoice!.InvoiceNumber}.pdf");
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{id}/download/excel")]
    public async Task<IActionResult> DownloadExcel(int id)
    {
        try
        {
            var (_, excelBytes) = await _invoiceService.GenerateInvoiceFilesAsync(id);
            var invoice = await _invoiceService.GetInvoiceByIdAsync(id);
            
            return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{invoice!.InvoiceNumber}.xlsx");
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{id}/download/both")]
    public async Task<IActionResult> DownloadBoth(int id)
    {
        try
        {
            var (pdfBytes, excelBytes) = await _invoiceService.GenerateInvoiceFilesAsync(id);
            var invoice = await _invoiceService.GetInvoiceByIdAsync(id);

            using var memoryStream = new MemoryStream();
            using (var archive = new System.IO.Compression.ZipArchive(memoryStream, System.IO.Compression.ZipArchiveMode.Create, true))
            {
                var pdfEntry = archive.CreateEntry($"{invoice!.InvoiceNumber}.pdf");
                using (var entryStream = pdfEntry.Open())
                {
                    await entryStream.WriteAsync(pdfBytes, 0, pdfBytes.Length);
                }

                var excelEntry = archive.CreateEntry($"{invoice.InvoiceNumber}.xlsx");
                using (var entryStream = excelEntry.Open())
                {
                    await entryStream.WriteAsync(excelBytes, 0, excelBytes.Length);
                }
            }

            memoryStream.Position = 0;
            return File(memoryStream.ToArray(), "application/zip", $"{invoice.InvoiceNumber}.zip");
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{id}/send-email")]
    public async Task<ActionResult> SendInvoiceEmail(int id, [FromBody] SendInvoiceEmailDto dto)
    {
        try
        {
            await _invoiceService.SendInvoiceEmailAsync(id, dto.RecipientEmails, dto.Subject, dto.Body);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("generate-test-invoices")]
    public async Task<ActionResult<List<InvoiceDto>>> GenerateTestInvoices(
        [FromQuery] int count = 5, 
        [FromQuery] int companyId = 1)
    {
        try
        {
            var invoices = await _invoiceService.GenerateTestInvoicesAsync(count, companyId);
            return Ok(invoices);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
