namespace FlexSharp
{
    public enum NvFlexCollisionShapeFlags
    {
        eNvFlexShapeFlagTypeMask = 0x7,
        eNvFlexShapeFlagDynamic = 0x8,
        eNvFlexShapeFlagTrigger = 0x10,
        eNvFlexShapeFlagReserved = unchecked((int)(0xffffff00)),
    }
}
