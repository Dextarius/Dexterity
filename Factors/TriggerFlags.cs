namespace Factors
{
    public static class TriggerFlags
    {
        public const long
            None               =  0b_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000L,
            Default            = ~None,
            ItemAdded          =  0b_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0001L,
            ItemRemoved        =  0b_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0010L,
            ItemMoved          =  0b_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0100L,
            ItemReplaced       =  0b_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_1000L,
            ItemsToLeftChanged = -0b_1000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000L,
            IndexMask          =  0b_0111_1111_1111_1111_1111_1111_1111_1111_0000_0000_0000_0000_0000_0000_0000_0000L,

            TriggerWhenItemAdded          = ItemAdded,
            TriggerWhenItemRemoved        = ItemRemoved,
            TriggerWhenItemMoved          = ItemMoved,
            TriggerWhenItemReplaced       = ItemReplaced,
            TriggerWhenItemsToLeftChanged = ItemsToLeftChanged;
    }
}