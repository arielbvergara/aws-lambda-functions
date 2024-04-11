using Amazon.Lambda.Core;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using LambdaFunctionsModels;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace CreateItemDynamoDB;

public class Function
{
    private readonly IAmazonDynamoDB _dynamoDbClient = new AmazonDynamoDBClient();

    public async Task FunctionHandler(List<ImageS3Values> imageS3Values, ILambdaContext context)
    {
        foreach (var image in imageS3Values)
        {   
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
    }
}