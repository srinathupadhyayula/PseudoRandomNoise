using AbstractInterfaces;
using Hash;
using Unity.Mathematics;
using Utilities;

namespace Noise.Gradients
{
    public struct Perlin : IGradient
    {
        public float4 Evaluate(SmallXxHash4 hash, float4 x)           => NoiseUtils.Line(hash, x);
        public float4 Evaluate(SmallXxHash4 hash, float4 x, float4 y) => NoiseUtils.Square(hash, x, y) * (2f / 0.53528f);
        public float4 Evaluate(SmallXxHash4 hash, float4 x, float4 y, float4 z) =>
            NoiseUtils.Octahedron(hash, x, y, z) * (1f / 0.56290f);
        public float4 EvaluateCombined(float4 value) => value;
    }
}