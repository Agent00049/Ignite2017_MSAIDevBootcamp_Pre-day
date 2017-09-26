using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.ProjectOxford.Vision;

namespace Bot.Services
{
  public class ComputerVisionService
  {
    private static readonly HttpClient Client = new HttpClient();
    private readonly VisionServiceClient _client;

    public ComputerVisionService()
    {
      _client = new VisionServiceClient(ConfigurationManager.AppSettings["ComputerVisionKey"]);
    }

    //public async Task<DetectResult> Detect(string imageUrl)
    //{
    //  var image = await Client.GetByteArrayAsync(imageUrl);

    //  var results = await _client.AnalyzeImageAsync(new MemoryStream(image),
    //    new[] {VisualFeature.Tags, VisualFeature.Description, VisualFeature.Categories});

    //  return new DetectResult
    //  {
    //    IsCar = results.Tags.Any(x => x.Name == "car") || results.Categories.Any(x => x.Name.Contains("trans_car")),
    //    Description = results.Description.Captions.First().Text
    //  };
    //}
  }

  public class DetectResult
  {
    public bool IsCar { get; set; }
    public string Description { get; set; }
  }
}
