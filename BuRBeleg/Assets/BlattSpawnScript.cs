using UnityEngine;

public class BlattSpawnScript : MonoBehaviour
{
    [Header("Prefabs / References")]
    public GameObject Blatt;
    public GameObject Baum;

    [Tooltip("Optional: Wenn gesetzt, werden die Bounds dieses Colliders als Spawn-Area genutzt (empfohlen).")]
    public BoxCollider2D spawnArea;

    [Tooltip("Fallback: Wenn kein spawnArea gesetzt ist, werden die Bounds vom SpriteRenderer des Baums genutzt.")]
    public SpriteRenderer baumRenderer;

    [Header("Spawning")]
    public float spawnRate = 2f;
    public int spawnAmount = 1;

    [Tooltip("Wie weit die Bounds nach innen geschrumpft werden (damit nicht am Rand gespawnt wird).")]
    public Vector2 innerPadding = new Vector2(0.2f, 0.2f);

    private float timer = 0f;
    public int blattCount = 0;

    void Awake()
    {
        // Fallbacks automatisch finden (minimal invasiv)
        if (Baum != null && baumRenderer == null)
            baumRenderer = Baum.GetComponent<SpriteRenderer>();

        if (spawnArea == null && Baum != null)
            spawnArea = Baum.GetComponent<BoxCollider2D>(); // falls ihr einen Collider als Kronen-Area nutzt
    }

    void Start()
    {
        spawnLeaves();
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnRate)
        {
            spawnLeaves();
            timer = 0f;
        }
    }

    void spawnLeaves()
    {
        for (int i = 0; i < spawnAmount; i++)
        {
            SpawnOneLeaf(i);
            blattCount++;
        }
    }

    void SpawnOneLeaf(int index)
    {
        Bounds b;

        if (spawnArea != null)
        {
            b = spawnArea.bounds;
        }
        else if (baumRenderer != null)
        {
            b = baumRenderer.bounds;
        }
        else if (Baum != null)
        {
            // Notfall-Fallback um den Baum herum
            b = new Bounds(Baum.transform.position, new Vector3(4f, 3f, 0f));
        }
        else
        {
            Debug.LogError("Kein Baum / spawnArea / Renderer gesetzt – kann keine Spawn-Area bestimmen.", this);
            return;
        }

        float minX = b.min.x + innerPadding.x;
        float maxX = b.max.x - innerPadding.x;
        float minY = b.min.y + innerPadding.y;
        float maxY = b.max.y - innerPadding.y;

        Vector3 pos = new Vector3(
            Random.Range(minX, maxX),
            Random.Range(minY, maxY),
            0f
        );

        GameObject leaf = Instantiate(Blatt, pos, Quaternion.Euler(0f, 0f, Random.Range(0f, 360f)));

        // Wichtig: Referenz setzen, sonst knallt BlattDestruction beim Decrement
        var behaviour = leaf.GetComponent<BlattBehaviourScript>();
        if (behaviour != null)
            behaviour.spawnScript = this;
    }

    // Für sauberen Count (statt direkt von außen blattCount--)
    public void NotifyLeafDestroyed()
    {
        blattCount = Mathf.Max(0, blattCount - 1);
    }
}
