namespace FlexSharpExt
{
    public unsafe partial struct NvFlexExtInstance
    {
        public int* particleIndices;

        public int numParticles;

        public int triangleIndex;

        public int shapeIndex;

        public int inflatableIndex;

        public float* shapeTranslations;

        public float* shapeRotations;

        [NativeTypeName("const NvFlexExtAsset *")]
        public NvFlexExtAsset* asset;

        public void* userData;
    }
}
