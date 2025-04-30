using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using RedstoneinventeGameStudio;

public class ShopManager : MonoBehaviour
{
    [Header("아이템 데이터 (ScriptableObject)")]
    [SerializeField] private Kinnly.Item swordItemData;
    [SerializeField] private Kinnly.Item axeItemData;
    [SerializeField] private Kinnly.Item pickaxeItemData;
    [SerializeField] private Kinnly.Item hammerItemData;
    [SerializeField] private Kinnly.Item bowItemData;

  [Header("상호작용 설정")]
    [SerializeField] private float interactionRange = 2.0f;
    [SerializeField] private KeyCode interactionKey = KeyCode.S;

    [Header("UI 설정")]
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private GameObject promptUI;
    [SerializeField] private GameObject tooltipPanel;
    [SerializeField] private TextMeshProUGUI tooltipText;

    [Header("리소스 설정")]
    [SerializeField] private TextMeshProUGUI goldText;

    [SerializeField] private Transform playerTransform;
    [SerializeField] private GameObject currentShop;
    private bool isInteracting = false;

    [Header("골드 설정")]
    [SerializeField] private List<Inventory> inventorySlots;

    [SerializeField] private Vector3 promptOffset = new Vector3(0, 2.0f, 0);
    [SerializeField] private Vector3 infoOffset = new Vector3(0, 2.5f, 0);

    private GameObject[] allShops; // 캐시된 상점들
    private Vector3 lastShopPosition; // UI 위치 갱신 최적화용

    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (playerTransform == null)
        {
            Debug.LogError("플레이어를 찾을 수 없습니다. 'Player' 태그가 설정되었는지 확인하세요.");
        }

        if (shopPanel) shopPanel.SetActive(false);
        if (promptUI) promptUI.SetActive(false);

        allShops = GameObject.FindGameObjectsWithTag("Shop"); // 상점 목록 캐싱

