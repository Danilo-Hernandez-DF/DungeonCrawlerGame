using UtilsModule;

namespace UtilsModule {
    public interface IEffector : IVisitor {
        public void ApplyEffect(Entity entity, ModifierEffect modifierEffect);
        public void ApplyStatus(Entity entity, StatusEffect statusEffect);
    }
}