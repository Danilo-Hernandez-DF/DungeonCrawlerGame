using System.Net;
using Game;
using UnityEngine;

namespace UtilsModule {
    [CreateAssetMenu(menuName = "ItemBehaviour/SwingAttackBehaviour")]
    public class SwingAttackBehaviour : OnUseBehaviour {
        [SerializeField] GameObject weaponPrefab;
        [SerializeField] GameObject damageAreaPrefab;
        [SerializeField] Vector2 attackArea;
        [SerializeField] float attackOffset;
        //[SerializeField] ModifierEffect hitEffect;
        //[SerializeField] StatusEffectData statusEffectData;
        //bool ApplyEffect => hitEffect != null;
        //bool ApplyStatus => statusEffectData != null;

        protected override void OnUse(Inventory source, int indexSource, Entity entitySource, int addData = 0) {
            if(!entitySource) return;
            Transform atkOrigin = entitySource.transform;

            var aimDirection = entitySource.FacingDirection;
            float actionAngle = Vector2.SignedAngle(atkOrigin.right, aimDirection);

            var sourceItem = source.items[indexSource];

            var effectModifiers = AdditionalDataManager.Instance.GetAdditionalsFromItem(sourceItem);
            var statusEffectData = AdditionalDataManager.Instance.GetStatusFromItem(sourceItem);

            //Debug.Log($"Swing Attack {addData}");
            GameObject weapon = Instantiate(weaponPrefab, atkOrigin.position, Quaternion.Euler(0f, 0f, actionAngle));
            weapon.GetComponentInChildren<SpriteRenderer>().sprite = sourceItem.data.DisplaySprite;
            weapon.transform.parent = atkOrigin;

            GameObject damageArea = Instantiate(damageAreaPrefab, (Vector2)atkOrigin.position + (aimDirection * attackOffset),
                Quaternion.Euler(0f, 0f, actionAngle));
            damageArea.GetComponent<BoxCollider2D>().size = addData == 2? new Vector2(attackArea.y, attackArea.x) : attackArea;
            damageArea.transform.parent = atkOrigin;
                
            DamageArea damageAreaComp =damageArea.GetComponent<DamageArea>();
            damageAreaComp.damage = entitySource.Stats.Attack;
            damageAreaComp.origin = atkOrigin;
                
            damageAreaComp.hitEffects = effectModifiers;
            damageAreaComp.hitStatus = statusEffectData;

            switch(addData) {
                case 0:
                    weapon.GetComponent<Animator>().SetTrigger("Swing");
                    break;
                case 1:
                    weapon.GetComponent<Animator>().SetTrigger("Reverse");
                    break;
                case 2:
                    weapon.GetComponent<Animator>().SetTrigger("Pierce");
                    break;
                default:
                    break;
            }
        }
    }
}