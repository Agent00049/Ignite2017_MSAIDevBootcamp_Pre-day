using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace Bot
{
  [Serializable]
  public class EchoDialog : IDialog<object>
  {
    public Task StartAsync(IDialogContext context)
    {
      try
      {
        context.Wait(MessageReceivedAsync);
      }
      catch (OperationCanceledException error)
      {
        return Task.FromCanceled(error.CancellationToken);
      }
      catch (Exception error)
      {
        return Task.FromException(error);
      }

      return Task.CompletedTask;
    }

    public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
    {
      var message = await argument;
      if (string.Equals(message.Text, "i need insurance", StringComparison.OrdinalIgnoreCase))
      {
        var child = DialogHelper.Build();
        context.Call(child, AfterCompleteAsync);
      }
      else
      {
        context.Wait(MessageReceivedAsync);
      }
    }

    private async Task AfterCompleteAsync(IDialogContext context, IAwaitable<Car> argument)
    {
      var result = await argument;
      if (result != null)
      {
        await context.PostAsync($"You want to insure: {result.Make} {result.Model} {result.Year}");
      }

      context.Wait(MessageReceivedAsync);
    }
  }
}
