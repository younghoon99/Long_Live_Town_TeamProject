using System.Collections;
using System.Collections.Generic;
using System.Linq; // For LINQ methods like Contains
using UnityEngine;
using UnityEngine.Tilemaps;

#if UNITY_EDITOR
using UnityEditor; // Unity Editor에서 버튼을 추가하기 위한 네임스페이스
#endif

public class ResourceTileSpawner : MonoBehaviour
{
    
    [SerializeField] private float fixedYPosition = 0f; // 스폰 위치의 고정된 Y 좌표
    [SerializeField] private Tilemap tilemap; // 타일맵 참조
    [SerializeField] private Tile[] woodTiles; // 나무 타일 변형 배열
    [SerializeField] private Tile[] stoneTiles; // 돌 타일 변형 배열
    [SerializeField] private int maxWoodTiles = 10; // 최대 나무 타일 개수
    [SerializeField] private int maxStoneTiles = 10; // 최대 돌 타일 개수
    [SerializeField] private float spawnInterval = 5f; // 스폰 시도 간격
    [SerializeField] private GameObject woodPrefab; // 나무 타일 제거 시 생성할 오브젝트
    [SerializeField] private GameObject stonePrefab; // 돌 타일 제거 시 생성할 오브젝트

    // 외부에서 접근 가능하도록 public으로 변경
    public List<Vector3> spawnedWoodTilePositions = new List<Vector3>();
    public List<Vector3> spawnedStoneTilePositions = new List<Vector3>();

    public Tile[] GetWoodTiles()
    {
        return woodTiles;
    }
    
    public Tile[] GetStoneTiles()
    {
        return stoneTiles;
    }
    
    // 가장 가까운 나무 타일 위치 반환
    public Vector3 GetNearestWoodTilePosition(Vector3 fromPosition)
    {
        if (spawnedWoodTilePositions.Count == 0)
            return Vector3.zero;
            
        Vector3 nearestPosition = Vector3.zero;
        float minDistance = float.MaxValue;
        
        foreach (Vector3 position in spawnedWoodTilePositions)
        {
            float distance = Vector3.Distance(fromPosition, position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestPosition = position;
            }
        }
        
        return nearestPosition;
    }
    
    // 가장 가까운 돌 타일 위치 반환
    public Vector3 GetNearestStoneTilePosition(Vector3 fromPosition)
    {
        if (spawnedStoneTilePositions.Count == 0)
            return Vector3.zero;
            
        Vector3 nearestPosition = Vector3.zero;
        float minDistance = float.MaxValue;
        
        foreach (Vector3 position in spawnedStoneTilePositions)
        {
            float distance = Vector3.Distance(fromPosition, position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestPosition = position;
            }
        }
        
        return nearestPosition;
    }

    private void Start()
    {
        StartCoroutine(SpawnTiles());
    }

    private IEnumerator SpawnTiles()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            // 나무 타일 생성
            if (spawnedWoodTilePositions.Count < maxWoodTiles && Random.value < 1f / (spawnedWoodTilePositions.Count + 1))
            {
                SpawnTile(woodTiles, spawnedWoodTilePositions);
            }

