namespace CampusLearn.MediaContent.API.Services;

public class MinioService
{
    private readonly IAmazonS3 s3Client;
    private readonly IConfiguration configuration;
    public MinioService(IConfiguration configuration)
    {
        this.configuration = configuration;
        var endpoint = this.configuration.GetValue<string>("MinIO:Endpoint");
        var accessKey = this.configuration.GetValue<string>("MinIO:AccessKey");
        var secretKey = this.configuration.GetValue<string>("MinIO:SecretKey");
        
        var config = new AmazonS3Config
        {
            ServiceURL = endpoint,
            ForcePathStyle = true,
            UseHttp = true
        };

        s3Client = new AmazonS3Client(accessKey,secretKey,config);
    }

    // checking if bucket exists
    private async Task<bool> BucketExistsAsync(string bucketName)
    {
        try
        {
            var response = await s3Client.ListBucketsAsync();
            return response.Buckets.Any(b => b.BucketName == bucketName);
        }
        catch
        {
            return false;
        }
    }

    // check if object exists
    private async Task<bool> ObjectExistsAsync(string bucketName, string objectName)
    {
        try
        {
            var request = new GetObjectMetadataRequest
            {
                BucketName = bucketName,
                Key = objectName
            };

            var response = await s3Client.GetObjectMetadataAsync(request);
            bool isObjectFound = (response.HttpStatusCode == System.Net.HttpStatusCode.OK);
            return isObjectFound;
        }
        catch (Amazon.S3.AmazonS3Exception ex)
        {
            bool isObjectNotfound = (ex.StatusCode == System.Net.HttpStatusCode.NotFound);
            if (isObjectNotfound) return false;
            return false;
        }
    }


    // create and update method
    public async Task UploadStreamAsync(string bucketName, string objectName, Stream data)
    {
        if (!await BucketExistsAsync(bucketName))
        {
            await s3Client.PutBucketAsync(new PutBucketRequest
            {
                BucketName = bucketName
            });
        }

        var putRequest = new PutObjectRequest
        {
            BucketName = bucketName,
            Key = objectName,
            InputStream = data
        };

        await s3Client.PutObjectAsync(putRequest);
    }

    // get method (returns) an encrypted url to the object
    public async Task<string> GetFileUrlAsync(string bucketName, string objectName)
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = bucketName,
            Key = objectName,
            Expires = DateTime.UtcNow.AddHours(1),
            Protocol = Protocol.HTTP, // Force HTTP
            Verb = HttpVerb.GET
        };

        // Override the URL to use localhost for external access
        var url = s3Client.GetPreSignedURL(request);

        // Replace internal address with localhost for external access
        url = url.Replace("http://minio:9000/", "http://localhost:9000/")
                .Replace("http://localhost:9000/", "http://localhost:9000/"); // Ensure localhost

        return url;
    }

    //direct streaming
    public async Task<GetObjectResponse> GetObjectStreamAsync(string bucketName, string objectName)
    {
        var request = new GetObjectRequest
        {
            BucketName = bucketName,
            Key = objectName
        };

        return await s3Client.GetObjectAsync(request);
    }

    //delete method
    public async Task DeleteFileAsync(string bucketName, string objectName)
    {
        var request = new DeleteObjectRequest
        {
            BucketName = bucketName,
            Key = objectName
        };

        await s3Client.DeleteObjectAsync(request);
    }
}
