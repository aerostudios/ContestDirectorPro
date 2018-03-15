//---------------------------------------------------------------
// Date: 12/21/2017
// Rights: 
// FileName: RepositoryBase.cs
//---------------------------------------------------------------

namespace CDP.Repository.WindowsStorage
{
    using CDP.AppDomain;
    using CDP.Common;
    using CDP.Common.Caching;
    using CDP.Common.Logging;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Base class implementations for repository classes.
    /// </summary>
    public abstract class RepositoryBase
    {
        /// <summary>
        /// The file store
        /// </summary>
        protected readonly WindowsFileManager FileManager;

        /// <summary>
        /// The cache
        /// </summary>
        protected readonly ICache cache;

        /// <summary>
        /// The logger
        /// </summary>
        protected readonly ILoggingService logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryBase" /> class.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="cache">The cache.</param>
        /// <param name="logger">The logger.</param>
        public RepositoryBase(string fileName, ICache cache, ILoggingService logger)
        {
            Validate.IsNotNullOrEmpty(fileName, nameof(fileName));
            this.FileManager = new WindowsFileManager(fileName);
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
                var fileContent = await this.FileManager.RetrieveFileContent();

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

            return listToReturn;
        }

        /// <summary>
        /// Writes all.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="allContent">All content.</param>
        /// <returns></returns>
        public virtual async Task<bool> WriteAll<T>(List<T> allContent)
        {
            var success = await this.FileManager.SaveObjectToFile<List<T>>(allContent);

            if (!success)
            {
                var ex = new Exception($"Could not save content to file.  File Name: {this.FileManager.FileName}.");
                logger.LogException(ex);
                throw ex;
            }

            return true;
        }
    }
}