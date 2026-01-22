using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StartMenuController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject startMenuRoot;
    [SerializeField] private Button startButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private CanvasGroup startMenuCanvasGroup; // <—
    [SerializeField] private CanvasGroup startButtonGroup;
    [SerializeField] private CanvasGroup exitButtonGroup;
    [SerializeField] private GameObject hudRoot; // z.B. CanvasHUD

    [Header("Fade")]
    [SerializeField] private float fadeOutDuration = 0.25f;

    [Header("Camera Slide")]
    [SerializeField] private Camera cam;
    [SerializeField] private Transform cameraStartTarget;
    [SerializeField] private Transform cameraGameTarget;
    [SerializeField] private float slideDuration = 1.0f;

    [Header("Gameplay")]
    [SerializeField] private MonoBehaviour blattSpawnerScript;

    private void Awake()
    {
        if (hudRoot != null)
            hudRoot.SetActive(false);

        if (cam == null) cam = Camera.main;

        // Auto-Grab CanvasGroup, falls nicht gesetzt
        if (startButtonGroup == null && startButton != null)
            startButtonGroup = startButton.GetComponent<CanvasGroup>() ?? startButton.gameObject.AddComponent<CanvasGroup>();

        if (exitButtonGroup == null && exitButton != null)
            exitButtonGroup = exitButton.GetComponent<CanvasGroup>() ?? exitButton.gameObject.AddComponent<CanvasGroup>();

        if (cameraStartTarget != null && cam != null)
        {
            var p = cameraStartTarget.position;
            p.z = cam.transform.position.z;
            cam.transform.position = p;
        }

        if (blattSpawnerScript != null)
            blattSpawnerScript.enabled = false;

        if (startButton != null) startButton.onClick.AddListener(OnStartClicked);
        if (exitButton != null) exitButton.onClick.AddListener(OnExitClicked);

        // Startmenü initial “klickbar”
        if (startMenuCanvasGroup != null)
        {
            startMenuCanvasGroup.alpha = 1f;
            startMenuCanvasGroup.interactable = true;
            startMenuCanvasGroup.blocksRaycasts = true;
        }
    }

    public void OnStartClicked()
    {
        if (startButton != null) startButton.interactable = false;
        if (exitButton != null) exitButton.interactable = false;

        // Direkt Klicks blocken, während Fade/Slide läuft
        if (startMenuCanvasGroup != null)
        {
            startMenuCanvasGroup.interactable = false;
            startMenuCanvasGroup.blocksRaycasts = false;
        }

        StartCoroutine(StartGameRoutine());
    }

    private IEnumerator StartGameRoutine()
    {
        // 1) Fade Out (Menü bleibt noch da, wird nur transparent)
        if (fadeOutDuration > 0f)
        {
            // Root-Fade (wenn vorhanden)
            if (startMenuCanvasGroup != null)
                yield return FadeCanvasGroup(startMenuCanvasGroup, 1f, 0f, fadeOutDuration);
            else
            {
                // Fallback: Buttons einzeln faden (parallel)
                yield return FadeButtonsParallel(fadeOutDuration);
            }
        }

        // 2) Menü deaktivieren (damit es wirklich weg ist)
        if (startMenuRoot != null)
            startMenuRoot.SetActive(false);

        // 3) Kamera sliden
        if (cam != null && cameraGameTarget != null)
            yield return SlideCamera(cam.transform, cameraGameTarget.position, slideDuration);

        // 3.5) HUD anzeigen (JETZT erst)
        if (hudRoot != null)
            hudRoot.SetActive(true);

        // 4) Spawner starten
        if (blattSpawnerScript != null)
            blattSpawnerScript.enabled = true;
    }

    private IEnumerator FadeButtonsParallel(float duration)
    {
        float t = 0f;
        float startA = startButtonGroup != null ? startButtonGroup.alpha : 1f;
        float exitA = exitButtonGroup != null ? exitButtonGroup.alpha : 1f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float p = Mathf.Clamp01(t / duration);
            p = p * p * (3f - 2f * p);

            if (startButtonGroup != null) startButtonGroup.alpha = Mathf.Lerp(startA, 0f, p);
            if (exitButtonGroup != null) exitButtonGroup.alpha = Mathf.Lerp(exitA, 0f, p);

            yield return null;
        }

        if (startButtonGroup != null) startButtonGroup.alpha = 0f;
        if (exitButtonGroup != null) exitButtonGroup.alpha = 0f;
    }


    private IEnumerator FadeCanvasGroup(CanvasGroup cg, float from, float to, float duration)
    {
        cg.alpha = from;
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float p = Mathf.Clamp01(t / duration);
            // Smooth
            p = p * p * (3f - 2f * p);
            cg.alpha = Mathf.Lerp(from, to, p);
            yield return null;
        }
        cg.alpha = to;
    }

    private IEnumerator SlideCamera(Transform camTf, Vector3 targetPos, float duration)
    {
        Vector3 startPos = camTf.position;
        targetPos.z = startPos.z;

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float p = Mathf.Clamp01(t / duration);
            p = p * p * (3f - 2f * p);
            camTf.position = Vector3.Lerp(startPos, targetPos, p);
            yield return null;
        }
        camTf.position = targetPos;
    }

    public void OnExitClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
