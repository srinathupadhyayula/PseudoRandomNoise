using Unity.Mathematics;

namespace AbstractInterfaces
{
    public interface IVoronoiFunction
    {
        float4 Evaluate(float4x2 minima);
    }
}