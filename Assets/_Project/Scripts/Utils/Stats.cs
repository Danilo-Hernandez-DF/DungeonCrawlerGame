using System;
using UnityEngine;

namespace UtilsModule {
    public enum StatType { 
        Defense,
        Speed,
        Health,
        DashForce,
        DashDuration,
        DashCooldown,
        Attack,
        AttackCooldown,
        AttackRange,
        AttackDistance,
        StunDuration
    }

    [Serializable]
    public class Stats {
        readonly BaseStats baseStats;
        public StatsMediator Mediator { get; }

        private float StatCalculation(Query q) {
            Mediator.PerformQuery(this, q);
            return q.Value * (q.FinalMultiplier / 100f);
        }

        public int Attack {
            get {
                var q = new Query(StatType.Attack, baseStats.attack);
                return Mathf.FloorToInt(StatCalculation(q));
            }
        }

        public float AttackCooldown {
            get {
                var q = new Query(StatType.AttackCooldown, baseStats.attackCooldown);
                return StatCalculation(q);
            }
        }

        public float AttackDistance {
            get {
                var q = new Query(StatType.AttackDistance, baseStats.attackDistance);
                return StatCalculation(q);
            }
        }

        public float AttackRange {
            get {
                var q = new Query(StatType.AttackRange, baseStats.attackRange);
                return StatCalculation(q);
            }
        }

        public int Defense {
            get {
                var q = new Query(StatType.Defense, baseStats.defense);
                return Mathf.FloorToInt(StatCalculation(q));
            }
        }

        public float Speed {
            get {
                var q = new Query(StatType.Speed, baseStats.speed);
                return Mathf.FloorToInt(StatCalculation(q));
            }
        }

        public int Health {
            get {
                var q = new Query(StatType.Health, baseStats.health);
                return Mathf.FloorToInt(StatCalculation(q));
            }
        }

        public float DashForce {
            get {
                var q = new Query(StatType.DashForce, baseStats.dashForce);
                return StatCalculation(q);
            }
        }

        public float DashDuration {
            get {
                var q = new Query(StatType.DashDuration, baseStats.dashDuration);
                return StatCalculation(q);
            }
        }

        public float DashCooldown {
            get {
                var q = new Query(StatType.DashCooldown, baseStats.dashCooldown);
                return StatCalculation(q);
            }
        }

        public float StunDuration {
            get {
                var q = new Query(StatType.StunDuration, baseStats.stunDuration);
                return StatCalculation(q);
            }
        }

        public Stats(StatsMediator mediator, BaseStats baseStats) {
            this.Mediator = mediator;
            this.baseStats = baseStats;
        }

        public override string ToString() => $"Attack: {Attack}, Defense: {Defense}, Speed: {Speed}, Health: {Health}, DashForce: {DashForce}, DashDuration: {DashDuration}, DashCooldown: {DashCooldown}, StunDuration: {StunDuration}, AttackCooldown: {AttackCooldown}, AttackRange: {AttackRange}, AttackDistance: {AttackDistance}";
    }
}