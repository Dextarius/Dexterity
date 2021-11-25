// using System;
// using System.Collections.Generic;
// using Core.Redirection;
// using Dexterity.Evaluators;
//
// namespace Factors
// {
//     
//     //- A type of Reactive method whose return value is automatically updated when
//     //  a factor it depended on changes
//     //- Note : There may be a Proactive version of this.  A  
//     public class Evaluator<TParam, TReturn>
//    {
//        #region Instance Fields
//
//        //- TODO : Consider if there's a downside to holding direct references to the argument values
//        private readonly Dictionary<TParam, Evaluation>      evaluationsByArgument = new Dictionary<TParam, Evaluation>();
//        private readonly IArgumentEvaluator<TParam, TReturn> evaluationSource;
//
//        //- Maybe use a ConditionalWeakTable if TParam is a reference type, to avoid keeping references to arguments forever?
//        
//        #endregion
//        
//        #region Instance Methods
//        
//        public TReturn Evaluate(TParam argumentValue) => FindOrCreateEvaluationFor(argumentValue).Value;
//
//        private Evaluation FindOrCreateEvaluationFor(TParam argumentValue)
//        {
//            Evaluation evaluationForArgument;
//            bool       argumentNotYetCreated = (evaluationsByArgument.TryGetValue(argumentValue, out evaluationForArgument)) == false;
//                 
//            if (argumentNotYetCreated)
//            {
//                evaluationsByArgument[argumentValue]  =  evaluationForArgument  =  new Evaluation(this, argumentValue);
//            }
//
//            return evaluationForArgument;
//        }
//
//        protected TReturn GetResultFor(TParam argument) => evaluationSource.GetResultFor(argument);
//
//        #endregion
//
//
//        #region Constructors
//
//        protected Evaluator(Func<TParam, TReturn> functionToGetResultsFrom) : 
//            this(new ArgumentEvaluator<TParam, TReturn>(functionToGetResultsFrom))
//        {
//        }
//        
//        protected Evaluator(IArgumentEvaluator<TParam, TReturn> evaluator)
//        {
//            evaluationSource = evaluator;
//        }
//
//        #endregion
//
//
//        #region Nested Classes
//
//        //- A return value that is automatically updated if any Factor used to determine its value changes.
//        public class Evaluation : Reactor<TReturn,TParam>
//        {
//            private readonly Evaluator<TParam, TReturn> parentEvaluator;
//            private readonly TParam                     assignedArgument;
//            private          TReturn                    currentValue;
//
//            public virtual TReturn Value { get { AttemptReaction(); return currentValue; }  }
//
//            public override bool AttemptReaction()
//            {
//                bool reactionHadAnEffect = false;
//             
//                lock (SyncLock) //- TODO : Consider if anything else should be using this lock   
//                {
//                    if (Tracker.IsTriggered)
//                    {
//                        bool isNotAlreadyReacting = Tracker.StartTracking();
//
//                        if (isNotAlreadyReacting)
//                        {
//                            try
//                            {
//                                reactionHadAnEffect = React();
//
//                                /* Be careful what you add below here, anything that affects other tracked objects will be added to our
//                                   dependencies since we are still being tracked. */
//                            }
//                            finally
//                            {
//                                Tracker.EndTracking();
//                                Tracker.NotifyInvolved();
//                            }
//
//                            if (reactionHadAnEffect)
//                            { 
//                                QueueSubscriberActionsUsingArgumentAnd(currentValue);
//                            }
//                        }
//                    }
//                }
//             
//                return reactionHadAnEffect;
//            }
//
//            //- TODO : Consider if the fact that Collections aren't recycled by this will be an issue.
//            protected override bool React()
//            {
//                bool valueChanged;
//
//                TReturn reactionResult = parentEvaluator.GetResultFor(assignedArgument);
//                
//                valueChanged = reactionResult.IsNotTheSameAs(currentValue);
//
//                if (valueChanged)
//                {
//                    currentValue = reactionResult;
//                }
//                
//                /* Be careful what you add here, anything that affects other tracked objects will be added to our
//                   dependencies since we are still being tracked. */
//             
//                return valueChanged;
//            }
//
//
//            #region Constructors
//
//            public Evaluation(Evaluator<TParam, TReturn> evaluatorToQuery, TParam argumentBeingEvaluated)
//            {
//                parentEvaluator  = evaluatorToQuery;
//                assignedArgument = argumentBeingEvaluated;
//                IsReflexive      = true;
//            }
//
//            #endregion
//        }
//        
//        
//
//
//        #endregion
//    }
//
//     //- We could use this as a return value, that way we only call NotifyInvolved if the caller actually uses the 
//     //  return value.  It should work fine, since it can be implicitly cast to the actual value.
//     public readonly struct EvaluatedValue<TParam, TReturn>
//     {
//         private readonly Evaluator<TParam, TReturn>.Evaluation evaluation;
//         private readonly TReturn                               value;
//
//         public TReturn Value
//         {
//             get
//             {
//                 evaluation.AttemptReaction();  
//                 //^ We could use this to avoid even calculating the return value, until the caller actually uses it.
//                 evaluation.NotifyInvolved();
//                 return value;
//             }
//         }
//
//         public static implicit operator TReturn(EvaluatedValue<TParam, TReturn> instance) => instance.Value;
//     }
// }