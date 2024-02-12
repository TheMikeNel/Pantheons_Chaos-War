using StarterAssets;
using UnityEngine;

public class CursorLockController : MonoBehaviour
{
    [SerializeField] private bool _cursorLockControl = true;

    private StarterAssetsInputs _playerInputs;

    private void Awake()
    {
        if (_cursorLockControl)
        {
            PlayerControl.player.TryGetComponent(out _playerInputs);
        }
    }

    private void OnEnable()
    {
        if (_cursorLockControl)
        {
            SetCursorLockState(false);
        }
    }

    private void OnDisable()
    {
        if ( _cursorLockControl)
        {
            SetCursorLockState(true);
        }
    }

    public void SetCursorLockState(bool isLocked)
    {
        if (_playerInputs)
        {
            _playerInputs.cursorLocked = isLocked;
            _playerInputs.cursorInputForLook = isLocked;
            _playerInputs.SetCursorState(isLocked);
        }
    }
}
