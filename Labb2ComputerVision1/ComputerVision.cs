using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Labb2BildTjanster
{
    public class ComputerVisionService
    {
        private readonly string _endpoint;
        private readonly string _subscriptionKey;
        private readonly ComputerVisionClient _client;

        public ComputerVisionService(IConfiguration configuration)
        {
            _endpoint = configuration["AzureComputerVision:Endpoint"];
            _subscriptionKey = configuration["AzureComputerVision:SubscriptionKey"];

            _client = new ComputerVisionClient(new ApiKeyServiceClientCredentials(_subscriptionKey))
            {
                Endpoint = _endpoint
            };
        }

        public async Task<ImageAnalysis> AnalyzeImageAsync(Stream imageStream)
        {
            var features = new List<VisualFeatureTypes?> { VisualFeatureTypes.Tags, VisualFeatureTypes.Objects };
            return await _client.AnalyzeImageInStreamAsync(imageStream, visualFeatures: features);
        }

        public async Task<ImageAnalysis> AnalyzeImageAsync(string imageUrl)
        {
            var features = new List<VisualFeatureTypes?> { VisualFeatureTypes.Tags, VisualFeatureTypes.Objects };
            return await _client.AnalyzeImageAsync(imageUrl, visualFeatures: features);
        }

        public async Task<Stream> GenerateThumbnailAsync(Stream imageStream, int width, int height)
        {
            return await _client.GenerateThumbnailInStreamAsync(width, height, imageStream, true);
        }
        public async Task<Stream> GenerateThumbnailAsync(string imageUrl, int width, int height)
        {
            return await _client.GenerateThumbnailAsync(width, height, imageUrl, true);
        }
    }
}
