namespace FlexSharp
{
    [System.Serializable]
    public unsafe partial struct NvFlexParams
    {
        public int numIterations;

        [NativeTypeName("float[3]")]
        public fixed float gravity[3];

        public float radius;

        public float solidRestDistance;

        public float fluidRestDistance;

        public float dynamicFriction;

        public float staticFriction;

        public float particleFriction;

        public float restitution;

        public float adhesion;

        public float sleepThreshold;

        public float maxSpeed;

        public float maxAcceleration;

        public float shockPropagation;

        public float dissipation;

        public float damping;

        [NativeTypeName("float[3]")]
        public fixed float wind[3];

        public float drag;

        public float lift;

        public float cohesion;

        public float surfaceTension;

        public float viscosity;

        public float vorticityConfinement;

        public float anisotropyScale;

        public float anisotropyMin;

        public float anisotropyMax;

        public float smoothing;

        public float solidPressure;

        public float freeSurfaceDrag;

        public float buoyancy;

        public float diffuseThreshold;

        public float diffuseBuoyancy;

        public float diffuseDrag;

        public int diffuseBallistic;

        public float diffuseLifetime;

        public float collisionDistance;

        public float particleCollisionMargin;

        public float shapeCollisionMargin;

        [NativeTypeName("float[8][4]")]
        public fixed float planes[8 * 4];

        public int numPlanes;

        public NvFlexRelaxationMode relaxationMode;

        public float relaxationFactor;
    }
}
