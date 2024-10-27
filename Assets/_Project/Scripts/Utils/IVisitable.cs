using UnityEngine;

namespace UtilsModule {
    public interface IVisitable {
        void Accept(IVisitor visitor);
    }
}