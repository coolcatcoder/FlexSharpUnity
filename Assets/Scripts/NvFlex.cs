using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class NvFlex
{
	const int NV_FLEX_VERSION = 120;
	public unsafe struct NvFlexLibrary { };

    public unsafe struct NvFlexSolver { };

    public unsafe struct NvFlexBuffer { };

    public enum NvFlexMapFlags
    {
        eNvFlexMapWait = 0, //!< Calling thread will be blocked until buffer is ready for access, default
        eNvFlexMapDoNotWait = 1,    //!< Calling thread will check if buffer is ready for access, if not ready then the method will return NULL immediately
    };

    public enum NvFlexBufferType
    {
        eNvFlexBufferHost = 0,  //!< A host mappable buffer, pinned memory on CUDA, staging buffer on DX
        eNvFlexBufferDevice = 1,    //!< A device memory buffer, mapping this on CUDA will return a device memory pointer, and will return a buffer pointer on DX
    };

    public enum NvFlexRelaxationMode
    {
        eNvFlexRelaxationGlobal = 0,    //!< The relaxation factor is a fixed multiplier on each constraint's position delta
        eNvFlexRelaxationLocal = 1      //!< The relaxation factor is a fixed multiplier on each constraint's delta divided by the particle's constraint count, convergence will be slower but more reliable
    };

    [StructLayout(LayoutKind.Sequential)]
	unsafe public struct NvFlexParams
	{
		int numIterations;                  //!< Number of solver iterations to perform per-substep

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
		float[] gravity;                    //!< Constant acceleration applied to all particles
		float radius;                       //!< The maximum interaction radius for particles
		float solidRestDistance;            //!< The distance non-fluid particles attempt to maintain from each other, must be in the range (0, radius]
		float fluidRestDistance;            //!< The distance fluid particles are spaced at the rest density, must be in the range (0, radius], for fluids this should generally be 50-70% of mRadius, for rigids this can simply be the same as the particle radius

		// common params
		float dynamicFriction;              //!< Coefficient of friction used when colliding against shapes
		float staticFriction;               //!< Coefficient of static friction used when colliding against shapes
		float particleFriction;             //!< Coefficient of friction used when colliding particles
		float restitution;                  //!< Coefficient of restitution used when colliding against shapes, particle collisions are always inelastic
		float adhesion;                     //!< Controls how strongly particles stick to surfaces they hit, default 0.0, range [0.0, +inf]
		float sleepThreshold;               //!< Particles with a velocity magnitude < this threshold will be considered fixed

		float maxSpeed;                     //!< The magnitude of particle velocity will be clamped to this value at the end of each step
		float maxAcceleration;              //!< The magnitude of particle acceleration will be clamped to this value at the end of each step (limits max velocity change per-second), useful to avoid popping due to large interpenetrations

		float shockPropagation;             //!< Artificially decrease the mass of particles based on height from a fixed reference point, this makes stacks and piles converge faster
		float dissipation;                  //!< Damps particle velocity based on how many particle contacts it has
		float damping;                      //!< Viscous drag force, applies a force proportional, and opposite to the particle velocity

		// cloth params
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
		float[] wind;						//!< Constant acceleration applied to particles that belong to dynamic triangles, drag needs to be > 0 for wind to affect triangles
		float drag;                         //!< Drag force applied to particles belonging to dynamic triangles, proportional to velocity^2*area in the negative velocity direction
		float lift;                         //!< Lift force applied to particles belonging to dynamic triangles, proportional to velocity^2*area in the direction perpendicular to velocity and (if possible), parallel to the plane normal

		// fluid params
		float cohesion;                     //!< Control how strongly particles hold each other together, default: 0.025, range [0.0, +inf]
		float surfaceTension;               //!< Controls how strongly particles attempt to minimize surface area, default: 0.0, range: [0.0, +inf]    
		float viscosity;                    //!< Smoothes particle velocities using XSPH viscosity
		float vorticityConfinement;         //!< Increases vorticity by applying rotational forces to particles
		float anisotropyScale;              //!< Control how much anisotropy is present in resulting ellipsoids for rendering, if zero then anisotropy will not be calculated, see NvFlexGetAnisotropy()
		float anisotropyMin;                //!< Clamp the anisotropy scale to this fraction of the radius
		float anisotropyMax;                //!< Clamp the anisotropy scale to this fraction of the radius
		float smoothing;                    //!< Control the strength of Laplacian smoothing in particles for rendering, if zero then smoothed positions will not be calculated, see NvFlexGetSmoothParticles()
		float solidPressure;                //!< Add pressure from solid surfaces to particles
		float freeSurfaceDrag;              //!< Drag force applied to boundary fluid particles
		float buoyancy;                     //!< Gravity is scaled by this value for fluid particles

		// diffuse params
		float diffuseThreshold;             //!< Particles with kinetic energy + divergence above this threshold will spawn new diffuse particles
		float diffuseBuoyancy;              //!< Scales force opposing gravity that diffuse particles receive
		float diffuseDrag;                  //!< Scales force diffuse particles receive in direction of neighbor fluid particles
		int diffuseBallistic;               //!< The number of neighbors below which a diffuse particle is considered ballistic
		float diffuseLifetime;              //!< Time in seconds that a diffuse particle will live for after being spawned, particles will be spawned with a random lifetime in the range [0, diffuseLifetime]

		// collision params
		float collisionDistance;            //!< Distance particles maintain against shapes, note that for robust collision against triangle meshes this distance should be greater than zero
		float particleCollisionMargin;      //!< Increases the radius used during neighbor finding, this is useful if particles are expected to move significantly during a single step to ensure contacts aren't missed on subsequent iterations
		float shapeCollisionMargin;         //!< Increases the radius used during contact finding against kinematic shapes

		fixed float planes[8*4];			//!< Collision planes in the form ax + by + cz + d = 0
		int numPlanes;                      //!< Num collision planes

		NvFlexRelaxationMode relaxationMode;//!< How the relaxation is applied inside the solver
		float relaxationFactor;             //!< Control the convergence rate of the parallel solver, default: 1, values greater than 1 may lead to instability
	};

	public enum NvFlexPhase
	{
		eNvFlexPhaseGroupMask = 0x000fffff,			//!< Bits [ 0, 19] represent the particle group for controlling collisions
		eNvFlexPhaseFlagsMask = 0x00f00000,			//!< Bits [20, 23] hold flags about how the particle behave 
		eNvFlexPhaseShapeChannelMask = 0x7f000000,  //!< Bits [24, 30] hold flags representing what shape collision channels particles will collide with, see NvFlexMakeShapeFlags() (highest bit reserved for now)

		eNvFlexPhaseSelfCollide = 1 << 20,			//!< If set this particle will interact with particles of the same group
		eNvFlexPhaseSelfCollideFilter = 1 << 21,    //!< If set this particle will ignore collisions with particles closer than the radius in the rest pose, this flag should not be specified unless valid rest positions have been specified using NvFlexSetRestParticles()
		eNvFlexPhaseFluid = 1 << 22,				//!< If set this particle will generate fluid density constraints for its overlapping neighbors
		eNvFlexPhaseUnused = 1 << 23,				//!< Reserved

		eNvFlexPhaseShapeChannel0 = 1 << 24,        //!< Particle will collide with shapes with channel 0 set (see NvFlexMakeShapeFlags())
		eNvFlexPhaseShapeChannel1 = 1 << 25,        //!< Particle will collide with shapes with channel 1 set (see NvFlexMakeShapeFlags())
		eNvFlexPhaseShapeChannel2 = 1 << 26,        //!< Particle will collide with shapes with channel 2 set (see NvFlexMakeShapeFlags())
		eNvFlexPhaseShapeChannel3 = 1 << 27,        //!< Particle will collide with shapes with channel 3 set (see NvFlexMakeShapeFlags())
		eNvFlexPhaseShapeChannel4 = 1 << 28,        //!< Particle will collide with shapes with channel 4 set (see NvFlexMakeShapeFlags())
		eNvFlexPhaseShapeChannel5 = 1 << 29,        //!< Particle will collide with shapes with channel 5 set (see NvFlexMakeShapeFlags())
		eNvFlexPhaseShapeChannel6 = 1 << 30,        //!< Particle will collide with shapes with channel 6 set (see NvFlexMakeShapeFlags())	
	};

	//inline int NvFlexMakePhaseWithChannels(int group, int particleFlags, int shapeChannels) { return (group & eNvFlexPhaseGroupMask) | (particleFlags & eNvFlexPhaseFlagsMask) | (shapeChannels & eNvFlexPhaseShapeChannelMask); }

	[StructLayout(LayoutKind.Sequential)]
	public struct NvFlexTimers
	{
		float predict;              //!< Time spent in prediction
		float createCellIndices;    //!< Time spent creating grid indices
		float sortCellIndices;      //!< Time spent sorting grid indices
		float createGrid;           //!< Time spent creating grid
		float reorder;              //!< Time spent reordering particles
		float collideParticles;     //!< Time spent finding particle neighbors
		float collideShapes;        //!< Time spent colliding convex shapes
		float collideTriangles;     //!< Time spent colliding triangle shapes
		float collideFields;        //!< Time spent colliding signed distance field shapes
		float calculateDensity;     //!< Time spent calculating fluid density
		float solveDensities;       //!< Time spent solving density constraints
		float solveVelocities;      //!< Time spent solving velocity constraints
		float solveShapes;          //!< Time spent solving rigid body constraints
		float solveSprings;         //!< Time spent solving distance constraints
		float solveContacts;        //!< Time spent solving contact constraints
		float solveInflatables;     //!< Time spent solving pressure constraints
		float applyDeltas;          //!< Time spent adding position deltas to particles
		float calculateAnisotropy;  //!< Time spent calculating particle anisotropy for fluid
		float updateDiffuse;        //!< Time spent updating diffuse particles
		float updateTriangles;      //!< Time spent updating dynamic triangles
		float updateNormals;        //!< Time spent updating vertex normals
		float finalize;             //!< Time spent finalizing state
		float updateBounds;         //!< Time spent updating particle bounds
		float total;                //!< Sum of all timers above
	};

	public enum NvFlexErrorSeverity
	{
		eNvFlexLogError = 0,    //!< Error messages
		eNvFlexLogInfo = 1,		//!< Information messages
		eNvFlexLogWarning = 2,  //!< Warning messages
		eNvFlexLogDebug = 4,    //!< Used only in debug version of dll
		eNvFlexLogAll = -1,		//!< All log types
	};

	public enum NvFlexSolverCallbackStage
	{
		eNvFlexStageIterationStart, //!< Called at the beginning of each constraint iteration
		eNvFlexStageIterationEnd,   //!< Called at the end of each constraint iteration
		eNvFlexStageSubstepBegin,   //!< Called at the beginning of each substep after the prediction step has been completed
		eNvFlexStageSubstepEnd,     //!< Called at the end of each substep after the velocity has been updated by the constraints
		eNvFlexStageUpdateEnd,      //!< Called at the end of solver update after the final substep has completed
		eNvFlexStageCount,          //!< Number of stages
	};

	[StructLayout(LayoutKind.Sequential)]
	unsafe public struct NvFlexSolverCallbackParams
	{
		NvFlexSolver* solver;               //!< Pointer to the solver that the callback is registered to
		void* userData;                     //!< Pointer to the user data provided to NvFlexRegisterSolverCallback()

		float* particles;                   //!< Device pointer to the active particle basic data in the form x,y,z,1/m
		float* velocities;                  //!< Device pointer to the active particle velocity data in the form x,y,z,w (last component is not used)
		int* phases;                        //!< Device pointer to the active particle phase data

		int numActive;                      //!< The number of active particles returned, the callback data only return pointers to active particle data, this is the same as NvFlexGetActiveCount()

		float dt;                           //!< The per-update time-step, this is the value passed to NvFlexUpdateSolver()

		int* originalToSortedMap;     //!< Device pointer that maps the sorted callback data to the original position given by SetParticles()
		int* sortedToOriginalMap;     //!< Device pointer that maps the original particle index to the index in the callback data structure
	};

	unsafe struct NvFlexSolverCallback
	{
		void* userData;
		delegate* unmanaged[Cdecl]<NvFlexSolverCallbackParams, void> function;
	}

	unsafe delegate* unmanaged[Cdecl]<NvFlexErrorSeverity, sbyte*, sbyte*, int, void> NvFlexErrorCallback;

	public enum NvFlexComputeType
	{
		eNvFlexCUDA,        //!< Use CUDA compute for Flex, the application must link against the CUDA libraries
		eNvFlexD3D11,       //!< Use DirectX 11 compute for Flex, the application must link against the D3D libraries
		eNvFlexD3D12,       //!< Use DirectX 12 compute for Flex, the application must link against the D3D libraries
	};

	[StructLayout(LayoutKind.Sequential)]
	unsafe public struct NvFlexInitDesc
	{
		int deviceIndex;                //!< The GPU device index that should be used, if there is already a CUDA context on the calling thread then this parameter will be ignored and the active CUDA context used. Otherwise a new context will be created using the suggested device ordinal.
		bool enableExtensions;          //!< Enable or disable NVIDIA/AMD extensions in DirectX, can lead to improved performance.
		void* renderDevice;             //!< Direct3D device to use for simulation, if none is specified a new device and context will be created.
		void* renderContext;            //!< Direct3D context that the app is using for rendering. In DirectX 12 this should be a ID3D12CommandQueue pointer.
		void* computeContext;           //!< Direct3D context to use for simulation, if none is specified a new context will be created, in DirectX 12 this should be a pointer to the ID3D12CommandQueue where compute operations will take place. 
		bool runOnRenderContext;        //!< If true, run Flex on D3D11 render context, or D3D12 direct queue. If false, run on a D3D12 compute queue, or vendor specific D3D11 compute queue, allowing compute and graphics to run in parallel on some GPUs.

		NvFlexComputeType computeType;  //!< Set to eNvFlexD3D11 if DirectX 11 should be used, eNvFlexD3D12 for DirectX 12, this must match the libraries used to link the application
	};

	//[DllImport("NvFlexReleaseD3D_x64.dll")]
	//unsafe static extern public NvFlexLibrary* NvFlexInit(int version = NV_FLEX_VERSION, NvFlexErrorCallback errorFunc = 0, NvFlexInitDesc* desc = 0);

	[DllImport("NvFlexReleaseD3D_x64.dll")]
	unsafe static extern public void NvFlexShutdown(NvFlexLibrary* lib);

	[DllImport("NvFlexReleaseD3D_x64.dll")]
	static extern public int NvFlexGetVersion();

	public enum NvFlexFeatureMode
	{
		eNvFlexFeatureModeDefault = 0,  //!< All features enabled
		eNvFlexFeatureModeSimpleSolids = 1, //!< Simple per-particle collision (no per-particle SDF normals, no fluids)
		eNvFlexFeatureModeSimpleFluids = 2, //!< Simple single phase fluid-only particles (no solids)
	};

	[StructLayout(LayoutKind.Sequential)]
	public struct NvFlexSolverDesc
	{
		NvFlexFeatureMode featureMode;  //!< Control which features are enabled

		int maxParticles;               //!< Maximum number of regular particles in the solver
		int maxDiffuseParticles;        //!< Maximum number of diffuse particles in the solver
		int maxNeighborsPerParticle;    //!< Maximum number of neighbors per-particle, for solids this can be around 32, for fluids up to 128 may be necessary depending on smoothing radius
		int maxContactsPerParticle;     //!< Maximum number of collision contacts per-particle
	};

	[DllImport("NvFlexReleaseD3D_x64.dll")]
	unsafe static extern public void NvFlexSetSolverDescDefaults(NvFlexSolverDesc* desc);

	[DllImport("NvFlexReleaseD3D_x64.dll")]
	unsafe static extern public NvFlexSolver* NvFlexCreateSolver(NvFlexLibrary* lib, NvFlexSolverDesc* desc);

	[DllImport("NvFlexReleaseD3D_x64.dll")]
	unsafe static extern public void NvFlexDestroySolver(NvFlexSolver* solver);

    [DllImport("NvFlexReleaseD3D_x64.dll")]
	unsafe static extern public int NvFlexGetSolvers(NvFlexLibrary* lib, NvFlexSolver** solvers, int n);

	[DllImport("NvFlexReleaseD3D_x64.dll")]
	unsafe static extern public NvFlexLibrary* NvFlexGetSolverLibrary(NvFlexSolver* solver);

	[DllImport("NvFlexReleaseD3D_x64.dll")]
	unsafe static extern public void NvFlexGetSolverDesc(NvFlexSolver* solver, NvFlexSolverDesc* desc);

	//[DllImport("NvFlexReleaseD3D_x64.dll")]
	//unsafe static extern public NvFlexSolverCallback NvFlexRegisterSolverCallback(NvFlexSolver* solver, NvFlexSolverCallback function, NvFlexSolverCallbackStage stage);

	[DllImport("NvFlexReleaseD3D_x64.dll")]
	unsafe static extern public void NvFlexUpdateSolver(NvFlexSolver* solver, float dt, int substeps, bool enableTimers);

	[DllImport("NvFlexReleaseD3D_x64.dll")]
	unsafe static extern public void NvFlexSetParams(NvFlexSolver* solver, IntPtr params_odd); // params_odd should be a NvFlexParams* type, but it is a managed type, good luck, you will need it!

	[DllImport("NvFlexReleaseD3D_x64.dll")]
	unsafe static extern public void NvFlexGetParams(NvFlexSolver* solver, IntPtr params_odd); // same as last but with get instead of set!

	[StructLayout(LayoutKind.Sequential)]
	public struct NvFlexCopyDesc
	{
		int srcOffset;          //<! Offset in elements from the start of the source buffer to begin reading from
		int dstOffset;          //<! Offset in elements from the start of the destination buffer to being writing to
		int elementCount;       //<! Number of elements to copy
	};

	[DllImport("NvFlexReleaseD3D_x64.dll")]
	unsafe static extern public void NvFlexSetActive(NvFlexSolver* solver, NvFlexBuffer* indices, NvFlexCopyDesc* desc);

	[DllImport("NvFlexReleaseD3D_x64.dll")]
	unsafe static extern public void NvFlexGetActive(NvFlexSolver* solver, NvFlexBuffer* indices, NvFlexCopyDesc* desc);

	[DllImport("NvFlexReleaseD3D_x64.dll")]
	unsafe static extern public void NvFlexSetActiveCount(NvFlexSolver* solver, int n);

	[DllImport("NvFlexReleaseD3D_x64.dll")]
	unsafe static extern public int NvFlexGetActiveCount(NvFlexSolver* solver);

	[DllImport("NvFlexReleaseD3D_x64.dll")]
	unsafe static extern public void NvFlexSetParticles(NvFlexSolver* solver, NvFlexBuffer* p, NvFlexCopyDesc* desc);

	[DllImport("NvFlexReleaseD3D_x64.dll")]
	unsafe static extern public void NvFlexGetParticles(NvFlexSolver* solver, NvFlexBuffer* p, NvFlexCopyDesc* desc);

	[DllImport("NvFlexReleaseD3D_x64.dll")]
	unsafe static extern public void NvFlexSetRestParticles(NvFlexSolver* solver, NvFlexBuffer* p, NvFlexCopyDesc* desc);

	[DllImport("NvFlexReleaseD3D_x64.dll")]
	unsafe static extern public void NvFlexGetRestParticles(NvFlexSolver* solver, NvFlexBuffer* p, NvFlexCopyDesc* desc);

	[DllImport("NvFlexReleaseD3D_x64.dll")]
	unsafe static extern public void NvFlexGetSmoothParticles(NvFlexSolver* solver, NvFlexBuffer* p, NvFlexCopyDesc* desc);

	[DllImport("NvFlexReleaseD3D_x64.dll")]
	unsafe static extern public void NvFlexSetVelocities(NvFlexSolver* solver, NvFlexBuffer* v, NvFlexCopyDesc* desc);

	[DllImport("NvFlexReleaseD3D_x64.dll")]
	unsafe static extern public void NvFlexGetVelocities(NvFlexSolver* solver, NvFlexBuffer* v, NvFlexCopyDesc* desc);

	[DllImport("NvFlexReleaseD3D_x64.dll")]
	unsafe static extern public void NvFlexSetPhases(NvFlexSolver* solver, NvFlexBuffer* phases, NvFlexCopyDesc* desc);

	[DllImport("NvFlexReleaseD3D_x64.dll")]
	unsafe static extern public void NvFlexGetPhases(NvFlexSolver* solver, NvFlexBuffer* phases, NvFlexCopyDesc* desc);

	[DllImport("NvFlexReleaseD3D_x64.dll")]
	unsafe static extern public void NvFlexSetNormals(NvFlexSolver* solver, NvFlexBuffer* normals, NvFlexCopyDesc* desc);

	[DllImport("NvFlexReleaseD3D_x64.dll")]
	unsafe static extern public void NvFlexGetNormals(NvFlexSolver* solver, NvFlexBuffer* normals, NvFlexCopyDesc* desc);

	[DllImport("NvFlexReleaseD3D_x64.dll")]
	unsafe static extern public void NvFlexSetSprings(NvFlexSolver* solver, NvFlexBuffer* indices, NvFlexBuffer* restLengths, NvFlexBuffer* stiffness, int numSprings);

	[DllImport("NvFlexReleaseD3D_x64.dll")]
	unsafe static extern public void NvFlexGetSprings(NvFlexSolver* solver, NvFlexBuffer* indices, NvFlexBuffer* restLengths, NvFlexBuffer* stiffness, int numSprings);

	[DllImport("NvFlexReleaseD3D_x64.dll")]
	unsafe static extern public void NvFlexSetRigids(NvFlexSolver* solver, NvFlexBuffer* offsets, NvFlexBuffer* indices, NvFlexBuffer* restPositions, NvFlexBuffer* restNormals, NvFlexBuffer* stiffness, NvFlexBuffer* thresholds, NvFlexBuffer* creeps, NvFlexBuffer* rotations, NvFlexBuffer* translations, int numRigids, int numIndices);

	[DllImport("NvFlexReleaseD3D_x64.dll")]
	unsafe static extern public void NvFlexGetRigids(NvFlexSolver* solver, NvFlexBuffer* offsets, NvFlexBuffer* indices, NvFlexBuffer* restPositions, NvFlexBuffer* restNormals, NvFlexBuffer* stiffness, NvFlexBuffer* thresholds, NvFlexBuffer* creeps, NvFlexBuffer* rotations, NvFlexBuffer* translations);

	[DllImport("NvFlexReleaseD3D_x64.dll")]
	unsafe static extern public uint NvFlexCreateTriangleMesh(NvFlexLibrary* lib);

	[DllImport("NvFlexReleaseD3D_x64.dll")]
	unsafe static extern public void NvFlexDestroyTriangleMesh(NvFlexLibrary* lib, uint mesh);

	[DllImport("NvFlexReleaseD3D_x64.dll")]
	unsafe static extern public int NvFlexGetTriangleMeshes(NvFlexLibrary* lib, uint* meshes, int n);

	[DllImport("NvFlexReleaseD3D_x64.dll")]
	unsafe static extern public void NvFlexUpdateTriangleMesh(NvFlexLibrary* lib, uint mesh, NvFlexBuffer* vertices, NvFlexBuffer* indices, int numVertices, int numTriangles, float* lower, float* upper);

	[DllImport("NvFlexReleaseD3D_x64.dll")]
	unsafe static extern public void NvFlexGetTriangleMeshBounds(NvFlexLibrary* lib, uint mesh, float* lower, float* upper);

	[DllImport("NvFlexReleaseD3D_x64.dll")]
	unsafe static extern public uint NvFlexCreateDistanceField(NvFlexLibrary* lib);

	[DllImport("NvFlexReleaseD3D_x64.dll")]
	unsafe static extern public void NvFlexDestroyDistanceField(NvFlexLibrary* lib, uint sdf);

	[DllImport("NvFlexReleaseD3D_x64.dll")]
	unsafe static extern public int NvFlexGetDistanceFields(NvFlexLibrary* lib, uint* sdfs, int n);

    [DllImport("NvFlexReleaseD3D_x64.dll")]
	unsafe static extern public void NvFlexUpdateDistanceField(NvFlexLibrary* lib, uint sdf, int dimx, int dimy, int dimz, NvFlexBuffer* field);

	[DllImport("NvFlexReleaseD3D_x64.dll")]
	unsafe static extern public uint NvFlexCreateConvexMesh(NvFlexLibrary* lib);

	[DllImport("NvFlexReleaseD3D_x64.dll")]
	unsafe static extern public void NvFlexDestroyConvexMesh(NvFlexLibrary* lib, uint convex);

	[DllImport("NvFlexReleaseD3D_x64.dll")]
	unsafe static extern public int NvFlexGetConvexMeshes(NvFlexLibrary* lib, uint* meshes, int n);

	[DllImport("NvFlexReleaseD3D_x64.dll")]
	unsafe static extern public void NvFlexUpdateConvexMesh(NvFlexLibrary* lib, uint convex, NvFlexBuffer* planes, int numPlanes, float* lower, float* upper);


	[DllImport("NvFlexReleaseD3D_x64.dll")]
	unsafe static extern public void NvFlexGetConvexMeshBounds(NvFlexLibrary* lib, uint mesh, float* lower, float* upper);

	[StructLayout(LayoutKind.Sequential)]
	struct NvFlexSphereGeometry
	{
		float radius;
	};

	[StructLayout(LayoutKind.Sequential)]
	struct NvFlexCapsuleGeometry
	{
		float radius;
		float halfHeight;
	};

	[StructLayout(LayoutKind.Sequential)]
	struct NvFlexBoxGeometry
	{
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
		float[] halfExtents;
	};

	[StructLayout(LayoutKind.Sequential)]
	struct NvFlexConvexMeshGeometry
	{
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
		float[] scale;
		uint mesh;
	};

	[StructLayout(LayoutKind.Sequential)]
	struct NvFlexTriangleMeshGeometry
	{
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
		float[] scale;         //!< The scale of the object from local space to world space
		uint mesh;  //!< A triangle mesh pointer created by NvFlexCreateTriangleMesh()
	};

	[StructLayout(LayoutKind.Sequential)]
	struct NvFlexSDFGeometry
	{
		float scale;                 //!< Uniform scale of SDF, this corresponds to the world space width of the shape
		uint field;     //!< A signed distance field pointer created by NvFlexCreateDistanceField()
	};


	//union NvFlexCollisionGeometry
	//{
	//	NvFlexSphereGeometry sphere;
	//	NvFlexCapsuleGeometry capsule;
	//	NvFlexBoxGeometry box;
	//	NvFlexConvexMeshGeometry convexMesh;
	//	NvFlexTriangleMeshGeometry triMesh;
	//	NvFlexSDFGeometry sdf;
	//};

	enum NvFlexCollisionShapeType
	{
		eNvFlexShapeSphere = 0,     //!< A sphere shape, see FlexSphereGeometry
		eNvFlexShapeCapsule = 1,        //!< A capsule shape, see FlexCapsuleGeometry
		eNvFlexShapeBox = 2,        //!< A box shape, see FlexBoxGeometry
		eNvFlexShapeConvexMesh = 3,     //!< A convex mesh shape, see FlexConvexMeshGeometry
		eNvFlexShapeTriangleMesh = 4,       //!< A triangle mesh shape, see FlexTriangleMeshGeometry
		eNvFlexShapeSDF = 5,        //!< A signed distance field shape, see FlexSDFGeometry
	};

	//enum NvFlexCollisionShapeFlags
	//{
	//	eNvFlexShapeFlagTypeMask = 0x7,     //!< Lower 3 bits holds the type of the collision shape given by the NvFlexCollisionShapeType enum
	//	eNvFlexShapeFlagDynamic = 0x8,      //!< Indicates the shape is dynamic and should have lower priority over static collision shapes
	//	eNvFlexShapeFlagTrigger = 0x10,     //!< Indicates that the shape is a trigger volume, this means it will not perform any collision response, but will be reported in the contacts array (see NvFlexGetContacts())

	//	eNvFlexShapeFlagReserved = 0xffffff00
	//};

	//[DllImport("NvFlexReleaseD3D_x64.dll")]
	//unsafe static extern public inline int NvFlexMakeShapeFlagsWithChannels(NvFlexCollisionShapeType type, bool dynamic, int shapeChannels) { return type | (dynamic ? eNvFlexShapeFlagDynamic : 0) | shapeChannels; }

	//[DllImport("NvFlexReleaseD3D_x64.dll")]
	//unsafe static extern public inline int NvFlexMakeShapeFlags(NvFlexCollisionShapeType type, bool dynamic) { return NvFlexMakeShapeFlagsWithChannels(type, dynamic, eNvFlexPhaseShapeChannelMask); }

	[DllImport("NvFlexReleaseD3D_x64.dll")]
	unsafe static extern public void NvFlexSetShapes(NvFlexSolver* solver, NvFlexBuffer* geometry, NvFlexBuffer* shapePositions, NvFlexBuffer* shapeRotations, NvFlexBuffer* shapePrevPositions, NvFlexBuffer* shapePrevRotations, NvFlexBuffer* shapeFlags, int numShapes);

	[DllImport("NvFlexReleaseD3D_x64.dll")]
	unsafe static extern public void NvFlexSetDynamicTriangles(NvFlexSolver* solver, NvFlexBuffer* indices, NvFlexBuffer* normals, int numTris);

	[DllImport("NvFlexReleaseD3D_x64.dll")]
	unsafe static extern public void NvFlexGetDynamicTriangles(NvFlexSolver* solver, NvFlexBuffer* indices, NvFlexBuffer* normals, int numTris);

	[DllImport("NvFlexReleaseD3D_x64.dll")]
	unsafe static extern public void NvFlexSetInflatables(NvFlexSolver* solver, NvFlexBuffer* startTris, NvFlexBuffer* numTris, NvFlexBuffer* restVolumes, NvFlexBuffer* overPressures, NvFlexBuffer* constraintScales, int numInflatables);

	[DllImport("NvFlexReleaseD3D_x64.dll")]
	unsafe static extern public void NvFlexGetDensities(NvFlexSolver* solver, NvFlexBuffer* densities, NvFlexCopyDesc* desc);

	[DllImport("NvFlexReleaseD3D_x64.dll")]
	unsafe static extern public void NvFlexGetAnisotropy(NvFlexSolver* solver, NvFlexBuffer* q1, NvFlexBuffer* q2, NvFlexBuffer* q3, NvFlexCopyDesc* desc);

	[DllImport("NvFlexReleaseD3D_x64.dll")]
	unsafe static extern public void NvFlexGetDiffuseParticles(NvFlexSolver* solver, NvFlexBuffer* p, NvFlexBuffer* v, NvFlexBuffer* count);

	[DllImport("NvFlexReleaseD3D_x64.dll")]
	unsafe static extern public void NvFlexSetDiffuseParticles(NvFlexSolver* solver, NvFlexBuffer* p, NvFlexBuffer* v, int n);

	[DllImport("NvFlexReleaseD3D_x64.dll")]
	unsafe static extern public void NvFlexGetContacts(NvFlexSolver* solver, NvFlexBuffer* planes, NvFlexBuffer* velocities, NvFlexBuffer* indices, NvFlexBuffer* counts);

	[DllImport("NvFlexReleaseD3D_x64.dll")]
	unsafe static extern public void NvFlexGetNeighbors(NvFlexSolver* solver, NvFlexBuffer* neighbors, NvFlexBuffer* counts, NvFlexBuffer* apiToInternal, NvFlexBuffer* internalToApi);

	[DllImport("NvFlexReleaseD3D_x64.dll")]
	unsafe static extern public void NvFlexGetBounds(NvFlexSolver* solver, NvFlexBuffer* lower, NvFlexBuffer* upper);

	[DllImport("NvFlexReleaseD3D_x64.dll")]
	unsafe static extern public float NvFlexGetDeviceLatency(NvFlexSolver* solver, ulong* begin, ulong* end, ulong* frequency);

	[DllImport("NvFlexReleaseD3D_x64.dll")]
	unsafe static extern public void NvFlexGetTimers(NvFlexSolver* solver, NvFlexTimers* timers);

	[StructLayout(LayoutKind.Sequential)]
	unsafe public struct NvFlexDetailTimer
	{
		char* name; // fix asap, and on all other ones like this
		float time;
	};

	[DllImport("NvFlexReleaseD3D_x64.dll")]
	unsafe static extern public int NvFlexGetDetailTimers(NvFlexSolver* solver, NvFlexDetailTimer** timers);

	[DllImport("NvFlexReleaseD3D_x64.dll")]
	unsafe static extern public NvFlexBuffer* NvFlexAllocBuffer(NvFlexLibrary* lib, int elementCount, int elementByteStride, NvFlexBufferType type);

	[DllImport("NvFlexReleaseD3D_x64.dll")]
	unsafe static extern public void NvFlexFreeBuffer(NvFlexBuffer* buf);

	[DllImport("NvFlexReleaseD3D_x64.dll")]
	unsafe static extern public void* NvFlexMap(NvFlexBuffer* buffer, int flags);

	[DllImport("NvFlexReleaseD3D_x64.dll")]
	unsafe static extern public void NvFlexUnmap(NvFlexBuffer* buffer);

	[DllImport("NvFlexReleaseD3D_x64.dll")]
	unsafe static extern public NvFlexBuffer* NvFlexRegisterOGLBuffer(NvFlexLibrary* lib, int buf, int elementCount, int elementByteStride);

	[DllImport("NvFlexReleaseD3D_x64.dll")]
	unsafe static extern public void NvFlexUnregisterOGLBuffer(NvFlexBuffer* buf);

	[DllImport("NvFlexReleaseD3D_x64.dll")]
	unsafe static extern public NvFlexBuffer* NvFlexRegisterD3DBuffer(NvFlexLibrary* lib, void* buffer, int elementCount, int elementByteStride);

	[DllImport("NvFlexReleaseD3D_x64.dll")]
	unsafe static extern public void NvFlexUnregisterD3DBuffer(NvFlexBuffer* buf);

	[DllImport("NvFlexReleaseD3D_x64.dll")]
	unsafe static extern public void NvFlexAcquireContext(NvFlexLibrary* lib);

	[DllImport("NvFlexReleaseD3D_x64.dll")]
	unsafe static extern public void NvFlexRestoreContext(NvFlexLibrary* lib);

	[DllImport("NvFlexReleaseD3D_x64.dll")]
	unsafe static extern public char* NvFlexGetDeviceName(NvFlexLibrary * lib); // fix this asap

	[DllImport("NvFlexReleaseD3D_x64.dll")]
	unsafe static extern public void NvFlexGetDeviceAndContext(NvFlexLibrary* lib, void** device, void** context);

	[DllImport("NvFlexReleaseD3D_x64.dll")]
	unsafe static extern public void NvFlexFlush(NvFlexLibrary* lib);

	[DllImport("NvFlexReleaseD3D_x64.dll")]
	unsafe static extern public void NvFlexWait(NvFlexLibrary* lib);

	[DllImport("NvFlexReleaseD3D_x64.dll")]
	unsafe static extern public void NvFlexSetDebug(NvFlexSolver* solver, bool enable);
	[DllImport("NvFlexReleaseD3D_x64.dll")]
	unsafe static extern public void NvFlexGetShapeBVH(NvFlexSolver* solver, void* bvh);
	[DllImport("NvFlexReleaseD3D_x64.dll")]
	unsafe static extern public void NvFlexCopySolver(NvFlexSolver* dst, NvFlexSolver* src);
	[DllImport("NvFlexReleaseD3D_x64.dll")]
	unsafe static extern public void NvFlexCopyDeviceToHost(NvFlexSolver* solver, NvFlexBuffer* pDevice, void* pHost, int size, int stride);
	[DllImport("NvFlexReleaseD3D_x64.dll")]
	unsafe static extern public void NvFlexComputeWaitForGraphics(NvFlexLibrary* lib);
	[DllImport("NvFlexReleaseD3D_x64.dll")]
	unsafe static extern public void NvFlexGetDataAftermath(NvFlexLibrary* lib, void* pDataOut, void* pStatusOut);
}
