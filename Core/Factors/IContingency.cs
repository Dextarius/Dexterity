
namespace Core.Factors
{
    /// <summary>  A reactive object representing a method that will only execute under a given set of conditions.  </summary>
    public interface IContingency 
    {
        #region Properties

        /// <summary>  Whether the conditions required for the method to execute are currently fulfilled  </summary>
        bool ConditionsMet { get; }

        /// <summary>
        ///     If true, this instance will monitor changes in the condition provided, and the governed action
        ///     will automatically execute every time the condition switches from false to true.
        /// </summary>
        bool IsImpulsive { get; set; }

        #endregion

        
        #region Instance Methods
        
        bool TryExecute();
        void ForceExecute();

        #endregion
    }
}