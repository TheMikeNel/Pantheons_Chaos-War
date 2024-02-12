using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [SerializeField] private Text enemiesCounter;
    [SerializeField] private Text wavesCounter;
    [SerializeField] private Image playerHealthBar;
    [SerializeField] private Image playerBlockStaminaBar;
    [SerializeField] private Image playerUltimateChargeBar;
    private int _currentEnemies = 0;

    //Properties
    private int CurrentEnemies
    {
        get => _currentEnemies;
        set
        {
            _currentEnemies = value;
            enemiesCounter.text = _currentEnemies.ToString();
        }
    }

    private Health PlayerHealth
    {
        set
        {
            if (value != null)
                playerHealthBar.fillAmount = value.CurrentHealth / value.MaxHealth;
        }
    }

    private PlayerBattleSystem PlayerBattleSystem 
    {
        set
        {
            if (value != null)
            {
                playerBlockStaminaBar.fillAmount = value.BlockStamina / value.maxBlockStamina;
                playerUltimateChargeBar.fillAmount = value.UltimateCharge / value.ultimateRechargingTime;
            }
        }
    }

    //Methods
    public void UpdatePlayerHealth(Health playerHealth)
    {
        if (playerHealth) PlayerHealth = playerHealth;
    }

    public void UpdatePlayerBattleSystem(PlayerBattleSystem pBS)
    {
        if (pBS)
        {
            PlayerBattleSystem = pBS;
        }
    }

    public void SetWaveAndMaxEnemiesCounts(int wave, int enems)
    {
        wavesCounter.text = wave.ToString();
        CurrentEnemies = enems;
    }

    public void SubtractEnemy(string tag, int layerInd)
    {
        if (tag == "Enemy") CurrentEnemies--;
    }

    public void SetUltimateImage(Sprite image)
    {
        playerUltimateChargeBar.sprite = image;
    }
}
