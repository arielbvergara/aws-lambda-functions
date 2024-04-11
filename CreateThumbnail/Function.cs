using Amazon.Lambda.Core;
using Amazon.Lambda.S3Events;
using Amazon.S3;
using Amazon.S3.Model;
using LambdaFunctionsModels;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace CreateThumbnail;

public class Function
{
    IAmazonS3 S3Client { get; set; }

    /// <summary>
    /// Default constructor. This constructor is used by Lambda to construct the instance. When invoked in a Lambda environment
    /// the AWS credentials will come from the IAM role associated with the function and the AWS region will be set to the
    /// region the Lambda function is executed in.
    /// </summary>
    public Function()
    {
        S3Client = new AmazonS3Client();
    }

    /// <summary>
    /// Constructs an instance with a preconfigured S3 client. This can be used for testing outside of the Lambda environment.
    /// </summary>
    /// <param name="s3Client">The service client to access Amazon S3.</param>
    public Function(IAmazonS3 s3Client)
    {
        S3Client = s3Client;
    }

    /// <summary>
    /// This method is called for every Lambda invocation. This method takes in an S3 event object and can be used 
    /// to respond to S3 notifications.
    /// </summary>
    /// <param name="event">The event for the Lambda function handler to process.</param>
    /// <param name="context">The ILambdaContext that provides methods for logging and describing the Lambda environment.</param>
    /// <returns></returns>
    public async Task<List<ImageS3Values>> FunctionHandler(S3Event @event, ILambdaContext context)
    {
        var result = new List<ImageS3Values>();
        var eventRecords = @event.Records ?? new List<S3Event.S3EventNotificationRecord>();
        foreach (var record in eventRecords)
        {
            var s3Event = record.S3;
            if (s3Event == null)
            {
                continue;
            }
            
            // Get bucket name and object key
            var bucketName = s3Event.Bucket.Name;
            var objectKey = s3Event.Object.Key;

            // Check if the object is in the specific folder you want to monitor
            if (!objectKey.StartsWith("images/"))
            {
                continue;
            }
            
            // Download the object from S3
            var getObjectRequest = new GetObjectRequest()
            {
                BucketName = bucketName,
                Key = objectKey
            };

            try
            {
                using var getObjectResponse = await S3Client.GetObjectAsync(getObjectRequest);
                await using var originalImageStream = getObjectResponse.ResponseStream;
                // Generate a thumbnail
                await using var thumbnailStream = await GenerateThumbnailAsync(originalImageStream);
                // Upload the thumbnail to a different folder in the same bucket
                var putObjectRequest = new PutObjectRequest
                {
                    BucketName = bucketName,
                    Key = objectKey.Replace("images/", "thumbnails/"),
                    InputStream = thumbnailStream,
                    ContentType = "image/jpeg"
                };

                var response = await S3Client.PutObjectAsync(putObjectRequest);

                context.Logger.LogInformation($"Thumbnail created successfully. {response?.VersionId}");
                
                result.Add(new ImageS3Values()
                {
                    ImageId = Guid.NewGuid().ToString(),
                    ImageUrl = objectKey,
                    ImageThumbnailUrl = objectKey.Replace("images/", "thumbnails/")
                });
            }
            catch (Exception e)
            {
                context.Logger.LogError(
                    $"Error getting object {s3Event.Object.Key} from bucket {s3Event.Bucket.Name}. Make sure they exist and your bucket is in the same region as this function.");
                context.Logger.LogError(e.Message);
                context.Logger.LogError(e.StackTrace);
                throw;
            }
        }

        return result;
    }
    
    private async Task<Stream> GenerateThumbnailAsync(Stream originalImageStream)
    {
        using var image = await Image.LoadAsync(originalImageStream);
        
        // Define the dimensions for the thumbnail
        var thumbWidth = (int)(image.Width * (30) / 100.0);
        var thumbHeight = (int)(image.Height * (30) / 100.0);
        
        // Resize the image to create a thumbnail
        image.Mutate(x => x.Resize(thumbWidth, thumbHeight));

        // Save the thumbnail to a memory stream
        var thumbnailStream = new MemoryStream();
        await image.SaveAsJpegAsync(thumbnailStream);
        thumbnailStream.Position = 0; // Reset the position for reading
        return thumbnailStream;
    }
}