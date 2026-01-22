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

    [SerializeField] private float maxScale = 2f;

    private Transform current;

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
        if (newTreeVisual != null && newTreeVisual) {
            current = newTreeVisual;
            treeVisual = newTreeVisual;

            baseScale = treeVisual.localScale;
        } 

        if (newFxAnchor != null) fxAnchor = newFxAnchor;
        if (newParticles != null) upgradeParticles = newParticles;

    }

    public void PlayUpgradeFX()
    {

        if (treeVisual != null) { 
            treeVisual.localScale = baseScale;
        }

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
        Vector3 max = baseScale * maxScale;

        float t = 0f;
        while (t < punchInTime)
        {
            t += Time.deltaTime;
            float p = Mathf.Clamp01(t / punchInTime);
            p = p * p * (3f - 2f * p);
            
            treeVisual.localScale = new Vector3(
                Mathf.Min(treeVisual.localScale.x, max.x),
                Mathf.Min(treeVisual.localScale.y, max.y),
                Mathf.Min(treeVisual.localScale.z, max.z)
            );
            yield return null;
        }

        t = 0f;
        while (t < punchOutTime)
        {
            t += Time.deltaTime;
            float p = Mathf.Clamp01(t / punchOutTime);
            p = p * p * (3f - 2f * p);
            treeVisual.localScale = new Vector3(
                Mathf.Min(treeVisual.localScale.x, max.x),
                Mathf.Min(treeVisual.localScale.y, max.y),
                Mathf.Min(treeVisual.localScale.z, max.z)
            );
            yield return null;
        }

        treeVisual.localScale = new Vector3(
            Mathf.Min(treeVisual.localScale.x, max.x),
            Mathf.Min(treeVisual.localScale.y, max.y),
            Mathf.Min(treeVisual.localScale.z, max.z)
        );
        punchRoutine = null;
    }
}
