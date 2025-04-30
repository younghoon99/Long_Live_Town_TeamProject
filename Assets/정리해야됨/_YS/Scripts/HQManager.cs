using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HQManager : MonoBehaviour
{
    [Header("상호작용 설정")]
    [SerializeField] private float interactionRange = 2.0f;
    [SerializeField] private KeyCode interactionKey = KeyCode.Q;

    [Header("UI 설정")]
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private GameObject promptUI;
    [SerializeField] private Vector3 promptOffset = new Vector3(0, 2.0f, 0);
    [SerializeField] private Vector3 infoOffset = new Vector3(0, 2.5f, 0);

    [Header("리소스 설정")]
    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private Transform playerTransform;

    [Header("기타 설정")]
    [SerializeField] private NpcMaker npcMaker;

    private GameObject currentHQ;
    private bool isInteracting = false;

    private void Start()
    {
        npcMaker = GetComponent<NpcMaker>();
        playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (playerTransform == null)
            Debug.LogError("플레이어를 찾을 수 없습니다. 'Player' 태그가 설정되었는지 확인하세요.");

        shopPanel?.SetActive(false);
        promptUI?.SetActive(false);

        UpdateGoldText();
    }

    private void Update()
    {
        if (playerTransform == null) return;

        GameObject nearestHQ = FindNearestHQ();

        if (nearestHQ != null)
        {
            if (promptUI && !isInteracting)
            {
                promptUI.SetActive(true);
                promptUI.transform.position = Camera.main.WorldToScreenPoint(nearestHQ.transform.position + promptOffset);
            }

            if (Input.GetKeyDown(interactionKey))
            {
                if (!isInteracting)
                    StartInteraction(nearestHQ);
                else
                    EndInteraction();
            }
        }
        else
        {
            promptUI?.SetActive(false);
            if (isInteracting) EndInteraction();
        }

        UpdateUIPositions();
    }

    private GameObject FindNearestHQ()
    {
        GameObject[] allHQs = GameObject.FindGameObjectsWithTag("Home");
        GameObject closest = null;
        float minDistance = interactionRange;

        foreach (GameObject hq in allHQs)
        {
            float dist = Vector3.Distance(playerTransform.position, hq.transform.position);
            if (dist < minDistance)
            {
                closest = hq;
                minDistance = dist;
            }
        }

        return closest;
    }

    private void StartInteraction(GameObject hq)
    {
        currentHQ = hq;
        isInteracting = true;

        // 플레이어 공격 비활성화
        Player player = playerTransform != null ? playerTransform.GetComponent<Player>() : null;
        if (player != null)
            player.isAttack = false;

        shopPanel?.SetActive(true);
        promptUI?.SetActive(false);
    }

    private void EndInteraction()
    {
        isInteracting = false;

        // 플레이어 공격 다시 활성화
        Player player = playerTransform != null ? playerTransform.GetComponent<Player>() : null;
        if (player != null)
            player.isAttack = true;

        shopPanel?.SetActive(false);
        promptUI?.SetActive(false);

        currentHQ = null;
    }

    public void HandleBuy()
    {
        int price = 5;

        if (GetGoldCount() < price)
        {
            Debug.Log("골드가 부족합니다!");
            return;
        }

        npcMaker.SpawnRandomNpc();
        RemoveGold(price);
        Debug.Log("랜덤 NPC를 생성했습니다.");
    }

    public void SetGoldCount(int gold)
    {
        GameManager.instance.AddGold(gold - GetGoldCount());
        UpdateGoldText();
    }

    private void UpdateGoldText()
    {
        if (goldText != null && GameManager.instance != null)
        {
            goldText.text = GameManager.instance.goldCount.ToString();
        }
    }

    public bool IsInteracting()
    {
        return isInteracting;
    }

    private void UpdateUIPositions()
    {
        if (currentHQ != null)
        {
            if (promptUI && promptUI.activeSelf)
            {
                promptUI.transform.position = Camera.main.WorldToScreenPoint(
                    currentHQ.transform.position + promptOffset);
            }

            if (shopPanel && shopPanel.activeSelf)
            {
                shopPanel.transform.position = Camera.main.WorldToScreenPoint(
                    currentHQ.transform.position + infoOffset);
            }
        }
    }

    public void AddGold(int amount)
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.AddGold(amount);
            UpdateGoldText();
        }
    }

    public void RemoveGold(int amount)
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.AddGold(-amount);
            UpdateGoldText();
        }
    }

    public int GetGoldCount()
    {
        return GameManager.instance != null ? GameManager.instance.goldCount : 0;
    }
}
