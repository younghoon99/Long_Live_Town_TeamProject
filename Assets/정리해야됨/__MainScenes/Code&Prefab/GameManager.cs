using System.Collections;
using System.Collections.Generic;
using RedstoneinventeGameStudio;
using TMPro;
using UnityEngine;
using Kinnly;
using System;

[System.Serializable]
public class SFXEntry
{
    public string key;      // 예: "Attack", "Hit", "Die" 등
    public AudioClip clip;
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public bool OnClock = true;
    public TextMeshProUGUI ClockText;
    public ResourcePoolManager resourcePoolScipt;
    public TextMeshProUGUI woodText; // 나무 개수를 표시할 UI 텍스트
    public TextMeshProUGUI stoneText; // 돌 개수를 표시할 UI 텍스트
    public TextMeshProUGUI goldText; // 골드 개수를 표시할 UI 텍스트
    public Player player;
    public BGScroll bGScroll;
    

    [Header("게임 시작 시간 설정 분단위ex) 1130분")]
    [SerializeField] public int ClockTime;


    [Header("효과음 관리")]
    public AudioSource sfxAudioSource; // 효과음 전용 AudioSource
    public List<SFXEntry> sfxList = new List<SFXEntry>();
    private Dictionary<string, AudioClip> sfxDict = new Dictionary<string, AudioClip>();

    [Header("현재 보유중인 재화")]
    public int woodCount; // 나무 개수
    public int stoneCount; // 돌 개수
    public int goldCount; // 골드 개수

    [Header("시간 배율 조정")]
    public bool OnTimeScaleControl;
    [Range(0,6f)]
    public float TimeScaleFloat;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // 효과음 딕셔너리 초기화
        foreach (var entry in sfxList)
        {
            if (!sfxDict.ContainsKey(entry.key) && entry.clip != null)
                sfxDict.Add(entry.key, entry.clip);
        }
        player = GameObject.FindFirstObjectByType<Player>();
    }
    public void PlaySFX(string key)
    {
        if (sfxAudioSource == null) return;
        if (sfxAudioSource.mute) return; // 뮤트면 재생 X
        if (sfxDict.ContainsKey(key) && sfxDict[key] != null)
            sfxAudioSource.PlayOneShot(sfxDict[key]);
    }
    private void Start()
    { 
        //처음 텍스트 초기화
        goldText.text = goldCount.ToString();
        stoneText.text = stoneCount.ToString();
        woodText.text = woodCount.ToString();
        StartCoroutine(Clock());
    }
    public void UpdateResourceUI()
    {
        if (woodText != null)
            woodText.text = woodCount.ToString();
        if (stoneText != null)
            stoneText.text = stoneCount.ToString();
        if (goldText != null)
            goldText.text = goldCount.ToString();
    }
    public void AddWood(int amount)
    {
        woodCount += amount;
        Debug.Log("Wood Count: " + woodCount);
        // UI 업데이트
        if (woodText != null)
        {
            woodText.text = woodCount.ToString();
        }
    }
    public void AddStone(int amount)
    {
        stoneCount += amount;
        Debug.Log("Stone Count: " + stoneCount);
        // UI 업데이트
        if (stoneText != null)
        {
            stoneText.text = stoneCount.ToString();
        }
    }
    public void AddGold(int amount)
    {
        goldCount += amount;
        Debug.Log("Gold Count: " + goldCount);
        // UI 업데이트
        if (goldText != null)
        {
            goldText.text = goldCount.ToString();
        }
    }
    IEnumerator Clock()
    {
        int ClockStop = 0;        
        while(OnClock)
        {
            ClockStop++;
            if (ClockStop > 14400)
            {
                ClockStop = 0;
                StopCoroutine(Clock());
            }
            ClockTime += 10;
            int ClockHour = ClockTime / 60;
            int ClockMin = ClockTime % 60; 
            ClockText.text = $"{ClockHour:D2}:{ClockMin:D2}";       
            if((ClockHour == 19 || ClockHour == 6)&& ClockMin == 0)
            {
                bGScroll.Twillight();
            }
            if(ClockHour == 19 && ClockMin == 40)
            {
                bGScroll.Night();
            }
            if(ClockHour == 6 && ClockMin == 40)
            {
                bGScroll.Morning();
            }
            if(ClockHour == 24 && ClockMin == 0)
            {
                ClockTime = 0;
            }                
            yield return new WaitForSeconds(5);
        }
        yield return null;
    }
    void LateUpdate()
    {
        if(OnTimeScaleControl)
        {
            Time.timeScale = TimeScaleFloat;
        }
    }
}