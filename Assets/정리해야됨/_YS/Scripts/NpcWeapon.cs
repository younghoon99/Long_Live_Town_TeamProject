using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcWeapon : MonoBehaviour
{
    private GameObject lWeapon; // L_Weapon 오브젝트
    public float blinkInterval = 0.5f; // 깜빡이는 간격 (초)
    private bool isBlinking = false;

    void Start()
    {
        // L_Weapon 태그를 가진 오브젝트 찾기
        lWeapon = GameObject.FindGameObjectWithTag("L_Weapon");

        if (lWeapon == null)
        {
            Debug.LogWarning("L_Weapon 태그를 가진 오브젝트를 찾을 수 없습니다. 태그가 올바르게 설정되었는지 확인하세요.");
        }
        else
        {
            StartBlinking();
        }
    }

    private void StartBlinking()
    {
        if (lWeapon != null && !isBlinking)
        {
            isBlinking = true;
            StartCoroutine(BlinkCoroutine());
        }
    }

    private IEnumerator BlinkCoroutine()
    {
        while (isBlinking)
        {
            // 활성화/비활성화 반복
            lWeapon.SetActive(!lWeapon.activeSelf);
            yield return new WaitForSeconds(blinkInterval);
        }
    }

    public void StopBlinking()
    {
        isBlinking = false;

        if (lWeapon != null)
        {
            lWeapon.SetActive(true); // 깜빡임 종료 후 항상 활성화
        }
    }
}
