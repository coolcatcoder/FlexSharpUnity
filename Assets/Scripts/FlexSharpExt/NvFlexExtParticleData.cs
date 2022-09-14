namespace FlexSharpExt
{
    public unsafe partial struct NvFlexExtParticleData
    {
        public float* particles;

        public float* restParticles;

        public float* velocities;

        public int* phases;

        public float* normals;

        [NativeTypeName("const float *")]
        public float* lower;

        [NativeTypeName("const float *")]
        public float* upper;
    }
}
