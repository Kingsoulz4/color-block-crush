using System;

namespace ColorBlockCrush.Tools
{
    public static class CantorPairing
    {
        public static ulong MakeId(ulong i, ulong j)
        {
            ulong s = i + j;
            return (s * (s + 1)) / 2 + j;
        }

        // Invert: ID -> (i, j)
        public static (ulong i, ulong j) Unpair(ulong id)
        {
            ulong w = (ulong)((Math.Sqrt(8.0 * id + 1) - 1) / 2);
            ulong t = w * (w + 1) / 2;
            ulong j = id - t;
            ulong i = w - j;
            return (i, j);
        }

        // k-dimensions:
        public static ulong MakeId3(ulong i, ulong j, ulong k)
        {
            ulong p = MakeId(i , j );
            return MakeId(p, k);
        }
    }
}
