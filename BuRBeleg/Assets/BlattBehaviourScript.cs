using UnityEngine;
using System.Collections;

public class BlattBehaviourScript : MonoBehaviour
{
    private Rigidbody2D BlattBody;

    public LogicManagerScript logic;
    public BlattSpawnScript spawnScript;

    [Header("Click / Radius")]
    [SerializeField] private float fallRadius = 2f;

    [Header("Wind")]
    [Tooltip("Maximaler Impuls bei einem Klick direkt am Blatt.")]
    public float pushForce = 2.5f;

    [Tooltip("Ab dieser Distanz ist der Windschub praktisch 0.")]
    [SerializeField] private float maxWindDistance = 6f;

    [Tooltip("Optional: Kurve für Falloff (0..1). Wenn leer, wird (1 - d/max) genutzt.")]
    public AnimationCurve windFalloff = AnimationCurve.EaseInOut(0, 1, 1, 0);

    [Header("VFX")]
    public WindGustEffect gustPrefab;

    private bool hasTouchedFloor = false;

    private void Awake()
    {
        BlattBody = GetComponent<Rigidbody2D>();
        BlattBody.gravityScale = 0f;
        BlattBody.angularVelocity = 0f;

        var sr = GetComponent<SpriteRenderer>();
        if (sr != null)
            sr.sortingOrder = Random.Range(0, 1000);
    }

    void Start()
    {

        if (gustPrefab == null)
            gustPrefab = Resources.Load<WindGustEffect>("WindGustPrefab");

        // Logic
        if (logic == null)
            logic = GameObject.FindGameObjectWithTag("Logic")?.GetComponent<LogicManagerScript>();

        if (logic == null)
            Debug.LogError("LogicManager nicht gefunden!", this);

        // SpawnScript (Fallback)
        if (spawnScript == null)
            spawnScript = FindFirstObjectByType<BlattSpawnScript>();

        if (BlattBody == null)
            Debug.LogError("Rigidbody2D fehlt", this);
    }

    private void Update()
    {
        if (!Input.GetMouseButtonDown(0)) return;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;

        // Nur dieses Blatt prüfen (kein FindObjectsByType mehr)
        if (Vector2.Distance(mousePos, transform.position) <= fallRadius)
        {
            ApplyWind(mousePos);
        }
    }

    private void ApplyWind(Vector3 klickOrigin)
    {

        Debug.Log($"Wind click at {klickOrigin} | gustPrefab={(gustPrefab ? gustPrefab.name : "NULL")}", this);


        BlattBody.bodyType = RigidbodyType2D.Dynamic;
        BlattBody.gravityScale = 1f;

        // Richtung vom Klick weg (normalisiert => Distanz beeinflusst NICHT die Richtung/Stärke)
        Vector2 dir = (Vector2)(transform.position - klickOrigin);
        float dist = dir.magnitude;

        if (dist < 0.0001f)
            dir = Random.insideUnitCircle.normalized;
        else
            dir /= dist;

        // Distanz-Falloff: weiter weg = schwächer
        float t = Mathf.Clamp01(dist / Mathf.Max(0.0001f, maxWindDistance)); // 0 nah, 1 weit
        float falloff = windFalloff != null ? windFalloff.Evaluate(t) : (1f - t);
        float strength = pushForce * falloff;

        // bisschen Variation
        dir = (dir + Random.insideUnitCircle * 0.2f).normalized;

        BlattBody.linearVelocity = Vector2.zero;
        BlattBody.AddForce(dir * strength, ForceMode2D.Impulse);

        // Wind-VFX (optional)
        if (gustPrefab != null)
        {
            var gust = Instantiate(gustPrefab, klickOrigin, Quaternion.identity);
            gust.Play(falloff); // 0..1
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!hasTouchedFloor && collision.gameObject.layer == LayerMask.NameToLayer("World"))
        {
            hasTouchedFloor = true;
            logic?.AddScore(1);
            Invoke(nameof(BlattDestruction), 4f);
        }
    }

    public void BlattDestruction()
    {
        Destroy(gameObject);

        // sauber runterzählen (kein NullRef mehr)
        if (spawnScript != null)
            spawnScript.NotifyLeafDestroyed();
    }
}
