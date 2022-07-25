using AbstractInterfaces;
using Types;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using static Noise.Noise;

namespace Visualizations
{
	public class NoiseVisualization : Visualization
	{
		private static readonly ScheduleDelegate[,] NoiseJobs =
		{
			{
				Job<Lattice1D<LatticeNormal, Perlin>>.ScheduleParallel
			  , Job<Lattice1D<LatticeTiling, Perlin>>.ScheduleParallel
			  , Job<Lattice2D<LatticeNormal, Perlin>>.ScheduleParallel
			  , Job<Lattice2D<LatticeTiling, Perlin>>.ScheduleParallel
			  , Job<Lattice3D<LatticeNormal, Perlin>>.ScheduleParallel
			  , Job<Lattice3D<LatticeTiling, Perlin>>.ScheduleParallel
			}
		   ,
			{
				Job<Lattice1D<LatticeNormal, Turbulence<Perlin>>>.ScheduleParallel
			  , Job<Lattice1D<LatticeTiling, Turbulence<Perlin>>>.ScheduleParallel
			  , Job<Lattice2D<LatticeNormal, Turbulence<Perlin>>>.ScheduleParallel
			  , Job<Lattice2D<LatticeTiling, Turbulence<Perlin>>>.ScheduleParallel
			  , Job<Lattice3D<LatticeNormal, Turbulence<Perlin>>>.ScheduleParallel
			  , Job<Lattice3D<LatticeTiling, Turbulence<Perlin>>>.ScheduleParallel
			}
		   ,
			{
				Job<Lattice1D<LatticeNormal, Value>>.ScheduleParallel
			  , Job<Lattice1D<LatticeTiling, Value>>.ScheduleParallel
			  , Job<Lattice2D<LatticeNormal, Value>>.ScheduleParallel
			  , Job<Lattice2D<LatticeTiling, Value>>.ScheduleParallel
			  , Job<Lattice3D<LatticeNormal, Value>>.ScheduleParallel
			  , Job<Lattice3D<LatticeTiling, Value>>.ScheduleParallel
			}
		   ,
			{
				Job<Lattice1D<LatticeNormal, Turbulence<Value>>>.ScheduleParallel
			  , Job<Lattice1D<LatticeTiling, Turbulence<Value>>>.ScheduleParallel
			  , Job<Lattice2D<LatticeNormal, Turbulence<Value>>>.ScheduleParallel
			  , Job<Lattice2D<LatticeTiling, Turbulence<Value>>>.ScheduleParallel
			  , Job<Lattice3D<LatticeNormal, Turbulence<Value>>>.ScheduleParallel
			  , Job<Lattice3D<LatticeTiling, Turbulence<Value>>>.ScheduleParallel
			}
		   ,
			{
				Job<Simplex1D<Simplex>>.ScheduleParallel
			  , Job<Simplex1D<Simplex>>.ScheduleParallel
			  , Job<Simplex2D<Simplex>>.ScheduleParallel
			  , Job<Simplex2D<Simplex>>.ScheduleParallel
			  , Job<Simplex3D<Simplex>>.ScheduleParallel
			  , Job<Simplex3D<Simplex>>.ScheduleParallel
			}
		   ,
			{
				Job<Simplex1D<Turbulence<Simplex>>>.ScheduleParallel
			  , Job<Simplex1D<Turbulence<Simplex>>>.ScheduleParallel
			  , Job<Simplex2D<Turbulence<Simplex>>>.ScheduleParallel
			  , Job<Simplex2D<Turbulence<Simplex>>>.ScheduleParallel
			  , Job<Simplex3D<Turbulence<Simplex>>>.ScheduleParallel
			  , Job<Simplex3D<Turbulence<Simplex>>>.ScheduleParallel
			}
		   ,
			{
				Job<Simplex1D<Value>>.ScheduleParallel
			  , Job<Simplex1D<Value>>.ScheduleParallel
			  , Job<Simplex2D<Value>>.ScheduleParallel
			  , Job<Simplex2D<Value>>.ScheduleParallel
			  , Job<Simplex3D<Value>>.ScheduleParallel
			  , Job<Simplex3D<Value>>.ScheduleParallel
			}
		   ,
			{
				Job<Simplex1D<Turbulence<Value>>>.ScheduleParallel
			  , Job<Simplex1D<Turbulence<Value>>>.ScheduleParallel
			  , Job<Simplex2D<Turbulence<Value>>>.ScheduleParallel
			  , Job<Simplex2D<Turbulence<Value>>>.ScheduleParallel
			  , Job<Simplex3D<Turbulence<Value>>>.ScheduleParallel
			  , Job<Simplex3D<Turbulence<Value>>>.ScheduleParallel
			}
		   ,
			{
				Job<Voronoi1D<LatticeNormal, WorleyDistance, F1>>.ScheduleParallel
			  , Job<Voronoi1D<LatticeTiling, WorleyDistance, F1>>.ScheduleParallel
			  , Job<Voronoi2D<LatticeNormal, WorleyDistance, F1>>.ScheduleParallel
			  , Job<Voronoi2D<LatticeTiling, WorleyDistance, F1>>.ScheduleParallel
			  , Job<Voronoi3D<LatticeNormal, WorleyDistance, F1>>.ScheduleParallel
			  , Job<Voronoi3D<LatticeTiling, WorleyDistance, F1>>.ScheduleParallel
			}
		   ,
			{
				Job<Voronoi1D<LatticeNormal, WorleyDistance, F2>>.ScheduleParallel
			  , Job<Voronoi1D<LatticeTiling, WorleyDistance, F2>>.ScheduleParallel
			  , Job<Voronoi2D<LatticeNormal, WorleyDistance, F2>>.ScheduleParallel
			  , Job<Voronoi2D<LatticeTiling, WorleyDistance, F2>>.ScheduleParallel
			  , Job<Voronoi3D<LatticeNormal, WorleyDistance, F2>>.ScheduleParallel
			  , Job<Voronoi3D<LatticeTiling, WorleyDistance, F2>>.ScheduleParallel
			}
		   ,
			{
				Job<Voronoi1D<LatticeNormal, WorleyDistance, F2MinusF1>>.ScheduleParallel
			  , Job<Voronoi1D<LatticeTiling, WorleyDistance, F2MinusF1>>.ScheduleParallel
			  , Job<Voronoi2D<LatticeNormal, WorleyDistance, F2MinusF1>>.ScheduleParallel
			  , Job<Voronoi2D<LatticeTiling, WorleyDistance, F2MinusF1>>.ScheduleParallel
			  , Job<Voronoi3D<LatticeNormal, WorleyDistance, F2MinusF1>>.ScheduleParallel
			  , Job<Voronoi3D<LatticeTiling, WorleyDistance, F2MinusF1>>.ScheduleParallel
			}
		   ,
			{
				Job<Voronoi1D<LatticeNormal, WorleyDistance, F1>>.ScheduleParallel
			  , Job<Voronoi1D<LatticeTiling, WorleyDistance, F1>>.ScheduleParallel
			  , Job<Voronoi2D<LatticeNormal, ChebyshevDistance, F1>>.ScheduleParallel
			  , Job<Voronoi2D<LatticeTiling, ChebyshevDistance, F1>>.ScheduleParallel
			  , Job<Voronoi3D<LatticeNormal, ChebyshevDistance, F1>>.ScheduleParallel
			  , Job<Voronoi3D<LatticeTiling, ChebyshevDistance, F1>>.ScheduleParallel
			}
		   ,
			{
				Job<Voronoi1D<LatticeNormal, WorleyDistance, F2>>.ScheduleParallel
			  , Job<Voronoi1D<LatticeTiling, WorleyDistance, F2>>.ScheduleParallel
			  , Job<Voronoi2D<LatticeNormal, ChebyshevDistance, F2>>.ScheduleParallel
			  , Job<Voronoi2D<LatticeTiling, ChebyshevDistance, F2>>.ScheduleParallel
			  , Job<Voronoi3D<LatticeNormal, ChebyshevDistance, F2>>.ScheduleParallel
			  , Job<Voronoi3D<LatticeTiling, ChebyshevDistance, F2>>.ScheduleParallel
			}
		   ,
			{
				Job<Voronoi1D<LatticeNormal, WorleyDistance, F2MinusF1>>.ScheduleParallel
			  , Job<Voronoi1D<LatticeTiling, WorleyDistance, F2MinusF1>>.ScheduleParallel
			  , Job<Voronoi2D<LatticeNormal, ChebyshevDistance, F2MinusF1>>.ScheduleParallel
			  , Job<Voronoi2D<LatticeTiling, ChebyshevDistance, F2MinusF1>>.ScheduleParallel
			  , Job<Voronoi3D<LatticeNormal, ChebyshevDistance, F2MinusF1>>.ScheduleParallel
			  , Job<Voronoi3D<LatticeTiling, ChebyshevDistance, F2MinusF1>>.ScheduleParallel
			}
		};