        UpdateGoldText();
    }

    private void Update()
    {
        if (playerTransform == null) return;

        GameObject nearestShop = FindNearestShop();

        if (nearestShop != null)
        {
            if (promptUI && !isInteracting)
            {
                promptUI.SetActive(true);
                UpdatePromptPosition(nearestShop); // 위치 변경 최적화 포함
            }

            if (Input.GetKeyDown(interactionKey))
            {
                if (!isInteracting)
                {
                    StartInteraction(nearestShop);
                }
                else
                {
                    EndInteraction();
                }
            }
        }
        else
        {
            if (promptUI) promptUI.SetActive(false);
            if (isInteracting) EndInteraction();
        }

        UpdateShopPanelPosition(); // UI 위치 변경 최적화 포함
    }

    private GameObject FindNearestShop()
    {
        GameObject closestShop = null;
        float closestDistance = interactionRange;

        foreach (GameObject shop in allShops)
        {
            float distance = Vector3.Distance(playerTransform.position, shop.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestShop = shop;
            }
        }

        return closestShop;
    }

    private void StartInteraction(GameObject shop)
    {
        currentShop = shop;
        isInteracting = true;

        // 플레이어 공격 비활성화
        Player player = playerTransform != null ? playerTransform.GetComponent<Player>() : null;
        if (player != null)
            player.isAttack = false;

        if (shopPanel) shopPanel.SetActive(true);
        if (promptUI) promptUI.SetActive(false);

        lastShopPosition = shop.transform.position;
        UpdateShopPanelPosition(); // 최초 오픈 시 위치 반영
    }

    private void EndInteraction()
    {
        isInteracting = false;
        tooltipPanel.SetActive(false);

        // 플레이어 공격 다시 활성화
        Player player = playerTransform != null ? playerTransform.GetComponent<Player>() : null;
        if (player != null)
            player.isAttack = true;

        if (shopPanel) shopPanel.SetActive(false);
        if (promptUI) promptUI.SetActive(false);

        currentShop = null;
    }

    public void HandlePurchase(string itemType)
    {
        Kinnly.Item selectedItem = null;
        int price = 0;

        if (itemType == "Sword" && GameManager.instance.goldCount >= 10)
        {
            selectedItem = swordItemData;
            price = 10;
        }
        else if (itemType == "Axe" && GameManager.instance.goldCount >= 5)
        {
            selectedItem = axeItemData;
            price = 5;
        }
        else if (itemType == "Pickaxe" && GameManager.instance.goldCount >= 5)
        {
            selectedItem = pickaxeItemData;
            price = 5;
        }
        else if (itemType == "Hammer" && GameManager.instance.goldCount >= 5)
        {
            selectedItem = hammerItemData;
            price = 5;
        }
        else if (itemType == "Bow" && GameManager.instance.goldCount >= 15)
        {
            selectedItem = bowItemData;
            price = 15;
        }

        if (selectedItem != null)
        {
            bool added = TryAddItemToInventory(selectedItem);

            if (added)
            {
                GameManager.instance.AddGold(-price);
                Debug.Log($"{itemType} 아이템을 인벤토리에 추가했습니다!");
            }
            else
            {
                Debug.Log("인벤토리에 빈 슬롯이 없습니다!");
            }
        }
        else
        {
            Debug.Log("골드가 부족하거나 아이템 정보가 없습니다!");
        }

        UpdateGoldText();
    }

    private bool TryAddItemToInventory(Kinnly.Item itemToAdd)
    {
        foreach (Inventory slot in inventorySlots)
        {
            if (!slot.isOccupied)
            {
                slot.SetItem(itemToAdd);
                // 아이템이 추가된 슬롯을 자동으로 선택
                InventoryUIManager.Instance.SelectSlot(inventorySlots.IndexOf(slot));
                return true;
            }
        }

        return false;
    }

    private void UpdateGoldText()
    {
        if (goldText != null && GameManager.instance != null)
        {
            goldText.text = GameManager.instance.goldCount.ToString();
        }
    }

    private void UpdatePromptPosition(GameObject shop)
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(shop.transform.position + promptOffset);
        if (Vector3.Distance(promptUI.transform.position, screenPos) > 1f)
        {
            promptUI.transform.position = screenPos;
        }
    }

    private void UpdateShopPanelPosition()
    {
        if (shopPanel && shopPanel.activeSelf && currentShop != null)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(currentShop.transform.position + infoOffset);
            if (Vector3.Distance(shopPanel.transform.position, screenPos) > 1f)
            {
                shopPanel.transform.position = screenPos;
            }
        }
    }

    public bool IsInteracting()
    {
        return isInteracting;
    }



    //Hover기능으로 툴팁표시
    // 예시: 마우스 Hover 시 호출되는 함수
    public void OnButtonTransformHover(RectTransform buttonRect)
    {
        // 버튼의 World Position 얻기 (왼쪽으로 100px 이동 예시)
        Vector3[] worldCorners = new Vector3[4];
        buttonRect.GetWorldCorners(worldCorners);
        Vector3 leftPos = worldCorners[1] + new Vector3(-150, 0, 0); // 왼쪽 아래 모서리에서 왼쪽으로 100px

        // 월드좌표 → 캔버스 로컬좌표 변환
        RectTransform canvasRect = tooltipPanel.GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        Vector2 anchoredPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            Camera.main.WorldToScreenPoint(leftPos),
            Camera.main,
            out anchoredPos
        );
        tooltipPanel.GetComponent<RectTransform>().anchoredPosition = anchoredPos;
    }
    public void OnItemButtonHover(Kinnly.Item itemData)
    {
        tooltipPanel.SetActive(true);
        tooltipText.text = itemData.itemDescription;
    }

    // 예시: 마우스 Hover Exit 시 호출되는 함수
    public void OnItemButtonExit()
    {
        tooltipPanel.SetActive(false);
    }

}
