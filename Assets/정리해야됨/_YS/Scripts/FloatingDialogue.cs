using TMPro;
using UnityEngine;
using System.Collections;

public class FloatingDialogue : MonoBehaviour
{
    public TextMeshProUGUI[] dialogues; // 여러 개의 TextMeshProUGUI 배열
    public float typingSpeed = 0.05f; // 글자 등장 속도
    public Transform player; // 플레이어의 Transform
    public Vector3 offset; // 말풍선의 위치 오프셋
    public RectTransform dialoguePrefab; // 다이얼로그 프리팹 이미지

    private int dialogueIndex = 0; // 현재 텍스트 인덱스

    void Update()
    {
        if (player != null)
        {
            // 말풍선의 위치를 플레이어 머리 위로 고정
            Vector3 targetPosition = player.position + offset;
            targetPosition.z = transform.position.z; // Z축 고정
            transform.position = targetPosition;

            // 다이얼로그 프리팹 이미지 위치도 업데이트
            if (dialoguePrefab != null)
            {
                dialoguePrefab.position = targetPosition;
            }
        }
    }

    void Start()
    {
        if (dialogues.Length > 0)
        {
            StartCoroutine(TypeDialogues()); // 여러 텍스트를 순차적으로 출력
        }
    }

    IEnumerator TypeDialogues()
    {
        while (dialogueIndex < dialogues.Length)
        {
            TextMeshProUGUI currentDialogue = dialogues[dialogueIndex];
            if (currentDialogue != null)
            {
                string fullText = currentDialogue.text; // 원래 텍스트 저장
                currentDialogue.text = ""; // 초기화
                currentDialogue.gameObject.SetActive(true); // 현재 텍스트 활성화

                foreach (char letter in fullText)
                {
                    currentDialogue.text += letter; // 한 글자씩 추가
                    yield return new WaitForSeconds(typingSpeed); // 글자 간격 설정
                }

                yield return new WaitForSeconds(1f); // 텍스트 간 대기 시간
                currentDialogue.gameObject.SetActive(false); // 현재 텍스트 비활성화
            }

            dialogueIndex++; // 다음 텍스트로 이동
        }

        // 모든 텍스트가 출력된 후 다이얼로그 프리팹 비활성화
        if (dialoguePrefab != null)
        {
            dialoguePrefab.gameObject.SetActive(false);
        }
    }
}