		private static readonly int NoiseId = Shader.PropertyToID("_Noise");
		
		public enum NoiseType
		{
			Perlin
		  , PerlinTurbulence
		  , Value
		  , ValueTurbulence
		  , Simplex
		  , SimplexTurbulence
		  , SimplexValue
		  , SimplexValueTurbulence
		  , VoronoiWorleyF1
		  , VoronoiWorleyF2
		  , VoronoiWorleyF2MinusF1
		  , VoronoiChebyshevF1
		  , VoronoiChebyshevF2
		  , VoronoiChebyshevF2MinusF1
		}

		[SerializeField]              private Settings  noiseSettings = Settings.Default;
		[SerializeField]              private NoiseType type;
		[SerializeField, Range(1, 3)] private int       dimensions = 3;
		[SerializeField]              private bool      tiling;
		[SerializeField]              private SpaceTRS  domain = new SpaceTRS {scale = 8f};

		private NativeArray<float4> m_noise;
		private ComputeBuffer       m_noiseBuffer;

		protected override void EnableVisualization(int dataLength, MaterialPropertyBlock propertyBlock)
		{
			m_noise       = new NativeArray<float4>(dataLength, Allocator.Persistent);
			m_noiseBuffer = new ComputeBuffer(dataLength * 4, 4);
			propertyBlock.SetBuffer(NoiseId, m_noiseBuffer);
		}

		protected override void DisableVisualization()
		{
			m_noise.Dispose();
			m_noiseBuffer.Release();
			m_noiseBuffer = null;
		}

		protected override void UpdateVisualization(NativeArray<float3x4> positions, int resolution, JobHandle handle)
		{
			NoiseJobs[(int) type, 2 * dimensions - (tiling ? 1 : 2)](positions, m_noise, noiseSettings, domain, resolution, handle)
			   .Complete();
			m_noiseBuffer.SetData(m_noise.Reinterpret<float>(4 * 4));
		}
	}
}