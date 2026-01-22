using System.Collections;
using UnityEngine;

public class TreeUpgradeFXController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform treeVisual;              // aktueller Baum-Transform
    [SerializeField] private Transform fxAnchor;                // wo Partikel erscheinen
    [SerializeField] private ParticleSystem upgradeParticles;   // sollte NICHT Kind vom SmallTree sein

    [Header("Punch Zoom")]
    [SerializeField] private float punchScale = 1.12f;
    [SerializeField] private float punchInTime = 0.08f;
    [SerializeField] private float punchOutTime = 0.12f;

    private Vector3 baseScale;
    private Coroutine punchRoutine;

    private void Awake()
    {
        // Falls im Inspector nix gesetzt wurde, wenigstens stabil bleiben:
        if (treeVisual == null) treeVisual = transform;
        if (fxAnchor == null) fxAnchor = treeVisual;

        baseScale = treeVisual.localScale;
    }

    /// <summary>
    /// Wechselt Ziel-Baum + Anchor (und optional ParticleSystem).
    /// WICHTIG: baseScale wird neu gesetzt, damit BigTree nicht auf SmallTree-Scale "zurückspringt".
    /// </summary>
    public void Bind(Transform newTreeVisual, Transform newFxAnchor, ParticleSystem newParticles = null)
    {
        if (newTreeVisual != null) treeVisual = newTreeVisual;
        if (newFxAnchor != null) fxAnchor = newFxAnchor;
        if (newParticles != null) upgradeParticles = newParticles;

        baseScale = treeVisual.localScale; // <- das war der entscheidende Punkt
    }

    public void PlayUpgradeFX()
    {
        // Partikel
        if (upgradeParticles != null)
        {
            upgradeParticles.transform.position = fxAnchor.position;
            upgradeParticles.Play(true);
        }

        // Punch
        if (punchRoutine != null) StopCoroutine(punchRoutine);
        punchRoutine = StartCoroutine(Punch());
    }

    private IEnumerator Punch()
    {
        Vector3 target = baseScale * punchScale;

        float t = 0f;
        while (t < punchInTime)
        {
            t += Time.deltaTime;
            float p = Mathf.Clamp01(t / punchInTime);
            p = p * p * (3f - 2f * p);
            treeVisual.localScale = Vector3.Lerp(baseScale, target, p);
            yield return null;
        }

        t = 0f;
        while (t < punchOutTime)
        {
            t += Time.deltaTime;
            float p = Mathf.Clamp01(t / punchOutTime);
            p = p * p * (3f - 2f * p);
            treeVisual.localScale = Vector3.Lerp(target, baseScale, p);
            yield return null;
        }

        treeVisual.localScale = baseScale;
        punchRoutine = null;
    }
}
