using System.Threading.Tasks;
using Bogus;
using Grpc.Net.Client;

namespace Html2PdfProcessor.Client.Tests
{
    internal static class Program
    {
        private static async Task Main()
        {
            var faker = new Faker();
            // await Task.Delay(5000);
            using var grpcChannel = GrpcChannel.ForAddress("http://localhost:8082");
            var grpcClient = new Html2PdfApi.Html2PdfApiClient(grpcChannel);
            var client = new Html2PdfApiClient(grpcClient);
            await client.CreatePdf(faker.Lorem.Paragraphs());
        }
    }
}
