//---------------------------------------------------------------
// Created By: Rick Rogahn
// Date: 12/21/2017
// Rights: 
// FileName: RepositoryBase.cs
//---------------------------------------------------------------

namespace CDP.Repository.FileSystem
{
    using CDP.AppDomain;
    using CDP.Common;
    using CDP.Common.Logging;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    /// Base class implementations for repository classes.
    /// </summary>
    public abstract class RepositoryBase
    {
        /// <summary>
        /// The file path
        /// </summary>
        protected readonly string filePath = string.Empty;

        /// <summary>
        /// The logger
        /// </summary>
        protected readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryBase"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public RepositoryBase(ILogger logger)
        {
            Validate.IsNotNull(logger, nameof(logger));
            this.logger = logger;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryBase"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public RepositoryBase(string filePath, ILogger logger) : this(logger)
        {
            Validate.IsNotNullOrEmpty(filePath, nameof(filePath));
            this.filePath = filePath;
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
            logger.LogTrace($"The call to {method} returned successfully.");
            return new Result<T>(value);
        }

        /// <summary>
        /// Gets all.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual Task<List<T>> GetAll<T>() => Task.Run(() =>
                                                              {
                                                                  if (!File.Exists(this.filePath))
                                                                  {
                                                                      return new List<T>();
                                                                  }

                                                                  var listToReturn = default(List<T>);

                                                                  try
                                                                  {

                                                                      var fileContent = File.ReadAllText(this.filePath);

                                                                      if (string.IsNullOrEmpty(fileContent))
                                                                      {
                                                                          return new List<T>();
                                                                      }

                                                                      listToReturn = new List<T>();
                                                                      listToReturn = JsonConvert.DeserializeObject<List<T>>(fileContent);
                                                                  }
                                                                  catch (FieldAccessException faEx)
                                                                  {
                                                                      logger.LogException(faEx);
                                                                      logger.LogTrace("Failed to open file.");
                                                                      return new List<T>();
                                                                  }
                                                                  catch (Exception ex)
                                                                  {
                                                                      logger.LogException(ex);
                                                                      logger.LogTrace("Error while reading file.");
                                                                      return new List<T>();
                                                                  }

                                                                  return listToReturn;
                                                              });

        /// <summary>
        /// Writes all.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="allContent">All content.</param>
        /// <returns></returns>
        public virtual Task<bool> WriteAll<T>(List<T> allContent) => Task.Run(() =>
                                                                               {
                                                                                   // FYI - this could throw, needs try catch around caller.
                                                                                   // Clear out the file
                                                                                   File.WriteAllText(this.filePath, string.Empty);
                                                                                   // Write.
                                                                                   File.WriteAllText(this.filePath, JsonConvert.SerializeObject(allContent));

                                                                                   return true;
                                                                               });
    }
}