using System;
using System.Collections.Generic;
using Core.Factors;
using JetBrains.Annotations;

namespace Factors.Cores.DirectReactorCores
{
    public abstract class HistoricDirectReactor<TInput> : DirectReactorCore
    {
        #region Instance Fields

        [NotNull]
        protected readonly IFactor<TInput> inputSource;
        protected TInput lastKnownValueOfInput;

        #endregion


        #region Properties

        protected override IEnumerable<IFactor> Triggers         { get { yield return inputSource; } }
        public override    int                  NumberOfTriggers => 1;
        public override    int                  UpdatePriority   => inputSource.UpdatePriority + 1;

        #endregion


        #region Constructors

        protected HistoricDirectReactor([NotNull] IFactor<TInput> valueSource)
        {
            inputSource = valueSource ?? throw new ArgumentNullException(nameof(valueSource));
        }

        #endregion
    }
}