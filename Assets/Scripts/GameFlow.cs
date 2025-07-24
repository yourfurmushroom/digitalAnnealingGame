using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class GameFlow : MonoBehaviour
{
    [SerializeField]
    GameObject animationArea;
    [SerializeField]
    GameObject buttonArea;
    [SerializeField]
    GameObject outputArea;
    

    private bool animationStart=false;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!animationStart&&Input.anyKeyDown)
        {
            animationStart = true;
            animationArea.SetActive(true);
            buttonArea.SetActive(true);
            outputArea.SetActive(true);
            this.gameObject.SetActive(false);
        }


    }
}
