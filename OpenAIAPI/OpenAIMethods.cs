using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Configuration;

namespace OpenAIAPI;

public static class OpenAIMethods {

    

    public static async Task<string> GenerateEmailSummary(string emailContents, string openAIAPIKey) {
        var chatCompletionsOptions = new ChatCompletionsOptions() {
            DeploymentName = "gpt-3.5-turbo",
            Messages = {
                new ChatRequestSystemMessage("You are an email assistant tasked with summarizing email contents"),
                new ChatRequestUserMessage(emailContents)
            }
        };
        OpenAIClient client = GenerateClient(openAIAPIKey);
        Response<ChatCompletions> response = await client.GetChatCompletionsAsync(chatCompletionsOptions);
        ChatResponseMessage responseMessage = response.Value.Choices[0].Message;
        return responseMessage.Content;
    }

    private static OpenAIClient GenerateClient(string openAIAPIKey) {
        OpenAIClient client = new OpenAIClient(openAIAPIKey);
        return client;
    }

}