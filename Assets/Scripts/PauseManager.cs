using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    
    void Start()
    {
        //Uncomment these when Luna is installed
        //Luna.Unity.LifeCycle.OnResume += Resume;
        //Luna.Unity.LifeCycle.OnPause += Pause;
    }

    private void Resume()
    {
        Time.timeScale = 1;
    }

    private void Pause()
    {
        Time.timeScale = 0;
    }

}
