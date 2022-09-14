using System.Runtime.InteropServices;

namespace FlexSharp
{
    [StructLayout(LayoutKind.Explicit)]
    public partial struct NvFlexCollisionGeometry
    {
        [FieldOffset(0)]
        public NvFlexSphereGeometry sphere;

        [FieldOffset(0)]
        public NvFlexCapsuleGeometry capsule;

        [FieldOffset(0)]
        public NvFlexBoxGeometry box;

        [FieldOffset(0)]
        public NvFlexConvexMeshGeometry convexMesh;

        [FieldOffset(0)]
        public NvFlexTriangleMeshGeometry triMesh;

        [FieldOffset(0)]
        public NvFlexSDFGeometry sdf;
    }
}
