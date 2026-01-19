using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class LogicManagerScript : MonoBehaviour
{
    private int playerScore;
    public Text scoreText;
    public GameObject StartMenu;





    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) == true)
        {
            Vector3 mousePos = Input.mousePosition;
            Debug.Log("Klick bei x:" + mousePos.x);
            Debug.Log("Klick bei y:" + mousePos.y);

            foreach (BlattBehaviourScript blatt in FindObjectsOfType<BlattBehaviourScript>())
            {
                blatt.enableBlattGravity(1f);
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
}
