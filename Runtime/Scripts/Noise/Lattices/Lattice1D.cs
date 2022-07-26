using AbstractInterfaces;
using Hash;
using Types;
using Unity.Mathematics;
using static Unity.Mathematics.math;


namespace Noise.Lattices
{ 
	public struct Lattice1D<TLattice, TGradient> : INoise
		where TLattice : struct, ILattice 
		where TGradient : struct, IGradient
	{
		public float4 GetNoise4(float4x3 positions, SmallXxHash4 hash, int frequency)
		{
			LatticeSpan4 x = default(TLattice).GetLatticeSpan4(positions.c0, frequency);

			var g = default(TGradient);
			return g.EvaluateCombined(lerp(g.Evaluate(hash.Eat(x.p0), x.g0), g.Evaluate(hash.Eat(x.p1), x.g1), x.t));
		}
	}
}