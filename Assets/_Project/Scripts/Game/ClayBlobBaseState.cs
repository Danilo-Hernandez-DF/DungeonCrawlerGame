using UnityEngine;

namespace Game
{
    public class ClayBlobBaseState : EnemyBaseState<ClayBlobEnemy> {
        //Animator Hashes
        public static int ClayBlobChaseHash = Animator.StringToHash("ClayBlobChase");
        public static int ClayBlobWanderHash = Animator.StringToHash("ClayBlobWander");
        
        public ClayBlobBaseState(ClayBlobEnemy enemy, int animHash = 0) : base(enemy, animHash) { }
    }
}