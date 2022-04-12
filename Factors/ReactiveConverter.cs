namespace Factors
{
    public class ReactiveConverter<TReceived, TCreated>
    {
        //- Make a class that takes a Factor's value and converts it into another type.
        //  We technically already have something like this with the observed Reactors
        //  since they already register any involved Factors.
        
        //- Actually, regular DirectResults can already do this as long as all the factors involved are provided
        //  as arguments.
        
    }
}