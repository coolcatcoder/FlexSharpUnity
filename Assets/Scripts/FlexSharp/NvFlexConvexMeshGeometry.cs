namespace FlexSharp
{
    public unsafe partial struct NvFlexConvexMeshGeometry
    {
        [NativeTypeName("float[3]")]
        public fixed float scale[3];

        [NativeTypeName("NvFlexConvexMeshId")]
        public uint mesh;
    }
}
