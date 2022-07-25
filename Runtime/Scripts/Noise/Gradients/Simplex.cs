using AbstractInterfaces;
using Hash;
using Unity.Mathematics;
using Utilities;

namespace Noise.Gradients
{
    public struct Simplex : IGradient
    {
        public float4 Evaluate(SmallXxHash4 hash, float4 x)           => NoiseUtils.Line(hash, x)      * (32f    / 27f);
        public float4 Evaluate(SmallXxHash4 hash, float4 x, float4 y) => NoiseUtils.Circle(hash, x, y) * (5.832f / math.sqrt(2f));
        public float4 Evaluate(SmallXxHash4 hash, float4 x, float4 y, float4 z) =>
            NoiseUtils.Sphere(hash, x, y, z) * (1024f / (125f * math.sqrt(3f)));
        public float4 EvaluateCombined(float4 value) => value;
    }
}