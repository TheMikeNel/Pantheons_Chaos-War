using StarterAssets;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class EventOnTrigger : MonoBehaviour
{
    [Header("On Trigger Events")]
    public UnityEvent onTriggerEnter;
    public UnityEvent onTriggerExit;

    [Header("On Button And In Trigger Event")]
    public bool eventOnPlayerButtonAndInTrigger = true;
    public KeyCode button;
    public UnityEvent onButtonEvent;
    public Text buttonToolTipText;

    //States
    private bool _enterEventIsAlreadyUsed = false;

    private void Update()
    {
        if (_enterEventIsAlreadyUsed && Input.GetKeyUp(button))
        {
            onButtonEvent?.Invoke();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(Constants.PlayerTag))
        {
            onTriggerEnter?.Invoke();
            _enterEventIsAlreadyUsed = true;

            if (buttonToolTipText) buttonToolTipText.text = $"Нажмите кнопку \"{button}\"";
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag(Constants.PlayerTag))
        {
            onTriggerExit?.Invoke();
            _enterEventIsAlreadyUsed = false;
        }
    }
}
