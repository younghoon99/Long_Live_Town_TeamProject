using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ItemIven : MonoBehaviour
{
    public GameObject[] itemResPrefabs; // 아이템 프리팹 배열
    public TextMeshProUGUI[] itemResTexts; // 리소스 텍스트 배열
    public ShopManager shopManager; // ShopManager 참조
    public HQManager hqManager; // HQManager 참조

    void OnTriggerEnter2D(Collider2D collision)
    {
        // 플레이어가 Resource 태그를 가진 오브젝트에 닿으면 처리
        if (collision.gameObject.CompareTag("Resource"))
        {
            ResManager res = collision.gameObject.GetComponent<ResManager>();
            if (res != null)
            {
                // 리소스 타입에 따라 처리
                if (res.type == "Wood")
                {
                    UpdateResourceText(0); // Wood는 첫 번째 텍스트
                }
                else if (res.type == "Stone")
                {
                    UpdateResourceText(1); // Stone은 두 번째 텍스트
                }
                else if (res.type == "Gold")
                {
                    UpdateResourceText(2); // Gold는 세 번째 텍스트
                    UpdateShopManagerGold(2); // ShopManager의 goldCount 업데이트
                    UpdateHQShopManagerGold(2); // HQShopManager의 goldCount 업데이트
                }

                // 아이템 비활성화
                collision.gameObject.SetActive(false);
            }
        }
    }

    private void UpdateResourceText(int index)
    {
        if (index >= 0 && index < itemResTexts.Length)
        {
            // 텍스트 숫자 증가
            int currentValue = int.Parse(itemResTexts[index].text);
            currentValue++;
            itemResTexts[index].text = currentValue.ToString();

           
        }
    }

    private void UpdateShopManagerGold(int index)
    {
        if (shopManager != null && index == 2) // Gold는 세 번째 텍스트
        {
            int goldValue = int.Parse(itemResTexts[index].text);
          
        }
    }

    private void UpdateHQShopManagerGold(int index)
    {
        if (hqManager != null && index == 2) // Gold는 세 번째 텍스트
        {
            int goldValue = int.Parse(itemResTexts[index].text);
            hqManager.SetGoldCount(goldValue); // ShopManager의 goldCount 업데이트
        }
    }
}
