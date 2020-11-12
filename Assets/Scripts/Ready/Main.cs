using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private int clickCnt = 0;
    public void ChangeScene()
    {
        if(clickCnt > 1)
        {
            SceneManager.LoadScene("RealGame");
        }
        clickCnt++;
        
    }
}
