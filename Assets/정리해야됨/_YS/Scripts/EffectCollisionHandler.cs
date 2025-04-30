using UnityEngine;

namespace YS.Effects
{
    public class EffectCollisionHandler : MonoBehaviour
    {
        [Header("공격력 설정")]
        private float Playerdamage; // 데미지 값 (인스펙터에서 조절 가능)
        private float Npcdamage; // 데미지 값 (인스펙터에서 조절 가능)
        public bool isNpcArrow = false; // true면 NPC, false면 Player
public int npcAttackDamage = 0; // NPC별 공격력 (ArrowShooter_Npc에서 할당)

        private void Update()
        {
            // ArrowShooter.isBowEquipped가 true면 플레이어의 활 데미지를 참조
            if (ArrowShooter.isBowEquipped)
            {
                Player player = FindObjectOfType<Player>();
                if (player != null && player.equippedItem != null)
                {
                    // 활 타입인지 체크 (ItemType.Bow 등으로 구분)
                    // 실제 프로젝트 구조에 맞게 조건 수정 필요
                    if (player.equippedItem.isBow)
                        Playerdamage = player.equippedItem.Damage;
                }
            }
            if(ArrowShooter_Npc.isBowEquipped)
            {
                Npcdamage = ArrowShooter_Npc.arrowDamage;
                Debug.Log($"NPC의 데미지는 {ArrowShooter_Npc.arrowDamage}입니다.");
            }
               
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.GetComponent<Enemy>() != null)
            {
                Debug.Log("몬스터와 화살이 접촉함");
                EnemyHealth enemyHealth = collision.GetComponent<EnemyHealth>();
                if (enemyHealth != null)
                {
                    // isNpcArrow가 true면 npcAttackDamage 사용, 아니면 Playerdamage 사용
                    if (isNpcArrow)
                        enemyHealth.TakeDamage(npcAttackDamage, (collision.transform.position - transform.position).normalized); // NPC별 공격력
                    else
                        enemyHealth.TakeDamage(Playerdamage, (collision.transform.position - transform.position).normalized); // 플레이어 공격력
                }
            }
        }
    }
}