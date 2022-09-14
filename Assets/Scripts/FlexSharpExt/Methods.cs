using System.Runtime.InteropServices;
using FlexSharp;

namespace FlexSharpExt
{
    public static unsafe partial class Methods
    {
        [DllImport("NvFlexExtReleaseD3D_x64.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void NvFlexExtMovingFrameInit(NvFlexExtMovingFrame* frame, [NativeTypeName("const float *")] float* worldTranslation, [NativeTypeName("const float *")] float* worldRotation);

        [DllImport("NvFlexExtReleaseD3D_x64.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void NvFlexExtMovingFrameUpdate(NvFlexExtMovingFrame* frame, [NativeTypeName("const float *")] float* worldTranslation, [NativeTypeName("const float *")] float* worldRotation, float dt);

        [DllImport("NvFlexExtReleaseD3D_x64.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void NvFlexExtMovingFrameApply(NvFlexExtMovingFrame* frame, float* positions, float* velocities, int numParticles, float linearScale, float angularScale, float dt);

        [DllImport("NvFlexExtReleaseD3D_x64.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int NvFlexExtCreateWeldedMeshIndices([NativeTypeName("const float *")] float* vertices, int numVertices, int* uniqueVerts, int* originalToUniqueMap, float threshold);

        [DllImport("NvFlexExtReleaseD3D_x64.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern NvFlexExtAsset* NvFlexExtCreateClothFromMesh([NativeTypeName("const float *")] float* particles, int numParticles, [NativeTypeName("const int *")] int* indices, int numTriangles, float stretchStiffness, float bendStiffness, float tetherStiffness, float tetherGive, float pressure);

        [DllImport("NvFlexExtReleaseD3D_x64.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern NvFlexExtAsset* NvFlexExtCreateTearingClothFromMesh([NativeTypeName("const float *")] float* particles, int numParticles, int maxParticles, [NativeTypeName("const int *")] int* indices, int numTriangles, float stretchStiffness, float bendStiffness, float pressure);

        [DllImport("NvFlexExtReleaseD3D_x64.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void NvFlexExtDestroyTearingCloth(NvFlexExtAsset* asset);

        [DllImport("NvFlexExtReleaseD3D_x64.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void NvFlexExtTearClothMesh(NvFlexExtAsset* asset, float maxStrain, int maxSplits, NvFlexExtTearingParticleClone* particleCopies, int* numParticleCopies, int maxCopies, NvFlexExtTearingMeshEdit* triangleEdits, int* numTriangleEdits, int maxEdits);

        [DllImport("NvFlexExtReleaseD3D_x64.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern NvFlexExtAsset* NvFlexExtCreateRigidFromMesh([NativeTypeName("const float *")] float* vertices, int numVertices, [NativeTypeName("const int *")] int* indices, int numTriangleIndices, float radius, float expand);

        [DllImport("NvFlexExtReleaseD3D_x64.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern NvFlexExtAsset* NvFlexExtCreateSoftFromMesh([NativeTypeName("const float *")] float* vertices, int numVertices, [NativeTypeName("const int *")] int* indices, int numTriangleIndices, float particleSpacing, float volumeSampling, float surfaceSampling, float clusterSpacing, float clusterRadius, float clusterStiffness, float linkRadius, float linkStiffness, float globalStiffness, float clusterPlasticThreshold, float clusterPlasticCreep);

        [DllImport("NvFlexExtReleaseD3D_x64.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void NvFlexExtDestroyAsset(NvFlexExtAsset* asset);

        [DllImport("NvFlexExtReleaseD3D_x64.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void NvFlexExtCreateSoftMeshSkinning([NativeTypeName("const float *")] float* vertices, int numVertices, [NativeTypeName("const float *")] float* bones, int numBones, float falloff, float maxDistance, float* skinningWeights, int* skinningIndices);

        [DllImport("NvFlexExtReleaseD3D_x64.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern NvFlexExtContainer* NvFlexExtCreateContainer(NvFlexLibrary* lib, NvFlexSolver* solver, int maxParticles);

        [DllImport("NvFlexExtReleaseD3D_x64.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void NvFlexExtDestroyContainer(NvFlexExtContainer* container);

        [DllImport("NvFlexExtReleaseD3D_x64.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int NvFlexExtAllocParticles(NvFlexExtContainer* container, int n, int* indices);

        [DllImport("NvFlexExtReleaseD3D_x64.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void NvFlexExtFreeParticles(NvFlexExtContainer* container, int n, [NativeTypeName("const int *")] int* indices);

        [DllImport("NvFlexExtReleaseD3D_x64.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int NvFlexExtGetActiveList(NvFlexExtContainer* container, int* indices);

        [DllImport("NvFlexExtReleaseD3D_x64.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern NvFlexExtParticleData NvFlexExtMapParticleData(NvFlexExtContainer* container);

        [DllImport("NvFlexExtReleaseD3D_x64.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void NvFlexExtUnmapParticleData(NvFlexExtContainer* container);

        [DllImport("NvFlexExtReleaseD3D_x64.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern NvFlexExtTriangleData NvFlexExtMapTriangleData(NvFlexExtContainer* container);

        [DllImport("NvFlexExtReleaseD3D_x64.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void NvFlexExtUnmapTriangleData(NvFlexExtContainer* container);

        [DllImport("NvFlexExtReleaseD3D_x64.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern NvFlexExtShapeData NvFlexExtMapShapeData(NvFlexExtContainer* container);

        [DllImport("NvFlexExtReleaseD3D_x64.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void NvFlexExtUnmapShapeData(NvFlexExtContainer* container);

        [DllImport("NvFlexExtReleaseD3D_x64.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern NvFlexExtInstance* NvFlexExtCreateInstance(NvFlexExtContainer* container, NvFlexExtParticleData* particleData, [NativeTypeName("const NvFlexExtAsset *")] NvFlexExtAsset* asset, [NativeTypeName("const float *")] float* transform, float vx, float vy, float vz, int phase, float invMassScale);

        [DllImport("NvFlexExtReleaseD3D_x64.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void NvFlexExtDestroyInstance(NvFlexExtContainer* container, [NativeTypeName("const NvFlexExtInstance *")] NvFlexExtInstance* instance);

        [DllImport("NvFlexExtReleaseD3D_x64.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void NvFlexExtNotifyAssetChanged(NvFlexExtContainer* container, [NativeTypeName("const NvFlexExtAsset *")] NvFlexExtAsset* asset);

        [DllImport("NvFlexExtReleaseD3D_x64.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void NvFlexExtTickContainer(NvFlexExtContainer* container, float dt, int numSubsteps, [NativeTypeName("bool")] byte enableTimers = 0);

        [DllImport("NvFlexExtReleaseD3D_x64.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void NvFlexExtPushToDevice(NvFlexExtContainer* container);

        [DllImport("NvFlexExtReleaseD3D_x64.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void NvFlexExtPullFromDevice(NvFlexExtContainer* container);

        [DllImport("NvFlexExtReleaseD3D_x64.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void NvFlexExtUpdateInstances(NvFlexExtContainer* container);

        [DllImport("NvFlexExtReleaseD3D_x64.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern NvFlexExtForceFieldCallback* NvFlexExtCreateForceFieldCallback(NvFlexSolver* solver);

        [DllImport("NvFlexExtReleaseD3D_x64.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void NvFlexExtDestroyForceFieldCallback(NvFlexExtForceFieldCallback* callback);

        [DllImport("NvFlexExtReleaseD3D_x64.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void NvFlexExtSetForceFields(NvFlexExtForceFieldCallback* callback, [NativeTypeName("const NvFlexExtForceField *")] NvFlexExtForceField* forceFields, int numForceFields);

        [DllImport("NvFlexExtReleaseD3D_x64.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern NvFlexExtSoftJoint* NvFlexExtCreateSoftJoint(NvFlexExtContainer* container, [NativeTypeName("const int *")] int* particleIndices, [NativeTypeName("const float *")] float* particleLocalPositions, [NativeTypeName("const int")] int numJointParticles, [NativeTypeName("const float")] float stiffness);

        [DllImport("NvFlexExtReleaseD3D_x64.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void NvFlexExtDestroySoftJoint(NvFlexExtContainer* container, NvFlexExtSoftJoint* joint);

        [DllImport("NvFlexExtReleaseD3D_x64.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void NvFlexExtSoftJointSetTransform(NvFlexExtContainer* container, NvFlexExtSoftJoint* joint, [NativeTypeName("const float *")] float* position, [NativeTypeName("const float *")] float* rotation);
    }
}
