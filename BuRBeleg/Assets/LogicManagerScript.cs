using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LogicManagerScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

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
}
