using UtilsModule;

namespace UtilsModule {
    public interface IEffector : IVisitor {
        public abstract void ApplyEffect(Entity entity);
    }
}