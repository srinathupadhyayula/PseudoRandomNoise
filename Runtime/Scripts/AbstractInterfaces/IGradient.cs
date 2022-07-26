using Hash;
using Unity.Mathematics;

namespace AbstractInterfaces
{
    public interface IGradient
    {
        float4 Evaluate(SmallXxHash4   hash, float4 x);
        float4 Evaluate(SmallXxHash4   hash, float4 x, float4 y);
        float4 Evaluate(SmallXxHash4   hash, float4 x, float4 y, float4 z);
        float4 EvaluateCombined(float4 value);
    }
}