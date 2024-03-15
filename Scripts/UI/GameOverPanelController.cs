using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class GameOverPanelController : MonoBehaviour
{
    [SerializeField] Text topScore;
    [SerializeField] Text yourScore;
    [SerializeField] Text newTopLabel;

    [SerializeField] Image[] buttonSoundImages;

    SoundsManager_cubele soundsManager;

    bool isAdShown;

    void Awake() {
        soundsManager = FindObjectOfType<SoundsManager_cubele>();
    }

    void Start() {
        if (PlayerStats.ShowGameOverPanel) {
            // show game over panel
            topScore.text = Settings.TopScore.ToString();
            yourScore.text = PlayerStats.Score.ToString();

            newTopLabel.enabled = PlayerStats.IsNewTop;

            if (PlayerStats.IsNewTop)
                soundsManager.NewTopScore();
           

            // move panel on screen
            transform.localPosition = Vector2.zero;
        }
        else {
            // remove game over panel off the screen
            transform.localPosition = new Vector2(-1000, -1000);
        }

        EventManager.AddListener("StartGame", OnStartGame);
    }

    void OnDestroy() {
        EventManager.RemoveListener("StartGame", OnStartGame);
    }

    void OnStartGame() {
        // remove game panel off the screen
        transform.localPosition = new Vector2(-1000, -1000);


    }


    public void OnPrivacyPolicyButtonClick() {
       

    }

    public void OnLeaderboardButtonClick() {
       

    }

    public void OnReplayButtonClick() {
        EventManager.TriggerEvent("StartGame");
    }

   
}
