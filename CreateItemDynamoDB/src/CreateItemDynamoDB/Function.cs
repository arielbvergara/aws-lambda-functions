using Amazon.Lambda.Core;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace CreateItemDynamoDB;

public class Function
{
    private readonly IAmazonDynamoDB _dynamoDbClient = new AmazonDynamoDBClient();

    public async Task FunctionHandler(ImageS3Values imageS3Values, ILambdaContext context)
    {
        try
        {
            // Create DynamoDB item
            var item = new Dictionary<string, AttributeValue>
            {
                { "ImageId", new AttributeValue { S = imageS3Values.ImageId } },
                { "ImageUrl", new AttributeValue { S = imageS3Values.ImageUrl } },
                { "ImageThumbnailUrl", new AttributeValue { S = imageS3Values.ImageThumbnailUrl } }
            };

            // Create PutItem request
            var request = new PutItemRequest
            {
                TableName = "innovalouvres",
                Item = item
            };

            // Insert item into DynamoDB table
            await _dynamoDbClient.PutItemAsync(request);

            context.Logger.LogLine("Item added to DynamoDB successfully");
        }
        catch (Exception ex)
        {
            context.Logger.LogLine($"Error adding item to DynamoDB: {ex.Message}");
            throw;
        }
    }
}

public class ImageS3Values
{
    public string ImageId { get; set; }
    public string ImageUrl { get; set; }
    public string ImageThumbnailUrl { get; set; }
}


