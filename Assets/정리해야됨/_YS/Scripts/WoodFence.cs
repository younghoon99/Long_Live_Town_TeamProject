using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WoodFence : MonoBehaviour
{
    public Tilemap tilemap; // 기준 타일맵 Ground
    public GameObject woodFencePrefab; // 나무벽 프리팹
    public GameObject hologramPrefab; // 홀로그램 프리팹

    private GameObject hologramObject; // 인스턴스화된 홀로그램 오브젝트
    private SpriteRenderer hologramRenderer;
    private float lastClickTime = 0f; // 마지막 클릭 시간
    private float doubleClickThreshold = 0.3f; // 더블클릭 간격 (초)
    private bool isActive = false; // 스크립트 활성화 상태

    void Start()
    {
        // 홀로그램 오브젝트를 인스턴스화하고 하이라키에 등록
        if (hologramPrefab != null)
        {
            hologramObject = Instantiate(hologramPrefab);
            hologramObject.SetActive(false); // 시작 시 비활성화
            hologramRenderer = hologramObject.GetComponent<SpriteRenderer>();

            if (hologramRenderer == null)
            {
                Debug.LogWarning("HologramPrefab에 SpriteRenderer가 없습니다.");
            }
        }
        else
        {
            Debug.LogError("HologramPrefab이 설정되지 않았습니다. Unity Editor에서 할당하세요.");
        }
    }

    void Update()
    {
        // KeyCode.E를 눌러 스크립트 활성화/비활성화
        if (Input.GetKeyDown(KeyCode.X))
        {
            isActive = !isActive;

            if (!isActive && hologramObject != null)
            {
                hologramObject.SetActive(false); // 비활성화 시 홀로그램 숨김
            }
        }

        if (!isActive || hologramObject == null || hologramRenderer == null) return;

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellPosition = tilemap.WorldToCell(mousePosition);

        // 홀로그램 위치 업데이트
        hologramObject.transform.position = tilemap.CellToWorld(cellPosition);
        hologramObject.transform.position = new Vector3(hologramObject.transform.position.x, -3.8f, hologramObject.transform.position.z); // Y좌표 고정

        // 홀로그램 활성화
        if (!hologramObject.activeSelf)
        {
            hologramObject.SetActive(true);
        }

        // 투명도 50%로 설정
        SetHologramTransparency(0.5f);

        if (Input.GetMouseButtonDown(1)) // 우클릭 입력
        {
            // 더블클릭 감지
            if (Time.time - lastClickTime < doubleClickThreshold)
            {
                PlaceWall(cellPosition); // 나무벽 생성
            }

            lastClickTime = Time.time; // 클릭 시간 업데이트
        }
    }

    public void PlaceWall(Vector3Int cellPosition)
    {
        // 월드 좌표 계산
        Vector3 worldPosition = tilemap.CellToWorld(cellPosition);
        worldPosition.y = -4f; // Y좌표를 -3.8로 고정

        // 벽 프리팹 생성
        GameObject wallObject = Instantiate(woodFencePrefab, worldPosition, Quaternion.identity);
        wallObject.tag = "Wall"; // 태그를 Wall로 설정

        // 투명도 100%로 설정
        SetHologramTransparency(1.0f);
    }

    private void SetHologramTransparency(float alpha)
    {
        if (hologramRenderer != null)
        {
            Color color = hologramRenderer.color;
            color.a = alpha;
            hologramRenderer.color = color;
        }
    }
}
