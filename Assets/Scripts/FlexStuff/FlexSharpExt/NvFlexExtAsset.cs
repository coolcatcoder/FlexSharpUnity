namespace FlexSharpExt
{
    public unsafe partial struct NvFlexExtAsset
    {
        public float* particles;

        public int numParticles;

        public int maxParticles;

        public int* springIndices;

        public float* springCoefficients;

        public float* springRestLengths;

        public int numSprings;

        public int* shapeIndices;

        public int numShapeIndices;

        public int* shapeOffsets;

        public float* shapeCoefficients;

        public float* shapeCenters;

        public int numShapes;

        public float* shapePlasticThresholds;

        public float* shapePlasticCreeps;

        public int* triangleIndices;

        public int numTriangles;

        [NativeTypeName("bool")]
        public byte inflatable;

        public float inflatableVolume;

        public float inflatablePressure;

        public float inflatableStiffness;
    }
}
