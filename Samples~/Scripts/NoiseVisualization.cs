using Noise;
using Noise.Gradients;
using Noise.Lattices;
using Noise.Voronoi;
using Noise.Voronoi.DistanceFunctions;
using Noise.Voronoi.VoronoiFunctions;
using Types;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using ScheduleNoiseDelegate = Noise.ScheduleNoiseDelegate;

public class NoiseVisualization : Visualization
{
    private static readonly ScheduleNoiseDelegate[,] NoiseJobs =
    {
        {
            NoiseJob<Lattice1D<LatticeNormal, Perlin>>.ScheduleParallel
          , NoiseJob<Lattice1D<LatticeTiling, Perlin>>.ScheduleParallel
          , NoiseJob<Lattice2D<LatticeNormal, Perlin>>.ScheduleParallel
          , NoiseJob<Lattice2D<LatticeTiling, Perlin>>.ScheduleParallel
          , NoiseJob<Lattice3D<LatticeNormal, Perlin>>.ScheduleParallel
          , NoiseJob<Lattice3D<LatticeTiling, Perlin>>.ScheduleParallel
        }
       ,
        {
            NoiseJob<Lattice1D<LatticeNormal, Turbulence<Perlin>>>.ScheduleParallel
          , NoiseJob<Lattice1D<LatticeTiling, Turbulence<Perlin>>>.ScheduleParallel
          , NoiseJob<Lattice2D<LatticeNormal, Turbulence<Perlin>>>.ScheduleParallel
          , NoiseJob<Lattice2D<LatticeTiling, Turbulence<Perlin>>>.ScheduleParallel
          , NoiseJob<Lattice3D<LatticeNormal, Turbulence<Perlin>>>.ScheduleParallel
          , NoiseJob<Lattice3D<LatticeTiling, Turbulence<Perlin>>>.ScheduleParallel
        }
       ,
        {
            NoiseJob<Lattice1D<LatticeNormal, Value>>.ScheduleParallel
          , NoiseJob<Lattice1D<LatticeTiling, Value>>.ScheduleParallel
          , NoiseJob<Lattice2D<LatticeNormal, Value>>.ScheduleParallel
          , NoiseJob<Lattice2D<LatticeTiling, Value>>.ScheduleParallel
          , NoiseJob<Lattice3D<LatticeNormal, Value>>.ScheduleParallel
          , NoiseJob<Lattice3D<LatticeTiling, Value>>.ScheduleParallel
        }
       ,
        {
            NoiseJob<Lattice1D<LatticeNormal, Turbulence<Value>>>.ScheduleParallel
          , NoiseJob<Lattice1D<LatticeTiling, Turbulence<Value>>>.ScheduleParallel
          , NoiseJob<Lattice2D<LatticeNormal, Turbulence<Value>>>.ScheduleParallel
          , NoiseJob<Lattice2D<LatticeTiling, Turbulence<Value>>>.ScheduleParallel
          , NoiseJob<Lattice3D<LatticeNormal, Turbulence<Value>>>.ScheduleParallel
          , NoiseJob<Lattice3D<LatticeTiling, Turbulence<Value>>>.ScheduleParallel
        }
       ,
        {
            NoiseJob<Simplex1D<Simplex>>.ScheduleParallel
          , NoiseJob<Simplex1D<Simplex>>.ScheduleParallel
          , NoiseJob<Simplex2D<Simplex>>.ScheduleParallel
          , NoiseJob<Simplex2D<Simplex>>.ScheduleParallel
          , NoiseJob<Simplex3D<Simplex>>.ScheduleParallel
          , NoiseJob<Simplex3D<Simplex>>.ScheduleParallel
        }
       ,
        {
            NoiseJob<Simplex1D<Turbulence<Simplex>>>.ScheduleParallel
          , NoiseJob<Simplex1D<Turbulence<Simplex>>>.ScheduleParallel
          , NoiseJob<Simplex2D<Turbulence<Simplex>>>.ScheduleParallel
          , NoiseJob<Simplex2D<Turbulence<Simplex>>>.ScheduleParallel
          , NoiseJob<Simplex3D<Turbulence<Simplex>>>.ScheduleParallel
          , NoiseJob<Simplex3D<Turbulence<Simplex>>>.ScheduleParallel
        }
       ,
        {
            NoiseJob<Simplex1D<Value>>.ScheduleParallel
          , NoiseJob<Simplex1D<Value>>.ScheduleParallel
          , NoiseJob<Simplex2D<Value>>.ScheduleParallel
          , NoiseJob<Simplex2D<Value>>.ScheduleParallel
          , NoiseJob<Simplex3D<Value>>.ScheduleParallel
          , NoiseJob<Simplex3D<Value>>.ScheduleParallel
        }
       ,
        {
            NoiseJob<Simplex1D<Turbulence<Value>>>.ScheduleParallel
          , NoiseJob<Simplex1D<Turbulence<Value>>>.ScheduleParallel
          , NoiseJob<Simplex2D<Turbulence<Value>>>.ScheduleParallel
          , NoiseJob<Simplex2D<Turbulence<Value>>>.ScheduleParallel
          , NoiseJob<Simplex3D<Turbulence<Value>>>.ScheduleParallel
          , NoiseJob<Simplex3D<Turbulence<Value>>>.ScheduleParallel
        }
       ,
        {
            NoiseJob<Voronoi1D<LatticeNormal, WorleyDistance, F1>>.ScheduleParallel
          , NoiseJob<Voronoi1D<LatticeTiling, WorleyDistance, F1>>.ScheduleParallel
          , NoiseJob<Voronoi2D<LatticeNormal, WorleyDistance, F1>>.ScheduleParallel
          , NoiseJob<Voronoi2D<LatticeTiling, WorleyDistance, F1>>.ScheduleParallel
          , NoiseJob<Voronoi3D<LatticeNormal, WorleyDistance, F1>>.ScheduleParallel
          , NoiseJob<Voronoi3D<LatticeTiling, WorleyDistance, F1>>.ScheduleParallel
        }
       ,
        {
            NoiseJob<Voronoi1D<LatticeNormal, WorleyDistance, F2>>.ScheduleParallel
          , NoiseJob<Voronoi1D<LatticeTiling, WorleyDistance, F2>>.ScheduleParallel
          , NoiseJob<Voronoi2D<LatticeNormal, WorleyDistance, F2>>.ScheduleParallel
          , NoiseJob<Voronoi2D<LatticeTiling, WorleyDistance, F2>>.ScheduleParallel
          , NoiseJob<Voronoi3D<LatticeNormal, WorleyDistance, F2>>.ScheduleParallel
          , NoiseJob<Voronoi3D<LatticeTiling, WorleyDistance, F2>>.ScheduleParallel
        }
       ,
        {
            NoiseJob<Voronoi1D<LatticeNormal, WorleyDistance, F2MinusF1>>.ScheduleParallel
          , NoiseJob<Voronoi1D<LatticeTiling, WorleyDistance, F2MinusF1>>.ScheduleParallel
          , NoiseJob<Voronoi2D<LatticeNormal, WorleyDistance, F2MinusF1>>.ScheduleParallel
          , NoiseJob<Voronoi2D<LatticeTiling, WorleyDistance, F2MinusF1>>.ScheduleParallel
          , NoiseJob<Voronoi3D<LatticeNormal, WorleyDistance, F2MinusF1>>.ScheduleParallel
          , NoiseJob<Voronoi3D<LatticeTiling, WorleyDistance, F2MinusF1>>.ScheduleParallel
        }
       ,
        {
            NoiseJob<Voronoi1D<LatticeNormal, WorleyDistance, F1>>.ScheduleParallel
          , NoiseJob<Voronoi1D<LatticeTiling, WorleyDistance, F1>>.ScheduleParallel
          , NoiseJob<Voronoi2D<LatticeNormal, ChebyshevDistance, F1>>.ScheduleParallel
          , NoiseJob<Voronoi2D<LatticeTiling, ChebyshevDistance, F1>>.ScheduleParallel
          , NoiseJob<Voronoi3D<LatticeNormal, ChebyshevDistance, F1>>.ScheduleParallel
          , NoiseJob<Voronoi3D<LatticeTiling, ChebyshevDistance, F1>>.ScheduleParallel
        }
       ,
        {
            NoiseJob<Voronoi1D<LatticeNormal, WorleyDistance, F2>>.ScheduleParallel
          , NoiseJob<Voronoi1D<LatticeTiling, WorleyDistance, F2>>.ScheduleParallel
          , NoiseJob<Voronoi2D<LatticeNormal, ChebyshevDistance, F2>>.ScheduleParallel
          , NoiseJob<Voronoi2D<LatticeTiling, ChebyshevDistance, F2>>.ScheduleParallel
          , NoiseJob<Voronoi3D<LatticeNormal, ChebyshevDistance, F2>>.ScheduleParallel
          , NoiseJob<Voronoi3D<LatticeTiling, ChebyshevDistance, F2>>.ScheduleParallel
        }
       ,
        {
            NoiseJob<Voronoi1D<LatticeNormal, WorleyDistance, F2MinusF1>>.ScheduleParallel
          , NoiseJob<Voronoi1D<LatticeTiling, WorleyDistance, F2MinusF1>>.ScheduleParallel
          , NoiseJob<Voronoi2D<LatticeNormal, ChebyshevDistance, F2MinusF1>>.ScheduleParallel
          , NoiseJob<Voronoi2D<LatticeTiling, ChebyshevDistance, F2MinusF1>>.ScheduleParallel
          , NoiseJob<Voronoi3D<LatticeNormal, ChebyshevDistance, F2MinusF1>>.ScheduleParallel
          , NoiseJob<Voronoi3D<LatticeTiling, ChebyshevDistance, F2MinusF1>>.ScheduleParallel
        }
    };

    private static readonly int NoiseId = Shader.PropertyToID("_Noise");

        

    [SerializeField]              private NoiseSettings noiseSettings = NoiseSettings.Default;
    [SerializeField]              private NoiseType     type;
    [SerializeField, Range(1, 3)] private int           dimensions = 3;
    [SerializeField]              private bool          tiling;
    [SerializeField]              private SpaceTRS      domain = new SpaceTRS {scale = 8f};

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