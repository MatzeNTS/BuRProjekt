using System.Linq;
using UnityEngine;
using UnityEngine.UI;



public class LogicManagerScript : MonoBehaviour
{
    public Text scoreText;
    public GameObject StartMenu;
    public BlattBehaviourScript Behaviour;
    public BlattSpawnScript spawnScript;
    public BaumManagerScript baumManagerScript;

    private int playerScore;
    public float timer = 0;
    private bool passiveActive = false; //Passives Einkommen aktiviert
    private int passiveIncome = 0;      //Menge passives Einkommen
    private int IncomeRate = 10;        //wie oft passives Einkommen
    private bool IsPurchasable = false;
    

    [ContextMenu("Score Increase")]
    public void AddScore(int scoreToAdd = 1)
    {
        playerScore += scoreToAdd;
        scoreText.text = playerScore.ToString();
        Debug.Log("Score hinzugef�gt");
    }

    public void deductScore(int scoreToDeduct) 
    {
        if(playerScore >= scoreToDeduct)
        {
            playerScore -= scoreToDeduct;
            scoreText.text = playerScore.ToString();
            IsPurchasable = true;
            Debug.Log("Score abgezogen und Upgrade gekauft");
        }
        else
        {
            Debug.Log("Nicht genug Bl�tter");
        }

    }

    [ContextMenu("SpawnAmount Increase")]
    public void IncreaseSpawnAmount()
    {
        if (IsPurchasable)
        {
            spawnScript.spawnAmount++;
            Debug.Log("Spawnamount ist jetzt: " + spawnScript.spawnAmount);
            baumManagerScript.changeTree(spawnScript.spawnAmount);
            IsPurchasable = false;
        }

    }

    public void addPassiveIncome()
    {
        if (IsPurchasable)
        {
            passiveActive = true;
            passiveIncome +=1;
            Debug.Log("Passives Einkommen auf " + passiveIncome + " erh�ht");
            IsPurchasable = false;
        }

    }

    private void Update()
    {

            if (timer < IncomeRate)
            {
                timer += Time.deltaTime; //Zeit zwischen Frames wird zum Timer dazugez�hlt, unterschiedliche FPS beeinflussen Spiel nicht
            }
            else if(passiveActive)
            {
                AddScore(passiveIncome);
                Debug.Log("Passives Einkommen erhalten");
                timer = 0;
            }
       
    }

}
