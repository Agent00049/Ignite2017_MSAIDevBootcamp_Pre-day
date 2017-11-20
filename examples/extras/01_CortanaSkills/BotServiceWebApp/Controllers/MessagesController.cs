using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Bot.Dialogs;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace Bot.Controllers
{
  [BotAuthentication]
  public class MessagesController : ApiController
  {
    /// <summary>
    /// POST: api/Messages
    /// receive a message from a user and send replies
    /// </summary>
    /// <param name="activity"></param>
    [ResponseType(typeof(void))]
    public virtual async Task<HttpResponseMessage> Post([FromBody] Activity activity)
    {
      // check if activity is of type message
      if (activity != null && activity.GetActivityType() == ActivityTypes.Message)
      {
        await Conversation.SendAsync(activity, () => new EchoDialog());
      }
      else
      {
        await HandleSystemMessage(activity);
      }
      return new HttpResponseMessage(System.Net.HttpStatusCode.Accepted);
    }

    private async Task HandleSystemMessage(Activity activity)
    {
      if (activity.Type == ActivityTypes.DeleteUserData)
      {
        // Implement user deletion here
        // If we handle user deletion, return a real message
      }
      else if (activity.Type == ActivityTypes.ConversationUpdate)
      {
        // Handle conversation state changes, like members being added and removed
        // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
        // Not available in all channels
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
                reply.Text += " to Contoso Ridesharing!";
                await client.Conversations.ReplyToActivityAsync(reply);
            }
        }
      }
      else if (activity.Type == ActivityTypes.ContactRelationUpdate)
      {
        // Handle add/remove from contact lists
        // Activity.From + Activity.Action represent what happened
      }
      else if (activity.Type == ActivityTypes.Typing)
      {
        // Handle knowing tha the user is typing
      }
      else if (activity.Type == ActivityTypes.Ping)
      {
      }
    }
  }
}