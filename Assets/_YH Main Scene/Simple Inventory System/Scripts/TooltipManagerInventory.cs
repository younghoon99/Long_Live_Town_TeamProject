using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace RedstoneinventeGameStudio
{
    // 툴팁 관리자 클래스: 아이템에 마우스를 올렸을 때 툴팁을 표시하는 기능을 담당
    public class TooltipManagerInventory : MonoBehaviour
    {
        // 싱글톤 인스턴스: 게임 전체에서 툴팁을 관리하기 위해 사용
        public static TooltipManagerInventory instance;

        // 툴팁 UI 요소들
        [SerializeField] TMP_Text tooltip;    // 툴팁 텍스트를 표시할 TMP_Text 컴포넌트
        [SerializeField] TMP_Text desc;      // 상세 설명을 표시할 TMP_Text 컴포넌트

        // 초기화 메서드: MonoBehaviour의 Awake 메서드
        public void Awake()
        {
            // 싱글톤 패턴으로 인스턴스 설정
            instance = this;
            // 시작할 때는 툴팁이 비활성화 상태
            gameObject.SetActive(false);
        }

        // 툴팁 표시 메서드
        public static void SetTooltip(InventoryItemData inventoryItemData)
        {
            // 툴팁 활성화
            instance.gameObject.SetActive(true);
            // 툴팁 텍스트 설정
            instance.tooltip.text = inventoryItemData.itemTooltip;
            // 상세 설명 텍스트 설정
            instance.desc.text = inventoryItemData.itemDescription;
        }

        // 툴팁 비활성화 메서드
        public static void UnSetToolTip()
        {
            if (instance != null)
            {
                instance.gameObject.SetActive(false);
            }
        }
    }
}