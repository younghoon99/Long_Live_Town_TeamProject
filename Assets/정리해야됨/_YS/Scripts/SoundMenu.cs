using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundMenu : MonoBehaviour
{
    // 임시/저장 볼륨값
    private float tempBGMVolume;
    private float tempSFXVolume;
    private float savedBGMVolume;
    private float savedSFXVolume;

    [Header("AudioSource")]
    public AudioSource bgmSource;
    public AudioSource[] sfxSources;

    [Header("사운드1 관련")]
    public Slider slider1;
    public Image soundWaveImg1;
    public List<Sprite> soundWaves1;
    [Header("사운드2 관련")]
    public Slider slider2;
    public Image soundWaveImg2;
    public List<Sprite> soundWaves2;

    void Start()
    {
        savedBGMVolume = PlayerPrefs.GetFloat("BGMVolume", 0.5f);
        savedSFXVolume = PlayerPrefs.GetFloat("SFXVolume", 0.5f);

        tempBGMVolume = savedBGMVolume;
        tempSFXVolume = savedSFXVolume;

        slider1.value = tempBGMVolume;
        slider2.value = tempSFXVolume;
        SetBGMVolume(tempBGMVolume);
        SetSFXVolume(tempSFXVolume);

        slider1.onValueChanged.AddListener(OnBGMVolumeChanged);
        slider2.onValueChanged.AddListener(OnSFXVolumeChanged);

        UpdateSoundWave1();
        UpdateSoundWave2();

        Debug.Log("BGMVolume: " + PlayerPrefs.GetFloat("BGMVolume", 0.5f));
        Debug.Log("SFXVolume: " + PlayerPrefs.GetFloat("SFXVolume", 0.5f));
    }
    
    public void OnBGMVolumeChanged(float value)
    {
        tempBGMVolume = value;
        SetBGMVolume(tempBGMVolume); // 임시 적용(미리듣기)
        UpdateSoundWave1();
    }
    public void OnSFXVolumeChanged(float value)
    {
        tempSFXVolume = value;
        SetSFXVolume(tempSFXVolume);
        UpdateSoundWave2();
    }

    public void SetBGMVolume(float value)
    {
        bgmSource.volume = value;
    }

    public void SetSFXVolume(float value)
    {
        foreach (AudioSource sfx in sfxSources)
            sfx.volume = value;
    }

    // 저장 버튼: 임시값을 진짜로 저장
    public void ClickSave()
    {
        PlayerPrefs.SetFloat("BGMVolume", tempBGMVolume);
        PlayerPrefs.SetFloat("SFXVolume", tempSFXVolume);
        PlayerPrefs.Save();
        savedBGMVolume = tempBGMVolume;
        savedSFXVolume = tempSFXVolume;
        Debug.Log("사운드 설정 저장 완료");
    }

    // 소리설정 UI 닫힐 때 호출 (ESC 등)
    public void RestoreVolumeIfNotSaved()
    {
        tempBGMVolume = savedBGMVolume;
        tempSFXVolume = savedSFXVolume;
        slider1.value = savedBGMVolume;
        slider2.value = savedSFXVolume;
        SetBGMVolume(savedBGMVolume);
        SetSFXVolume(savedSFXVolume);
        UpdateSoundWave1();
        UpdateSoundWave2();
    }



    //사운드 관련 매서드
    void UpdateSoundWave1()
    {
        float v = slider1.value;
        if (v == 0) soundWaveImg1.sprite = soundWaves1[0];
        else if (v < 0.25f) soundWaveImg1.sprite = soundWaves1[1];
        else if (v < 0.5f) soundWaveImg1.sprite = soundWaves1[2];
        else if (v >= 0.75f) soundWaveImg1.sprite = soundWaves1[3];
    }
    void UpdateSoundWave2()
    {
        float v = slider2.value;
        if (v == 0) soundWaveImg2.sprite = soundWaves2[0];
        else if (v < 0.25f) soundWaveImg2.sprite = soundWaves2[1];
        else if (v < 0.5f) soundWaveImg2.sprite = soundWaves2[2];
        else if (v >= 0.75f) soundWaveImg2.sprite = soundWaves2[3];
    }



}