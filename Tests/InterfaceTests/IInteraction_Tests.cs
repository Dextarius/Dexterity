using Core.States;
using Factors.Outcomes.ObservedOutcomes;
using NUnit.Framework;
using Tests.Tools.Interfaces;

namespace Tests.InterfaceTests
{
    [TestFixture(typeof(ObservedResponse   ), typeof(Response_Factory  ))]
    [TestFixture(typeof(Result<int>), typeof(Reactor_Int_Factory))]
    public class IOutcome_Tests<TResult, TFactory> 
        where TResult  : IOutcome
        where TFactory : IFactory<TResult>, new()
    {
        private TFactory resultFactory = new TFactory();


    }
}