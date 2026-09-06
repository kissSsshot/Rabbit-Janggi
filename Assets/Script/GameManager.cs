using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public Team currentTurn = Team.White;
    public GridManager gridManager;

    public bool isMenu = true; 
    public bool IsInMenu => isMenu;

    public KingRabbit kingRabbitWhite;
    public KingRabbit kingRabbitBlack;

    public Vector2Int whiteGoal = new Vector2Int(-3, 3);   // 검정 진영 중앙
    public Vector2Int blackGoal = new Vector2Int(3, -3);   // 흰 진영 중앙

    private Vector2Int whiteStartCenter; // 화이트왕의 시작 좌표
    private Vector2Int blackStartCenter; // 블랙왕의 시작 좌표

    public static event System.Action<Team> OnTurnChanged;

    public bool isGameOver = false;
    public bool IsJumpChainActive { get; private set; } = false;
    private Rabbit jumpChainRabbit = null;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        gridManager = FindObjectOfType<GridManager>();

        // 왕토끼 참조 연결
        if (!kingRabbitWhite || !kingRabbitBlack)
        {
            foreach (KingRabbit king in FindObjectsOfType<KingRabbit>())
            { if (king.team == Team.White) kingRabbitWhite = king;
                else if (king.team == Team.Black) kingRabbitBlack = king; }
        }
    }

    void Start()
    {
        RefreshKings();
        whiteStartCenter = gridManager.GetGridIndex(kingRabbitWhite.transform.position);
        blackStartCenter = gridManager.GetGridIndex(kingRabbitBlack.transform.position);
    }

    private void RefreshKings()
    {
    #if UNITY_600_OR_NEWER
        var kings = FindObjectsByType<KingRabbit>(FindObjectsInactive.Include, FindObjectsSortMode.None);
    #else
        var kings = FindObjectsOfType<KingRabbit>(true);
    #endif
        foreach (var k in kings)
        { if (k.team == Team.White) kingRabbitWhite = k;
        else if (k.team == Team.Black) kingRabbitBlack = k; }
    }

    public void EnterMenu()
    {
        isMenu = true;
        isGameOver = false;

        currentTurn = Team.White;
        OnTurnChanged?.Invoke(currentTurn);
      
        Time.timeScale = 0f;
    }

    public void StartGameFromMenu()
    {
        isMenu = false;
        isGameOver = false;
        Time.timeScale = 1f;

        OnTurnChanged?.Invoke(currentTurn);
        // FindObjectOfType<TurnManager>()?.BeginFirstTurn();
    }

    public void SwitchTurn()
    {
        currentTurn = (currentTurn == Team.White) ? Team.Black : Team.White;
        Debug.Log("현재 턴: " + currentTurn);
        OnTurnChanged?.Invoke(currentTurn);

        CheckVictoryAfterTurn();
    }

    public void StartJumpChain(Rabbit rabbit)
    {
        IsJumpChainActive = true;
        jumpChainRabbit = rabbit;
    }

    public void EndJumpChain()
    {
        IsJumpChainActive = false;
        jumpChainRabbit = null;
    }

    public bool IsInJumpChain(Rabbit rabbit)
    {
        return IsJumpChainActive && rabbit == jumpChainRabbit;
    }

    public void EndTurnManually()
    {
        if (IsJumpChainActive)
        { EndJumpChain();
            SwitchTurn(); }
    }

    public bool CheckWinCondition(Team team)
    {
        Vector2Int center = (team == Team.White) ? blackStartCenter : whiteStartCenter;
        return IsSquareFormation(team, center);
    }

    private bool IsSquareFormation(Team team, Vector2Int center)
    {
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++){
                Vector2Int p = center + new Vector2Int(dx, dy);
                if (!gridManager.IsWithinBounds(p)) return false;

                var go = gridManager.GetObject(p);
                if (dx == 0 && dy == 0){
                    // 중앙에는 같은 팀의 왕토끼
                    if (go == null) return false;
                    var kr = go.GetComponent<KingRabbit>()
                          ?? go.GetComponentInChildren<KingRabbit>()
                          ?? go.GetComponentInParent<KingRabbit>();
                    if (kr == null || kr.team != team) return false;}
                else{
                    if (go == null) return false;
                    var rb = go.GetComponent<Rabbit>()
                           ?? go.GetComponentInChildren<Rabbit>()
                           ?? go.GetComponentInParent<Rabbit>();
                    if (rb == null || rb.team != team) return false;
                    if (rb.GetComponent<KingRabbit>() != null) return false;}
            }
        }
        return true;
    }

    public void CheckVictoryAfterTurn()
    {
        if (isGameOver) return;
        if (!kingRabbitWhite || !kingRabbitBlack) RefreshKings();

        if (CheckWinCondition(Team.White)) { EndGame(Team.White); return; }
        if (CheckWinCondition(Team.Black)) { EndGame(Team.Black); return; }
    }

    public void EndGame(Team winningTeam)
    {
        if (isGameOver) return;
        isGameOver = true;
        Time.timeScale = 0f;

    #if UNITY_600_OR_NEWER
        var ui = FindFirstObjectByType<TurnControlUI>(FindObjectsInactive.Include);
    #else
        var ui = FindObjectOfType<TurnControlUI>(true);
    #endif
        if (ui != null) ui.ShowWinPanel(winningTeam);
        else Debug.LogWarning("TurnControlUI를 못찾아서 GameOver 패널이 표시안댐.");
    }

    /*public void ResetGame()
    {
        // 1. 모든 말 초기 위치로 되돌리기
        foreach (Rabbit rabbit in FindObjectsOfType<Rabbit>())
        {
            rabbit.ResetPosition();
        }

        foreach (KingRabbit king in FindObjectsOfType<KingRabbit>())
        {
            king.ResetPosition();
        }

        // 2. 턴 초기화
        currentTeam = Team.White;

        // 3. UI 갱신
        TurnControlUI.Instance.UpdateTurnUI(currentTeam);

        // 4. 입력 활성화
        PlayerController.Instance.EnableInput();
    }*/

}
