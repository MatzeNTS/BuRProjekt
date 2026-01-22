using UnityEngine;
using UnityEngine.InputSystem.Processors;
using UnityEngine.Sprites;

public class BaumManagerScript : MonoBehaviour
{

    public BlattSpawnScript spawner;
    public GameObject Baum;
    public GameObject SmallTree;
    public SpriteRenderer spriteRenderer;
    public Sprite dead_tree_1;
    public Sprite dead_tree_2;
    public Sprite dead_tree_3;
    public Sprite dead_tree_4;
    public Sprite dead_tree_5;

 
    private void Start()
    {
        var sr = SmallTree.GetComponent<SpriteRenderer>();
        sr.sprite = dead_tree_1;
    }

    //Ändern des Sprites basierend auf der Anzahl zu spawnender Blätter
    public void changeTree(int spawnAmount) 
    {
        var sr = SmallTree.GetComponent<SpriteRenderer>();

        if (spawnAmount <= 5)
        {
            SmallTree.SetActive(true);
            Baum.SetActive(false);

            switch (spawnAmount)
            {
                case 1:
                    sr.sprite = dead_tree_1;
                    break;
                case 2:
                    sr.sprite = dead_tree_2;
                    break;
                case 3:
                    sr.sprite = dead_tree_3;
                    break;
                case 4:
                    sr.sprite = dead_tree_4;
                    break;
                case 5:
                    sr.sprite = dead_tree_5;
                    break;
            }
        }
        else
        {
            SmallTree.SetActive(false);
            Baum.SetActive(true);
        }
    }

}
