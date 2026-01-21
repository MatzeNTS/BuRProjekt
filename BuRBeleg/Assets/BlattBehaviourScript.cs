using UnityEngine;
using System.Collections;


public class BlattBehaviourScript : MonoBehaviour
{
    public GameObject Blatt;
    private Rigidbody2D BlattBody;
    public LogicManagerScript logic;
    public BlattSpawnScript spawnScript;

    //Kraft zum Wegstoßen
    public float pushForce = 2.5f;
    //Gravitation nach Wegstoßen
    public float gravityAfter = 0.3f;
    //Größe des Anklickradiuses zum Blätter fallen lassen
    private int FallRadius = 2;

    private void Awake()
    {
        BlattBody = GetComponent<Rigidbody2D>();
        BlattBody.gravityScale = 0f;
        BlattBody.angularVelocity = 0f;

        var sr = GetComponent<SpriteRenderer>();
        sr.sortingOrder = Random.Range(0, 1000);

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Logic script
        logic = GameObject.FindGameObjectWithTag("Logic")?.GetComponent<LogicManagerScript>();
        if (logic == null)
            Debug.LogError("LogicManager nicht gefunden!", this);
        if (BlattBody == null)
        {
            Debug.LogError("Rigidbody2D fehlwt", this);
            return;
        }

    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) == true)
        {
            //Vector3 mousePos = Input.mousePosition; //alte Variante

            //ScreenToWorldPoint ist weil Maus und Objects unterschiedliche Koordinatensysteme haben
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Debug.Log("Klick bei x:" + mousePos.x + "y" + mousePos.y);

            foreach (BlattBehaviourScript blatt in FindObjectsByType<BlattBehaviourScript>(FindObjectsSortMode.None))
            {
                //Im Radius von FallRadius werden Blätter fallen gelassen, sonst nicht
                if (Input.GetMouseButton(0) && mousePos.x < blatt.transform.position.x + FallRadius && mousePos.y < blatt.transform.position.y + FallRadius)
                {
                    Flugrichtung(mousePos);
                    Debug.Log("KlickOrigin :" + mousePos);
                }

            }

        }
    }

    //Bl�tter werden fallen gelassen
    public void enableBlattGravity()
    {
        BlattBody.gravityScale = 1f;
    }

    //Kollision mit Boden
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("World")) {
            Invoke(nameof(BlattDestruction), 4f);
            logic.AddScore(1);
        }
 
    }

    //Blattzerst�rung
    public void BlattDestruction()
    {
        Debug.Log("Blatt wird zerst�rt");
        Destroy(gameObject);
        //Blätter von Gesamtcount abziehen
        spawnScript.blattCount--;
    }

    //wohin Fliegt Blatt bei Mausklick 
    public void Flugrichtung(Vector3 klickOrigin)
    {
        //Dynamic damit linearVelocity funktioniert
        BlattBody.bodyType = RigidbodyType2D.Dynamic;
        BlattBody.gravityScale = 1f;

        //Richtung vom Klick weg
        Vector2 direction = (transform.position - klickOrigin);
        direction += Random.insideUnitCircle * 0.2f;//Variation der Richtung

        //Windstoß geben
        BlattBody.linearVelocity = Vector2.zero;
        BlattBody.AddForce(direction * pushForce, ForceMode2D.Impulse);
    }
}
