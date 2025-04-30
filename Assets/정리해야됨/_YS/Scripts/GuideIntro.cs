using System.Collections;
using UnityEngine;

public class GuideIntro : MonoBehaviour
{
    [Header("Guide UI 설정")]
    [SerializeField] private GameObject guideInfoUI; // GuideInfo UI 오브젝트
    [SerializeField] private float blinkDuration = 5f; // 깜빡이는 지속 시간
    [SerializeField] private float blinkInterval = 0.5f; // 깜빡이는 간격

    private void Start()
    {
        if (guideInfoUI != null)
        {
            StartCoroutine(BlinkAndDisappear());
            
        }
       
    }

    private IEnumerator BlinkAndDisappear()
    {
        float elapsedTime = 0f;

        while (elapsedTime < blinkDuration)
        {
            // UI 활성화/비활성화 반복
            guideInfoUI.SetActive(true);
            yield return new WaitForSeconds(blinkInterval);
            elapsedTime += blinkInterval;
        }

        // 깜빡임 종료 후 UI 비활성화
        guideInfoUI.SetActive(false);
    }
}
