using Hash;
using Unity.Mathematics;
using static Unity.Mathematics.math;

namespace Noise
{
	public static partial class Noise
	{

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

		public struct Simplex2D<TGradient> : INoise where TGradient : struct, IGradient
		{

			public float4 GetNoise4(float4x3 positions, SmallXxHash4 hash, int frequency)
			{
				positions *= frequency * (1f                / sqrt(3f));
				float4 skew = (positions.c0 + positions.c2) * ((sqrt(3f) - 1f) / 2f);
				float4 sx   = positions.c0 + skew, sz = positions.c2 + skew;
				int4
					x0 = (int4) floor(sx), x1 = x0 + 1, z0 = (int4) floor(sz), z1 = z0 + 1;

				bool4 xGz = sx - x0 > sz - z0;
				int4  xC  = select(x0, x1, xGz), zC = select(z1, z0, xGz);

				SmallXxHash4
					h0 = hash.Eat(x0), h1 = hash.Eat(x1), hC = SmallXxHash4.Select(h0, h1, xGz);

				return default(TGradient).EvaluateCombined(Kernel(h0.Eat(z0), x0, z0, positions)
												 + Kernel(h1.Eat(z1), x1, z1, positions)
												 + Kernel(hC.Eat(zC), xC, zC, positions));
			}

			private static float4 Kernel(SmallXxHash4 hash, float4 lx, float4 lz, float4x3 positions)
			{
				float4 unskew = (lx + lz) * ((3f - sqrt(3f)) / 6f);
				float4 x      = positions.c0 - lx + unskew, z = positions.c2 - lz + unskew;
				float4 f      = 0.5f              - x * x                         - z * z;
				f = f * f * f * 8f;
				return max(0f, f) * default(TGradient).Evaluate(hash, x, z);
			}
		}

		public struct Simplex3D<TGradient> : INoise where TGradient : struct, IGradient
		{

			public float4 GetNoise4(float4x3 positions, SmallXxHash4 hash, int frequency)
			{
				positions *= frequency * 0.6f;
				float4 skew = (positions.c0 + positions.c1 + positions.c2) * (1f / 3f);
				float4
					sx = positions.c0 + skew, sy = positions.c1 + skew, sz = positions.c2 + skew;
				int4
					x0 = (int4) floor(sx), x1 = x0 + 1, y0 = (int4) floor(sy), y1 = y0 + 1, z0 = (int4) floor(sz), z1 = z0 + 1;

				bool4
					xGy = sx - x0 > sy - y0, xGz = sx - x0 > sz - z0, yGz = sy - y0 > sz - z0;

				bool4
					xA = xGy & xGz
				  , xB = xGy | (xGz & yGz)
				  , yA = !xGy & yGz
				  , yB = !xGy | (xGz & yGz)
				  , zA = (xGy        & !xGz) | (!xGy & !yGz)
				  , zB = !(xGz       & yGz);

				int4
					xCa = select(x0, x1, xA)
				  , xCb = select(x0, x1, xB)
				  , yCa = select(y0, y1, yA)
				  , yCb = select(y0, y1, yB)
				  , zCa = select(z0, z1, zA)
				  , zCb = select(z0, z1, zB);

				SmallXxHash4
					h0 = hash.Eat(x0), h1 = hash.Eat(x1), hA = SmallXxHash4.Select(h0, h1, xA), hB = SmallXxHash4.Select(h0, h1, xB);

				return default(TGradient).EvaluateCombined(Kernel(h0.Eat(y0).Eat(z0),   x0,  y0,  z0,  positions)
												 + Kernel(h1.Eat(y1).Eat(z1),   x1,  y1,  z1,  positions)
												 + Kernel(hA.Eat(yCa).Eat(zCa), xCa, yCa, zCa, positions)
												 + Kernel(hB.Eat(yCb).Eat(zCb), xCb, yCb, zCb, positions));
			}

			private static float4 Kernel(SmallXxHash4 hash, float4 lx, float4 ly, float4 lz, float4x3 positions)
			{
				float4 unskew = (lx + ly + lz) * (1f / 6f);
				float4
					x = positions.c0 - lx + unskew, y = positions.c1 - ly + unskew, z = positions.c2 - lz + unskew;
				float4 f = 0.5f - x * x - y * y - z * z;
				f = f * f * f * 8f;
				return max(0f, f) * default(TGradient).Evaluate(hash, x, y, z);
			}
		}
	}
}