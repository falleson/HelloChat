// See https://aka.ms/new-console-template for more information
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

var builder = Kernel.CreateBuilder();
builder.AddAzureOpenAIChatCompletion("gpt-35-turbo-16k",
    Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT")!,
    Environment.GetEnvironmentVariable("AZURE_OPENAI_KEY")!);

// Build the kernel
var kernel = builder.Build();

// Create chat history
ChatHistory history = new ChatHistory();
history.AddSystemMessage(@"You're a virtual assistant that helps people find information.");

// Get chat completion service
var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

// Start the conversation
while (true)
{
    // Get user input
    Console.ForegroundColor = ConsoleColor.White;
    Console.Write("User > ");
    history.AddUserMessage(Console.ReadLine()!);

    // Enable auto function calling
    OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
    {
        MaxTokens = 200
    };

    // Get the response from the AI
    var response = chatCompletionService.GetStreamingChatMessageContentsAsync(
                   history,
                   executionSettings: openAIPromptExecutionSettings,
                   kernel: kernel);


    Console.ForegroundColor = ConsoleColor.Green;
    Console.Write("\nAssistant > ");

    string combinedResponse = string.Empty;
    await foreach (var message in response)
    {
        //Write the response to the console
        Console.Write(message);
        combinedResponse += message;
    }

    Console.WriteLine();

    // Add the message from the agent to the chat history
    history.AddAssistantMessage(combinedResponse);
}
