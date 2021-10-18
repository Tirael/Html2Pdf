using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;

namespace Html2PdfProcessor.Client
{
    public class Html2PdfApiClient : IHtml2PdfApiClient
    {
        private readonly Html2PdfApi.Html2PdfApiClient _client;

        public Html2PdfApiClient(Html2PdfApi.Html2PdfApiClient client) => _client = client;

        public async Task CreatePdf(string content, CancellationToken cancellationToken = default)
        {
            var request = new CreatePdfRequest { Html = content };

            using var call = _client.CreatePdf(request, cancellationToken: cancellationToken);

            await foreach (var response in call.ResponseStream.ReadAllAsync(cancellationToken))
            {
                await using var fileStream = File.Create("response.pdf");
                response.Pdf.WriteTo(fileStream);
            }
        }
    }
}
