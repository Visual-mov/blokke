using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour {
    public void LoadGameScene() {
        SceneManager.LoadScene("Game");
    }

    public void LoadMenu() {
        SceneManager.LoadScene("Menu");
    }

    public void LoadAbout() {
        SceneManager.LoadScene("About");
    }
}
