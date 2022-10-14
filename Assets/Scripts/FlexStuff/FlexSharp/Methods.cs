using System.Runtime.InteropServices;
using Unity.Collections.LowLevel.Unsafe;
using static FlexSharp.NvFlexCollisionShapeFlags;
using static FlexSharp.NvFlexPhase;

namespace FlexSharp
{
    public static unsafe partial class Methods
    {
        public static unsafe void ConvertArray<T>(T[] FromArray, T* ToArray, int Stride, int Length) where T : unmanaged
        {
            fixed (void* FromArrayPtr = FromArray)
            {
                UnsafeUtility.MemCpy((void*)ToArray, FromArrayPtr, Stride*Length);
            }
        }

        public static int NvFlexMakePhaseWithChannels(int group, int particleFlags, int shapeChannels)
        {
            return (group & (int)(eNvFlexPhaseGroupMask)) | (particleFlags & (int)(eNvFlexPhaseFlagsMask)) | (shapeChannels & (int)(eNvFlexPhaseShapeChannelMask));
        }

        public static int NvFlexMakePhase(int group, int particleFlags)
        {
            return NvFlexMakePhaseWithChannels(group, particleFlags, (int)(eNvFlexPhaseShapeChannelMask));
        }

        [DllImport("NvFlexReleaseD3D_x64.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern NvFlexLibrary* NvFlexInit(int version = 120, [NativeTypeName("NvFlexErrorCallback")] delegate* unmanaged[Cdecl]<NvFlexErrorSeverity, sbyte*, sbyte*, int, void> errorFunc = null, NvFlexInitDesc* desc = null);

        [DllImport("NvFlexReleaseD3D_x64.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void NvFlexShutdown(NvFlexLibrary* lib);

        [DllImport("NvFlexReleaseD3D_x64.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int NvFlexGetVersion();

        [DllImport("NvFlexReleaseD3D_x64.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void NvFlexSetSolverDescDefaults(NvFlexSolverDesc* desc);

        [DllImport("NvFlexReleaseD3D_x64.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern NvFlexSolver* NvFlexCreateSolver(NvFlexLibrary* lib, [NativeTypeName("const NvFlexSolverDesc *")] NvFlexSolverDesc* desc);

        [DllImport("NvFlexReleaseD3D_x64.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void NvFlexDestroySolver(NvFlexSolver* solver);

        [DllImport("NvFlexReleaseD3D_x64.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int NvFlexGetSolvers(NvFlexLibrary* lib, NvFlexSolver** solvers, int n);

        [DllImport("NvFlexReleaseD3D_x64.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern NvFlexLibrary* NvFlexGetSolverLibrary(NvFlexSolver* solver);

        [DllImport("NvFlexReleaseD3D_x64.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void NvFlexGetSolverDesc(NvFlexSolver* solver, NvFlexSolverDesc* desc);

        [DllImport("NvFlexReleaseD3D_x64.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern NvFlexSolverCallback NvFlexRegisterSolverCallback(NvFlexSolver* solver, NvFlexSolverCallback function, NvFlexSolverCallbackStage stage);

        [DllImport("NvFlexReleaseD3D_x64.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void NvFlexUpdateSolver(NvFlexSolver* solver, float dt, int substeps, [NativeTypeName("bool")] byte enableTimers);

        [DllImport("NvFlexReleaseD3D_x64.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void NvFlexSetParams(NvFlexSolver* solver, [NativeTypeName("const NvFlexParams *")] NvFlexParams* @params);

        [DllImport("NvFlexReleaseD3D_x64.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void NvFlexGetParams(NvFlexSolver* solver, NvFlexParams* @params);

        [DllImport("NvFlexReleaseD3D_x64.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void NvFlexSetActive(NvFlexSolver* solver, NvFlexBuffer* indices, [NativeTypeName("const NvFlexCopyDesc *")] NvFlexCopyDesc* desc);

        [DllImport("NvFlexReleaseD3D_x64.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void NvFlexGetActive(NvFlexSolver* solver, NvFlexBuffer* indices, [NativeTypeName("const NvFlexCopyDesc *")] NvFlexCopyDesc* desc);

        [DllImport("NvFlexReleaseD3D_x64.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void NvFlexSetActiveCount(NvFlexSolver* solver, int n);

        [DllImport("NvFlexReleaseD3D_x64.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int NvFlexGetActiveCount(NvFlexSolver* solver);

        [DllImport("NvFlexReleaseD3D_x64.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void NvFlexSetParticles(NvFlexSolver* solver, NvFlexBuffer* p, [NativeTypeName("const NvFlexCopyDesc *")] NvFlexCopyDesc* desc);

        [DllImport("NvFlexReleaseD3D_x64.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void NvFlexGetParticles(NvFlexSolver* solver, NvFlexBuffer* p, [NativeTypeName("const NvFlexCopyDesc *")] NvFlexCopyDesc* desc);

        [DllImport("NvFlexReleaseD3D_x64.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void NvFlexSetRestParticles(NvFlexSolver* solver, NvFlexBuffer* p, [NativeTypeName("const NvFlexCopyDesc *")] NvFlexCopyDesc* desc);

        [DllImport("NvFlexReleaseD3D_x64.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void NvFlexGetRestParticles(NvFlexSolver* solver, NvFlexBuffer* p, [NativeTypeName("const NvFlexCopyDesc *")] NvFlexCopyDesc* desc);

        [DllImport("NvFlexReleaseD3D_x64.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void NvFlexGetSmoothParticles(NvFlexSolver* solver, NvFlexBuffer* p, [NativeTypeName("const NvFlexCopyDesc *")] NvFlexCopyDesc* desc);

        [DllImport("NvFlexReleaseD3D_x64.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void NvFlexSetVelocities(NvFlexSolver* solver, NvFlexBuffer* v, [NativeTypeName("const NvFlexCopyDesc *")] NvFlexCopyDesc* desc);

        [DllImport("NvFlexReleaseD3D_x64.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void NvFlexGetVelocities(NvFlexSolver* solver, NvFlexBuffer* v, [NativeTypeName("const NvFlexCopyDesc *")] NvFlexCopyDesc* desc);

        [DllImport("NvFlexReleaseD3D_x64.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void NvFlexSetPhases(NvFlexSolver* solver, NvFlexBuffer* phases, [NativeTypeName("const NvFlexCopyDesc *")] NvFlexCopyDesc* desc);

        [DllImport("NvFlexReleaseD3D_x64.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void NvFlexGetPhases(NvFlexSolver* solver, NvFlexBuffer* phases, [NativeTypeName("const NvFlexCopyDesc *")] NvFlexCopyDesc* desc);

        [DllImport("NvFlexReleaseD3D_x64.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void NvFlexSetNormals(NvFlexSolver* solver, NvFlexBuffer* normals, [NativeTypeName("const NvFlexCopyDesc *")] NvFlexCopyDesc* desc);

        [DllImport("NvFlexReleaseD3D_x64.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void NvFlexGetNormals(NvFlexSolver* solver, NvFlexBuffer* normals, [NativeTypeName("const NvFlexCopyDesc *")] NvFlexCopyDesc* desc);

        [DllImport("NvFlexReleaseD3D_x64.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void NvFlexSetSprings(NvFlexSolver* solver, NvFlexBuffer* indices, NvFlexBuffer* restLengths, NvFlexBuffer* stiffness, int numSprings);

        [DllImport("NvFlexReleaseD3D_x64.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void NvFlexGetSprings(NvFlexSolver* solver, NvFlexBuffer* indices, NvFlexBuffer* restLengths, NvFlexBuffer* stiffness, int numSprings);

        [DllImport("NvFlexReleaseD3D_x64.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void NvFlexSetRigids(NvFlexSolver* solver, NvFlexBuffer* offsets, NvFlexBuffer* indices, NvFlexBuffer* restPositions, NvFlexBuffer* restNormals, NvFlexBuffer* stiffness, NvFlexBuffer* thresholds, NvFlexBuffer* creeps, NvFlexBuffer* rotations, NvFlexBuffer* translations, int numRigids, int numIndices);

        [DllImport("NvFlexReleaseD3D_x64.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void NvFlexGetRigids(NvFlexSolver* solver, NvFlexBuffer* offsets, NvFlexBuffer* indices, NvFlexBuffer* restPositions, NvFlexBuffer* restNormals, NvFlexBuffer* stiffness, NvFlexBuffer* thresholds, NvFlexBuffer* creeps, NvFlexBuffer* rotations, NvFlexBuffer* translations);

        [DllImport("NvFlexReleaseD3D_x64.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("NvFlexTriangleMeshId")]
        public static extern uint NvFlexCreateTriangleMesh(NvFlexLibrary* lib);

        [DllImport("NvFlexReleaseD3D_x64.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void NvFlexDestroyTriangleMesh(NvFlexLibrary* lib, [NativeTypeName("NvFlexTriangleMeshId")] uint mesh);

        [DllImport("NvFlexReleaseD3D_x64.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int NvFlexGetTriangleMeshes(NvFlexLibrary* lib, [NativeTypeName("NvFlexTriangleMeshId *")] uint* meshes, int n);

        [DllImport("NvFlexReleaseD3D_x64.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void NvFlexUpdateTriangleMesh(NvFlexLibrary* lib, [NativeTypeName("NvFlexTriangleMeshId")] uint mesh, NvFlexBuffer* vertices, NvFlexBuffer* indices, int numVertices, int numTriangles, [NativeTypeName("const float *")] float* lower, [NativeTypeName("const float *")] float* upper);

        [DllImport("NvFlexReleaseD3D_x64.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void NvFlexGetTriangleMeshBounds(NvFlexLibrary* lib, [NativeTypeName("const NvFlexTriangleMeshId")] uint mesh, float* lower, float* upper);

        [DllImport("NvFlexReleaseD3D_x64.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("NvFlexDistanceFieldId")]
        public static extern uint NvFlexCreateDistanceField(NvFlexLibrary* lib);

        [DllImport("NvFlexReleaseD3D_x64.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void NvFlexDestroyDistanceField(NvFlexLibrary* lib, [NativeTypeName("NvFlexDistanceFieldId")] uint sdf);

        [DllImport("NvFlexReleaseD3D_x64.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int NvFlexGetDistanceFields(NvFlexLibrary* lib, [NativeTypeName("NvFlexDistanceFieldId *")] uint* sdfs, int n);

        [DllImport("NvFlexReleaseD3D_x64.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void NvFlexUpdateDistanceField(NvFlexLibrary* lib, [NativeTypeName("NvFlexDistanceFieldId")] uint sdf, int dimx, int dimy, int dimz, NvFlexBuffer* field);

        [DllImport("NvFlexReleaseD3D_x64.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("NvFlexConvexMeshId")]
        public static extern uint NvFlexCreateConvexMesh(NvFlexLibrary* lib);

        [DllImport("NvFlexReleaseD3D_x64.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void NvFlexDestroyConvexMesh(NvFlexLibrary* lib, [NativeTypeName("NvFlexConvexMeshId")] uint convex);

        [DllImport("NvFlexReleaseD3D_x64.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int NvFlexGetConvexMeshes(NvFlexLibrary* lib, [NativeTypeName("NvFlexConvexMeshId *")] uint* meshes, int n);

        [DllImport("NvFlexReleaseD3D_x64.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void NvFlexUpdateConvexMesh(NvFlexLibrary* lib, [NativeTypeName("NvFlexConvexMeshId")] uint convex, NvFlexBuffer* planes, int numPlanes, [NativeTypeName("const float *")] float* lower, [NativeTypeName("const float *")] float* upper);

        [DllImport("NvFlexReleaseD3D_x64.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void NvFlexGetConvexMeshBounds(NvFlexLibrary* lib, [NativeTypeName("NvFlexConvexMeshId")] uint mesh, float* lower, float* upper);

        public static int NvFlexMakeShapeFlagsWithChannels(NvFlexCollisionShapeType type, bool dynamic, int shapeChannels)
        {
            return (int)type | (dynamic ? (int)(eNvFlexShapeFlagDynamic) : 0) | shapeChannels;
        }

        public static int NvFlexMakeShapeFlags(NvFlexCollisionShapeType type, bool dynamic)
        {
            return NvFlexMakeShapeFlagsWithChannels(type, dynamic, (int)(eNvFlexPhaseShapeChannelMask));
        }

        [DllImport("NvFlexReleaseD3D_x64.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void NvFlexSetShapes(NvFlexSolver* solver, NvFlexBuffer* geometry, NvFlexBuffer* shapePositions, NvFlexBuffer* shapeRotations, NvFlexBuffer* shapePrevPositions, NvFlexBuffer* shapePrevRotations, NvFlexBuffer* shapeFlags, int numShapes);

        [DllImport("NvFlexReleaseD3D_x64.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void NvFlexSetDynamicTriangles(NvFlexSolver* solver, NvFlexBuffer* indices, NvFlexBuffer* normals, int numTris);

        [DllImport("NvFlexReleaseD3D_x64.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void NvFlexGetDynamicTriangles(NvFlexSolver* solver, NvFlexBuffer* indices, NvFlexBuffer* normals, int numTris);

        [DllImport("NvFlexReleaseD3D_x64.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void NvFlexSetInflatables(NvFlexSolver* solver, NvFlexBuffer* startTris, NvFlexBuffer* numTris, NvFlexBuffer* restVolumes, NvFlexBuffer* overPressures, NvFlexBuffer* constraintScales, int numInflatables);

        [DllImport("NvFlexReleaseD3D_x64.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void NvFlexGetDensities(NvFlexSolver* solver, NvFlexBuffer* densities, [NativeTypeName("const NvFlexCopyDesc *")] NvFlexCopyDesc* desc);

        [DllImport("NvFlexReleaseD3D_x64.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void NvFlexGetAnisotropy(NvFlexSolver* solver, NvFlexBuffer* q1, NvFlexBuffer* q2, NvFlexBuffer* q3, [NativeTypeName("const NvFlexCopyDesc *")] NvFlexCopyDesc* desc);

        [DllImport("NvFlexReleaseD3D_x64.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void NvFlexGetDiffuseParticles(NvFlexSolver* solver, NvFlexBuffer* p, NvFlexBuffer* v, NvFlexBuffer* count);

        [DllImport("NvFlexReleaseD3D_x64.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void NvFlexSetDiffuseParticles(NvFlexSolver* solver, NvFlexBuffer* p, NvFlexBuffer* v, int n);

        [DllImport("NvFlexReleaseD3D_x64.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void NvFlexGetContacts(NvFlexSolver* solver, NvFlexBuffer* planes, NvFlexBuffer* velocities, NvFlexBuffer* indices, NvFlexBuffer* counts);

        [DllImport("NvFlexReleaseD3D_x64.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void NvFlexGetNeighbors(NvFlexSolver* solver, NvFlexBuffer* neighbors, NvFlexBuffer* counts, NvFlexBuffer* apiToInternal, NvFlexBuffer* internalToApi);

        [DllImport("NvFlexReleaseD3D_x64.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void NvFlexGetBounds(NvFlexSolver* solver, NvFlexBuffer* lower, NvFlexBuffer* upper);

        [DllImport("NvFlexReleaseD3D_x64.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float NvFlexGetDeviceLatency(NvFlexSolver* solver, [NativeTypeName("unsigned long long *")] ulong* begin, [NativeTypeName("unsigned long long *")] ulong* end, [NativeTypeName("unsigned long long *")] ulong* frequency);

        [DllImport("NvFlexReleaseD3D_x64.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void NvFlexGetTimers(NvFlexSolver* solver, NvFlexTimers* timers);

        [DllImport("NvFlexReleaseD3D_x64.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int NvFlexGetDetailTimers(NvFlexSolver* solver, NvFlexDetailTimer** timers);

        [DllImport("NvFlexReleaseD3D_x64.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern NvFlexBuffer* NvFlexAllocBuffer(NvFlexLibrary* lib, int elementCount, int elementByteStride, NvFlexBufferType type);

        [DllImport("NvFlexReleaseD3D_x64.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void NvFlexFreeBuffer(NvFlexBuffer* buf);

        [DllImport("NvFlexReleaseD3D_x64.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* NvFlexMap(NvFlexBuffer* buffer, int flags);

        [DllImport("NvFlexReleaseD3D_x64.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void NvFlexUnmap(NvFlexBuffer* buffer);

        [DllImport("NvFlexReleaseD3D_x64.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern NvFlexBuffer* NvFlexRegisterOGLBuffer(NvFlexLibrary* lib, int buf, int elementCount, int elementByteStride);

        [DllImport("NvFlexReleaseD3D_x64.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void NvFlexUnregisterOGLBuffer(NvFlexBuffer* buf);

        [DllImport("NvFlexReleaseD3D_x64.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern NvFlexBuffer* NvFlexRegisterD3DBuffer(NvFlexLibrary* lib, void* buffer, int elementCount, int elementByteStride);

        [DllImport("NvFlexReleaseD3D_x64.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void NvFlexUnregisterD3DBuffer(NvFlexBuffer* buf);

        [DllImport("NvFlexReleaseD3D_x64.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void NvFlexAcquireContext(NvFlexLibrary* lib);

        [DllImport("NvFlexReleaseD3D_x64.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void NvFlexRestoreContext(NvFlexLibrary* lib);

        [DllImport("NvFlexReleaseD3D_x64.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const char *")]
        public static extern sbyte* NvFlexGetDeviceName(NvFlexLibrary* lib);

        [DllImport("NvFlexReleaseD3D_x64.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void NvFlexGetDeviceAndContext(NvFlexLibrary* lib, void** device, void** context);

        [DllImport("NvFlexReleaseD3D_x64.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void NvFlexFlush(NvFlexLibrary* lib);

        [DllImport("NvFlexReleaseD3D_x64.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void NvFlexWait(NvFlexLibrary* lib);

        [DllImport("NvFlexReleaseD3D_x64.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void NvFlexSetDebug(NvFlexSolver* solver, [NativeTypeName("bool")] byte enable);

        [DllImport("NvFlexReleaseD3D_x64.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void NvFlexGetShapeBVH(NvFlexSolver* solver, void* bvh);

        [DllImport("NvFlexReleaseD3D_x64.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void NvFlexCopySolver(NvFlexSolver* dst, NvFlexSolver* src);

        [DllImport("NvFlexReleaseD3D_x64.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void NvFlexCopyDeviceToHost(NvFlexSolver* solver, NvFlexBuffer* pDevice, void* pHost, int size, int stride);

        [DllImport("NvFlexReleaseD3D_x64.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void NvFlexComputeWaitForGraphics(NvFlexLibrary* lib);

        [DllImport("NvFlexReleaseD3D_x64.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void NvFlexGetDataAftermath(NvFlexLibrary* lib, void* pDataOut, void* pStatusOut);
    }
}
