using AbstractInterfaces;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Utilities;

namespace Types.Shapes
{
    [BurstCompile(FloatPrecision.Standard, FloatMode.Fast, CompileSynchronously = true)]
    public struct ShapeJob<TShape> : IJobFor where TShape : struct, IShape
    {
        [WriteOnly] private NativeArray<float3x4> m_positions;
        [WriteOnly] private NativeArray<float3x4> m_normals;

        private float    m_resolution;
        private float    m_invResolution;
        private float3x4 m_positionTRS;
        private float3x4 m_normalTRS;

        public void Execute(int i)
        {
            Point4 p = default(TShape).GetPoint4(i, m_resolution, m_invResolution);
            m_positions[i] = math.transpose(m_positionTRS.TransformVectors(p.positions));

            float3x4 n = math.transpose(m_normalTRS.TransformVectors(p.normals, 0f));
            m_normals[i] = math.float3x4(math.normalize(n.c0), math.normalize(n.c1), math.normalize(n.c2), math.normalize(n.c3));
        }

        public static JobHandle ScheduleParallel(NativeArray<float3x4> positions
                                               , NativeArray<float3x4> normals
                                               , int                   resolution
                                               , float4x4              trs
                                               , JobHandle             dependency) =>
            new ShapeJob<TShape>
            {
                m_positions     = positions
              , m_normals       = normals
              , m_resolution    = resolution
              , m_invResolution = 1f / resolution
              , m_positionTRS   = trs.Get3X4()
              , m_normalTRS     = math.transpose(math.inverse(trs)).Get3X4()
            }.ScheduleParallel(positions.Length, resolution, dependency);
    }
}