﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GeneralGameControl : MonoBehaviour
{
            [Header("Scripts")]
          public GameObject playerElements;
          public PlayerMove playerMove;
          public AdManager adManager;

          [Header("GUI")]
          public Text scoreText;
          public Text coinText;

          public GameObject onDeadMenu;
          public GameObject onPauseMenu;
          public GameObject receiveExtraLifeBtn;
          public GameObject receiveDoubleCoinsBtn;
          public GameObject  pauseButton;
          public GameObject showExtraLifeAdBtn;
          public GameObject doubleCoinsAdBtn;

          public InputField playerScoreIF;
          public Toggle enemyToggle;

          [SerializeField]
          private static bool haveLost=false;
          private static bool passLevel=false;
          private static bool extraLife=false;

          private static bool enemyActive = true;

          static bool extraLifeGived = false;
          static bool doubleCoinsGived = false;
          private bool paused=true;

          [Header("GameObjects Scene")]
          public GameObject GameElements;
          public GameObject mapGenerator;
          public GameObject meteorEnemy;

          public GameObject MenuElements;
          public GameObject solarSystemParent;
           GameObject solarSystemInstanced;

          public GameObject blackHoleParent;
           GameObject blackHoleInstanced;

          public Text highScore;
          public Text totalCoins;
          public Text totalCoinShop;
          public GameObject options;
          public GameObject credits;
          public GameObject shop;
          public float timeAfterLose;

          public Camera gameCamera;
          public Color [] bgColors;
          public static PlayerData playerData;

          public static int level = 0;
          private static float cameraSize;
          public SoundControl soundCtrl;
          public Tutorial tutorial;

    void Start()
    {

              if (haveLost) {
                        PlayGame();
             }else{
                       StartCoroutine(ShowMenu());
             }
    }

    void Update(){
        if (Input.GetKey("up")) {
                  PauseGame();
        }
        if (Input.GetKey("down")) {
                  RestartGame();
        }

    }

    public IEnumerator ShowMenu(){

              soundCtrl.PlaySound("menu");
              Time.timeScale = 1;
              solarSystemInstanced = GameObject.Instantiate(solarSystemParent, Vector3.zero  , Quaternion.identity, transform ) as GameObject;
              meteorEnemy.SetActive(false);
              yield return null;
              mapGenerator.SetActive(false);

              if (SaveSystem.LoadData<PlayerData>("player.dta")!=null) {
                        playerData = new PlayerData(PlayerMove.score,0,SaveSystem.LoadData<PlayerData>("player.dta").getTutorialViewed());
              }else{
                        playerData = new PlayerData(PlayerMove.score,0,false);
                        SaveSystem.SaveData<PlayerData>(playerData ,"player.dta");
              }
              highScore.text = SaveSystem.LoadData<PlayerData>("player.dta").getScore().ToString();
              totalCoins.text = SaveSystem.LoadData<PlayerData>("player.dta").getCoins().ToString();
              totalCoinShop.text = totalCoins.text;
    }

    public void PlayGame()
    {
              if (SaveSystem.LoadData<PlayerData>("player.dta").getTutorialViewed()) {
                          if (SaveSystem.LoadData<CharactersData>("characters.dta")!=null) {
                              PlayerMove.characterIndex = SaveSystem.LoadData<CharactersData>("characters.dta").GetSelectedIndex();
                          }else{
                              PlayerMove.characterIndex = 0;

                          }

                        Time.timeScale = paused?1:0;
                        paused = false;
                        if (passLevel) {
                                  blackHoleInstanced = GameObject.Instantiate(blackHoleParent, Vector3.zero  , Quaternion.identity, transform ) as GameObject;
                                  Destroy(blackHoleInstanced.GetComponent<Collider>() );
                                  gameCamera.backgroundColor = bgColors[level%bgColors.Length];
                        }
                        if(extraLife){
                            gameCamera.fieldOfView=cameraSize;
                            extraLife = false;
                        }
                        Destroy(solarSystemInstanced);

                        coinText.text = playerData.getCoins().ToString();
                        scoreText.text = playerData.getScore().ToString();
                        passLevel=false;

                        playerElements.SetActive(true);
                        mapGenerator.SetActive(true);

                        meteorEnemy.SetActive(enemyActive);

                        MenuElements.SetActive(false);
                        soundCtrl.PlaySound("bgMusic");

                        //soundCtrl.PlaySound("fire");
              }else{
                        Time.timeScale = 0;
                        tutorial.Show(true);
              }

    }

// in game calls

    public IEnumerator PassLevel()
    {
              yield return new WaitForSeconds(timeAfterLose);
              PlayerMove.score=0;
              level++;
              passLevel=true;
              haveLost=true;
              SceneManager.LoadScene("MainScene");
    }

    public void ShowExtraLifeBtn(){
        receiveExtraLifeBtn.SetActive(true);
        onDeadMenu.SetActive(false);
    }
    public void ExtraLife(){
        receiveExtraLifeBtn.SetActive(false);
        extraLifeGived = true;
        extraLife = true;
        paused=false;
        haveLost=true;
        Debug.Log("LOADING SCENE");
        cameraSize = gameCamera.fieldOfView;
        SceneManager.LoadScene("MainScene");

    }

    public void ShowDoubleCoinsBtn(){
        receiveDoubleCoinsBtn.SetActive(true);
        onDeadMenu.SetActive(false);
    }
    public void GiveDoubleCoins(){
        receiveDoubleCoinsBtn.SetActive(false);
        playerData.setCoins( playerData.getCoins()*2 );
        coinText.text = playerData.getCoins().ToString();
        onPauseMenu.SetActive(true);

    }

    public IEnumerator OnLost(){
                if (!Purchaser.adsPersistance.getAdsRemoved()) {
                    adManager.ShowInterstitialAd();
                }

              Time.timeScale =0.8f;
              yield return new WaitForSeconds(timeAfterLose);
              Time.timeScale = 0;

              //displaying ad

              pauseButton.SetActive(false);
              onDeadMenu.SetActive(!onDeadMenu.activeSelf);
              if (extraLifeGived || !adManager.RewardedAdIsLoaded("ExtraLife")) {
                  showExtraLifeAdBtn.GetComponent<Button>().interactable = false;
              }else{
                  showExtraLifeAdBtn.GetComponent<Button>().interactable = true;
              }
              extraLifeGived = false;

              if (doubleCoinsGived || !adManager.RewardedAdIsLoaded("DoubleCoins")) {
                  doubleCoinsAdBtn.GetComponent<Button>().interactable = false;
              }else{
                  doubleCoinsAdBtn.GetComponent<Button>().interactable = true;
              }
              doubleCoinsGived = false;
              /*restartButton.SetActive(true);
              menuButton.SetActive(true);
              backgroundImg.SetActive(true);*/
   }

   public void UpdateScore ( int scoreGived){
            soundCtrl.PlaySound("score");
            playerData.setScore( playerData.getScore() + scoreGived);
            scoreText.text = playerData.getScore().ToString();
            gameCamera.fieldOfView+=0.1f;
            if (gameCamera.fieldOfView>25) {
                      gameCamera.fieldOfView=25;
            }
   }

   public void GiveCoin(){
            playerData.setCoins( playerData.getCoins() +5 );
            coinText.text = playerData.getCoins().ToString();
  }

// input button calls
    public void PauseGame(){
            //soundCtrl.StopSound("fire");
              Time.timeScale = paused?1:0;
              paused = !paused;
              onPauseMenu.SetActive(!onPauseMenu.activeSelf);
              //restartButton.SetActive(!restartButton.activeSelf);
              //menuButton.SetActive(!menuButton.activeSelf);
              //backgroundImg.SetActive(!backgroundImg.activeSelf);
    }

   public void GoToMenu()
   {
            haveLost=false;
            DoSelection();
   }

   public void RestartGame()
   {
            haveLost=true;
            DoSelection();
   }



   public void DoSelection(){
            paused=false;
            SaveActualData();
            PlayerMove.score=0;
            level=0;
            SceneManager.LoadScene("MainScene");
     }



    public void ShowOptions(){
              credits.SetActive(false);
              options.SetActive(!options.activeSelf);
    }



    public void ShowCredits(){
              credits.SetActive(true);
              options.SetActive(false);
    }


    /// Saving Data
   public void SaveActualData(){
             if (playerData.getScore() < SaveSystem.LoadData<PlayerData>("player.dta").getScore()) {
                       playerData.setScore( SaveSystem.LoadData<PlayerData>("player.dta").getScore());
             }
             playerData.setCoins(playerData.getCoins() +SaveSystem.LoadData<PlayerData>("player.dta").getCoins() );
             SaveSystem.SaveData<PlayerData>( playerData , "player.dta");
             playerData.Reiniciar();
   }

   public void SaveTutorial(){
             playerData.setTutorialViewed(true);
             SaveActualData();
   }

   public void BorrarDatos(){
       playerData = new PlayerData(0,0,false);
       SaveSystem.SaveData<PlayerData>( playerData , "player.dta");
       highScore.text = SaveSystem.LoadData<PlayerData>("player.dta").getScore().ToString();
       totalCoins.text = SaveSystem.LoadData<PlayerData>("player.dta").getCoins().ToString();
       totalCoinShop.text = totalCoins.text;
       
       Purchaser.adsPersistance.setAdsRemoved(false);
       SaveSystem.SaveData<AdsPersistance>(Purchaser.adsPersistance ,"adsPersistance.dta");
       //SaveSystem.DeleteFile("C:/Users/AlanPalacios/AppData/LocalLow/AlanPalacios/Metheor\\characters.dta");
   }

   public void SetPlayerScore(){
       PlayerMove.score = int.Parse(playerScoreIF.text);
       playerData = new PlayerData(PlayerMove.score,0,SaveSystem.LoadData<PlayerData>("player.dta").getTutorialViewed());
       ObjectGenerator.modifyAllPlacementValues(playerMove.objPlacingList, PlayerMove.score);
   }

   public void SetEnemyActive(){
       enemyActive = enemyToggle.isOn;
   }
}
