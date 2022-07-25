using Types;
using Unity.Mathematics;

namespace AbstractInterfaces
{
    public interface ILattice
    {
        LatticeSpan4 GetLatticeSpan4(float4  coordinates, int frequency);
        int4         ValidateSingleStep(int4 points,      int frequency);
    }
}