using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{

    public override void Init()
    {
        if (Application.isPlaying)
            DontDestroyOnLoad(gameObject);
    }
}
