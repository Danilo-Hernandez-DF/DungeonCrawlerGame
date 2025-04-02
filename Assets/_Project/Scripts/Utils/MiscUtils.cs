namespace _Project.Scripts.Utils {
    public static class MiscUtils {
        public static float LinearMapping(float currentValue, float currentMin, float currentMax, float newMin, float newMax) {
            float newSize = newMax - newMin;
            float oldSize = currentMax - currentMin;
            float oldScale = currentValue - currentMin;
            return (newSize * oldScale / oldSize) + newMin;
        }
    }
}