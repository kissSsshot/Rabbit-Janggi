using System.Collections.Generic;
using UnityEngine;

public class Rabbit : MonoBehaviour
{
    public Team team;
    public GameObject moveIndicatorPrefab;
    
    protected GridManager gridManager;
    protected Animator anim;
    protected Vector2Int CurrentPosition;
    protected bool isSelected = false;

    protected virtual void Start()
    {
        gridManager = GridManager.Instance;
        CurrentPosition = gridManager.GetGridIndex(transform.position);
        transform.position = gridManager.GridToWorldCentered(CurrentPosition); // 셀 중앙에 정확히 위치시킴
        gridManager.RegisterObject(CurrentPosition, this.gameObject); // 격자에 내 위치 등록
        anim = GetComponentInChildren<Animator>();
    }

    void OnMouseDown()
    {
        if (GameManager.Instance.isGameOver) return; // 게임 오버 시 클릭 안되게
        if (GameManager.Instance.IsInMenu) return;  // 메뉴에서는 선택 불가
        if (GameManager.Instance.currentTurn != team) return;

        if (GameManager.Instance.IsJumpChainActive && !GameManager.Instance.IsInJumpChain(this))
        return; // 점프 체인 중 현재 말만 조작가능

        if (!isSelected)
        {
            isSelected = true;
            ShowValidMoves();
        }
        else
        {
            isSelected = false;
            MoveIndicatorManager.Instance.ClearAll();
        }
    }

    protected void ShowValidMoves()
    {
        List<Vector2Int> validMoves = GetValidMoves();
        MoveIndicatorManager.Instance.ShowIndicators(validMoves, moveIndicatorPrefab, this);

        foreach (Vector2Int pos in validMoves)
        {   
            Vector3 worldPos = gridManager.GridToWorldCentered(pos);
            Debug.Log($"이동좌표 {pos} -> 월드위치 {worldPos}");
        }
    }

    protected List<Vector2Int> GetValidMoves()
    {
        List<Vector2Int> moves = new List<Vector2Int>();
        Vector2Int[] directions = {
        Vector2Int.up, Vector2Int.down,
        Vector2Int.left, Vector2Int.right
        };

        foreach (Vector2Int dir in directions)
        {
            Vector2Int next = CurrentPosition + dir;
            Vector2Int jump = CurrentPosition + dir * 2;

            // 점프만 가능할 때
            if (GameManager.Instance.IsJumpChainActive)
            {
                if (gridManager.IsWithinBounds(jump) && gridManager.IsOccupied(next) && !gridManager.IsOccupied(jump))
                {
                    moves.Add(jump);
                }
            }
            else
            {
                // 일반이동 또는 점프
                if (gridManager.IsWithinBounds(next) && !gridManager.IsOccupied(next))
                {
                    moves.Add(next);
                }
                else if (gridManager.IsWithinBounds(jump) && gridManager.IsOccupied(next) && !gridManager.IsOccupied(jump))
                {
                    moves.Add(jump);
                }
            }
        }
        return moves;
    }

    public virtual void MoveTo(Vector2Int newPosition)
    {
        Vector2Int delta = newPosition - CurrentPosition;
        if (anim != null)
        {
            anim.SetFloat("moveX", Mathf.Clamp(delta.x, -1, 1));
            anim.SetFloat("moveY", Mathf.Clamp(delta.y, -1, 1));
        }
        // 이동
        bool wasJump = Mathf.Abs(delta.x) == 2 || Mathf.Abs(delta.y) == 2;
        if (gridManager.IsOccupied(newPosition))
        {
            Debug.LogWarning("해당 위치에 다른 말이 있어 이동할 수 없습니다: " + newPosition);
            return;
        }

        Vector2Int oldPos = CurrentPosition;
        transform.position = gridManager.GridToWorldCentered(newPosition);
        CurrentPosition = newPosition;
        gridManager.UpdateGridPosition(oldPos, newPosition, gameObject);
        isSelected = false;
        MoveIndicatorManager.Instance.ClearAll();

        // 점프
        if (wasJump)
        {
            GameManager.Instance.StartJumpChain(this);

            if (CanJumpAgain())
            {   
                
                ShowValidMoves();
                return; // 턴 종료 안함
            }
            else
            {
                GameManager.Instance.EndJumpChain();
                GameManager.Instance.SwitchTurn();
                GameManager.Instance.CheckVictoryAfterTurn(); // 점프 끝나고 턴 넘긴 직후
                ResetAnimToIdle();
                return;
            }
        }

        GameManager.Instance.SwitchTurn();
        GameManager.Instance.CheckVictoryAfterTurn(); // 일반이동 끝나고 ;;
        ResetAnimToIdle();
    }

    private void ResetAnimToIdle()
    {
        if (anim == null) return;
        anim.SetFloat("moveX", 0f);
        anim.SetFloat("moveY", 0f);
    }

    protected bool CanJumpAgain()
    {
        Vector2Int[] directions = {
            Vector2Int.up, Vector2Int.down,
            Vector2Int.left, Vector2Int.right
        };

        foreach (Vector2Int dir in directions)
        {
            Vector2Int mid = CurrentPosition + dir;
            Vector2Int end = CurrentPosition + dir * 2;

            if (gridManager.IsWithinBounds(end) && gridManager.IsOccupied(mid) && !gridManager.IsOccupied(end))
                return true;
        }

        return false;
    }
}
