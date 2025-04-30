// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using Kinnly; // Item과 PlayerInventory 클래스를 사용하기 위한 네임스페이스 추가

// // 아이템 드롭 시스템 클래스: 땅에 떨어진 아이템을 플레이어가 줍는 기능을 달성
// public class ItemDrop : MonoBehaviour
// {
//     // 코어 컴포넌트들
//     [Header("Core")]
//     [SerializeField] private SpriteRenderer spriteRenderer; // 아이템 이미지를 표시할 SpriteRenderer
//     [SerializeField] private Item item;                    // 드롭된 아이템 데이터
//     [SerializeField] private int amount;                    // 아이템 수량

//     // 플레이어 관련 참조
//     [SerializeField] private GameObject playerObject;      // 플레이어 오브젝트 참조
//     // private PlayerInventory playerInventory;              // 플레이어 인벤토리 컴포넌트

//     bool isDelay;
//     bool isNear;
//     bool isSlotAvailable;

//     // private PlayerInventory playerInventory; // 플레이어 인벤토리 직접 참조
//     float speed;
//     float delay;

//     // Start is called before the first frame update
//     void Start()
//     {
//         // 플레이어를 태그로 찾기
//         if (playerObject == null)
//         {
//             playerObject = GameObject.FindGameObjectWithTag("Player");
           

//             if (playerObject == null)
//             {
//                 playerObject = GameObject.Find("Player");
//             }
//         }

//         // 플레이어 인벤토리 컴포넌트 찾기
//         if (playerObject != null)
//         {
//             // playerInventory = playerObject.GetComponent<PlayerInventory>();
//         }

//         // 아이템 설정
//         if (item == null)
//         {
//             return;
//         }

//         // 초기값 설정
//         speed = 10f;
//         delay = 0.25f;
//         isDelay = true;
//         spriteRenderer.sprite = item.image;
//     }

//     // Update is called once per frame
//     void Update()
//     {
//         // 딜레이 상태 처리
//         if (isDelay)
//         {
//             TimeCountDown();
//         }
//         else
//         {
//             // 플레이어 인벤토리가 없을 경우 다시 찾기 시도
//             if (playerInventory == null)
//             {
//                 // 플레이어를 태그로 찾기
//                 if (playerObject == null)
//                 {
//                     playerObject = GameObject.FindGameObjectWithTag("Player");

//                     if (playerObject == null)
//                     {
//                         playerObject = GameObject.Find("Player");
//                     }
//                 }

//                 if (playerObject != null)
//                 {
//                     // 플레이어 인벤토리 컴포넌트 직접 찾기
//                     playerInventory = playerObject.GetComponent<PlayerInventory>();
//                 }

//                 if (playerInventory == null) return; // 플레이어 인벤토리를 찾지 못하면 건너뛰기
//             }

//             // 플레이어와의 거리 체크
//             CheckDistance();
//             if (isNear)
//             {
                
//                 CheckSlotAvailability();
//                 if (isSlotAvailable)
//                 {
//                     // 플레이어에게 이동
//                     MovingtoTarget();
//                     // 아이템 추가 처리
//                     AddingItem();
//                 }
//                 else
//                 {
                    
//                 }
//             }
//             else
//             {
               
//             }
//         }
//     }

//     // 아이템 설정 메서드
//     public void SetItem(Item item, int amount)
//     {
//         this.item = item;
//         this.amount = amount;
//         this.gameObject.name = item.name;
//         if (spriteRenderer != null)
//         {
//             spriteRenderer.sprite = item.image;
//         }
//     }

//     // 딜레이 카운트다운 메서드
//     private void TimeCountDown()
//     {
//         delay -= 1f * Time.deltaTime;
//         if (delay <= 0f)
//         {
//             isDelay = false;
//         }
//     }

//     // 플레이어와의 거리 체크 메서드
//     private void CheckDistance()
//     {
//         if (playerObject == null)
//         {
           
//             return;
//         }

//         float distance = Vector2.Distance(this.transform.position, playerObject.transform.position);
//         if (distance <= 2f)
//         {
//             isNear = true;
//         }
//         else
//         {
//             isNear = false;
//         }
//     }

//     // 인벤토리 슬롯 체크 메서드
//     private void CheckSlotAvailability()
//     {
//         if (playerInventory == null)
//         {
//             return;
//         }

//         // 인벤토리 슬롯 확인
//         if (playerInventory.IsSlotAvailable(item, amount))
//         {
//             isSlotAvailable = true;
//         }
//         else
//         {
//             isDelay = false;
//             isNear = false;
//             delay = 1f;
//         }
//     }

//     // 플레이어에게 이동 메서드
//     private void MovingtoTarget()
// {
//     if (playerObject == null)
//     {
//         return;
//     }

//     Vector3 direction = playerObject.transform.position - transform.position;
//     direction.Normalize();
//         transform.Translate(direction * speed * Time.deltaTime);
//         speed += 20f * Time.deltaTime;
      
//     }

//     // 아이템 추가 메서드
//     private void AddingItem()
//     {
        
//         if (playerObject == null || playerInventory == null)
//         {
         
//             return;
//         }

//         float distance = Vector2.Distance(this.transform.position, playerObject.transform.position);

//         // 플레이어 근처에 도달했을 때
//         if (distance <= 0.5f)
//         {
//             // 인벤토리에 아이템 추가
//             playerInventory.AddItem(item, amount);
//             // 드롭 오브젝트 제거
//             Destroy(gameObject);
//         }
//         else
//         {
        
//         }
//     }
// }