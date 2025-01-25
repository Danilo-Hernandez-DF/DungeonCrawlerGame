using System;

namespace UtilsModule {
    public class FuncPredicate : IPredicate {
        readonly Func<bool> func;

        public FuncPredicate(Func<bool> func) {
            this.func = func;
        }

        public bool Evaluate() => func.Invoke();
    }

    public class FuncPredicate<T> : IPredicate {
        T argument;
        
        readonly Func<T, bool> func;

        public FuncPredicate(Func<T, bool> func) {
            this.func = func;
        }

        public bool Evaluate() => func.Invoke(argument);

        public bool Evaluate(T argument) {
            SetArgument(argument);
            return Evaluate();
        }

        private void SetArgument(T argument) {
            this.argument = argument;
        }
    }
}