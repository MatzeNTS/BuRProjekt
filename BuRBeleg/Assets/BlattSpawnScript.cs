using UnityEngine;
using static UnityEngine.Audio.ProcessorInstance;

public class BlattSpawnScript : MonoBehaviour
{
    public GameObject Blatt;
    public float spawnRate = 5;
    public float timer = 0;
    public float heightOffset = 1;
    public float widthOffset = 1;
    public int spawnAmount = 1;

    private static int startCount = 0; //Zähler für Blätter
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("Spawner gestartet: " + gameObject.name);
        Debug.Log("Parameter spawnAmount = " + spawnAmount);
        spawnLeaves();

    }

    // Update is called once per frame
    void Update()
    {
        if (timer < spawnRate)
        {
            timer += Time.deltaTime; //Zeit zwischen Frames wird zum Timer dazugezählt, unterschiedliche FPS beeinflussen Spiel nicht
        }
        else
        {
            spawnLeaves();
            timer = 0;
        }
    }

    void spawnLeaves()
    {
        for (int i = 0; i < spawnAmount; i++)
        {
            spawnLeaf(heightOffset, widthOffset, i);  
        }
    }

    void spawnLeaf(float heightOffset, float widthOffset, int i)
    {
        //zufällige Position für neues Blatt
        Vector3 pos = new Vector3(
            Random.Range(-11f, -5.2f),      //Koordinaten breite Baumkrone
            Random.Range(2.7f, -1.14f),     //Koordinaten Höhe Baumkrone
            0
        );

        //Blatt wird erstellt
        GameObject leaf = Instantiate(Blatt, pos, Quaternion.Euler(0.0f, 0.0f, Random.Range(0.0f, 360.0f)));
        Debug.Log("Spawned "+ i+ " at: " + pos);
    }

    //alte Version
    /*
    void spawnLeaf(float heightOffset, float widthOffset)
    {
        float lowestPoint = transform.position.y - heightOffset; //Position Höhe
        float highestPoint =  transform.position.y + heightOffset;
        float leftestPoint = transform.position.x - widthOffset; //Position Seitlich
        float rightestPoint = transform.position.x + widthOffset;
        Debug.Log("Y-Koordinate: " + transform.position);
        Debug.Log("X-Koordinate: " + transform.position);

        GameObject leaf = Instantiate(Blatt, new Vector3(Random.Range(leftestPoint, rightestPoint), Random.Range(lowestPoint, highestPoint), 0), Quaternion.identity, null); //null = kein parent
        leaf.GetComponent<BlattBehaviourScript>().enableBlattGravity(0f);
        leaf.transform.SetParent(null);

        Debug.Log("Spawn: " + leaf.transform.position);
    }*/

}
