using UnityEngine;
using System.Collections;

public class WindGustEffect : MonoBehaviour
{
    [SerializeField] private float lifeTime = 0.25f;
    [SerializeField] private float startScale = 0.2f;
    [SerializeField] private float endScale = 1.2f;

    private SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void Play(float intensity01)
    {

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayWindGust(transform.position, intensity01);

        // Intensität beeinflusst Scale ein bisschen
        float scaleMul = Mathf.Lerp(0.8f, 1.2f, intensity01);
        StartCoroutine(Animate(scaleMul));
    }

    private IEnumerator Animate(float scaleMul)
    {
        float t = 0f;

        // start
        transform.localScale = Vector3.one * startScale * scaleMul;

        Color c = sr != null ? sr.color : Color.white;
        float startA = c.a;

        while (t < lifeTime)
        {
            t += Time.deltaTime;
            float p = Mathf.Clamp01(t / lifeTime);

            // scale out
            float s = Mathf.Lerp(startScale, endScale, p) * scaleMul;
            transform.localScale = Vector3.one * s;

            // fade out
            if (sr != null)
            {
                c.a = Mathf.Lerp(startA, 0f, p);
                sr.color = c;
            }

            yield return null;
        }

        Destroy(gameObject);
    }
}
