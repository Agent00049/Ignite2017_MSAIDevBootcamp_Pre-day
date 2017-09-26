using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;

namespace Bot.Services
{
  /// <summary>
  /// Service to communicate with the Face API
  /// </summary>
  public class FaceRecognitionService
  {
    private static readonly HttpClient Client = new HttpClient();
    private static readonly Guid Me = Guid.Parse(ConfigurationManager.AppSettings["FaceId"]);

    private static string _personGroupId;
    private readonly FaceServiceClient _client;

    public FaceRecognitionService()
    {
      _client = new FaceServiceClient(ConfigurationManager.AppSettings["FaceKey"]);
    }

    //public async Task<bool> Verify(string imageUrl)
    //{
    //  var face = await Client.GetByteArrayAsync(imageUrl);

    //  // Get face id from image
    //  var detectedFaces = await Detect(face);
    //  var personGroupId = await GetPersonGroupId();

    //  VerifyResult finalResponse = null;
    //  foreach (var current in detectedFaces)
    //  {
    //    var verifyResult = await _client.VerifyAsync(current.FaceId, personGroupId, Me);
    //    if (verifyResult.IsIdentical)
    //    {
    //      finalResponse = verifyResult;
    //      break;
    //    }
    //  }

    //  return finalResponse?.IsIdentical ?? false;
    //}

    private Task<Face[]> Detect(byte[] face)
    {
      return _client.DetectAsync(new MemoryStream(face), returnFaceId: true, returnFaceLandmarks: true);
    }

    private async Task<string> GetPersonGroupId()
    {
      if (string.IsNullOrEmpty(_personGroupId))
      {
        var groups = await _client.ListPersonGroupsAsync(top: 1);
        if (groups.Any())
        {
          _personGroupId = groups.First().PersonGroupId;
        }
        else
        {
          _personGroupId = Guid.NewGuid().ToString();
          await _client.CreatePersonGroupAsync(_personGroupId, _personGroupId);
        }
      }

      return _personGroupId;
    }
  }
}
