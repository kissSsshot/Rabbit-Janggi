using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Rabbit rabbit;  // 이동할 토끼

    public static PlayerController Instance { get; private set; }

    public bool InputEnabled { get; private set; } = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void EnableInput()
    {
        InputEnabled = true;
    }

    public void DisableInput()
    {
        InputEnabled = false;
    }
    void Update()
    {
        
    }
}
