//---------------------------------------------------------------

// Date: 11/17/2014
// Rights: 
// FileName: AnnouncerVoiceConfig.cs
//---------------------------------------------------------------

namespace CDP.CoreApp.Features.Timer.BasicAudioTimer
{
    using System;
    using System.Collections.Generic;

    public sealed class AnnouncerVoiceConfig
    {
        /// <summary>
        /// Gets or sets the voice reference to use.
        /// </summary>
        /// <value>
        /// The voice reference to use.
        /// </value>
        public int VoiceReferenceToUse { get; set; }

        /// <summary>
        /// Gets or sets the task start announcement.
        /// </summary>
        /// <value>
        /// The task start announcement.
        /// </value>
        public string TaskStartAnnouncement { get; set; }

        /// <summary>
        /// Gets or sets the checkpoints to announce.
        /// </summary>
        /// <value>
        /// The checkpoints to announce.
        /// </value>
        public List<TimeSpan> CheckpointsToAnnounce { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [announce last ten seconds].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [announce last ten seconds]; otherwise, <c>false</c>.
        /// </value>
        public bool AnnounceLastTenSeconds { get; set; }
    }
}
