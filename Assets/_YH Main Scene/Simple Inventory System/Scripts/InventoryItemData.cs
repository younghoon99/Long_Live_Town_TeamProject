using UnityEngine;

namespace RedstoneinventeGameStudio
{
    [CreateAssetMenu(fileName = "Inventory Item", menuName = "Inventory Item")]
    // 아이템 데이터 클래스: 각 아이템의 정보를 저장하는 데이터 클래스
    [System.Serializable]
    public class InventoryItemData : ScriptableObject
    {
        // 아이템의 기본 정보
        public string itemName;          // 아이템 이름
        public Sprite itemIcon;         // 아이템 아이콘
        public int itemAmount;          // 아이템 수량

        // 아이템의 설명 정보
        public string itemTooltip;      // 툴팁 텍스트
        public string itemDescription;  // 상세 설명

        // 아이템의 특성
        public bool isStackable;        // 스택 가능 여부
        public int maxStack;            // 최대 스택 수량
    }
}