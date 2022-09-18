namespace FlexSharpExt
{
    public unsafe partial struct NvFlexExtSoftJoint
    {
        public int* particleIndices;

        public float* particleLocalPositions;

        public int shapeIndex;

        public int numParticles;

        [NativeTypeName("float[3]")]
        public fixed float shapeTranslations[3];

        [NativeTypeName("float[4]")]
        public fixed float shapeRotations[4];

        public float stiffness;

        [NativeTypeName("bool")]
        public byte initialized;
    }
}
