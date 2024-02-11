using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class EventOnTrigger : MonoBehaviour
{
    [Header("On Trigger Events")]
    public UnityEvent onTriggerEnterEvent;
    public UnityEvent onTriggerExitEvent;

    [Header("On Button And In Trigger Event")]
    public bool eventOnPlayerButtonAndInTrigger = true;
    public KeyCode button;
    public UnityEvent onButtonEvent;
    public Text buttonToolTipText;

    private bool enterEventIsAlreadyUsed = false;

    private void Update()
    {
        if (enterEventIsAlreadyUsed && Input.GetKeyUp(button))
        {
            onButtonEvent?.Invoke();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(Constants.PlayerTag))
        {
            onTriggerEnterEvent?.Invoke();
            enterEventIsAlreadyUsed = true;

            if (buttonToolTipText) buttonToolTipText.text = $"Нажмите кнопку \"{button}\"";
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag(Constants.PlayerTag))
        {
            onTriggerExitEvent?.Invoke();
            enterEventIsAlreadyUsed = false;
        }
    }
}
