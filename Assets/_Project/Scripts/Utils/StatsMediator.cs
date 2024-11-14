using System;
using System.Collections.Generic;

namespace UtilsModule {
    public class StatsMediator { 
        readonly LinkedList<StatModifier> modifiers = new();

        public event EventHandler<Query> Queries;
        public void PerformQuery(object sender, Query query) => Queries?.Invoke(sender, query);

        public void AddModifier(StatModifier modifier, string id = "") {
            modifier.id = id;
            modifiers.AddLast(modifier);
            Queries += modifier.Handle;

            modifier.OnDispose += _ => {
                Queries -= modifier.Handle;
            };
        }

        public void Update(float deltaTime) {
            var node = modifiers.First;
            while(node != null) {
                var modifier = node.Value;
                modifier.Update(deltaTime);
                node = node.Next;
            }

            node = modifiers.First;
            while(node != null) {
                var nextNode = node.Next;

                if(node.Value.MarkedForRemoval) {
                    node.Value.Dispose();
                }

                node = nextNode;
            }
        }

        public void RemoveModifiers(string id) {
            var node = modifiers.First;
            while(node != null) {
                var modifier = node.Value;
                if(modifier.id == id) {
                    modifier.MarkedForRemoval = true;
                }
                node = node.Next;
            }
        }
    }
}