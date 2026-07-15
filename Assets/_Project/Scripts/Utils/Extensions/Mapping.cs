namespace Utils
{
    public static class Mapping
    {
        public static int LinearMapping(int currentValue, int currentMin, int currentMax, int newMin, int newMax)
        {
            int newSize = newMax - newMin;
            int oldSize = currentMax - currentMin;
            int oldScale = currentValue - currentMin;
            return (newSize * oldScale / oldSize) + newMin;
        }

        public static float LinearMapping(float currentValue, float currentMin, float currentMax, float newMin, float newMax) {
            float newSize = newMax - newMin;
            float oldSize = currentMax - currentMin;
            float oldScale = currentValue - currentMin;
            return (newSize * oldScale / oldSize) + newMin;
        }
    }
}