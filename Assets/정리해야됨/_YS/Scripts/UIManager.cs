using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // UI 관련 요소를 다룰 때 필요
using UnityEngine.SceneManagement;
using TMPro; // 씬 전환을 위해 필요

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    public TextMeshProUGUI clearText; // UI에 표시할 클리어 텍스트
    public TextMeshProUGUI waveText; // UI에 표시할 웨이브 텍스트
    public TextMeshProUGUI coolTimeText; // 쿨타임 텍스트
    public TextMeshProUGUI shootDistText; // 발사 거리 제한 텍스트
    public Button yesButton; // "Yes" 버튼 참조
    public Button StartButton; // "Start" 버튼 참조
    public Button bgmButton; // "BGM" 버튼 참조
    public AudioSource bgmAudioSource; // BGM 오디오 소스
    public Image bgmImage; // 버튼 이미지
    public Sprite[] bgmSprites; // 이미지 배열
    public GameObject player; // Player GameObject
    public GameObject npc; // NPC GameObject
    public float waveTextDuration = 3f; // 텍스트 표시 시간
    public float coolTimeTextDuration = 3f; // 쿨타임 텍스트 표시 시간
    public float shootDistTextDuration = 2f; // 발사 거리 텍스트 표시 시간

    private bool isBgmMuted = false; // BGM 음소거 상태 추적
    private int currentSpriteIndex = 0; // 현재 이미지 인덱스

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        // "Yes" 버튼 클릭 시 QuitApplication 함수 연결
        if (yesButton != null)
        {
            yesButton.onClick.AddListener(QuitApplication);
        }

        // "Start" 버튼 클릭 시 ChangeScene 함수 연결
        if (StartButton != null)
        {
            StartButton.onClick.AddListener(ChangeScene);
        }

        // "BGM" 버튼 클릭 시 ToggleBgmMute 함수 연결
        if (bgmButton != null)
        {
            bgmButton.onClick.AddListener(ToggleBgmMuteAndChangeImage);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    // 애플리케이션 종료 메서드
    public void QuitApplication()
    {
        Application.Quit(); // 애플리케이션 종료
        Debug.Log("애플리케이션 종료 중...");
    }

    // 씬 전환 메서드
    public void ChangeScene()
    {
        SceneManager.LoadScene(1); // Build Index가 1인 씬으로 전환
        Debug.Log("씬 전환 완료");
    }

    // BGM 음소거 및 버튼 이미지 변경
    public void ToggleBgmMuteAndChangeImage()
    {
        if (bgmAudioSource != null)
        {
            // 음소거 상태 토글
            isBgmMuted = !isBgmMuted;
            bgmAudioSource.mute = isBgmMuted;
            Debug.Log(isBgmMuted ? "BGM 음소거됨." : "BGM 음소거 해제됨.");
        }
        else
        {
            Debug.LogWarning("BGM AudioSource가 연결되지 않았습니다.");
        }

        // 이미지 변경 로직
        if (bgmImage != null && bgmSprites.Length > 0)
        {
            // 다음 스프라이트로 변경
            currentSpriteIndex = (currentSpriteIndex + 1) % bgmSprites.Length;
            bgmImage.sprite = bgmSprites[currentSpriteIndex];
            Debug.Log($"이미지가 {currentSpriteIndex}번으로 변경되었습니다.");
        }
        else
        {
            Debug.LogWarning("BGM 이미지 또는 스프라이트 배열이 설정되지 않았습니다.");
        }
    }

    // 플레이어와 NPC 위치 저장
    public void GameSave()
    {
        if (player != null)
        {
            // 플레이어 위치 저장
            PlayerPrefs.SetFloat("PlayerX", player.transform.position.x);
            PlayerPrefs.SetFloat("PlayerY", player.transform.position.y);
        }

        if (npc != null)
        {
            // NPC 위치 저장
            PlayerPrefs.SetFloat("NPCX", npc.transform.position.x);
            PlayerPrefs.SetFloat("NPCY", npc.transform.position.y);
        }

        PlayerPrefs.Save();
        Debug.Log("게임 데이터가 저장되었습니다.");
    }

    // 플레이어와 NPC 위치 불러오기
    public void GameLoad()
    {
        if (!PlayerPrefs.HasKey("PlayerX") || !PlayerPrefs.HasKey("NPCX")) // 저장된 데이터가 없으면 리턴
        {
            Debug.LogWarning("저장된 데이터가 없습니다.");
            return;
        }

        // 플레이어 위치 불러오기
        if (player != null)
        {
            float playerX = PlayerPrefs.GetFloat("PlayerX");
            float playerY = PlayerPrefs.GetFloat("PlayerY");
            player.transform.position = new Vector3(playerX, playerY, player.transform.position.z);
            Debug.Log("플레이어 위치가 로드되었습니다.");
        }

        // NPC 위치 불러오기
        if (npc != null)
        {
            float npcX = PlayerPrefs.GetFloat("NPCX");
            float npcY = PlayerPrefs.GetFloat("NPCY");
            npc.transform.position = new Vector3(npcX, npcY, npc.transform.position.z);
            Debug.Log("NPC 위치가 로드되었습니다.");
        }
    }

    // 웨이브 텍스트 표시
    public void ShowWaveText(string waveMessage)
    {
        if (waveText != null)
        {
            waveText.text = waveMessage;
            waveText.gameObject.SetActive(true);
            StartCoroutine(HideWaveTextAfterDelay());
        }
    }

    private IEnumerator HideWaveTextAfterDelay()
    {
        yield return new WaitForSeconds(waveTextDuration);
        if (waveText != null)
        {
            waveText.gameObject.SetActive(false);
        }
    }

    // 클리어 텍스트 표시
    public void ShowClearText(string clearMessage)
    {
        if (clearText != null)
        {
            clearText.text = clearMessage;
            clearText.gameObject.SetActive(true);
            StartCoroutine(HideClearTextAfterDelay());
        }
    }

    private IEnumerator HideClearTextAfterDelay()
    {
        yield return new WaitForSeconds(3f); // 3초 후 텍스트 숨김
        if (clearText != null)
        {
            clearText.gameObject.SetActive(false);
        }
    }

    // 쿨타임 텍스트 표시
    public void ShowCoolTimeText(string message)
    {
        if (coolTimeText != null)
        {
            coolTimeText.text = message;
            coolTimeText.gameObject.SetActive(true);
            StartCoroutine(HideCoolTimeTextAfterDelay());
        }
    }

    private IEnumerator HideCoolTimeTextAfterDelay()
    {
        yield return new WaitForSeconds(coolTimeTextDuration);
        if (coolTimeText != null)
        {
            coolTimeText.gameObject.SetActive(false);
        }
    }

    // 발사 거리 제한 텍스트 표시
    public void ShowShootDistText(string message)
    {
        if (shootDistText != null)
        {
            shootDistText.text = message;
            shootDistText.gameObject.SetActive(true);
            StartCoroutine(HideShootDistTextAfterDelay());
        }
    }

    private IEnumerator HideShootDistTextAfterDelay()
    {
        yield return new WaitForSeconds(shootDistTextDuration);
        if (shootDistText != null)
        {
            shootDistText.gameObject.SetActive(false);
        }
    }
}