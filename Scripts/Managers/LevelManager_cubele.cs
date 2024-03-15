using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager_cubele : MonoBehaviour {

    public GameObject panel_loading;

    void Awake() {
        
    }

    void Start() {
        Application.targetFrameRate = 60;

        IsGameRunning = false;

        EventManager.AddListener("StartGame", OnStartGame);
        EventManager.AddListener("PlayerDie", OnPlayerDie);
        EventManager.AddListener("BonusCollision", OnBonusCollision);

    }

    void OnDestroy() {
        EventManager.RemoveListener("StartGame", OnStartGame);
        EventManager.RemoveListener("PlayerDie", OnPlayerDie);
        EventManager.RemoveListener("BonusCollision", OnBonusCollision);
    }

    public static bool IsGameRunning {
        get; set;
    }

    void OnStartGame() {
        PlayerStats.ResetAll();
        PlayerStats.GamePlayCount++;

        IsGameRunning = true;
    }

    void OnPlayerDie() {
        IsGameRunning = false;

#if UNITY_IPHONE || UNITY_ANDROID
        if (Settings.Vibrations)
            Handheld.Vibrate();
#endif

        
    }

    void OnBonusCollision(GameObject bonus) {
        // StartCoroutine(SlowMotion());
    }

    IEnumerator SlowMotion() {
        // slow down and wait 1s
        Time.timeScale = 0.5f;
        yield return new WaitForSeconds(0.2f);

        Time.timeScale = 1f;
    }

    public void click_menu()
    {
        panel_loading.SetActive(true); 
        SceneManager.LoadSceneAsync(0 , LoadSceneMode.Single);  
    }


}
