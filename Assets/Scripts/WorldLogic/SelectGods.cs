using UnityEngine;

public class SelectGods : MonoBehaviour
{
    public Sprite odinSprite;
    public Sprite thorSprite;

    public void SelectOfGod(string name)
    {
        if (name == "Odin" && odinSprite != null)
        {
            EventBus.Instance.GodOfPlayerIsChanged?.Invoke(odinSprite);
        }
        else if (name == "Thor" && thorSprite != null)
        {
            EventBus.Instance.GodOfPlayerIsChanged?.Invoke(thorSprite);
        }
    }
}
