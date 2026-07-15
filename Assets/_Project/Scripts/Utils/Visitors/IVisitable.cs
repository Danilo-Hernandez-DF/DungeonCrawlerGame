namespace Utils {
    public interface IVisitable {
        void Accept(IVisitor visitor);
    }
}