using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Tools.Storage
{
    public class StorageBlobClient
    {
        CloudBlobClient _client;
        CloudBlobContainer _container;

        public StorageBlobClient(string connectionString)
        {
            if(CloudStorageAccount.TryParse(connectionString, out var storageAccount))
            {
                _client = storageAccount.CreateCloudBlobClient();
            }
            else
            {
                throw new ArgumentException("Can farse blob connection string from " + connectionString);
            }
        }

        public async Task<StorageBlobClient> SetContainer(string containerName, bool createIfNull = false)
        {
            if (_client == null)
                throw new Exception();

            _container = _client.GetContainerReference(containerName);
            if (createIfNull)
            {
                if(await _container.CreateIfNotExistsAsync())
                {
                    await _container.SetPermissionsAsync(new BlobContainerPermissions
                    {
                        PublicAccess = BlobContainerPublicAccessType.Blob,
                    });
                }
            }

            return this;
        }

        public async Task<string> WriteText(string content)
        {
            var name = Guid.NewGuid().ToString();
            var response = await WriteText(name, content);

            if (response)
                return name;

            return string.Empty;
        }

        public async Task<bool> WriteText(string name, string content)
        {
            if (_container == null)
                throw new Exception();

            var reference = _container.GetBlockBlobReference(name);
            try
            {
                await reference.UploadTextAsync(content);
                return true;
            }
            catch(Exception ex)
            {
                var e = ex;
            }

            return false;
        }

        public async Task<string> ReadText(string name)
        {
            if (_container == null)
                throw new Exception();

            var reference = _container.GetBlockBlobReference(name);
            if (reference == null)
                return string.Empty;

            var result = await reference.DownloadTextAsync();
            return result;
        }

        public async Task<bool> Delete(string name)
        {
            if (_container == null)
                throw new Exception();

            var reference = _container.GetBlockBlobReference(name);
            try
            {
                await reference.DeleteIfExistsAsync();
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}
