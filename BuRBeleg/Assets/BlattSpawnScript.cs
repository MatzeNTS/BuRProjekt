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
    [Tooltip("Zeit (Sekunden) zwischen Spawn-Wellen.")]
    public float spawnRate = 2f;

    [Tooltip("Wie viele Blätter pro Spawn-Welle erzeugt werden.")]
    public int spawnAmount = 1;

    [Tooltip("Wie weit die Bounds nach innen geschrumpft werden (damit nicht am Rand gespawnt wird).")]
    public Vector2 innerPadding = new Vector2(0.2f, 0.2f);

    [Header("Preset-Spots (Baumlevel = spawnAmount)")]
    [Tooltip("Wenn aktiv: Blätter werden an festen Spots gespawnt. Je höher spawnAmount, desto mehr Spots sind 'freigeschaltet'.")]
    public bool usePresetSpots = true;

    [System.Serializable]
    public struct SpawnSpot
    {
        public Vector3 position;
        public float rotationZ;
    }

    [Tooltip("Feste Spawn-Spots. Freischaltung: Anzahl nutzbarer Spots = min(spawnAmount, spots.Length).")]
    public SpawnSpot[] spots = new SpawnSpot[]
    {
        new SpawnSpot { position = new Vector3(1.83f, -3.69f, 0f), rotationZ = -100f },
        new SpawnSpot { position = new Vector3(0.843f, -3.4f, 0f), rotationZ = -290f },
        new SpawnSpot { position = new Vector3(1.474f, -1.985f, 0f), rotationZ = -107f },
        new SpawnSpot { position = new Vector3(-0.933f, -3.6f, 0f), rotationZ = -132f },
        new SpawnSpot { position = new Vector3(-1.845f, -1.854f, 0f), rotationZ = -280f }
    };

    private float timer = 0f;

    [Header("Debug/State")]
    public int blattCount = 0;

    void Awake()
    {
        // automatische Fallbacks finden
        if (Baum != null && baumRenderer == null)
            baumRenderer = Baum.GetComponent<SpriteRenderer>();

        if (spawnArea == null && Baum != null)
            spawnArea = Baum.GetComponent<BoxCollider2D>();
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

    public void spawnLeaves()
    {
        for (int i = 0; i < spawnAmount; i++)
        {
            SpawnOneLeaf(i);
            blattCount++;
        }
    }

    int GetUnlockedSpotCount()
    {
        if (spots == null) return 0;

        // spawnAmount ist bei euch das Baum-Upgrade/Level (LogicManager erhöht spawnAmount)
        return Mathf.Clamp(spawnAmount, 0, spots.Length);
    }

    void SpawnOneLeaf(int index)
    {
        // 1) Preset-Spots: zufällig aus den aktuell freigeschalteten Spots
        int unlocked = GetUnlockedSpotCount();
        if (usePresetSpots && unlocked > 0)
        {
            int idx = Random.Range(0, unlocked);
            SpawnSpot spot = spots[idx];

            GameObject leafPreset = Instantiate(Blatt, spot.position, Quaternion.Euler(0f, 0f, spot.rotationZ));
            LinkLeafToSpawner(leafPreset);
            return;
        }

        // 2) Fallback: random innerhalb Bounds (dev-Logik)
        Bounds b;

        if (spawnArea != null)
            b = spawnArea.bounds;
        else if (baumRenderer != null)
            b = baumRenderer.bounds;
        else if (Baum != null)
            b = new Bounds(Baum.transform.position, new Vector3(4f, 3f, 0f));
        else
        {
            Debug.LogError("Kein Baum / spawnArea / Renderer gesetzt – kann keine Spawn-Area bestimmen.", this);
            return;
        }

        float minX = b.min.x + innerPadding.x;
        float maxX = b.max.x - innerPadding.x;
        float minY = b.min.y + innerPadding.y;
        float maxY = b.max.y - innerPadding.y;

        Vector3 pos = new Vector3(Random.Range(minX, maxX), Random.Range(minY, maxY), 0f);
        GameObject leaf = Instantiate(Blatt, pos, Quaternion.Euler(0f, 0f, Random.Range(0f, 360f)));
        LinkLeafToSpawner(leaf);
    }

    void LinkLeafToSpawner(GameObject leaf)
    {
        if (leaf == null) return;

        // Wichtig: Referenz setzen, sonst knallt BlattDestruction beim Decrement
        var behaviour = leaf.GetComponent<BlattBehaviourScript>();
        if (behaviour != null)
            behaviour.spawnScript = this;
    }

    public void NotifyLeafDestroyed()
    {
        blattCount = Mathf.Max(0, blattCount - 1);
    }
}
