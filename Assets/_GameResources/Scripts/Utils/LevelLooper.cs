using System.Runtime.CompilerServices;

public static class LevelLooper
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetLoopBaseLevelId(int L, int N, int K)
    {
        if (N <= 0) throw new System.ArgumentOutOfRangeException(nameof(N), "N must be > 0.");
        if (L <= 0) throw new System.ArgumentOutOfRangeException(nameof(L), "L must be > 0.");
        
        K = Mod(K, N);
        if (K == 0) K = 1;

        int loopIndex = (L - 1) / N;
        int pos = (L - 1) % N;
        int offset = (int)((long)loopIndex * K % N);

        int baseLevelId = (pos + offset) % N + 1;
        return baseLevelId;
    }
    
    public static int AdjustKToCoprime(int K, int N)
    {
        if (N <= 1) return 1;
        K = Mod(K, N);
        if (K == 0) K = 1;
        
        for (int d = 0; d < N; d++)
        {
            int plus = Mod(K + d, N);
            if (plus == 0) plus = 1;
            if (Gcd(plus, N) == 1) return plus;

            int minus = Mod(K - d, N);
            if (minus == 0) minus = 1;
            if (Gcd(minus, N) == 1) return minus;
        }

        return 1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int Mod(int a, int m) => (int)((a % m + m) % m);

    private static int Gcd(int a, int b)
    {
        while (b != 0)
        {
            int t = a % b;
            a = b;
            b = t;
        }

        return a < 0 ? -a : a;
    }
}