//---------------------------------------------------------------
// Date: 12/10/2017
// Rights: 
// FileName: IAnnouncer.cs
//---------------------------------------------------------------

namespace CDP.CoreApp.Interfaces.Announcer
{
    using System.Threading.Tasks;

    /// <summary>
    /// Base class for an annoucer.  TODO: Not sure if this is ever used...
    /// </summary>
    public abstract class AnnouncerBase
    {
        /// <summary>
        /// Says it asynchronous.
        /// </summary>
        /// <param name="whatToSay">The what to say.</param>
        /// <returns></returns>
        public virtual Task SayItAsync(string whatToSay) { return Task.CompletedTask; }
    }
}