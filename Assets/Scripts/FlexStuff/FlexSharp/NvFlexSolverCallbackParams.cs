namespace FlexSharp
{
    public unsafe partial struct NvFlexSolverCallbackParams
    {
        public NvFlexSolver* solver;

        public void* userData;

        public float* particles;

        public float* velocities;

        public int* phases;

        public int numActive;

        public float dt;

        [NativeTypeName("const int *")]
        public int* originalToSortedMap;

        [NativeTypeName("const int *")]
        public int* sortedToOriginalMap;
    }
}
