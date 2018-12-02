using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour {


    public void RestartGame()
    {
        GameManager.instance.loadScene("Game", true);
        //SceneManager.LoadScene("Game");
    }
}
