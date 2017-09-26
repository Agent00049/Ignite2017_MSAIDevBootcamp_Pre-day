using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Bot.Services;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Bot.Builder.Azure;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;

// Bot Storage: Register the optional private state storage for your bot. 

// For Azure Table Storage, set the following environment variables in your bot app:
// -UseTableStorageForConversationState set to 'true'
// -AzureWebJobsStorage set to your table connection string

// For CosmosDb, set the following environment variables in your bot app:
// -UseCosmosDbForConversationState set to 'true'
// -CosmosDbEndpoint set to your cosmos db endpoint
// -CosmosDbKey set to your cosmos db key

namespace Bot
{
  public static class Messages
  {
    [FunctionName("messages")]
    public static async Task<HttpResponseMessage> Run(
      [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequestMessage req, TraceWriter log)
    {
      log.Info($"Webhook was triggered!");

      // Initialize the azure bot
      using (new AzureFunctionsResolveAssembly())
      using (BotService.Initialize())
      {
        // Deserialize the incoming activity
        string jsonContent = await req.Content.ReadAsStringAsync();
        var activity = JsonConvert.DeserializeObject<Activity>(jsonContent);

        // authenticate incoming request and add activity.ServiceUrl to MicrosoftAppCredentials.TrustedHostNames
        // if request is authenticated
        if (!await BotService.Authenticator.TryAuthenticateAsync(req, new[] {activity}, CancellationToken.None))
        {
          return BotAuthenticator.GenerateUnauthorizedResponse(req);
        }

        if (activity != null)
        {
          // one of these will have an interface and process it
          switch (activity.GetActivityType())
          {
            case ActivityTypes.Message:
              //await Translate(activity);
              await Conversation.SendAsync(activity, () => new EchoLuisDialog());
              break;
            case ActivityTypes.ConversationUpdate:
              var client = new ConnectorClient(new Uri(activity.ServiceUrl));
              IConversationUpdateActivity update = activity;
              if (update.MembersAdded.Any())
              {
                var reply = activity.CreateReply();
                var newMembers = update.MembersAdded?.Where(t => t.Id != activity.Recipient.Id);
                foreach (var newMember in newMembers)
                {
                  reply.Text = "Welcome";
                  if (!string.IsNullOrEmpty(newMember.Name))
                  {
                    reply.Text += $" {newMember.Name}";
                  }
                  reply.Text += "!";
                  await client.Conversations.ReplyToActivityAsync(reply);
                }
              }
              break;
            case ActivityTypes.ContactRelationUpdate:
            case ActivityTypes.Typing:
            case ActivityTypes.DeleteUserData:
            case ActivityTypes.Ping:
            default:
              log.Error($"Unknown activity type ignored: {activity.GetActivityType()}");
              break;
          }
        }
        return req.CreateResponse(HttpStatusCode.Accepted);
      }
    }

    // J'ai besoin d'assurance
    private static async Task Translate(Activity activity)
    {
      var conversationData = await GetConversationDataAsync(activity);

      //var from = conversationData.GetProperty<string>("lang") 
      //  ?? await TranslatorService.Detect(activity.Text);
      //if (from != "en")
      //{
      //  activity.Text = await TranslatorService.Translate(activity.Text, from, "en");
      //}

      //conversationData.SetProperty("lang", from);
      //await SaveConversationDataAsync(activity, conversationData);
    }

    private static Task<BotData> GetConversationDataAsync(Activity activity)
    {
      if (Utils.GetAppSetting("UseTableStorageForConversationState") == "true")
      {
        IBotDataStore<BotData> store = new TableBotDataStore(Utils.GetAppSetting("AzureWebJobsStorage"));
        return store.LoadAsync(
          new Address(
            activity.Recipient.Id,
            activity.ChannelId,
            activity.From.Id,
            activity.Conversation.Id,
            activity.ServiceUrl),
          BotStoreType.BotConversationData,
          CancellationToken.None);
      }

      StateClient stateClient = activity.GetStateClient();
      return stateClient.BotState.GetConversationDataAsync(activity.ChannelId, activity.Conversation.Id);
    }

    private static Task SaveConversationDataAsync(Activity activity, BotData botData)
    {
      if (Utils.GetAppSetting("UseTableStorageForConversationState") == "true")
      {
        IBotDataStore<BotData> store = new TableBotDataStore(Utils.GetAppSetting("AzureWebJobsStorage"));
        return store.SaveAsync(
          new Address(
            activity.Recipient.Id,
            activity.ChannelId,
            activity.From.Id,
            activity.Conversation.Id,
            activity.ServiceUrl),
          BotStoreType.BotConversationData,
          botData,
          CancellationToken.None);
      }

      StateClient stateClient = activity.GetStateClient();
      return stateClient.BotState.SetConversationDataAsync(activity.ChannelId, activity.Conversation.Id, botData);
    }
  }
}
