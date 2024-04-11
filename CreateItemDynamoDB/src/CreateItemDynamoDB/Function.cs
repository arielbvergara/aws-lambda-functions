using System.Text.Json;
using Amazon.Lambda.Core;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using LambdaFunctionsModels.Parameters;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace CreateItemDynamoDB;

public class Function
{
    private readonly IAmazonDynamoDB _dynamoDbClient = new AmazonDynamoDBClient();

    public async Task FunctionHandler(DynamoDBItem param, ILambdaContext context)
    {
        context.Logger.LogLine($"Starting CreateItemDynamoDB...{JsonSerializer.Serialize(param)}");

        if (param.Value != null && !param.Value.Any())
        {
            context.Logger.LogLine($"Finishing CreateItemDynamoDB. No value sent.");
            return;
        }
        
        foreach (var image in param.Value)
        {   
            context.Logger.LogLine($"Processing {image.ImageId}.");
            // Create DynamoDB item
            var item = new Dictionary<string, AttributeValue>
            {
                { "ImageId", new AttributeValue { S = image.ImageId } },
                { "ImageUrl", new AttributeValue { S = image.ImageUrl } },
                { "ImageThumbnailUrl", new AttributeValue { S = image.ImageThumbnailUrl } }
            };

            // Create PutItem request
            var request = new PutItemRequest
            {
                TableName = "innovalouvres",
                Item = item
            };

            // Insert item into DynamoDB table
            await _dynamoDbClient.PutItemAsync(request);

            context.Logger.LogLine($"Image { image.ImageId} added to DynamoDB successfully");
        }
        
        context.Logger.LogLine("Finishing CreateItemDynamoDB...");
    }
}