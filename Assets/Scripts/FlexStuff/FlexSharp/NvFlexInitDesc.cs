namespace FlexSharp
{
    public unsafe partial struct NvFlexInitDesc
    {
        public int deviceIndex;

        [NativeTypeName("bool")]
        public byte enableExtensions;

        public void* renderDevice;

        public void* renderContext;

        public void* computeContext;

        [NativeTypeName("bool")]
        public byte runOnRenderContext;

        public NvFlexComputeType computeType;
    }
}
