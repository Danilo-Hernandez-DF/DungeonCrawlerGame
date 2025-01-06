using System;

namespace UtilsModule {
    public static class SeededRandom {
		static Random _rand = new Random();
        static int _seed = 0;

        public static void SetSeed(int seed) {
            _rand = new Random(seed);
            SeededRandom._seed = seed;
        }

        public static int GetSeed() {
            return _seed;
        }

        public static int GetRange(int min, int max) {
            return _rand.Next(min, max);
        }
	}
}