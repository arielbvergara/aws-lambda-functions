namespace LambdaFunctionsModels.Parameters;

public class DynamoDBItem
{
    public List<ImageS3Values>? Value { get; set; }
}

public class ImageS3Values
{
    public string ImageId { get; set; }
    public string ImageUrl { get; set; }
    public string ImageThumbnailUrl { get; set; }
}