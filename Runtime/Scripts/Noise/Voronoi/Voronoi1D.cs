using AbstractInterfaces;
using Hash;
using Types;
using Unity.Mathematics;
using Utilities;

namespace Noise.Voronoi
{
	public struct Voronoi1D<TLattice, TVoronoiDistance, TVoronoiFunction> : INoise
		where TLattice : struct, ILattice
		where TVoronoiDistance : struct, IVoronoiDistance
		where TVoronoiFunction : struct, IVoronoiFunction
	{

		public float4 GetNoise4(float4x3 positions, SmallXxHash4 hash, int frequency)
		{
			var          l = default(TLattice);
			var          d = default(TVoronoiDistance);
			LatticeSpan4 x = l.GetLatticeSpan4(positions.c0, frequency);

			float4x2 minima = 2f;
			for (int u = -1; u <= 1; u++)
			{
				SmallXxHash4 h = hash.Eat(l.ValidateSingleStep(x.p0 + u, frequency));
				minima =
					NoiseUtils.UpdateVoronoiMinima(minima, d.GetDistance(h.Floats01A + u - x.g0));
			}

			return default(TVoronoiFunction).Evaluate(d.Finalize1D(minima));
		}
	}
}