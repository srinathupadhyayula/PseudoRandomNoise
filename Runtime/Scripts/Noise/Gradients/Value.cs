using AbstractInterfaces;
using Hash;
using Unity.Mathematics;

namespace Noise.Gradients
{
    public struct Value : IGradient
    {
        public float4 Evaluate(SmallXxHash4   hash, float4 x)                     => hash.Floats01A * 2f - 1f;
        public float4 Evaluate(SmallXxHash4   hash, float4 x, float4 y)           => hash.Floats01A * 2f - 1f;
        public float4 Evaluate(SmallXxHash4   hash, float4 x, float4 y, float4 z) => hash.Floats01A * 2f - 1f;
        public float4 EvaluateCombined(float4 value) => value;
    }
}