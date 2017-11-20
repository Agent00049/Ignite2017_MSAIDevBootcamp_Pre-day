using System;
using System.Configuration;
using Microsoft.Bot.Builder.CognitiveServices.QnAMaker;

namespace Bot.Dialogs
{
  [Serializable]
  public class QnADialog : QnAMakerDialog
  {
    public QnADialog()
      : base(new QnAMakerService(new QnAMakerAttribute(ConfigurationManager.AppSettings["QnASubscriptionKey"], ConfigurationManager.AppSettings["QnAKnowledgebaseId"], "Sorry but I can't find an answer. Could you please rephrase your question?", 0.1)))
    {
    }
  }
}