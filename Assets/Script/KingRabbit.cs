using UnityEngine;

public class KingRabbit : Rabbit
{
    
    protected override void Start()
    {
        base.Start(); // Rabbit의 Start 호출
        /*if (team == Team.White)
        transform.position = GridManager.Instance.GridToWorld(new Vector2Int(-4, 4));
        else
        transform.position = GridManager.Instance.GridToWorld(new Vector2Int(4, -4));*/ // 위치 테스트용
    }
    

    public override void MoveTo(Vector2Int newPosition)
    {
        base.MoveTo(newPosition); // 말 이동, 턴 전환

        Vector3 KingWorldPosition = transform.position;
        Vector3 TargetGoal = (team == Team.White) 
            ? new Vector3(-3f, 3.5f, 0f) 
            : new Vector3(3f, -3.5f, 0f);

        float DistanceToGoal = Vector3.Distance(KingWorldPosition, TargetGoal);
        Debug.Log($"{team} 왕의 현재 위치: {KingWorldPosition}, 목표: {TargetGoal}, 거리: {DistanceToGoal}");

        // 이동 후 승리 조건 확인
        if (GameManager.Instance.CheckWinCondition(this.team))
        {
            Debug.Log(team + " 팀 승리");
            GameManager.Instance.EndGame(team);
        }
    }
}