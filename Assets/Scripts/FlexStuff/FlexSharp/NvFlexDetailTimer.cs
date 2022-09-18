namespace FlexSharp
{
    public unsafe partial struct NvFlexDetailTimer
    {
        [NativeTypeName("char *")]
        public sbyte* name;

        public float time;
    }
}
