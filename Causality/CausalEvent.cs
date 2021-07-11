using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Core.Causality;
using Core.Factors;
using JetBrains.Annotations;

namespace Causality
{
    public static partial class Observer
    {
        private class CausalEvent
        {
                      internal IOutcome        Outcome     { get; set; }
                      internal CausalEvent     PriorEvent { get; set; }
            [NotNull] private  HashSet<IState> Influences { get;      } = new HashSet<IState>();

        
            internal void AddInfluence([NotNull] IState contributingFactor)
            {
                Debug.Assert(Outcome != null, 
                    $"A process attempted to call {nameof(AddInfluence)} on a {nameof(CausalEvent)} who's {nameof(Outcome)} field was null.");

                if (contributingFactor.IsValid)
                {
                    if (Outcome.IsValid && contributingFactor.AddDependent(Outcome))
                    {
                        Influences.Add(contributingFactor);
                    }
                }
                else
                {
                    Outcome.Invalidate(contributingFactor);
                }
            }
        
            public CausalEvent Conclude()
            {
                CausalEvent eventToReturn = PriorEvent;
                IState[]    factors;

                if (Influences.Count > 0 && Outcome.IsValid)
                {
                    factors = Influences.ToArray();
                    Influences.Clear();
                }
                else
                {
                    factors = Array.Empty<IState>();
                }
                
                Outcome?.SetInfluences(factors);
                Outcome    = null;
                PriorEvent = null;

                return eventToReturn;
            }
        }
    }
}