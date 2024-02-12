using UnityEngine;
using UnityEngine.Events;

public class EventBus : MonoBehaviour
{
    [Header("Battle Events")]
    public UnityEvent<int, int> WaveAndEnemiesCountChanged;
    public UnityEvent<int> SpawnEnemyLayerInd;
    public UnityEvent<string, int> BeingDeadLayerInd;
    public UnityEvent<Health> PlayerHealthHasChanges;
    public UnityEvent<PlayerBattleSystem> PlayerBattleSystemHasChanges;
    public UnityEvent PlayerIsDead;

    [Header("Choice Events")]
    public UnityEvent<Sprite> GodOfPlayerIsChanged; // Skill image

public static EventBus Instance;

    void Awake()
    {
        Instance = this;
    }
}
