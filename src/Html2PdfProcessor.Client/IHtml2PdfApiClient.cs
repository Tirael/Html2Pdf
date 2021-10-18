using System.Threading;
using System.Threading.Tasks;

namespace Html2PdfProcessor.Client
{
    public interface IHtml2PdfApiClient
    {
        Task CreatePdf(string content, CancellationToken cancellationToken = default);
    }
}
