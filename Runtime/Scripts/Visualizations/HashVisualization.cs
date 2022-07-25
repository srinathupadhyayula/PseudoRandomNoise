using AbstractInterfaces;
using Hash;
using Types;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using Utilities;
using static Unity.Mathematics.math;

namespace Visualizations
{
	public class HashVisualization : Visualization
	{
		[BurstCompile(FloatPrecision.Standard, FloatMode.Fast, CompileSynchronously = true)]
		private struct HashJob : IJobFor
		{
			[ReadOnly]  public NativeArray<float3x4> positions;
			[WriteOnly] public NativeArray<uint4>    hashes;

			public SmallXxHash4 hash;
			public float3x4     domainTRS;

			public void Execute(int i)
			{
				float4x3 p = domainTRS.TransformVectors(transpose(positions[i]));
				int4     u = (int4) floor(p.c0);
				int4     v = (int4) floor(p.c1);
				int4     w = (int4) floor(p.c2);

				hashes[i] = hash.Eat(u).Eat(v).Eat(w);
			}
		}

		private static readonly int HashesId = Shader.PropertyToID("_Hashes");

		[SerializeField] private int      seed;
		[SerializeField] private SpaceTRS domain = new SpaceTRS {scale = 8f};

		private NativeArray<uint4> m_hashes;
		private ComputeBuffer      m_hashesBuffer;

		protected override void EnableVisualization(int dataLength, MaterialPropertyBlock propertyBlock)
		{
			m_hashes       = new NativeArray<uint4>(dataLength, Allocator.Persistent);
			m_hashesBuffer = new ComputeBuffer(dataLength * 4, 4);
			propertyBlock.SetBuffer(HashesId, m_hashesBuffer);
		}

		protected override void DisableVisualization()
		{
			m_hashes.Dispose();
			m_hashesBuffer.Release();
			m_hashesBuffer = null;
		}

		protected override void UpdateVisualization(NativeArray<float3x4> positions, int resolution, JobHandle handle)
		{
			new HashJob {positions = positions, hashes = m_hashes, hash = SmallXxHash.Seed(seed), domainTRS = domain.Matrix}
			   .ScheduleParallel(m_hashes.Length, resolution, handle)
			   .Complete();

			m_hashesBuffer.SetData(m_hashes.Reinterpret<uint>(4 * 4));
		}
	}
}