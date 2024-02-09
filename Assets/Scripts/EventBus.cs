using UnityEngine;
using UnityEngine.Events;

public class EventBus : MonoBehaviour
{
    public UnityEvent<int, int> WaveAndEnemiesCountChanged;
    public UnityEvent<int> SpawnEnemyLayerInd;
    public UnityEvent<string, int> BeingDeadLayerInd;
    public UnityEvent<Health> PlayerHealthHasChanges;
    public UnityEvent<PlayerBattleSystem> PlayerBattleSystemHasChanges;
    public UnityEvent PlayerIsDead;

    public static EventBus Instance;

    void Awake()
    {
        Instance = this;
    }
}
