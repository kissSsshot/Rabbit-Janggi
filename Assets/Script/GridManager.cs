using System;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance {get; private set;}

    public int minX = -4, maxX = 4;            // X축 범위: -4 ~ 4
    public float minY = -4.5f, maxY = 4.5f;    // Y축 범위: -4.5 ~ 4.5
    public float cellSize = 1.0f;              // 한 칸 크기

    // 월드좌표 저장
    private Dictionary<Vector2Int, GameObject> gridObjects = new Dictionary<Vector2Int, GameObject>();

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    // 월드좌표를 격자 인덱스로 변환
    public Vector2Int GetGridIndex(Vector3 worldPos)
    {
        int x = Mathf.RoundToInt(worldPos.x / cellSize);
        int y = Mathf.FloorToInt(worldPos.y / cellSize); // 중심기준으로 계산
        return new Vector2Int(x, y);
    }

    // 격자 인덱스를 월드좌표로 변환 (빨간 점 표시하는용 등)
    public Vector3 GridToWorld(Vector2Int gridPos)
    {
        return new Vector3(gridPos.x * cellSize, gridPos.y * cellSize, 0);
    }

    // 말, 빨간 점 등이 셀 중앙에 위치하게 보정
    public Vector3 GridToWorldCentered(Vector2Int gridPos)
    {   
        return new Vector3(gridPos.x * cellSize, gridPos.y * cellSize + (cellSize / 2f), 0);
    }

    // 보드 범위 안에 있는지 확인
    public bool IsWithinBounds(Vector2Int pos)
    {
        return pos.x >= minX && pos.x <= maxX &&
               pos.y >= Mathf.FloorToInt(minY) && pos.y <= Mathf.FloorToInt(maxY);
    }

    // 말이 이미 해당 위치에 있는지 확인
    public bool IsOccupied(Vector2Int pos)
    {
        return gridObjects.ContainsKey(pos);
    }

    // 말이 이동할 때 호출해서 위치 정보 갱신
    public void UpdateGridPosition(Vector2Int oldPos, Vector2Int newPos, GameObject obj)
    {
        if (gridObjects.ContainsKey(oldPos) && gridObjects[oldPos] == obj)
        {
            gridObjects.Remove(oldPos);
        }

        if (!gridObjects.ContainsKey(newPos))
        {
            gridObjects[newPos] = obj;
        }
        else
        {
            Debug.LogWarning("위치중복감지: " + newPos);
        }
    }

    // 말이 처음 생성될 때 등록용
    public void RegisterObject(Vector2Int pos, GameObject obj)
    {
        if (!gridObjects.ContainsKey(pos))
        {
            gridObjects[pos] = obj;
        }
        else
        {
            Debug.LogWarning("초기배치겹침감지: " + pos);
        }
    }

    // 말이 제거될때
    public void RemoveObject(Vector2Int pos)
    {
        if (gridObjects.ContainsKey(pos))
        {
            gridObjects.Remove(pos);
        }
    }

    // 격자 좌표의 오브젝트 반환
    public GameObject GetObject(Vector2Int pos)
    {
        return gridObjects.ContainsKey(pos) ? gridObjects[pos] : null;
    }

    /*internal void UpdateGridPosition(Vector2Int oldPos, Vector2Int newPosition, GameObject gameObject)
    {
        throw new NotImplementedException();
    }*/

}

