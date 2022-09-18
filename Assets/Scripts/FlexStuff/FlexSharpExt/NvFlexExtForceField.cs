namespace FlexSharpExt
{
    public unsafe partial struct NvFlexExtForceField
    {
        [NativeTypeName("float[3]")]
        public fixed float mPosition[3];

        public float mRadius;

        public float mStrength;

        public NvFlexExtForceMode mMode;

        [NativeTypeName("bool")]
        public byte mLinearFalloff;
    }
}
