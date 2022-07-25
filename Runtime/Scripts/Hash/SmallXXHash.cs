using Unity.Mathematics;

namespace Hash
{
	public readonly struct SmallXxHash
	{
		private const uint k_primeA = 0b10011110001101110111100110110001;
		private const uint k_primeB = 0b10000101111010111100101001110111;
		private const uint k_primeC = 0b11000010101100101010111000111101;
		private const uint k_primeD = 0b00100111110101001110101100101111;
		private const uint k_primeE = 0b00010110010101100110011110110001;

		private readonly uint m_accumulator;

		public SmallXxHash(uint accumulator)
		{
			m_accumulator = accumulator;
		}
		public SmallXxHash Eat(int  data) => RotateLeft(m_accumulator + (uint) data * k_primeC, 17) * k_primeD;
		public SmallXxHash Eat(byte data) => RotateLeft(m_accumulator + data        * k_primeE, 11) * k_primeA;

		public static implicit operator SmallXxHash(uint            accumulator)     => new SmallXxHash(accumulator);
		public static                   SmallXxHash Seed(int        seed)            => (uint) seed + k_primeE;
		private static                  uint        RotateLeft(uint data, int steps) => (data << steps) | (data >> 32 - steps);
		public static implicit operator uint(SmallXxHash hash)
		{
			uint avalanche = hash.m_accumulator;
			avalanche ^= avalanche >> 15;
			avalanche *= k_primeB;
			avalanche ^= avalanche >> 13;
			avalanche *= k_primeC;
			avalanche ^= avalanche >> 16;
			return avalanche;
		}

		public static implicit operator SmallXxHash4(SmallXxHash hash) => new SmallXxHash4(hash.m_accumulator);
	}

	public readonly struct SmallXxHash4
	{
		private const uint k_primeB = 0b10000101111010111100101001110111;
		private const uint k_primeC = 0b11000010101100101010111000111101;
		private const uint k_primeD = 0b00100111110101001110101100101111;
		private const uint k_primeE = 0b00010110010101100110011110110001;

		private readonly uint4 m_accumulator;

		public uint4 BytesA => (uint4) this         & 255;
		public uint4 BytesB => ((uint4) this >> 8)  & 255;
		public uint4 BytesC => ((uint4) this >> 16) & 255;
		public uint4 BytesD => (uint4) this >> 24;

		public float4 Floats01A => (float4) BytesA * (1f / 255f);
		public float4 Floats01B => (float4) BytesB * (1f / 255f);
		public float4 Floats01C => (float4) BytesC * (1f / 255f);
		public float4 Floats01D => (float4) BytesD * (1f / 255f);

		public SmallXxHash4(uint4 accumulator)
		{
			m_accumulator = accumulator;
		}
		
		public SmallXxHash4 Eat(int4              data)             => RotateLeft(m_accumulator + (uint4) data * k_primeC, 17) * k_primeD;
		public uint4        GetBits(int           count, int shift) => ((uint4) this >> shift) & (uint) ((1 << count) - 1);
		public float4       GetBitsAsFloats01(int count, int shift) => (float4) GetBits(count, shift) * (1f / ((1 << count) - 1));

		public static implicit operator SmallXxHash4(uint4            accumulator)     => new SmallXxHash4(accumulator);
		public static                   SmallXxHash4 Seed(int4        seed)            => (uint4) seed + k_primeE;
		private static                  uint4        RotateLeft(uint4 data, int steps) => (data << steps) | (data >> 32 - steps);
		public static SmallXxHash4 Select(SmallXxHash4 a, SmallXxHash4 b, bool4 c) => math.select(a.m_accumulator, b.m_accumulator, c);
		public static SmallXxHash4 operator +(SmallXxHash4 h, int v) => h.m_accumulator + (uint) v;
		public static implicit operator uint4(SmallXxHash4 hash)
		{
			uint4 avalanche = hash.m_accumulator;
			avalanche ^= avalanche >> 15;
			avalanche *= k_primeB;
			avalanche ^= avalanche >> 13;
			avalanche *= k_primeC;
			avalanche ^= avalanche >> 16;
			return avalanche;
		}
	}
}