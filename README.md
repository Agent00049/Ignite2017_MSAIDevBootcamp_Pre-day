# Microsoft Ignite 2017: AI Developer Bootcamp Pre-day

## Overview

This repository contains the materials presented at the AI Developer Bootcamp Pre-day training class at Microsoft Ignite 2017.

Also, here are some links to sites mentioned during the class:

* Running Bots as pre-compiled Azure Functions: see issue [here](https://github.com/Microsoft/BotBuilder/issues/2407) and add the `AzureFunctionsResolveAssembly` class to work around the issue.
* LUIS: [language support](https://docs.microsoft.com/en-us/azure/cognitive-services/luis/luis-concept-language-support)
* Text Analytics and U-SQL: [tutorial](https://github.com/Azure-Samples/usql-cognitive-text-hello-world)
* Trying Cognitive Services without Azure: sign up [here](https://azure.microsoft.com/en-us/try/cognitive-services/)

## Examples

Make sure to update the config files with the required configuration values before attempting to run the examples. The only exception here is the properties that are prefixed with `x`. These are only required if you want to run the bot with ngrok and use a public channel rather than the emulator.

Also, keep in mind these are intended to be run as demos. To see the full functionality of the sample, you'll need to uncomment the relevant blocks of code.

For those demos that require [LUIS](www.luis.ai) or [QnA Maker](qnamaker.ai), you'll need to deploy the service first and then add the app ID and key to the relevant attributes in code. The LUIS model is under `examples/02_Bots/03_luis` and the QnA Maker model is under `examples/04_Knowledge/01_qna`.

## Contributing

This project welcomes contributions and suggestions.  Most contributions require you to agree to a
Contributor License Agreement (CLA) declaring that you have the right to, and actually do, grant us
the rights to use your contribution. For details, visit https://cla.microsoft.com.

When you submit a pull request, a CLA-bot will automatically determine whether you need to provide
a CLA and decorate the PR appropriately (e.g., label, comment). Simply follow the instructions
provided by the bot. You will only need to do this once across all repos using our CLA.

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).
For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or
contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.
