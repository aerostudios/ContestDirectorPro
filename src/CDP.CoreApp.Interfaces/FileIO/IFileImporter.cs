//---------------------------------------------------------------
// Date: 12/21/2017
// Rights: 
// FileName: IFileImporter.cs
//---------------------------------------------------------------

namespace CDP.CoreApp.Interfaces.FileIO
{
    using CDP.AppDomain.FIleIO;

    /// <summary>
    /// Defines a file importer
    /// </summary>
    public interface IFileImporter
    {
        /// <summary>
        /// Imports the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        FileFormat Import(string path);
    }
}
