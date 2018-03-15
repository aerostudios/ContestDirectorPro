//---------------------------------------------------------------
// Date: 12/21/2017
// Rights: 
// FileName: FileIOImportFileInteractor.cs
//---------------------------------------------------------------

namespace CDP.CoreApp.Features.FileIO.Commands
{
    using CDP.Common.Logging;
    using CDP.CoreApp.Interfaces.FileIO;
    using System;

    /// <summary>
    /// Handles importing contest data from a file.
    /// </summary>
    /// <seealso cref="CDP.CoreApp.InteractorBase" />
    public sealed class FileIOImportFileInteractor : InteractorBase
    {
        /// <summary>
        /// The file importer
        /// </summary>
        private readonly IFileImporter fileImporter;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileIOImportFileInteractor" /> class.
        /// </summary>
        /// <param name="fileImporter">The file importer.</param>
        /// <param name="logger">The logger.</param>
        /// <exception cref="System.ArgumentNullException">fileImporter</exception>
        public FileIOImportFileInteractor(IFileImporter fileImporter, ILoggingService logger) : base(logger)
        {
            this.fileImporter = fileImporter ?? throw new ArgumentNullException($"{nameof(fileImporter)} cannot be null.");
            throw new NotImplementedException();  //TODO: Implement this...
        }
    }
}
