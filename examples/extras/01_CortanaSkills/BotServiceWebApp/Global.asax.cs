using System.Web.Http;
using Microsoft.Bot.Builder.Azure;

namespace Bot
{
  public class WebApiApplication : System.Web.HttpApplication
  {
    protected void Application_Start()
    {
      GlobalConfiguration.Configure(WebApiConfig.Register);
      BotService.Initialize();
    }
  }
}
