using System;
using System.Threading.Tasks;
using Google.Protobuf;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using PuppeteerSharp;
using PuppeteerSharp.Media;

namespace Html2PdfProcessor.Services
{
    public class Html2PdfApiService : Html2PdfApi.Html2PdfApiBase
    {
        private readonly ILogger<Html2PdfApiService> _logger;
        private readonly LaunchOptions _options;
        private readonly PdfOptions _pdfOptions;

        public Html2PdfApiService(ILogger<Html2PdfApiService> logger)
        {
            _logger = logger;
            
            _options = new LaunchOptions
            {
                Headless = true,
                Args = new[]
                {
                    "--disable-gpu",
                    "--disable-dev-shm-usage",
                    "--disable-setuid-sandbox",
                    "--no-sandbox",
                }
            };
            
            _pdfOptions = new PdfOptions
            {
                Format = PaperFormat.A4,
                DisplayHeaderFooter = true,
                PrintBackground = true,
            };
        }

        public override async Task CreatePdf(CreatePdfRequest request, IServerStreamWriter<CreatePdfReply> responseStream, ServerCallContext context)
        {
            _logger.LogDebug("Generating PDF");

            await using var browser = await Puppeteer.LaunchAsync(_options);

            await using var page = await browser.NewPageAsync();

            await page.SetContentAsync(request.Html);

            await using var pdfStream = await page.PdfStreamAsync(_pdfOptions);
            
            await responseStream.WriteAsync(new CreatePdfReply
            {
                Pdf = await ByteString.FromStreamAsync(pdfStream) 
            });

            _logger.LogDebug("Export completed");
        }
    }
}
