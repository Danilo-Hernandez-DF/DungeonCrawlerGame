namespace Game
{
    public class Counter
    {
        public int current;
        public int max;

        public Counter(int max) {
            this.max = max;
            current = 0;
        }
        
        public virtual int Count() {
            current++;
            return current;
        }
        
        public virtual void Reset() {
            current = 0;
        }
        
        public virtual bool ReachedMax() {
            return current >= max;
        }
    }
}