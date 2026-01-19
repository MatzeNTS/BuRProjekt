using UnityEngine;
using static UnityEngine.Audio.ProcessorInstance;

public class BlattSpawnScript : MonoBehaviour
{
    public GameObject Blatt;
    public float spawnRate = 2;
    public float timer = 0;
    public float heightOffset = 1;
    public float widthOffset = 50;
    public int spawnAmount = 3;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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
            spawnLeaf(heightOffset, widthOffset);  
        }
    }

    void spawnLeaf(float heightOffset, float widthOffset)
    {
        float lowestPoint = transform.position.y - heightOffset; //Position Höhe
        float highestPoint =  transform.position.y + heightOffset;
        float leftestPoint = transform.position.x - widthOffset; //Position Seitlich
        float rightestPoint = transform.position.x + widthOffset;

        GameObject leaf = Instantiate(Blatt, new Vector3(transform.position.x, Random.Range(lowestPoint, highestPoint), 0), transform.rotation);
        leaf.GetComponent<BlattBehaviourScript>().enableBlattGravity(0f);

        Debug.Log("Spawn: " + leaf.transform.position);
    }

}
