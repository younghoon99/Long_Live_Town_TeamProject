using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // 엔딩 UI 제어용
using UnityEngine.SceneManagement; // 씬 전환용
using TMPro;

public class EndManager : MonoBehaviour
{
#if UNITY_EDITOR
    [ContextMenu("[디버그] 엔딩 바로 보기")]
    public void OnClickDebugEnding()
    {
        ShowEnding();
    }
#endif
    // 게임 내 디버그 버튼용 (UI Button에서 연결)
    public void DebugShowEndingButton()
    {
        ShowEnding();
    }
    public GameObject endingPanel; // 엔딩 패널(미리 비활성화)
    public GameObject blackPanel; // 블랙 패널(미리 비활성화)
    public GameObject endingAnim; // 엔딩 애니메이션(미리 비활성화)
    public GameObject endingCreditObj; // 위로올라가는 텍스트

    public TextMeshProUGUI endingText;        // 엔딩 텍스트(옵션)
    public TextMeshProUGUI endingTextShadow; // 엔딩 텍스트 그림자(옵션)

    public AudioSource endingMusic;  // 엔딩 음악(옵션)

    public AudioSource endingAnimAudio; // 엔딩 애니메이션 사운드(옵션)
    private bool isEndingShown = false;

    // 페이드 인/아웃 효과를 위한 CanvasGroup
    private CanvasGroup endingPanelCanvasGroup;

    // 페이드 인/아웃 속도(초)
    public float fadeDuration = 2.5f; // 패널 페이드 인/아웃 시간 (초, 천천히)조절 가능

    void Update()
    {
        // 10스테이지 클리어 시 엔딩
        if (!isEndingShown && MobManager.instance.currentStage >= 10)
        {
            ShowEnding();
        }
    }

    void ShowEnding()
    {
        Debug.Log("엔딩을 보여줍니다.");
        isEndingShown = true;

        // CanvasGroup 자동 추가(없으면)
        if (endingPanel != null)
        {
            endingPanel.SetActive(true);
            endingPanelCanvasGroup = endingPanel.GetComponent<CanvasGroup>();
            if (endingPanelCanvasGroup == null)
                endingPanelCanvasGroup = endingPanel.AddComponent<CanvasGroup>();
            endingPanelCanvasGroup.alpha = 0f;
            endingPanelCanvasGroup.interactable = true;
            endingPanelCanvasGroup.blocksRaycasts = true;
            StartCoroutine(FadeInPanel());
            blackPanel.SetActive(true);
        }

        string endingMsg = "축하합니다!\n모든 스테이지를 클리어했습니다!\n마을의 평화가 찾아왔습니다.\n당신의 이야기는 계속됩니다.";
        endingText.text = endingMsg;
        endingTextShadow.text = endingMsg;

        // 모든 소리 끄기
        if (UIManager.instance != null && UIManager.instance.bgmAudioSource != null)
            UIManager.instance.bgmAudioSource.mute = true;
        if (GameManager.instance != null && GameManager.instance.sfxAudioSource != null)
            GameManager.instance.sfxAudioSource.enabled = false;
       
        endingMusic.Play(); // 볼륨 페이드아웃을 위해 Play로 재생
        StartCoroutine(FadeOutEndingMusic(60f)); // 1분(60초) 동안 볼륨 서서히 감소

        // 연출 시퀀스 코루틴 실행 (페이드 아웃 → 1초 대기 → 애니메이션 활성화)
        StartCoroutine(ShowEndingSequence());
    }

    // 엔딩 연출 시퀀스: 페이드 아웃 → 1초 대기 → 엔딩 애니메이션 활성화
    private IEnumerator ShowEndingSequence()
    {
        // 1. 패널 페이드 아웃 효과가 끝날 때까지 대기
        yield return StartCoroutine(FadeOutPanel());
        // 2. 1초 대기
        yield return new WaitForSeconds(1f);
        // 3. 엔딩 애니메이션 오브젝트 활성화
        if (endingAnim != null)
            endingAnim.SetActive(true);
        yield return new WaitForSeconds(1.3f);
        // 4. 애니매이션에 맞는 사운드 재생
        endingAnimAudio.PlayOneShot(endingAnimAudio.clip);
        yield return new WaitForSeconds(4f);
        endingCreditObj.SetActive(true);

        // 5. 크레딧 오브젝트를 위로 천천히 올리는 연출
        RectTransform creditRect = endingCreditObj.GetComponent<RectTransform>();
        float creditStartY = -2500f; // 시작 위치 (화면 아래)
        float creditEndY = 2500f;    // 끝 위치 (화면 위)
        float creditMoveDuration = 20f; // 이동 시간(초)
        yield return StartCoroutine(MoveCreditUp(creditRect, creditStartY, creditEndY, creditMoveDuration));
    }

    // 크레딧 올라가는 연출 코루틴
    private IEnumerator MoveCreditUp(RectTransform creditRect, float startY, float endY, float duration)
    {
        Vector2 startPos = creditRect.anchoredPosition;
        startPos.y = startY;
        Vector2 endPos = startPos;
        endPos.y = endY;

        creditRect.anchoredPosition = startPos;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            creditRect.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
            yield return null;
        }
        creditRect.anchoredPosition = endPos;
    }

    // 엔딩 음악 볼륨을 1분(60초) 동안 점점 줄이는 코루틴
    private IEnumerator FadeOutEndingMusic(float duration)
    {
        float startVolume = endingMusic.volume;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            endingMusic.volume = Mathf.Lerp(startVolume, 0f, t);
            yield return null;
        }
        endingMusic.volume = 0f;
    }

    // 엔딩 패널 페이드 인 효과
    private IEnumerator FadeInPanel()
    {
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);
            if (endingPanelCanvasGroup != null)
                endingPanelCanvasGroup.alpha = t;
            yield return null;
        }
        if (endingPanelCanvasGroup != null)
            endingPanelCanvasGroup.alpha = 1f;
    }

    // 엔딩 패널 페이드 아웃 효과(필요시 호출)
    public IEnumerator FadeOutPanel()
    {
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = 1f - Mathf.Clamp01(elapsed / fadeDuration);
            if (endingPanelCanvasGroup != null)
                endingPanelCanvasGroup.alpha = t;
            yield return null;
        }
        if (endingPanelCanvasGroup != null)
            endingPanelCanvasGroup.alpha = 0f;
        if (endingPanel != null)
            endingPanel.SetActive(false);
    }

    // 엔딩에서 메인화면으로 돌아가기 등 추가 기능은 버튼 이벤트로 구현
    public void OnClickReturnToTitle()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("0"); // 타이틀 씬 이름에 맞게 수정
    }
}