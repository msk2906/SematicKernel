using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel;
using Plugins;
using Microsoft.SemanticKernel.PromptTemplates.Handlebars;
using Microsoft.SemanticKernel.Planning.Handlebars;
using ConsoleApp4.Plugin;

var builder = Kernel.CreateBuilder().AddAzureOpenAIChatCompletion("chatdj", "https://azoaidj.openai.azure.com/", "7f629b03e6524736a64127d31abf5029");
builder.Plugins.AddFromType<MathPlugin>();
builder.Plugins.AddFromType<DateandTimePlugin>();
Kernel kernel = builder.Build();

ChatHistory history = new(); 
var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

#pragma warning disable
var planner = new HandlebarsPlanner();

Console.Write("User > ");
string? userInput;
while ((userInput = Console.ReadLine()) != null)
{
    history.AddUserMessage(userInput);

    var plan = await planner.CreatePlanAsync(kernel, userInput);
    Console.WriteLine(plan);
    var res = (await plan.InvokeAsync(kernel)).Trim();


    OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
    {
        ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
    };

    var result = chatCompletionService.GetStreamingChatMessageContentsAsync(
                        history,
                        executionSettings: openAIPromptExecutionSettings,
                        kernel: kernel);

    string fullMessage = "";
    var first = true;
    await foreach (var content in result)
    {
        if (content.Role.HasValue && first)
        {
            Console.Write("Assistant > ");
            first = false;
        }
        Console.Write(content.Content);
        fullMessage += content.Content;
    }
    Console.WriteLine();

    history.AddAssistantMessage(fullMessage);

    Console.Write("User > ");
}