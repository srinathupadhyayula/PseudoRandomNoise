using Hash;
using Unity.Mathematics;

namespace AbstractInterfaces
{
    public interface INoise
    {
        float4 GetNoise4(float4x3 positions, SmallXxHash4 hash, int frequency);
    }
}