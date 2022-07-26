using AbstractInterfaces;
using Hash;
using Unity.Mathematics;
using static Unity.Mathematics.math;


public struct Simplex1D<TGradient> : INoise where TGradient : struct, IGradient
{
	public float4 GetNoise4(float4x3 positions, SmallXxHash4 hash, int frequency)
	{
		positions *= frequency;
		int4 x0 = (int4) floor(positions.c0), x1 = x0 + 1;

		return default(TGradient).EvaluateCombined(Kernel(hash.Eat(x0), x0, positions) + Kernel(hash.Eat(x1), x1, positions));
	}
	private static float4 Kernel(SmallXxHash4 hash, float4 lx, float4x3 positions)
	{
		float4 x = positions.c0 - lx;
		float4 f = 1f           - x * x;
		f = f * f * f;
		return f * default(TGradient).Evaluate(hash, x);
	}
}