// {
//   "image_id": {
//     "S": "af5dc1d2-a06c-458e-aaef-50fadd7a1812"
//   },
//   "image_thumbnail_url": {
//     "S": "https://innovalouvres-s3.s3.eu-central-1.amazonaws.com/thumbnails/roof1.jpeg?response-content-disposition=inline&X-Amz-Security-Token=IQoJb3JpZ2luX2VjEPT%2F%2F%2F%2F%2F%2F%2F%2F%2F%2FwEaDGV1LWNlbnRyYWwtMSJHMEUCIEEDCNSHUK5jRdA8ZCwhp7udTkdGhyHYNEeTy0RcNjUJAiEA69E9mKtSHgmplgDA1Bqi94tbEeLq8HZ3tDEmclSVjpMq%2BwIILRAAGgw5OTIzODI0MTU2MzQiDM2PU5TiuUxTVj8qyCrYAp%2FSnt07GF%2F7KKohdqMVamBVkRhIL0FHZOFZ%2BSIdWDTrMX1Rr1qLSMPYhuz79VEWRFBK9V71Kdof0GDMGhErVbDNC922eNGYGLCZyYdYg5o%2BfRSj2eJXFwAmNAmCfa0kEcyxCobeAsWYwV%2FkyGpJpFMhkJl%2BL55RGuAVGwbpagEWO0F5xJU4HiETQe0BDzBQlPWK6gz2zlbzD9%2Fm5AADZnm2lGephnTIUUTfgmNBaCiIQwwU%2Buw%2FI%2BKf1aGe51S690tIg3XJZ72QHoFXzISd%2F%2BpDQhW4yJWQmRHcs3wAfoO%2BNsfAFhyvsIuAEsKTabStcmnkYPl2bqvbv9CXNReDi20bazaJkrWIy8UJljvz3Jmm%2FZ6px2qmRByNy8h1BuHRHCJSOeTyvoWWo3q3AdTBzSeANgdW%2BEi%2FWL3ff%2BmoBvB6udz%2BqzjzmPm5zL5qCkHMINGC5h1gsgUkMMzm2bAGOrMClwlK9PEthOU4ZRccoIlIgpj6e3c7kgZ8CMgpWGrOci4WzvIMHWEFB0nYgSAP0xM2HNnLkNmbyxvreyaGMS%2Bp0AFwOW%2BT1lynMhiTBsxviT1mH3cIcXyn3Y7NDdAEsrVOfixGpx0cSdbiGrL%2BDtF7jslCDNCfyxzzbzuNcncObdUOGcKyJp0ImqOooalKdHzUPELEIYGXm%2BCeop8e77QwW%2BDe%2FlcS6ZPWzlxbnqmweOV8AQ6zrExiB2BN1SS7VdnFyMSDiFgL6eSa6rZ5oWU2VpCQrZLKQ9najbiwy4ZRRvupi7%2B6OxKuQ4TpmXgqdzgF2GEubsq7vf6v1Wtc86ZCTpBIBdW0nUmoM4Ll40RX7X1pNRXyZjk9riS7quzj%2FH%2FPAsE2NtyKx2QIsUYUnKHrpQRM3A%3D%3D&X-Amz-Algorithm=AWS4-HMAC-SHA256&X-Amz-Date=20240410T201830Z&X-Amz-SignedHeaders=host&X-Amz-Expires=300&X-Amz-Credential=ASIA6ODUZ6MJEU4EPOWT%2F20240410%2Feu-central-1%2Fs3%2Faws4_request&X-Amz-Signature=dfcdf86052c030fc50daf3b715a14cea57f2e745634704960d9fe4ee83f66f67"
//   },
//   "image_url": {
//     "S": "https://innovalouvres-s3.s3.eu-central-1.amazonaws.com/images/roof1.jpeg?response-content-disposition=inline&X-Amz-Security-Token=IQoJb3JpZ2luX2VjEPT%2F%2F%2F%2F%2F%2F%2F%2F%2F%2FwEaDGV1LWNlbnRyYWwtMSJHMEUCIEEDCNSHUK5jRdA8ZCwhp7udTkdGhyHYNEeTy0RcNjUJAiEA69E9mKtSHgmplgDA1Bqi94tbEeLq8HZ3tDEmclSVjpMq%2BwIILRAAGgw5OTIzODI0MTU2MzQiDM2PU5TiuUxTVj8qyCrYAp%2FSnt07GF%2F7KKohdqMVamBVkRhIL0FHZOFZ%2BSIdWDTrMX1Rr1qLSMPYhuz79VEWRFBK9V71Kdof0GDMGhErVbDNC922eNGYGLCZyYdYg5o%2BfRSj2eJXFwAmNAmCfa0kEcyxCobeAsWYwV%2FkyGpJpFMhkJl%2BL55RGuAVGwbpagEWO0F5xJU4HiETQe0BDzBQlPWK6gz2zlbzD9%2Fm5AADZnm2lGephnTIUUTfgmNBaCiIQwwU%2Buw%2FI%2BKf1aGe51S690tIg3XJZ72QHoFXzISd%2F%2BpDQhW4yJWQmRHcs3wAfoO%2BNsfAFhyvsIuAEsKTabStcmnkYPl2bqvbv9CXNReDi20bazaJkrWIy8UJljvz3Jmm%2FZ6px2qmRByNy8h1BuHRHCJSOeTyvoWWo3q3AdTBzSeANgdW%2BEi%2FWL3ff%2BmoBvB6udz%2BqzjzmPm5zL5qCkHMINGC5h1gsgUkMMzm2bAGOrMClwlK9PEthOU4ZRccoIlIgpj6e3c7kgZ8CMgpWGrOci4WzvIMHWEFB0nYgSAP0xM2HNnLkNmbyxvreyaGMS%2Bp0AFwOW%2BT1lynMhiTBsxviT1mH3cIcXyn3Y7NDdAEsrVOfixGpx0cSdbiGrL%2BDtF7jslCDNCfyxzzbzuNcncObdUOGcKyJp0ImqOooalKdHzUPELEIYGXm%2BCeop8e77QwW%2BDe%2FlcS6ZPWzlxbnqmweOV8AQ6zrExiB2BN1SS7VdnFyMSDiFgL6eSa6rZ5oWU2VpCQrZLKQ9najbiwy4ZRRvupi7%2B6OxKuQ4TpmXgqdzgF2GEubsq7vf6v1Wtc86ZCTpBIBdW0nUmoM4Ll40RX7X1pNRXyZjk9riS7quzj%2FH%2FPAsE2NtyKx2QIsUYUnKHrpQRM3A%3D%3D&X-Amz-Algorithm=AWS4-HMAC-SHA256&X-Amz-Date=20240410T201531Z&X-Amz-SignedHeaders=host&X-Amz-Expires=300&X-Amz-Credential=ASIA6ODUZ6MJEU4EPOWT%2F20240410%2Feu-central-1%2Fs3%2Faws4_request&X-Amz-Signature=92174e0fc2961e576e9d16fb4b2ee885dd09eaffc8f9df95b08efc509125f27c"
//   }
// }