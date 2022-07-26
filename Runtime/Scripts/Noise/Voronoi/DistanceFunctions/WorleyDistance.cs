using AbstractInterfaces;
using Unity.Mathematics;
using static Unity.Mathematics.math;


namespace Noise.Voronoi.DistanceFunctions
{
	public struct WorleyDistance : IVoronoiDistance
	{
		public float4   GetDistance(float4  x)                     => abs(x);
		public float4   GetDistance(float4  x, float4 y)           => x * x + y * y;
		public float4   GetDistance(float4  x, float4 y, float4 z) => x * x + y * y + z * z;
		public float4x2 Finalize1D(float4x2 minima) => minima;
			
		public float4x2 Finalize2D(float4x2 minima)
		{
			minima.c0 = sqrt(min(minima.c0, 1f));
			minima.c1 = sqrt(min(minima.c1, 1f));
			return minima;
		}

		public float4x2 Finalize3D(float4x2 minima) => Finalize2D(minima);
	}
}