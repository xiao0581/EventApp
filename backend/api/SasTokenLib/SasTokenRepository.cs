using System;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;

namespace SasTokenLib
{
    using System;
    using Azure.Storage;
    using Azure.Storage.Blobs;
    using Azure.Storage.Sas;

    namespace SasTokenLib
    {
        public class SasTokenRepository
        {
            public string GenerateSas(string accountName, string accountKey, string containerName, SasTokenRequest request)
            {
                var blobServiceClient = new BlobServiceClient(new Uri($"https://{accountName}.blob.core.windows.net"),
                                                              new StorageSharedKeyCredential(accountName, accountKey));
                var containerClient = blobServiceClient.GetBlobContainerClient(containerName);

               
                const string storageVersion = "2023-01-03";

               
                DateTimeOffset startOn = DateTimeOffset.UtcNow.AddMinutes(-5).ToUniversalTime();
                DateTimeOffset expiresOn = DateTimeOffset.UtcNow.AddDays(1).ToUniversalTime(); 

                BlobSasPermissions sasPermissions = ParsePermissions(request.Permission);

             
                BlobSasBuilder sasBuilder = new BlobSasBuilder
                {
                    StartsOn = startOn,
                    ExpiresOn = expiresOn,
                    Version = storageVersion, 
                    
                };

                if (string.IsNullOrEmpty(request.BlobName))
                {
                    sasBuilder.BlobContainerName = containerName;
                    sasBuilder.SetPermissions(sasPermissions);
                }
                else
                {
                    sasBuilder.BlobContainerName = containerName;
                    sasBuilder.BlobName = request.BlobName;
                    sasBuilder.SetPermissions(sasPermissions);
                }

               
                StorageSharedKeyCredential credential = new StorageSharedKeyCredential(accountName, accountKey);
                string sasToken = "?" + sasBuilder.ToSasQueryParameters(credential).ToString();

               
                Console.WriteLine("Generated SAS Token: " + sasToken);
                Console.WriteLine($"StartsOn: {startOn:o}"); 
                Console.WriteLine($"ExpiresOn: {expiresOn:o}");

                return sasToken;
            }

            private BlobSasPermissions ParsePermissions(string permission)
            {
                BlobSasPermissions permissions = new BlobSasPermissions();
                if (string.IsNullOrEmpty(permission)) return BlobSasPermissions.Read;

                foreach (char c in permission.ToLower())
                {
                    switch (c)
                    {
                        case 'r': permissions |= BlobSasPermissions.Read; break;
                        case 'w': permissions |= BlobSasPermissions.Write; break;
                        case 'd': permissions |= BlobSasPermissions.Delete; break;
                        case 'l': permissions |= BlobSasPermissions.List; break;
                        default: throw new ArgumentException("Invalid permission flag. Use 'r' (Read), 'w' (Write), 'd' (Delete), 'l' (List).");
                    }
                }

                return permissions;
            }
        }
    }


}
