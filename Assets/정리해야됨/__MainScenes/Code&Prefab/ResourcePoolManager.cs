using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
public class ResourcePoolManager : MonoBehaviour
{
    public GameObject resourcePrefab; // 리소스 프리팹
    public Transform BasePosition;
    public Vector2 BacePos; // 베이스의 위치
    public Sprite[] resourceSprites; // 리소스에 사용할 스프라이트 배열
    private Queue<GameObject> resourcePool = new Queue<GameObject>(); // 리소스 풀 (Queue)

    // 리소스 요청을 저장하는 대기열 (Deferred Queue)
    private Queue<GameObject> activeResources = new Queue<GameObject>(); // 활성화된 리소스 대기열

    void Start()
    {
        BacePos = new Vector2(BasePosition.position.x, BasePosition.position.y); // 베이스의 위치를 Vector2로 변환
        // 리소스 풀 초기화: 비활성화된 리소스 오브젝트를 생성하여 풀에 추가
        for (int i = 0; i < 30; i++)
        {
            GameObject resource = Instantiate(resourcePrefab, transform);
            resource.SetActive(false); // 생성된 리소스를 비활성화 상태로 설정
            resourcePool.Enqueue(resource); // 풀에 추가
        }

        // 대기열을 처리하는 코루틴 시작
        StartCoroutine(ProcessActiveResources());
    }

    /// <summary>
    /// 리소스를 활성화하는 메서드
    /// </summary>
    /// <param name="objectName">리소스의 이름</param>
    /// <param name="spriteIndex">리소스에 사용할 스프라이트 인덱스</param>
    /// <param name="position">리소스를 배치할 위치</param>
    /// <returns>활성화된 리소스 오브젝트</returns>
    public GameObject ActivateResource(string objectName, int spriteIndex, Vector3 position)
    {
        // 리소스 풀에서 비활성화된 리소스를 찾음
        if (resourcePool.Count > 0)
        {
            GameObject resource = resourcePool.Dequeue(); // 큐에서 리소스를 가져옴

            // 리소스 설정
            resource.name = objectName;

            // 스프라이트 설정
            SpriteRenderer spriteRenderer = resource.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = resourceSprites[spriteIndex];
            }
            Resource resourceComponent = resource.GetComponent<Resource>();
            if (resourceComponent != null)
            {
                resourceComponent.SetResourceValue(spriteIndex); // 기본값 설정
            }

            // 리소스 위치 설정 및 활성화
            resource.transform.position = position;
            resource.SetActive(true);

            // 활성화된 리소스를 대기열에 추가
            activeResources.Enqueue(resource);

            return resource; // 활성화된 리소스를 반환
        }

        Debug.LogWarning("풀에 비활성화된 리소스가 없습니다.");
        return null;
    }

    /// <summary>
    /// 활성화된 리소스를 3초마다 일괄적으로 처리하는 코루틴
    /// </summary>
    private IEnumerator ProcessActiveResources()
    {
        while (true)
        {
            var delay = 0.1f;
            // 활성화된 리소스를 모두 처리
            int resourceCount = activeResources.Count; // 현재 활성화된 리소스 개수 저장
            GameObject[] resources = new GameObject[resourceCount]; // 활성화된 리소스 배열로 변환
            for (int i = 0; i < resourceCount; i++)
            {
                // 대기열에서 리소스를 가져옴
                GameObject resource = activeResources.Dequeue();
                resources[i] = resource; // 배열에 저장
                resource.transform.DOScale(1f, 0.3f).SetDelay(delay).SetEase(Ease.OutBack);
                resource.transform.DOMove(BacePos, 0.8f).SetDelay(delay + 0.5f).SetEase(Ease.InBack);
                resource.transform.DORotate(Vector3.zero, 0.5f).SetDelay(delay + 0.5f).SetEase(Ease.Flash);
                resource.transform.DOScale(0f, 0.3f).SetDelay(delay + 1.5f).SetEase(Ease.OutBack);
                if (resource.name == "Wood")
                {
                    GameManager.instance.AddWood(1); // 나무 개수 증가
                }
                else if (resource.name == "Stone")
                {
                    GameManager.instance.AddStone(1); // 돌 개수 증가
                }
                else if (resource.name == "Gold")
                {
                    GameManager.instance.AddGold(2); // 금 개수 증가
                }
                // 작업 수행
                PerformAction(resource);
                 // 작업 수행 후 대기
                // 리소스를 비활성화하고 풀로 반환
                

                Debug.Log($"리소스 {resource.name} 작업 완료 후 비활성화되었습니다.");                
            }
            yield return new WaitForSeconds(delay * 4 + 2.0f);
            for(int i = 0; i < resourceCount; i++)
            {
                GameObject resource = resources[i];
                
                resource.SetActive(false);
                resourcePool.Enqueue(resource);
            }

            Debug.Log("대기열에 있는 모든 활성화된 리소스가 처리되었습니다.");
        }
    }

    /// <summary>
    /// 리소스와 관련된 작업을 수행하는 메서드
    /// </summary>
    /// <param name="resource">작업을 수행할 리소스</param>
    private void PerformAction(GameObject resource)
    {
        // 리소스와 관련된 작업을 여기에 구현
        Debug.Log($"리소스 {resource.name} 작업 수행 중...");
        // 예: 리소스의 위치를 변경하거나, 데이터를 업데이트하는 작업
    }
}