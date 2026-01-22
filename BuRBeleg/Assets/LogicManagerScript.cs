using System.Linq;
using UnityEngine;
using UnityEngine.UI;



public class LogicManagerScript : MonoBehaviour
{
    private string playerScore;
    private int highScore = 0;
    public Text scoreText;
    public GameObject StartMenu;
    public BlattBehaviourScript Behaviour;
    

    [ContextMenu("Score Increase")]
    public void AddScore(int scoreToAdd = 1)
    {
        //playerScore += scoreToAdd;
        highScore += scoreToAdd;
        playerScore = highScore.ToString() + " $";
        scoreText.text = playerScore;
        Debug.Log("Score hinzugefügt");
    }


    public void CloseGame()
    {
        
    }

    public void StartGame()
    {

    }
}
