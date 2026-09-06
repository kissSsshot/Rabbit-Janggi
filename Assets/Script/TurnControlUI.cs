using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class TurnControlUI : MonoBehaviour
{
    [Header("Game UI")]
    public Button StopButton;
    public TMP_Text GameOverText;
    public GameObject GameOverPanel;
    public TMP_Text TurnText;

    [Header("Panels")]
    public GameObject StartPanel; // 시작화면 패널 
    public GameObject GamePanel; // 게임 패널

    [Header("Optional Fade")]
    public CanvasGroup FadeCanvasGroup; // 페이드용
    public float fadeDuration = 0.25f;

    void Start()
    {
        // 시작화면 초기화
        StartPanel?.SetActive(true);
        GamePanel?.SetActive(false);

        GameOverPanel?.SetActive(false);

        PlayerController.Instance.DisableInput();

        // 메뉴상태 진입(정지, 기본턴 알림)
        GameManager.Instance.EnterMenu();
        UpdateTurnText(GameManager.Instance.currentTurn);
        GameManager.OnTurnChanged += UpdateTurnText;

        GameOverPanel?.SetActive(false); // 게임오버 UI 숨기기
        UpdateTurnText(GameManager.Instance.currentTurn);  // 턴 UI 초기화
        GameManager.OnTurnChanged += UpdateTurnText; // 턴 변경 이벤트 등록

        if (StartPanel) StartPanel.SetActive(true);
        if (GamePanel)  GamePanel.SetActive(false);
        if (GameOverPanel) GameOverPanel.SetActive(false);
    }

    public void OnStartButtonPressed() // 게임시작 버튼
    {
        GameManager.Instance.StartGameFromMenu();
        if (StartPanel) StartPanel.SetActive(false);
        if (GamePanel)  GamePanel.SetActive(true);
        PlayerController.Instance.EnableInput();
    }

    public void OnStopButtonPressed() // 턴 종료(넘기기) 버튼
    {
        MoveIndicatorManager.Instance.ClearAll(); // 빨간 점 전체 제거

        if (GameManager.Instance.IsJumpChainActive) // 점프 체인 있을때만 적용시키기
        {
            GameManager.Instance.EndJumpChain();
            GameManager.Instance.SwitchTurn();

            foreach (var rabbit in FindObjectsOfType<Rabbit>()) // 애니메이터 Idle 상태로 리셋
            {
                var anim = rabbit.GetComponentInChildren<Animator>();
                if (anim != null)
                {
                    anim.SetFloat("moveX", 0f);
                    anim.SetFloat("moveY", 0f);
                }
            }
        }
    }

    public void OnRestartButtonPressed() // 재시작 버튼
    {
        MoveIndicatorManager.Instance.ClearAll();
        GameManager.Instance.EnterMenu();
        if (GameOverPanel) GameOverPanel.SetActive(false);
        if (StartPanel) StartPanel.SetActive(true);
        if (GamePanel)  GamePanel.SetActive(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void OnExitButtonPressed() // 종료 버튼
    {
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    public void ShowWinPanel(Team winner)
    {
        if (GameOverText) GameOverText.text = $"{winner} 팀 승리!";
        if (GameOverPanel) GameOverPanel.SetActive(true);
        else Debug.LogWarning("GameOverPanel이 연결되지 않았습니다.");

        PlayerController.Instance.DisableInput();
    }

    private void UpdateTurnText(Team currentTeam)
    {
        /*if (TurnText == null) return;

        TurnText.text = (currentTeam == Team.White) ? "White 턴입니다" : "Black 턴입니다";
        TurnText.color = (currentTeam == Team.White) ? Color.white : Color.black;

        if (StopButton != null)
            StopButton.interactable = GameManager.Instance.IsJumpChainActive;*/

        Debug.Log("턴 UI 업데이트: " + currentTeam);
        if (TurnText == null)
        {
            Debug.LogWarning("TurnText 연결안댐");
            return;
        }

        TurnText.text = (currentTeam == Team.White) ? "White's Trun" : "Black's Turn";
        TurnText.color = (currentTeam == Team.White) ? Color.white : Color.black;
    }

    private void OnDestroy()
    {
        GameManager.OnTurnChanged -= UpdateTurnText;
    }

}
