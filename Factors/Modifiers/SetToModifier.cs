namespace Factors.Modifiers
{
    public class SetToModifier<T>
    {
        private bool isEnabled;

        public T ValueToSet { get; set; }

        public bool IsEnabled
        {
            get => isEnabled;
            set
            {
                isEnabled = value;
                ModifierChanged.TriggerSubscribers();
            }
        }
        
        protected T Modify(T valueToModify) => ValueToSet;
        
        public SetToModifier(T valueToSet)
        {
            ValueToSet = valueToSet;
        }
    }
}