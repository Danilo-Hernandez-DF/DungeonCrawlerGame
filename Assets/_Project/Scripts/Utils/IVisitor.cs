using UnityEngine;

namespace UtilsModule {
    public interface IVisitor {
        void Visit<T>(T visitable) where T : Component, IVisitable;
    }
}