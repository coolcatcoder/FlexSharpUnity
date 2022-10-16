using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public class IsoSurfaceExtractor : MonoBehaviour
{
    public float CubeHalfSize = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public unsafe int CalcEmpty(float3* PosArray, int LengthOfArray, float3 Pos, float DistanceToCount)
    {
        int Emptiness = 0;

        for(int i = 0; i < LengthOfArray; i++)
        {
            if (math.distance(PosArray[i], Pos) <= DistanceToCount)
            {
                Emptiness++;
            }
        }

        return Emptiness;
    }

    [BurstCompile]
    public unsafe struct CalcEmptinessJob : IJobParallelFor
    {
        [WriteOnly]
        NativeArray<int> Emptiness;

        [ReadOnly]
        [NativeDisableUnsafePtrRestriction]
        float3* PosArray;

        [ReadOnly]
        float3 Pos;

        [ReadOnly]
        float DistanceToCount;

        public void Execute(int i)
        {
            if (math.distance(PosArray[i], Pos) <= DistanceToCount)
            {
                Emptiness[0]++;
            }
        }
    }
}
