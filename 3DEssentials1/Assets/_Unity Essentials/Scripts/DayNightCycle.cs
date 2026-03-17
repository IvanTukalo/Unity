using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [Header("Time Settings")]
    [Tooltip("Кількість реальних секунд для повної віртуальної доби")]
    public float fullDayDurationInSeconds = 120f;

    [Tooltip("Множник швидкості для ночі. Значення 2 зробить ніч удвічі коротшою")]
    public float nightSpeedMultiplier = 2f;

    [Range(0f, 1f)]
    [Tooltip("Поточний час доби. 0.25 дорівнює сходу сонця")]
    public float currentTimeOfDay = 0.5f;

    private void Update()
    {
        CalculateTime();
        ApplyRotation();
    }

    private void CalculateTime()
    {
        float currentMultiplier = 1f;

        bool isNight = currentTimeOfDay <= 0.25f || currentTimeOfDay >= 0.75f;
        if (isNight)
        {
            currentMultiplier = nightSpeedMultiplier;
        }

        float timeStep = (Time.deltaTime / fullDayDurationInSeconds) * currentMultiplier;
        currentTimeOfDay += timeStep;

        if (currentTimeOfDay >= 1f)
        {
            currentTimeOfDay -= 1f;
        }
    }

    private void ApplyRotation()
    {
        float currentAngleX = (currentTimeOfDay * 360f) - 90f;
        transform.localRotation = Quaternion.Euler(currentAngleX, -30f, 0f);
    }
}