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

        protected override void OnUse(Inventory source, int indexSource, Entity entitySource, int addData = 0)
        {
            if (!entitySource) return;
            Transform atkOrigin = entitySource.transform;

            var aimDirection = entitySource.FacingDirection;
            float actionAngle = Vector2.SignedAngle(atkOrigin.right, aimDirection);

            var sourceItem = source.items[indexSource];

            var effectModifiers = AdditionalDataManager.Instance.GetAdditionalsFromItem(sourceItem);
            var statusEffectData = AdditionalDataManager.Instance.GetAdditionalStatusFromItem(sourceItem);

            //Debug.Log($"Swing Attack {addData}");
            GameObject weapon = Instantiate(weaponPrefab, atkOrigin.position, Quaternion.Euler(0f, 0f, actionAngle));
            weapon.GetComponentInChildren<SpriteRenderer>().sprite = sourceItem.data.DisplaySprite;
            weapon.transform.parent = atkOrigin;
            Animator animator = weapon.GetComponent<Animator>();
            GameObject weaponChild = weapon.GetComponentInChildren<SpriteRenderer>().gameObject;

            GameObject damageArea = Instantiate(damageAreaPrefab, (Vector2)weaponChild.transform.position + (aimDirection * attackOffset),
                Quaternion.Euler(0f, 0f, actionAngle));
            damageArea.GetComponent<BoxCollider2D>().size = addData == 2 ? new Vector2(attackArea.y, attackArea.x) : attackArea;
            damageArea.transform.parent = weaponChild.transform;
            damageArea.GetComponent<DestroyAfter>().time = 0.12f;

            DamageArea damageAreaComp = damageArea.GetComponent<DamageArea>();
            damageAreaComp.damage = entitySource.Stats.Attack;
            damageAreaComp.origin = atkOrigin;

            damageAreaComp.hitEffects = effectModifiers;
            damageAreaComp.hitStatus = statusEffectData;

            switch (addData)
            {
                case 0:
                    animator.SetTrigger("Swing");
                    break;
                case 1:
                    animator.SetTrigger("Reverse");
                    break;
                case 2:
                    animator.SetTrigger("Pierce");
                    break;
                default:
                    break;
            }
        }
    }
}