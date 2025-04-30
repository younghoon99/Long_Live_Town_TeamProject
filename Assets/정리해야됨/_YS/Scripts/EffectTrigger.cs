using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class EffectTrigger : MonoBehaviour
{
    public GameObject effectPrefab; // 이펙트 프리팹
    private GameObject hologramObject; // 홀로그램 오브젝트
    private bool isZKeyPressed = false;
    private float doubleClickTime = 0.3f; // 더블클릭을 감지할 시간 간격
    private float lastClickTime = 0f;

    public int poolSize = 5; // 오브젝트 풀 크기
    private Queue<GameObject> effectPool = new Queue<GameObject>(); // 이펙트 오브젝트 풀

    private int effectUsageCount = 0; // 현재 이펙트 사용 횟수
    private bool isCooldownActive = false; // 쿨타임 활성화 여부

    private UIManager uiManager; // UIManager 참조

    void Start()
    {
        // 오브젝트 풀 초기화
        for (int i = 0; i < poolSize; i++)
        {
            GameObject effect = Instantiate(effectPrefab);
            effect.SetActive(false);
            effectPool.Enqueue(effect);
        }

        uiManager = FindObjectOfType<UIManager>();
        if (uiManager == null)
        {
            Debug.LogError("UIManager를 찾을 수 없습니다!");
        }
    }

    void Update()
    {
        // Z키 입력 처리
        if (Input.GetKeyDown(KeyCode.Z))
        {
            isZKeyPressed = true;

            // 홀로그램 오브젝트 생성
            if (effectPrefab != null && hologramObject == null)
            {
                hologramObject = Instantiate(effectPrefab);
                Collider2D collider = hologramObject.GetComponent<Collider2D>();
                if (collider != null)
                {
                    collider.enabled = false; // 충돌 비활성화
                }
                hologramObject.SetActive(true);

                // 투명도 50%로 설정
                SetHologramTransparency(0.5f);
            }
        }
        else if (Input.GetKeyUp(KeyCode.Z))
        {
            isZKeyPressed = false;

            // 홀로그램 오브젝트 제거
            if (hologramObject != null)
            {
                Destroy(hologramObject);
                hologramObject = null;
            }
        }

        // 홀로그램 오브젝트가 마우스를 따라다니도록 설정
        if (isZKeyPressed && hologramObject != null)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0f; // Z축 고정
            mousePosition.y = -3.25f; // Y축 고정
            hologramObject.transform.position = mousePosition;
        }

        // 마우스 우클릭 더블클릭 처리
        if (Input.GetMouseButtonDown(1))
        {
            float timeSinceLastClick = Time.time - lastClickTime;
            lastClickTime = Time.time;

            if (timeSinceLastClick <= doubleClickTime)
            {
                TriggerEffect();
            }
        }
    }

    void TriggerEffect()
    {
        if (isCooldownActive)
        {
            Debug.Log("쿨타임 중입니다. 이펙트를 사용할 수 없습니다.");
            if (uiManager != null)
            {
                uiManager.ShowCoolTimeText("Cool Time..");
            }
            return;
        }

        if (effectUsageCount >= 5)
        {
            Debug.Log("60초 동안 최대 사용 횟수(5회)를 초과했습니다.");
            StartCoroutine(StartCooldown());
            return;
        }

        if (isZKeyPressed && effectPrefab != null)
        {
            // 이펙트 가져오기
            GameObject effectInstance = GetPooledEffect();
            if (effectInstance == null) return;

            // 마우스 커서 위치에 이펙트 프리팹 생성
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0f; // Z축 고정
            mousePosition.y = -3.25f; // Y축 고정
            effectInstance.transform.position = mousePosition;

            // Z축 90도 회전
            effectInstance.transform.rotation = Quaternion.Euler(0f, 0f, 90f);

            // 이펙트 활성화
            effectInstance.SetActive(true);

            // 5초 후 이펙트 비활성화
            StartCoroutine(DeactivateEffectAfterDelay(effectInstance, 5f));

            // 사용 횟수 증가
            effectUsageCount++;

            // 60초 후 사용 횟수 초기화
            StartCoroutine(ResetUsageCountAfterDelay(60f));
        }
    }

    GameObject GetPooledEffect()
    {
        if (effectPool.Count > 0)
        {
            GameObject effect = effectPool.Dequeue();
            return effect;
        }
        return null; // 풀에 사용 가능한 이펙트가 없으면 null 반환
    }

    IEnumerator DeactivateEffectAfterDelay(GameObject effect, float delay)
    {
        yield return new WaitForSeconds(delay);
        effect.SetActive(false);
        effectPool.Enqueue(effect); // 풀로 반환
    }

    IEnumerator ResetUsageCountAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        effectUsageCount = 0;
        Debug.Log("이펙트 사용 횟수가 초기화되었습니다.");
    }

    IEnumerator StartCooldown()
    {
        isCooldownActive = true;
        Debug.Log("쿨타임 시작: 30초");
        if (uiManager != null)
        {
            uiManager.ShowCoolTimeText("쿨타임 시작: 30초");
        }
        yield return new WaitForSeconds(30f);
        isCooldownActive = false;
        Debug.Log("쿨타임 종료. 이펙트를 다시 사용할 수 있습니다.");
    }

    private void SetHologramTransparency(float alpha)
    {
        if (hologramObject != null)
        {
            SpriteRenderer renderer = hologramObject.GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                Color color = renderer.color;
                color.a = alpha;
                renderer.color = color;
            }
        }
    }
}