namespace FlexSharpExt
{
    [System.Flags]
    public enum ExtDat
    {
        None = 0,
        Fun1 = 1,
        Fun2 = 2,
        Fun3 = 4
    }

    public enum ExtDatApplyType
    {
        Set,
        Unset,
        Toggle,
        Replace,
        Nothing
    }
}