using UnityEngine;

public enum ScreenId
{
    NONE = 0,
    BATTLE = 1,
    BATTLE_REWARDS = 2,
    MAIN_MENU = 3,
}

public class ScreenController : MonoBehaviour
{
    public ScreenId screenId;
    
    public virtual void Initialize()
    {
    }

    public virtual void ShowAsScreen()
    {
        gameObject.SetActive(true);
    }

    public virtual void ShowAsPopUp()
    {
    }

    public virtual ScreenId GetNextScreenId()
    {
        return screenId;
    }

    public virtual void Close()
    {
        gameObject.SetActive(false);
    }
}
