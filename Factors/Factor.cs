using Core.Factors;

namespace Factors
{


    public abstract class Factor : IFactor
    {
        #region Constants

        private const string DefaultName = nameof(Factor);

        #endregion
        
        
        #region Properties
        
        public string Name { get; }
        public abstract bool IsConsequential { get; }

        #endregion


        #region Constructors

        protected Factor(string nameToGive = DefaultName)
        {
            Name = nameToGive ?? DefaultName; 
        }
        
        #endregion
    }
}