using System;
using Hash;
using Types;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using Utilities;
using static Unity.Mathematics.math;

namespace Noise
{
	public static partial class Noise
	{
		[Serializable]
		public struct Settings
		{
			public                 int   seed;
			[Min(1)]        public int   frequency;
			[Range(1,  6)]  public int   octaves;
			[Range(2,  4)]  public int   lacunarity;
			[Range(0f, 1f)] public float persistence;

			public static Settings Default => new Settings {frequency = 4, octaves = 1, lacunarity = 2, persistence = 0.5f};
		}

		public interface INoise
		{
			float4 GetNoise4(float4x3 positions, SmallXxHash4 hash, int frequency);
		}

		[BurstCompile(FloatPrecision.Standard, FloatMode.Fast, CompileSynchronously = true)]
		public struct Job<N> : IJobFor where N : struct, INoise
		{
			[ReadOnly]  private NativeArray<float3x4> m_positions;
			[WriteOnly] private NativeArray<float4>   m_noise;

			private Settings m_settings;
			private float3x4 m_domainTRS;

			public void Execute(int i)
			{
				float4x3 position  = m_domainTRS.TransformVectors(transpose(m_positions[i]));
				var      hash      = SmallXxHash4.Seed(m_settings.seed);
				int      frequency = m_settings.frequency;
				float    amplitude = 1f, amplitudeSum = 0f;
				float4   sum       = 0f;

				for (int o = 0; o < m_settings.octaves; o++)
				{
					sum          += amplitude * default(N).GetNoise4(position, hash + o, frequency);
					amplitudeSum += amplitude;
					frequency    *= m_settings.lacunarity;
					amplitude    *= m_settings.persistence;
				}

				m_noise[i] = sum / amplitudeSum;
			}

			public static JobHandle ScheduleParallel(NativeArray<float3x4> positions, NativeArray<float4> noise,
													 Settings settings, SpaceTRS domainTRS, int resolution, JobHandle dependency) =>
				new Job<N>
				{
					m_positions = positions
				  , m_noise     = noise
				  , m_settings  = settings
				  , m_domainTRS = domainTRS.Matrix
				   ,
				}.ScheduleParallel(positions.Length, resolution, dependency);
		}

		public delegate JobHandle ScheduleDelegate(NativeArray<float3x4> positions, NativeArray<float4> noise,
												   Settings settings, SpaceTRS domainTRS, int resolution, JobHandle dependency);
	}
}