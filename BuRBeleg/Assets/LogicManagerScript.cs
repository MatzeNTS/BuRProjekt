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
