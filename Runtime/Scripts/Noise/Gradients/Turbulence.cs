using AbstractInterfaces;
using Hash;
using Unity.Mathematics;

namespace Noise.Gradients
{
    public struct Turbulence<TGradient> : IGradient where TGradient : struct, IGradient
    {
        public float4 Evaluate(SmallXxHash4   hash, float4 x)                     => default(TGradient).Evaluate(hash, x);
        public float4 Evaluate(SmallXxHash4   hash, float4 x, float4 y)           => default(TGradient).Evaluate(hash, x, y);
        public float4 Evaluate(SmallXxHash4   hash, float4 x, float4 y, float4 z) => default(TGradient).Evaluate(hash, x, y, z);
        public float4 EvaluateCombined(float4 value) => math.abs(default(TGradient).EvaluateCombined(value));
    }
}