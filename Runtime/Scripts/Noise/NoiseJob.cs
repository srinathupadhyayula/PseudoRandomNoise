using AbstractInterfaces;
using Hash;
using Types;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Utilities;

namespace Noise
{
    public delegate JobHandle ScheduleNoiseDelegate(NativeArray<float3x4> positions
                                                  , NativeArray<float4>   noise
                                                  , NoiseSettings         noiseSettings
                                                  , SpaceTRS              domainTRS
                                                  , int                   resolution
                                                  , JobHandle             dependency);
    
    [BurstCompile(FloatPrecision.Standard, FloatMode.Fast, CompileSynchronously = true)]
    public struct NoiseJob<TNoise> : IJobFor where TNoise : struct, INoise
    {
        [ReadOnly]  private NativeArray<float3x4> m_positions;
        [WriteOnly] private NativeArray<float4>   m_noise;

        private NoiseSettings m_noiseSettings;
        private float3x4      m_domainTRS;

        public void Execute(int i)
        {
            float4x3 position  = m_domainTRS.TransformVectors(math.transpose(m_positions[i]));
            var      hash      = SmallXxHash4.Seed(m_noiseSettings.seed);
            int      frequency = m_noiseSettings.frequency;
            float    amplitude = 1f, amplitudeSum = 0f;
            float4   sum       = 0f;

            for (int o = 0; o < m_noiseSettings.octaves; o++)
            {
                sum          += amplitude * default(TNoise).GetNoise4(position, hash + o, frequency);
                amplitudeSum += amplitude;
                frequency    *= m_noiseSettings.lacunarity;
                amplitude    *= m_noiseSettings.persistence;
            }

            m_noise[i] = sum / amplitudeSum;
        }

        public static JobHandle ScheduleParallel(NativeArray<float3x4> positions, NativeArray<float4> noise,
                                                 NoiseSettings noiseSettings, SpaceTRS domainTRS, int resolution, JobHandle dependency) =>
            new NoiseJob<TNoise>
            {
                m_positions     = positions
              , m_noise         = noise
              , m_noiseSettings = noiseSettings
              , m_domainTRS     = domainTRS.Matrix
               ,
            }.ScheduleParallel(positions.Length, resolution, dependency);
    }
}