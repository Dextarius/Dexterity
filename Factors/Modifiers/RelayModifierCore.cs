using System.Collections.Generic;
using Core.Factors;
using Core.Tools;

namespace Factors.Modifiers
{
    public class RelayModifierCore : ReactiveModifierCore
    {
        #region Instance Fields

        protected readonly IFactor<double> valueSource;
        private            double          currentAmount;

        #endregion


        #region Properties

        public override int NumberOfTriggers => 1;

        public override double Amount
        {
            get
            {
                NotifyInvolved();
                return currentAmount;
            }
        }

        protected override IEnumerable<IFactor> Triggers
        {
            get
            {
                yield return valueSource;
            }
        }

        #endregion
        

        #region Instance Methods

        protected override bool CreateOutcome()
        {
            var newAmount = valueSource.Value;

            if (Numerics.DoublesAreNotEqual(currentAmount, newAmount))
            {
                currentAmount = newAmount;
                return true;
            }
            else return false;
        }

        #endregion
        

        #region Constructors

        public RelayModifierCore(IFactor<double> factorToUseAsAmount, NumericModType modType)
        {
            valueSource = factorToUseAsAmount;
            ModType = modType;
        }

        #endregion
    }
}