using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Utilities;

namespace Hash
{
    public delegate JobHandle ScheduleShapeDelegate(NativeArray<float3x4> positions,  NativeArray<float3x4> normals,
                                                    int                   resolution, float4x4              trs, JobHandle dependency);
    
    [BurstCompile(FloatPrecision.Standard, FloatMode.Fast, CompileSynchronously = true)]
    public struct HashJob : IJobFor
    {
        [ReadOnly]  public NativeArray<float3x4> positions;
        [WriteOnly] public NativeArray<uint4>    hashes;

        public SmallXxHash4 hash;
        public float3x4     domainTRS;

        public void Execute(int i)
        {
            float4x3 p = domainTRS.TransformVectors(math.transpose(positions[i]));
            int4     u = (int4) math.floor(p.c0);
            int4     v = (int4) math.floor(p.c1);
            int4     w = (int4) math.floor(p.c2);

            hashes[i] = hash.Eat(u).Eat(v).Eat(w);
        }
    }
}