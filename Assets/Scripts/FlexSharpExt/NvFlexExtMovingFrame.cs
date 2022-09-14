namespace FlexSharpExt
{
    public unsafe partial struct NvFlexExtMovingFrame
    {
        [NativeTypeName("float[3]")]
        public fixed float position[3];

        [NativeTypeName("float[4]")]
        public fixed float rotation[4];

        [NativeTypeName("float[3]")]
        public fixed float velocity[3];

        [NativeTypeName("float[3]")]
        public fixed float omega[3];

        [NativeTypeName("float[3]")]
        public fixed float acceleration[3];

        [NativeTypeName("float[3]")]
        public fixed float tau[3];

        [NativeTypeName("float[4][4]")]
        public fixed float delta[4 * 4];
    }
}