            // 돌 타일 생성
            if (spawnedStoneTilePositions.Count < maxStoneTiles && Random.value < 1f / (spawnedStoneTilePositions.Count + 1))
            {
                SpawnTile(stoneTiles, spawnedStoneTilePositions);
            }
        }
    }

    private void SpawnTile(Tile[] tileArray, List<Vector3> spawnedTilePositions)
    {
        // 배열에서 랜덤 타일 선택
        int randomIndex = Random.Range(0, tileArray.Length);

        // 타일맵에 랜덤 위치 선택
        Vector3 randomPosition = GetRandomTilePosition();

        // Z 값이 일관되게 유지되도록 함
        randomPosition.z = 0f; // Z 값을 0으로 설정

        // 랜덤 위치를 셀 좌표로 변환
        Vector3Int cellPosition = tilemap.WorldToCell(randomPosition);

        // 셀 위치가 타일맵 경계 내에 있는지 확인
        if (!tilemap.cellBounds.Contains(cellPosition))
        {
            Debug.LogWarning($"위치 {cellPosition}가 타일맵 경계를 벗어났습니다.");
            return;
        }

        // 위치가 이미 점유되어 있는지 확인
        if (tilemap.GetTile(cellPosition) == null)
        {
            // 타일맵에 타일 배치
            tilemap.SetTile(cellPosition, tileArray[randomIndex]);

            // 타일이 올바르게 렌더링되도록 새로고침
            tilemap.RefreshTile(cellPosition);

            // 생성된 타일 목록에 위치 추가
            Vector3 cellCenterPosition = tilemap.CellToWorld(cellPosition);
            spawnedTilePositions.Add(cellCenterPosition);

            // 필요한 경우 타일맵 경계 크기 조정
            if (!tilemap.cellBounds.Contains(cellPosition))
            {
                tilemap.ResizeBounds();
            }
        }
    }

    [SerializeField] private float minX = -10f; // 스폰 가능한 최소 X 위치
    [SerializeField] private float maxX = 10f;  // 스폰 가능한 최대 X 위치
    [SerializeField] private float minDistanceBetweenTiles = 3f; // 생성된 타일 간 최소 거리

    private Vector3 GetRandomTilePosition()
    {
        Vector3 randomPosition;
        bool positionIsValid;

        do
        {
            // 지정된 범위 내에서 랜덤 X 위치 생성
            float randomX = Random.Range(minX, maxX);
            randomPosition = new Vector3(randomX, fixedYPosition, 0f);

            // 위치가 모든 생성된 타일로부터 최소 거리 이상 떨어져 있는지 확인
            positionIsValid = true;
            foreach (var position in spawnedWoodTilePositions)
            {
                if (Vector3.Distance(randomPosition, position) < minDistanceBetweenTiles)
                {
                    positionIsValid = false;
                    break;
                }
            }

            if (positionIsValid)
            {
                foreach (var position in spawnedStoneTilePositions)
                {
                    if (Vector3.Distance(randomPosition, position) < minDistanceBetweenTiles)
                    {
                        positionIsValid = false;
                        break;
                    }
                }
            }
        } while (!positionIsValid);

        return randomPosition;
    }

    private readonly object tileLock = new object(); // 동기화를 위한 lock 객체
    private float prefabYOffset = 1.0f; // 프리팹 생성 시 Y축 오프셋

    // 타일 삭제 처리
    public void RemoveTile(Vector3Int cellPosition)
    {
        lock (tileLock) // 동기화 블록
        {
            // 타일맵에서 타일 삭제
            TileBase tile = tilemap.GetTile(cellPosition);
            if (tile != null)
            {
                // 월드 좌표로 변환
                Vector3 worldPosition = tilemap.CellToWorld(cellPosition);

                // 나무 타일 제거 처리
                if (spawnedWoodTilePositions.Contains(worldPosition))
                {
                    // 나무 타일 위치에 오브젝트 생성 (Y값 조정)
                    if (woodPrefab != null)
                    {
                        Vector3 spawnPosition = new Vector3(worldPosition.x, worldPosition.y + prefabYOffset, worldPosition.z);
                        GameManager.instance.resourcePoolScipt.ActivateResource("Wood", 0, spawnPosition); // 나무 오브젝트 생성
                        Debug.Log($"나무 오브젝트 생성됨: {spawnPosition}");
                    }

                    // 나무 타일 제거
                    spawnedWoodTilePositions.Remove(worldPosition);
                    Debug.Log($"나무 타일 삭제됨: {cellPosition}");
                }
                // 돌 타일 제거 처리
                else if (spawnedStoneTilePositions.Contains(worldPosition))
                {
                    // 돌 타일 위치에 오브젝트 생성 (Y값 조정)
                    if (stonePrefab != null)
                    {
                        Vector3 spawnPosition = new Vector3(worldPosition.x, worldPosition.y + prefabYOffset, worldPosition.z);
                        GameManager.instance.resourcePoolScipt.ActivateResource("Stone", 1, spawnPosition); // 돌 오브젝트 생성
                        Debug.Log($"돌 오브젝트 생성됨: {spawnPosition}");
                    }

                    // 돌 타일 제거
                    spawnedStoneTilePositions.Remove(worldPosition);
                    Debug.Log($"돌 타일 삭제됨: {cellPosition}");
                }

                // 타일맵에서 타일 삭제
                tilemap.SetTile(cellPosition, null);
                tilemap.RefreshTile(cellPosition); // 타일맵 갱신
            }
        }
    }
    /// <summary>
    /// 가장 가까운 나무 타일 위치 반환 (다른 NPC가 이미 타겟한 나무 제외)
    /// </summary>
    /// <param name="fromPosition">시작 위치(월드 좌표)</param>
    /// <returns>가장 가까운 나무 타일의 위치. 없으면 Vector3.zero 반환</returns>

    private HashSet<Vector3> reservedWoodTiles = new HashSet<Vector3>(); // 누가 타겟 잡은 나무 위치
  
    public Vector3 GetNearestAvailableWoodTilePosition(Vector3 fromPosition)
    {
        // 나무 타일이 하나도 없으면 Vector3.zero 반환
        if (spawnedWoodTilePositions.Count == 0)
            return Vector3.zero;

        Vector3 nearestPosition = Vector3.zero; // 가장 가까운 위치 저장용
        float minDistance = float.MaxValue;     // 최소 거리 초기화

        // 모든 나무 타일 위치를 순회
        foreach (Vector3 position in spawnedWoodTilePositions)
        {
            // 이미 예약된(다른 NPC가 타겟 중인) 타일은 스킵
            if (reservedWoodTiles.Contains(position)) continue;

            // 거리 계산
            float distance = Vector3.Distance(fromPosition, position);
            // 더 가까운 타일이면 갱신
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestPosition = position;
            }
        }

        // 가장 가까운 타일 위치 반환 (없으면 Vector3.zero)
        return nearestPosition;
    }

    /// <summary>
    /// 주어진 위치에서 가장 가까운 예약되지 않았고, 적이 가까이 없는 나무 타일의 위치를 반환합니다.
    /// </summary>
    /// <param name="fromPosition">NPC의 현재 위치</param>
    /// <param name="enemyPositions">적들의 위치 리스트</param>
    /// <param name="enemyDangerRadius">적 근처로 간주할 반경</param>
    /// <returns>조건을 만족하는 가장 가까운 나무 타일 위치, 없으면 Vector3.zero</returns>
    public Vector3 GetNearestSafeWoodTilePosition(Vector3 fromPosition, List<Vector3> enemyPositions, float enemyDangerRadius)
    {
        if (spawnedWoodTilePositions.Count == 0)
            return Vector3.zero;

        Vector3 nearestPosition = Vector3.zero;
        float minDistance = float.MaxValue;

        // 0. Enemy 유무/위치에 따라 x좌표 범위 산출 (정확한 분기)
        float npcX = fromPosition.x;
        float? leftEnemy = null;
        float? rightEnemy = null;
        foreach (var enemyPos in enemyPositions)
        {
            if (enemyPos.x < npcX)
            {
                if (leftEnemy == null || enemyPos.x > leftEnemy) leftEnemy = enemyPos.x;
            }
            if (enemyPos.x > npcX)
            {
                if (rightEnemy == null || enemyPos.x < rightEnemy) rightEnemy = enemyPos.x;
            }
        }
        float minX, maxX;
        if (leftEnemy == null && rightEnemy == null)
        {
            // enemy 없음
            minX = float.MinValue;
            maxX = float.MaxValue;
        }
        else if (leftEnemy != null && rightEnemy == null)
        {
            // 왼쪽에만 enemy
            minX = leftEnemy.Value;
            maxX = npcX + enemyDangerRadius; // 오른쪽 범위 약간 확장
        }
        else if (leftEnemy == null && rightEnemy != null)
        {
            // 오른쪽에만 enemy
            minX = npcX - enemyDangerRadius; // 왼쪽 범위 약간 확장
            maxX = rightEnemy.Value;
        }
        else
        {
            // 양쪽에 enemy
            minX = leftEnemy.Value;
            maxX = rightEnemy.Value;
        }

        foreach (Vector3 position in spawnedWoodTilePositions)
        {
            // 1. 이미 예약된 타일은 제외
            if (reservedWoodTiles.Contains(position)) continue;

            

            // 3. NPC와 적의 x좌표 범위 내에 있는지 체크
            if (position.x < minX || position.x > maxX) continue; // 범위 밖이면 제외

            // 4. 거리 계산 및 후보 선정
            float distance = Vector3.Distance(fromPosition, position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestPosition = position;
            }
        }
        return nearestPosition;
    }

    /// <summary>
    /// 주어진 위치에서 가장 가까운 예약되지 않았고, 적이 가까이 없는 돌 타일의 위치를 반환합니다.
    /// </summary>
    /// <param name="fromPosition">NPC의 현재 위치</param>
    /// <param name="enemyPositions">적들의 위치 리스트</param>
    /// <param name="enemyDangerRadius">적 근처로 간주할 반경</param>
    /// <returns>조건을 만족하는 가장 가까운 돌 타일 위치, 없으면 Vector3.zero</returns>
    public Vector3 GetNearestSafeStoneTilePosition(Vector3 fromPosition, List<Vector3> enemyPositions, float enemyDangerRadius)
    {
        if (spawnedStoneTilePositions.Count == 0)
            return Vector3.zero;

        Vector3 nearestPosition = Vector3.zero;
        float minDistance = float.MaxValue;

        // 0. Enemy 유무/위치에 따라 x좌표 범위 산출 (정확한 분기)
        float npcX = fromPosition.x;
        float? leftEnemy = null;
        float? rightEnemy = null;
        foreach (var enemyPos in enemyPositions)
        {
            if (enemyPos.x < npcX)
            {
                if (leftEnemy == null || enemyPos.x > leftEnemy) leftEnemy = enemyPos.x;
            }
            if (enemyPos.x > npcX)
            {
                if (rightEnemy == null || enemyPos.x < rightEnemy) rightEnemy = enemyPos.x;
            }
        }
        float minX, maxX;
        if (leftEnemy == null && rightEnemy == null)
        {
            // enemy 없음
            minX = float.MinValue;
            maxX = float.MaxValue;
        }
        else if (leftEnemy != null && rightEnemy == null)
        {
            // 왼쪽에만 enemy
            minX = leftEnemy.Value;
            maxX = npcX + enemyDangerRadius; // 오른쪽 범위 약간 확장
        }
        else if (leftEnemy == null && rightEnemy != null)
        {
            // 오른쪽에만 enemy
            minX = npcX - enemyDangerRadius; // 왼쪽 범위 약간 확장
            maxX = rightEnemy.Value;
        }
        else
        {
            // 양쪽에 enemy
            minX = leftEnemy.Value;
            maxX = rightEnemy.Value;
        }

        foreach (Vector3 position in spawnedStoneTilePositions)
        {
            // 1. 이미 예약된 타일은 제외
            if (reservedStoneTiles.Contains(position)) continue;

           

            // 3. NPC와 적의 x좌표 범위 내에 있는지 체크
            if (position.x < minX || position.x > maxX) continue; // 범위 밖이면 제외

            // 4. 거리 계산 및 후보 선정
            float distance = Vector3.Distance(fromPosition, position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestPosition = position;
            }
        }
        return nearestPosition;
    }

    /// <summary>
    /// 나무 타일을 예약(타겟) 처리합니다. (중복 예약 방지)
    /// </summary>
    /// <param name="position">예약할 나무 타일의 월드 좌표</param>
    public void ReserveWoodTile(Vector3 position)
    {
        // 이미 예약되어 있지 않으면 예약 리스트에 추가
        if (!reservedWoodTiles.Contains(position))
            reservedWoodTiles.Add(position);
    }

    /// <summary>
    /// 작업 완료 혹은 취소 시 해당 타일 타겟 해제
    /// </summary>
    public void ReleaseWoodTile(Vector3 position)
    {
        reservedWoodTiles.Remove(position);
    }


    /// <summary>
    /// 가장 가까운 돌돌 타일 위치 반환 (다른 NPC가 이미 타겟한 나무 제외)
    /// </summary>
    /// <param name="fromPosition"></param>
    /// <returns></returns>
    /// 
    private HashSet<Vector3> reservedStoneTiles = new HashSet<Vector3>(); // 누가 타겟 잡은 돌 위치

    public Vector3 GetNearestAvailableStoneTilePosition(Vector3 fromPosition)
    {
        if (spawnedStoneTilePositions.Count == 0)
            return Vector3.zero;

        Vector3 nearestPosition = Vector3.zero;
        float minDistance = float.MaxValue;

        foreach (Vector3 position in spawnedStoneTilePositions)
        {
            if (reservedStoneTiles.Contains(position)) continue; // 이미 타겟된 돌은 제외

            float distance = Vector3.Distance(fromPosition, position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestPosition = position;
            }
        }
        return nearestPosition;
    }

    public void ReserveStoneTile(Vector3 position)
    {
        if (!reservedStoneTiles.Contains(position))
            reservedStoneTiles.Add(position);
    }
    /// <summary>
    /// 작업 완료 혹은 취소 시 해당 타일 타겟 해제
    /// </summary>
    public void ReleaseStoneTile(Vector3 position)
    {
        reservedStoneTiles.Remove(position);
    }

}