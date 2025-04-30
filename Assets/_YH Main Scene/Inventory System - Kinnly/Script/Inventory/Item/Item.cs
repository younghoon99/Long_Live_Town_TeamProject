using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Kinnly
{
    [CreateAssetMenu(fileName = "item", menuName = "ScriptableObjects/item")]
    public class Item : ScriptableObject
    {
        [Header("기본 정보")]
        public string itemName; // 아이템 이름
        public string itemDescription; // 아이템 설명
        public Sprite image; // 아이템 아이콘
        public float Damage; // 무기별 데미지
        
        
        [Header("아이템 타입")]
        public bool isAxe;
        public bool isPickaxe;
        public bool isSword;
        public bool isHammer;
        public bool isBow;

        // NPC 인벤토리에 들어갈 수 있는지 여부
        public bool CanAddToNpcInventory()
        {
            // NPC 인벤토리에 들어갈 수 있는 아이템 타입 (곡괭이, 검, 도끼만 허용)
            return isPickaxe || isSword || isBow || isAxe;
        }
    }
}