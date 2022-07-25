using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace Types
{
    public delegate JobHandle ScheduleNoiseDelegate(NativeArray<float3x4> positions,  NativeArray<float3x4> normals,
                                               int                   resolution, float4x4              trs, JobHandle dependency);
}