//---------------------------------------------------------------
// Date: 12/21/2017
// Rights: 
// FileName: RepositoryBase.cs
//---------------------------------------------------------------

namespace CDP.ContestHost.FileStoreRepository
{
    using CDP.AppDomain;
    using CDP.Common;
    using CDP.Common.Caching;
    using CDP.Common.Logging;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Base class implementations for repository classes.
    /// </summary>
    public abstract class RepositoryBase
    {
        /// <summary>
        /// The storage file name
        /// </summary>
        protected readonly string storageFileName;

        /// <summary>
        /// The cache
        /// </summary>
        protected readonly ICache cache;

        /// <summary>
        /// The logger
        /// </summary>
        protected readonly ILoggingService logger;

        /// <summary>
        /// The synchronize lock
        /// </summary>
        private SemaphoreSlim SyncLock = new SemaphoreSlim(1, 1);

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryBase" /> class.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="cache">The cache.</param>
        /// <param name="logger">The logger.</param>
        public RepositoryBase(string fileName, ICache cache, ILoggingService logger)
        {
            Validate.IsNotNullOrEmpty(fileName, nameof(fileName));
            this.storageFileName = fileName;
            this.cache = cache;
            this.logger = logger;
        }

        /// <summary>
        /// Generates an identifier.  Default is a GUID string, but you can override to whatever you would like.
        /// </summary>
        /// <returns></returns>
        public virtual string GenerateId() => Guid.NewGuid().ToString();

        /// <summary>
        /// Builds an Error response for the specified value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <param name="ex">The ex.</param>
        /// <returns></returns>
        public virtual Result<T> Error<T>(T value, Exception ex)
        {
            logger.LogException(ex);
            var result = new Result<T>(value, new ResultError(ex));
            return result;
        }

        /// <summary>
        /// Builds an Error response for the specified value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <returns></returns>
        public virtual Result<T> Error<T>(T value, string errorMessage)
        {
            logger.LogException(new Exception(errorMessage));
            var result = new Result<T>(value, new ResultError(errorMessage));
            return result;
        }

        /// <summary>
        /// Builds a Success response for the specified value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <param name="method">The method.</param>
        /// <returns></returns>
        public virtual Result<T> Success<T>(T value, string method)
        {
            logger?.LogTrace($"The call to {method} returned successfully.");
            return new Result<T>(value);
        }

        /// <summary>
        /// Gets all.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual async Task<List<T>> GetAll<T>()
        {
            var listToReturn = default(List<T>);

            try
            {
                if (!File.Exists(storageFileName))
                {
                    return new List<T>();
                }

                var fileContent = File.ReadAllText(storageFileName);

                if (string.IsNullOrWhiteSpace(fileContent))
                {
                    return new List<T>();
                }

                listToReturn = JsonConvert.DeserializeObject<List<T>>(fileContent);
            }
            catch (FieldAccessException faEx)
            {
                logger.LogException(faEx);
                logger.LogTrace("Failed to open file.");
                throw faEx;
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
                logger.LogTrace("Error while reading file.");
                throw ex;
            }

            return await Task.FromResult(listToReturn);
        }

        /// <summary>
        /// Writes all.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="allContent">All content.</param>
        /// <returns></returns>
        public virtual async Task<bool> WriteAll<T>(List<T> allContent)
        {
            var success = await SaveObjectToFile<List<T>>(allContent);

            if (!success)
            {
                var ex = new Exception($"Could not save content to file.  File Name: {this.storageFileName}.");
                logger.LogException(ex);
                throw ex;
            }

            return true;
        }

        /// <summary>
        /// Saves the object to file.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="itemToSave">The item to save.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">fileName</exception>
        /// <exception cref="System.Exception"></exception>
        private async Task<bool> SaveObjectToFile<T>(T itemToSave)
        {
            if (string.IsNullOrEmpty(this.storageFileName))
            {
                throw new ArgumentNullException("fileName");
            }

            var success = false;

            try
            {
                // Let's try and lock when doing file ops...
                await SyncLock.WaitAsync();

                string json = JsonConvert.SerializeObject(itemToSave);
                if (File.Exists(storageFileName))
                {
                    using (var fileStream = new FileStream(storageFileName, FileMode.OpenOrCreate))
                    {
                        fileStream.SetLength(0);
                        fileStream.Close();
                    };
                }

                using (var fileStream = new FileStream(storageFileName, FileMode.OpenOrCreate))
                {
                    var size = fileStream.Length;
                    if (json.Length < size)
                    {

                    }
                    using (var writer = new StreamWriter(fileStream))
                    {
                        writer.WriteLine(json);
                        writer.Flush();
                        writer.Close();
                    }

                    fileStream.Close();
                };

                success = true;
                
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to save the file: {storageFileName}", ex);
            }
            finally
            {
                SyncLock.Release();
            }

            return success;
        }
    }
}
