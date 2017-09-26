using System;
using Microsoft.Bot.Builder.CognitiveServices.QnAMaker;

namespace Bot
{
  [QnAMaker("", "", "I don't know."), Serializable]
  public class EchoQnADialog : QnAMakerDialog
  {
  }
}
