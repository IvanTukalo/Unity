using UnityEngine;
using UnityEngine.VFX;

public class ToggleObject : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Об'єкт, який буде вмикатися/вимикатися")]
    public GameObject targetObject;

    [Tooltip("Клавіша для перемикання")]
    public KeyCode toggleKey = KeyCode.E;

    private bool isOn = true;

    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            if (targetObject == null) return;

            if (targetObject == gameObject)
            {
                isOn = !isOn;
                ToggleSelf(isOn);
            }
            else
            {
                bool newState = !targetObject.activeSelf;
                targetObject.SetActive(newState);

                if (newState)
                    RestartEffects(targetObject);
            }
        }
    }

    private void ToggleSelf(bool state)
    {
        // СПОЧАТКУ вмикаємо дочірні об'єкти (щоб Play працював)
        for (int i = 0; i < transform.childCount; i++)
            transform.GetChild(i).gameObject.SetActive(state);

        // Рендерери
        foreach (var r in GetComponentsInChildren<Renderer>(true))
            r.enabled = state;

        // ПОТІМ запускаємо або зупиняємо ефекти
        foreach (var ps in GetComponentsInChildren<ParticleSystem>(true))
        {
            if (state) { ps.Clear(); ps.Play(); }
            else ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        foreach (var vfx in GetComponentsInChildren<VisualEffect>(true))
        {
            if (state) { vfx.Reinit(); vfx.Play(); }
            else vfx.Stop();
        }
    }

    private void RestartEffects(GameObject target)
    {
        foreach (var ps in target.GetComponentsInChildren<ParticleSystem>(true))
        {
            ps.Clear();
            ps.Play();
        }

        foreach (var vfx in target.GetComponentsInChildren<VisualEffect>(true))
        {
            vfx.Reinit();
            vfx.Play();
        }
    }
}
