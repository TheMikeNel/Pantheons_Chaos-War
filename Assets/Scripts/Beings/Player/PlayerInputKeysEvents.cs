using UnityEngine;
using UnityEngine.Events;

public class PlayerInputKeysEvents : MonoBehaviour
{
    public UnityEvent onKeyEscape;
    public UnityEvent onKeyU;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) onKeyEscape?.Invoke();

        if (Input.GetKeyDown(KeyCode.U)) onKeyU?.Invoke();
    }
}