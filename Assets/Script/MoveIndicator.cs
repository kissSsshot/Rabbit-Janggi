using UnityEngine;
using UnityEngine.EventSystems;

public class MoveIndicator : MonoBehaviour, IPointerClickHandler
{
    public Vector2Int targetPosition;
    public Rabbit rabbit;

    public void OnPointerClick(PointerEventData eventData)
    {
        // UI 위 클릭이면 무시
        //if (EventSystem.current.IsPointerOverGameObject()) return;

        Debug.Log("✅ Indicator Clicked");
        Debug.Log("InputEnabled = " + PlayerController.Instance.InputEnabled);

        // 입력 잠겨 있으면 무시
        if (!PlayerController.Instance.InputEnabled)
            return;

        OnTapped();
    }

    private void OnTapped()
    {
        // 🔽 기존 OnMouseDown 안에 있던 내용 전부 여기로 이동 🔽
        ExecuteMove();
    }

    private void ExecuteMove()
    {
        if (GameManager.Instance.isGameOver) return;
        if (GameManager.Instance.IsInMenu) return;
        if (rabbit != null)
        {
            rabbit.MoveTo(targetPosition);
        }// 기존 이동/점프/턴 처리 로직 그대로
    }
}
