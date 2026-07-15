namespace Utils
{
    public class DestroyOnAnimEnd : StateMachineBehaviour
    {
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            Destroy(animator.gameObject);
        }
    }
}
