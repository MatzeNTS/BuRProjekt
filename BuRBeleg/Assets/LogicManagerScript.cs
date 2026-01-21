using UnityEngine;
using UnityEngine.UI;



public class LogicManagerScript : MonoBehaviour
{
    private int playerScore;
    public Text scoreText;
    public GameObject StartMenu;
    public BlattBehaviourScript Behaviour;
    

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
