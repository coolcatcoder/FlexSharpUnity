namespace FlexSharp
{
    public unsafe partial struct NvFlexTriangleMeshGeometry
    {
        [NativeTypeName("float[3]")]
        public fixed float scale[3];

        [NativeTypeName("NvFlexTriangleMeshId")]
        public uint mesh;
    }
}
