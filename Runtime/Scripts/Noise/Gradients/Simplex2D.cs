using AbstractInterfaces;
using Hash;
using Unity.Mathematics;

public struct Simplex2D<TGradient> : INoise where TGradient : struct, IGradient
{
    public float4 GetNoise4(float4x3 positions, SmallXxHash4 hash, int frequency)
    {
        positions *= frequency * (1f                / math.sqrt(3f));
        float4 skew = (positions.c0 + positions.c2) * ((math.sqrt(3f) - 1f) / 2f);
        float4 sx   = positions.c0 + skew, sz = positions.c2 + skew;
        int4
            x0 = (int4) math.floor(sx), x1 = x0 + 1, z0 = (int4) math.floor(sz), z1 = z0 + 1;

        bool4 xGz = sx - x0 > sz - z0;
        int4  xC  = math.select(x0, x1, xGz), zC = math.select(z1, z0, xGz);

        SmallXxHash4
            h0 = hash.Eat(x0), h1 = hash.Eat(x1), hC = SmallXxHash4.Select(h0, h1, xGz);

        return default(TGradient).EvaluateCombined(Kernel(h0.Eat(z0), x0, z0, positions)
                                                 + Kernel(h1.Eat(z1), x1, z1, positions)
                                                 + Kernel(hC.Eat(zC), xC, zC, positions));
    }

    private static float4 Kernel(SmallXxHash4 hash, float4 lx, float4 lz, float4x3 positions)
    {
        float4 unskew = (lx + lz) * ((3f - math.sqrt(3f)) / 6f);
        float4 x      = positions.c0 - lx + unskew, z = positions.c2 - lz + unskew;
        float4 f      = 0.5f              - x * x                         - z * z;
        f = f * f * f * 8f;
        return math.max(0f, f) * default(TGradient).Evaluate(hash, x, z);
    }
}