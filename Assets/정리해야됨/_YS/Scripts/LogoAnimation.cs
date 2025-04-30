using UnityEngine;

public class LogoAnimation : MonoBehaviour
{
    public RectTransform logo; // 로고 UI의 RectTransform
    public float duration = 1.0f; // 애니메이션 지속 시간
    public Vector3 startScale = new Vector3(1f, 1f, 1f); // 시작 크기 (50% 줄임)
    public Vector3 endScale = new Vector3(0.5f, 0.5f, 0.5f); // 끝 크기 (50% 줄임)
    public float moveDelay = 1.0f; // 이동 시작 시간
    public float moveDuration = 1.0f; // Y축 이동 지속 시간
    public float moveDistance = 300f; // Y축 이동 거리

    private float timer = 0f;
    private bool isMoving = false; // 이동 여부 확인
    private float moveTimer = 0f; // 이동 타이머

    void Start()
    {
        // 로고의 시작 크기 설정
        logo.localScale = startScale;
    }

    void Update()
    {
        // 애니메이션 진행
        if (timer < duration)
        {
            timer += Time.deltaTime;
            float progress = timer / duration;

            // 선형 보간으로 크기 조정
            logo.localScale = Vector3.Lerp(startScale, endScale, progress);

            // 화면 가운데 배치 (optional)
            logo.anchoredPosition = Vector2.zero;
        }

        // 이동 처리
        if (timer >= moveDelay && !isMoving)
        {
            isMoving = true;
            moveTimer = 0f; // 이동 타이머 초기화
        }

        if (isMoving && moveTimer < moveDuration)
        {
            moveTimer += Time.deltaTime;
            float moveProgress = moveTimer / moveDuration;

            // 부드럽게 Y축 이동
            logo.anchoredPosition = Vector2.Lerp(Vector2.zero, new Vector2(0, moveDistance), moveProgress);
        }
    }
}