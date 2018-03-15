//---------------------------------------------------------------
// Date: 7/7/2014
// Rights: 
// FileName: ITaskCalculator.cs
//---------------------------------------------------------------

namespace CDP.AppDomain.Scoring.Interfaces
{
    public interface ITaskCalculator
    {
        /// <summary>
        /// Scores the task
        /// </summary>
        /// <param name="contestTaskToScore">The contest task to score.</param>
        /// <returns></returns>
        double ScoreTask(RoundScoreBase contestTaskToScore);
    }
}
