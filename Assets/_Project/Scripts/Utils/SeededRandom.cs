using System;

namespace UtilsModule {
    public static class SeededRandom {
		static Random rand = new Random();
        static int seed = 0;

        public static void SetSeed(int seed) {
            rand = new Random(seed);
            SeededRandom.seed = seed;
        }

        public static int GetSeed() {
            return seed;
        }

        public static int GetRange(int min, int max) {
            return rand.Next(min, max);
        }
	}
}