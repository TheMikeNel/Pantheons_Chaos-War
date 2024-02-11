using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LevelConsumersSetter : MonoBehaviour
{
    [SerializeField] private Text wavesCounterText;

    [Header("Difficult Enemies Settings")]

    [Header("Easy")]
    [SerializeField] private int eStartCount = 5;
    [SerializeField] private float eCountMultiplier = 1.25f;
    [SerializeField] private float eBaseHealthMultiplier = 0.5f;
    [SerializeField] private float eWaveHealthMultiplier = 1.0f;
    [SerializeField] private float eBaseDamageMultiplier = 0.5f;
    [SerializeField] private float eWaveDamageMultiplier = 1.0f;

    [Header("Medium")]
    [SerializeField] private int mStartCount = 8;
    [SerializeField] private float mCountMultiplier = 1.5f;
    [SerializeField] private float mBaseHealthMultiplier = 1f;
    [SerializeField] private float mWaveHealthMultiplier = 1.2f;
    [SerializeField] private float mBaseDamageMultiplier = 1f;
    [SerializeField] private float mWaveDamageMultiplier = 1.2f;

    [Header("Hard")]
    [SerializeField] private int hStartCount = 10;
    [SerializeField] private float hCountMultiplier = 1.75f;
    [SerializeField] private float hBaseHealthMultiplier = 1.25f;
    [SerializeField] private float hWaveHealthMultiplier = 1.5f;
    [SerializeField] private float hBaseDamageMultiplier = 1.25f;
    [SerializeField] private float hWaveDamageMultiplier = 1.5f;

    [Header("Impossible")]
    [SerializeField] private int iStartCount = 13;
    [SerializeField] private float iCountMultiplier = 2f;
    [SerializeField] private float iBaseHealthMultiplier = 1.5f;
    [SerializeField] private float iWaveHealthMultiplier = 1.75f;
    [SerializeField] private float iBaseDamageMultiplier = 1.5f;
    [SerializeField] private float iWaveDamageMultiplier = 1.75f;

    private int MaxWaves => LevelManagerScript.maxWaves;

    private void SetWavesCounterText(int value)
    {
        if (wavesCounterText) wavesCounterText.text = value.ToString();
    }

    public void AddWaves(int addWaves)
    {
        LevelManagerScript.maxWaves += addWaves;
        wavesCounterText.text = MaxWaves.ToString();
    }

    public void SetWaves(int value)
    {
        LevelManagerScript.maxWaves = value;
        wavesCounterText.text = MaxWaves.ToString();
    }

    public void SetWaves(string value)
    {
        int count = 0;

        if (int.TryParse(value, out count))
        {
            LevelManagerScript.maxWaves = count;
            wavesCounterText.text = MaxWaves.ToString();
        }
    }

    public void SetDifficult(int difficult)
    {
        switch (difficult)
        {
            case (int)DifficultType.Easy:
                SetEnemyStartCount(eStartCount);
                SetEnemyCountMultiplier(eCountMultiplier);
                SetEnemyBaseHealthMultiplier(eBaseHealthMultiplier);
                SetEnemyWaveHealthMultiplier(eWaveHealthMultiplier);
                SetEnemyBaseDamageMultiplier(eBaseDamageMultiplier);
                SetEnemyWaveDamageMultiplier(eWaveDamageMultiplier);
                break;

            case (int)DifficultType.Medium:
                SetEnemyStartCount(mStartCount);
                SetEnemyCountMultiplier(mCountMultiplier);
                SetEnemyBaseHealthMultiplier(mBaseHealthMultiplier);
                SetEnemyWaveHealthMultiplier(mWaveHealthMultiplier);
                SetEnemyBaseDamageMultiplier(mBaseDamageMultiplier);
                SetEnemyWaveDamageMultiplier(mWaveDamageMultiplier);
                break;

            case (int)DifficultType.Hard:
                SetEnemyStartCount(hStartCount);
                SetEnemyCountMultiplier(hCountMultiplier);
                SetEnemyBaseHealthMultiplier(hBaseHealthMultiplier);
                SetEnemyWaveHealthMultiplier(hWaveHealthMultiplier);
                SetEnemyBaseDamageMultiplier(hBaseDamageMultiplier);
                SetEnemyWaveDamageMultiplier(hWaveDamageMultiplier);
                break;

            case (int)DifficultType.Impossible:
                SetEnemyStartCount(iStartCount);
                SetEnemyCountMultiplier(iCountMultiplier);
                SetEnemyBaseHealthMultiplier(iBaseHealthMultiplier);
                SetEnemyWaveHealthMultiplier(iWaveHealthMultiplier);
                SetEnemyBaseDamageMultiplier(iBaseDamageMultiplier);
                SetEnemyWaveDamageMultiplier(iWaveDamageMultiplier);
                break;
        }
    }

    public void AddMaxEnemies(int addEnems)
    {
        LevelManagerScript.enemCountOnFirstWave += addEnems;
    }

    public void SetEnemyStartCount(int value)
    {
        LevelManagerScript.enemCountOnFirstWave = value;
    }

    public void SetEnemyCountMultiplier(float value)
    {
        LevelManagerScript.enemCountMultiplier = value;
    }

    public void SetEnemyBaseHealthMultiplier(float value)
    {
        LevelManagerScript.enemBaseHealthMultiplier = value;
    }

    public void SetEnemyBaseDamageMultiplier(float value)
    {
        LevelManagerScript.enemBaseDamageMultiplier = value;
    }

    public void SetEnemyWaveHealthMultiplier(float value)
    {
        LevelManagerScript.enemBaseDamageMultiplier = value;
    }

    public void SetEnemyWaveDamageMultiplier(float value)
    {
        LevelManagerScript.enemWaveDamageMultiplier = value;
    }
}

public enum DifficultType
{
    Easy = 0,
    Medium = 1,
    Hard = 2,
    Impossible = 3
}
