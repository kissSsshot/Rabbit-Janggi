using System.Collections.Generic;
using UnityEngine;

public class MoveIndicatorManager : MonoBehaviour
{
    public static MoveIndicatorManager Instance { get; private set; }
    private List<GameObject> currentIndicators = new List<GameObject>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void ShowIndicators(List<Vector2Int> positions, GameObject prefab, Rabbit rabbitOwner)
    {
        ClearAll();

        foreach (var pos in positions)
        {
            Vector3 worldPos = GridManager.Instance.GridToWorldCentered(pos);
            GameObject indicator = Instantiate(prefab, worldPos, Quaternion.identity);
            MoveIndicator script = indicator.AddComponent<MoveIndicator>();
            script.targetPosition = pos;
            script.rabbit = rabbitOwner;
            currentIndicators.Add(indicator);
        }
    }

    public void ClearAll()
    {
        foreach (GameObject indicator in currentIndicators)
        {
            Destroy(indicator);
        }
        currentIndicators.Clear();
    }
}
