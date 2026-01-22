using UnityEngine;

public class BaumManagerScript : MonoBehaviour
{
    public BlattSpawnScript spawner;
    public GameObject Baum;
    public GameObject SmallTree;

    public Sprite dead_tree_1;
    public Sprite dead_tree_2;
    public Sprite dead_tree_3;
    public Sprite dead_tree_4;
    public Sprite dead_tree_5;

    [Header("FX")]
    [SerializeField] private TreeUpgradeFXController upgradeFx;
    [SerializeField] private Transform fxAnchorSmall;
    [SerializeField] private Transform fxAnchorBig;

    [Header("Spawn Areas")]
    [SerializeField] private BoxCollider2D crownAreaBig;   // Baum/CrownArea
    [SerializeField] private BoxCollider2D fallbackSmall;  // optional (SmallTree Collider), sonst null


    [Header("Sound")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip upgradeClip;
    [Range(0f, 1f)][SerializeField] private float upgradeVolume = 0.35f;

    [SerializeField] private int bigTreeAtLevel = 5; // <- im Inspector einstellbar (5 oder 6)

    private void Awake()
    {
        if (upgradeFx == null)
            upgradeFx = GetComponentInChildren<TreeUpgradeFXController>(true);

        if (sfxSource == null)
            sfxSource = GetComponentInChildren<AudioSource>(true);

        // Auto-Find CrownArea am großen Baum (auch wenn Baum deaktiviert ist)
        if (crownAreaBig == null && Baum != null)
            crownAreaBig = Baum.GetComponentInChildren<BoxCollider2D>(true);

        // Optional: SmallTree Collider als Fallback (wenn du willst)
        if (fallbackSmall == null && SmallTree != null)
            fallbackSmall = SmallTree.GetComponentInChildren<BoxCollider2D>(true);
    }


    private void Start()
    {
        // Start-Zustand
        if (Baum != null) Baum.SetActive(false);
        if (SmallTree != null) SmallTree.SetActive(true);

        // Optional: initial binden, damit baseScale korrekt ist
        if (upgradeFx != null && SmallTree != null)
            upgradeFx.Bind(SmallTree.transform, fxAnchorSmall);
    }



    public void changeTree(int spawnAmount)
    {
        if (SmallTree == null || Baum == null) return;

        var sr = SmallTree.GetComponent<SpriteRenderer>();

        bool useBig = spawnAmount >= bigTreeAtLevel;

        if (!useBig)
        {
            SmallTree.SetActive(true);
            Baum.SetActive(false);

            if (upgradeFx != null)
                upgradeFx.Bind(SmallTree.transform, fxAnchorSmall);

            //  zurück auf Presets + optional Small-Fallback
            if (spawner != null)
            {
                spawner.spawnArea = fallbackSmall;  // oder null, dann nimmt er baumRenderer.bounds
                spawner.usePresetSpots = true;      // bis Big Tree
            }

            // Sprites 1..5
            switch (spawnAmount)
            {
                case 1: sr.sprite = dead_tree_1; break;
                case 2: sr.sprite = dead_tree_2; break;
                case 3: sr.sprite = dead_tree_3; break;
                case 4: sr.sprite = dead_tree_4; break;
                case 5: sr.sprite = dead_tree_5; break;
            }

            PlayUpgradeFeedback();
        }
        else
        {

            Baum.SetActive(true);
            SmallTree.SetActive(false);

            if (upgradeFx != null)
                upgradeFx.Bind(Baum.transform, fxAnchorBig);

            // SpawnArea auf CrownArea umschalten + Presets aus
            if (spawner != null)
            {
                spawner.spawnArea = crownAreaBig;
                spawner.usePresetSpots = false;   // ab Big Tree random in Area
            }

            PlayUpgradeFeedback();
        }
    }

    private void PlayUpgradeFeedback()
    {
        // Particles + Punch
        upgradeFx?.PlayUpgradeFX();

        // Sound
        if (sfxSource != null && upgradeClip != null)
            sfxSource.PlayOneShot(upgradeClip, upgradeVolume);
    }
}
