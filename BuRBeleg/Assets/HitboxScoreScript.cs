using UnityEngine;

public class HitboxScoreScript : MonoBehaviour
{
    public LogicManagerScript logic;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        logic = GameObject.FindGameObjectWithTag("Logic").GetComponent<LogicManagerScript>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Blatt")
        {
            logic.AddScore(1);
        }
        ;
    }
}
