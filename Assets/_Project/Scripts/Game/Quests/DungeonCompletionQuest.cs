namespace Game
{
    [CreateAssetMenu(fileName = "New Quest", menuName = "Quest/DungeonCompletion")]
    public class DungeonCompletionQuest : Quest {  
        public override void OnCompletion(NPC questGiver = null) {
            DungeonController.Instance.CompleteRequirements();
        }
    }
}