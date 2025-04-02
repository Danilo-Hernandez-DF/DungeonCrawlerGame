using System;
using System.Collections.Generic;
using UnityEngine;

namespace UtilsModule {
    public class SlotFilter : ScriptableObject {
        IPredicate predicate;
        Item toEval;
        bool isInitialized = false;

        private void Init() {
            predicate = new FuncPredicate<Item>(i => Evaluate(toEval));
            isInitialized = true;
            //Debug.Log("Initialized");
        }

        public bool MatchesFilter(Item item) {
            if(!isInitialized) Init();
            toEval = item;
            return predicate.Evaluate();
        }

        public virtual bool Evaluate(Item item) {
            return true;
        }
    }

    public enum ComparisonType {
        Equal,
        NotEqual,
        GreaterThan,
        LessThan
    }
}