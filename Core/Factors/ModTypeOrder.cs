namespace Core.Factors
{
    public readonly struct ModTypeOrder
    {
        private readonly NumericModType[] modTypesByPriority;
        private readonly int[]            order;

        public NumericModType[] ModTypesByPriority => modTypesByPriority;
        
        public int GetPriorityForModType(NumericModType modType)
        {
            return order[((int)modType) - 1];
        }

        public ModTypeOrder(NumericModType[] modTypesInPriorityOrder)
        {
            order = new[] { 1, 2, 3, 4 };
            modTypesByPriority = modTypesInPriorityOrder;

            if (modTypesInPriorityOrder != null)
            {
                for (int i = 0; i < ModTypesByPriority.Length; i++)
                {
                    NumericModType modType = ModTypesByPriority[i];
                
                    order[(int)modType] = i + 1;
                }
            }
        }
    }
}