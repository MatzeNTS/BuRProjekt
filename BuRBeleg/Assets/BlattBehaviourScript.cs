using UnityEngine;
using System.Collections;


public class BlattBehaviourScript : MonoBehaviour
{

    private Rigidbody2D BlattBody;
    public LogicManagerScript logic;

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
        logic = GameObject.FindGameObjectWithTag("Logic")?.GetComponent<LogicManagerScript>();
        

        if (logic == null)
            Debug.LogError("LogicManager nicht gefunden!", this);


        if (BlattBody == null)
        {
            Debug.LogError("Rigidbody2D fehlwt", this);
            return;
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void enableBlattGravity(float gravity = 1f)
    {
        BlattBody.gravityScale = gravity;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Boden")) {
            Invoke(nameof(BlattDestruction), 4f);
            logic.AddScore(1);
        }
 
    }

    public void BlattDestruction()
    {
        Debug.Log("Blatt wird zerstört");
        Destroy(gameObject);
    }
}
