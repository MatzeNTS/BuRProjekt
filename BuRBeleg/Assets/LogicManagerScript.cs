using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;


public class LogicManagerScript : MonoBehaviour
{
    private int playerScore;
    public Text scoreText;
    public GameObject StartMenu;
    public BlattBehaviourScript Behaviour;
    private int FallRadius = 2;




    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) == true)
        {
            //Vector3 mousePos = Input.mousePosition; //alte Variante

            //ScreenToWorldPoint ist weil Maus und Objects unterschiedliche Koordinatensysteme haben
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Debug.Log("Klick bei x:" + mousePos.x+ "y" + mousePos.y);

            foreach (BlattBehaviourScript blatt in FindObjectsByType<BlattBehaviourScript>(FindObjectsSortMode.None))
            {
                //Im Radius von FallRadius werden Blätter fallen gelassen, sonst nicht
                if (Input.GetMouseButton(0) && mousePos.x < blatt.transform.position.x + FallRadius && mousePos.y < blatt.transform.position.y + FallRadius)
                {
                    blatt.enableBlattGravity(1f);
                }
                
            }

        }
    }

    [ContextMenu("Score Increase")]
    public void AddScore(int scoreToAdd = 1)
    {
        playerScore += scoreToAdd;
        scoreText.text = playerScore.ToString();
        Debug.Log("Score hinzugefügt");
    }


    public void CloseGame()
    {
        
    }

    public void StartGame()
    {

    }
}
