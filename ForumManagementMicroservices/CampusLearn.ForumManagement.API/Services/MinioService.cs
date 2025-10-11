using Minio;
using Minio.DataModel.Args;
using Minio.DataModel.Result;

namespace CampusLearn.ForumManagement.API.Services;

public class MinioService(IMinioClient minio)
{
    private const string bucketName = "forum";

    //retrieves all buckets in minio, for testing connection only
    public async Task<ListAllMyBucketsResult> ListBucketsAsync()
    {
        var bucketsResult = await minio.ListBucketsAsync().ConfigureAwait(false);
        foreach (var bucket in bucketsResult.Buckets)
        {
            Console.WriteLine($"Bucket: {bucket.Name}");
        }
        return bucketsResult;
    }


    // File uploader task.
    public async Task UploadFileAsync(string objectName, Stream fileStream, string contentType = "application/octet-stream")
    {
        // Validate stream
        if (fileStream == null)
            throw new ArgumentException("File stream is null");

        if (!fileStream.CanRead)
            throw new ArgumentException("File stream is not readable");

        // Reset stream position
        if (fileStream.CanSeek)
            fileStream.Position = 0;

        Console.WriteLine($"Uploading file: {objectName}, Size: {fileStream.Length}, ContentType: {contentType}");

        // Check bucket exists
        var beArgs = new BucketExistsArgs().WithBucket(bucketName);
        bool found = await minio.BucketExistsAsync(beArgs).ConfigureAwait(false);
        Console.WriteLine($"Bucket exists: {found}");

        if (!found)
        {
            var mbArgs = new MakeBucketArgs().WithBucket(bucketName);
            await minio.MakeBucketAsync(mbArgs).ConfigureAwait(false);
            Console.WriteLine($"Created bucket: {bucketName}");
        }

        // Upload the stream to bucket
        var putObjectArgs = new PutObjectArgs()
            .WithBucket(bucketName)
            .WithObject(objectName)
            .WithStreamData(fileStream)
            .WithObjectSize(fileStream.Length)
            .WithContentType(contentType);

        await minio.PutObjectAsync(putObjectArgs).ConfigureAwait(false);
        Console.WriteLine("Upload successful");
    }


    //Get file 
    public async Task<(Stream, string)> OnRetrieveFile(string fileName)
    {
        var memoryStream = new MemoryStream();
        string contentType = "application/octet-stream"; // Default

        var getObjectArgs = new GetObjectArgs()
            .WithBucket(bucketName)
            .WithObject(fileName)
            .WithCallbackStream(stream =>
            {
                stream.CopyTo(memoryStream);
            });

        await minio.GetObjectAsync(getObjectArgs);
        memoryStream.Position = 0;

        // Get content type from MinIO object metadata
        var statArgs = new StatObjectArgs()
            .WithBucket(bucketName)
            .WithObject(fileName);

        var objectStat = await minio.StatObjectAsync(statArgs);
        contentType = objectStat.ContentType ?? "application/octet-stream";

        return (memoryStream, contentType);
    }
}
