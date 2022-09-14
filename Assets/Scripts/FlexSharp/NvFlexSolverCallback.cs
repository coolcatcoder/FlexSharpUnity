namespace FlexSharp
{
    public unsafe partial struct NvFlexSolverCallback
    {
        public void* userData;

        [NativeTypeName("void (*)(NvFlexSolverCallbackParams)")]
        public delegate* unmanaged[Cdecl]<NvFlexSolverCallbackParams, void> function;
    }
}
