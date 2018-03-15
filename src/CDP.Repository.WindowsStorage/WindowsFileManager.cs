//---------------------------------------------------------------
// Date: 1/10/2018
// Rights: 
// FileName: WindowsFileStore.cs
//---------------------------------------------------------------

namespace CDP.Repository.WindowsStorage
{
    using CDP.Common;
    using Newtonsoft.Json;
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Windows.Storage;

    /// <summary>
    /// Describes a class that can manipulate files in the windows system.
    /// </summary>
    public class WindowsFileManager
    {
        /// <summary>
        /// The file name
        /// </summary>
        private readonly string fileName = string.Empty;

        /// <summary>
        /// The local folder
        /// </summary>
        private StorageFolder localFolder = ApplicationData.Current.LocalFolder;

        /// <summary>
        /// Gets the name of the file.
        /// </summary>
        /// <value>
        /// The name of the file.
        /// </value>
        public string FileName { get { return fileName; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowsFileManager"/> class.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        public WindowsFileManager(string fileName)
        {
            Validate.IsNotNull(fileName, nameof(fileName));
            this.fileName = fileName;
        }

        /// <summary>
        /// Creates the local file.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">filename</exception>
        public async Task<IStorageFile> CreateLocalFile()
        {
            var file = await localFolder.CreateFileAsync(this.fileName, CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(file, " ", Windows.Storage.Streams.UnicodeEncoding.Utf8);

            return file;
        }

        /// <summary>
        /// Retrieves the file.
        /// </summary>
        /// <returns>
        /// File requested.
        /// </returns>
        /// <exception cref="System.Exception"></exception>
        public async Task<string> RetrieveFileContent()
        {
            // Open the file if it exists from the local folder
            IStorageFile fileToGet = null;

            try
            {
                fileToGet = (await FileExists(localFolder, fileName)) ?
                    await StorageFile.GetFileFromPathAsync(Path.Combine(localFolder.Path, fileName)) :
                    await CreateLocalFile();

                return await FileIO.ReadTextAsync(fileToGet);

            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to read the file: {fileName}.  Message: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Saves the object to file.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="itemToSave">The item to save.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">fileName</exception>
        /// <exception cref="System.Exception"></exception>
        public async Task<bool> SaveObjectToFile<T>(T itemToSave)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException("fileName");
            }

            var success = false;

            try
            {
                string json = JsonConvert.SerializeObject(itemToSave);
                var exists = await FileExists(this.localFolder, fileName);

                IStorageFile fileToSave;
                fileToSave = (exists) ? await this.localFolder.GetFileAsync(fileName) : await CreateLocalFile();
                
                await FileIO.WriteTextAsync(fileToSave, json, Windows.Storage.Streams.UnicodeEncoding.Utf8);
                success = true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to save the file: {fileName}", ex);
            }

            return success;
        }

        /// <summary>
        /// Files the exists.
        /// </summary>
        /// <param name="folder">The folder.</param>
        /// <param name="itemName">Name of the item.</param>
        /// <returns></returns>
        private async Task<bool> FileExists(StorageFolder folder, string itemName)
        {
            try
            {
                IStorageItem item = await folder.TryGetItemAsync(itemName);
                return (item != null);
            }
            catch (Exception)
            { 
                return false;
            }
        }
    }
}
