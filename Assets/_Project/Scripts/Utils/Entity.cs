using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

namespace UtilsModule {
    public abstract class Entity : MonoBehaviour, IVisitable {
        [SerializeField] BaseStats baseStats; 
        public Stats Stats {get; private set;}

        protected void Awake() {
            Stats = new Stats(new StatsMediator(), baseStats);
        }

        public void Update() {
            Stats.Mediator.Update(Time.deltaTime);
        }

        public void Accept(IVisitor visitor) => visitor.Visit(this);
    }
}