using NCMB;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

namespace Communication
{
    public class LobbyManager : MonoBehaviour
    {
        public GameObject panelWalls;
        public GameObject buttonRoomCreate;
        public GameObject buttonRoomCreateExit;
        public GameObject buttonRoomEnter;
        public GameObject buttonRoomEnterExit;
        public GameObject buttonEnter;
        public GameObject buttonGame;
        public GameObject buttonMessage;
        public GameObject buttonMessageText;
        public GameObject roomCreateObject;
        public GameObject roomEnterObject;

        // ★追加
        //public GameObject gift;
        //public Gift gift;
        
        private bool isGameStart;
        private string userName;
        private string password;

        //オブジェクトと結びつける
        public InputField inputField;
        public Text text;
        public Text textNickname;
        public Text textRoomNumber;
        public Text textInputRoomNumber;
        private string createRoomNumber; 

        private DataManager dataManager;

        // Start is called before the first frame update
        void Start()
        {
            // DBアクセス用クラス宣言
            dataManager = new DataManager();

            // ローカルからプレイヤー情報取得
            userName = PlayerPrefs.GetString(PlayerPrefsKey.NCMB_USERNAME);
            password = PlayerPrefs.GetString(PlayerPrefsKey.NCMB_PASSWORD);

            //初期化
            isGameStart = false;
            inputField = inputField.GetComponent<InputField> ();
            buttonRoomCreate.GetComponent<Button>().interactable = false;
            buttonRoomEnter.GetComponent<Button>().interactable = false;
            text = text.GetComponent<Text> ();
            textRoomNumber.GetComponent<Text> ();
            roomCreateObject.SetActive(false);
            roomEnterObject.SetActive(false);

            // ログイン処理、マイプレイヤー取得
            StartCoroutine(InitCoroutine());
        }
        // 初期データ取得をコルーチンにて処理
        public IEnumerator InitCoroutine()
        {
            yield return StartCoroutine(dataManager.LoginAndSelectMyPlayerCoroutine(userName, password));
            textNickname.GetComponent<Text>().text = NCMBUser.CurrentUser[NCMBUserKey.NICKNAME] as string;

            bool changeFlag = false;
            if(dataManager.myPlayer[0][PlayerKey.PHASE] as string == PhaseKey.LOBBY_DESTROY){
                DisplayMessage("部屋を爆破しました🔥");
                changeFlag = true;
            }else if(dataManager.myPlayer[0][PlayerKey.PHASE] as string  == PhaseKey.LOBBY_EXIT){
                DisplayMessage("部屋が爆破されました💣");
                changeFlag = true;
            }else if(dataManager.myPlayer[0][PlayerKey.PHASE] as string  == PhaseKey.LOBBY_KICK){
                DisplayMessage("部屋から追い出されました（笑）");
                changeFlag = true;
            }
            if(changeFlag){
                dataManager.myPlayer[0][PlayerKey.PHASE] = PhaseKey.LOBBY;
                yield return StartCoroutine(dataManager.UpdateMyPlayerCoroutine());
            }

            // データ取得完了すればボタンを有効化
            buttonRoomCreate.GetComponent<Button>().interactable = true;
            buttonRoomEnter.GetComponent<Button>().interactable = true;
        }        

        private void DisplayMessage(string mes)
        {
            buttonMessage.SetActive(true);
            buttonMessageText.GetComponent<Text>().text = mes;
        }

        public void InputText(){
            //テキストにinputFieldの内容を反映
            text.text = inputField.text;
        }

        // 「部屋を作成する」ボタンを選んだ時の処理 
        public void PushButtonRoomCreate()
        {
            createRoomNumber = UnityEngine.Random.Range(1000,9999).ToString();
            textRoomNumber.GetComponent<Text> ().text = "部屋番号は" + createRoomNumber + "です";
            roomCreateObject.SetActive(true);
            buttonRoomCreate.GetComponent<Button>().interactable = false;
            buttonRoomEnter.GetComponent<Button>().interactable = false;
        }

        // 部屋作成後「退室」を選んだ時の処理 
        public void PushButtonRoomCreateExit()
        {
            roomCreateObject.SetActive(false);
            buttonRoomCreate.GetComponent<Button>().interactable = true;
            buttonRoomEnter.GetComponent<Button>().interactable = true;
        }

        // 部屋作成後「入室」を選んだ時の処理 
        public void PushButtonRoomIn()
        {
            StartCoroutine(PushButtonRoomInCoroutine());
        }
        // 部屋作成後「入室」を選んだ時の処理 (コルーチン)
        public IEnumerator PushButtonRoomInCoroutine()
        {
            yield return StartCoroutine(dataManager.InsertRoomDataCoroutine(createRoomNumber)) ;
            DisplayMessage("部屋の作成に成功しました");

            yield return StartCoroutine(dataManager.InitMyPlayerCoroutine(createRoomNumber));
            DisplayMessage("部屋に接続中…");
            
            SceneManager.LoadScene("GameScene");
        }

        // 「部屋に入る」を選んだ時の処理
        public void PushButtonRoomEnter()
        {
            roomEnterObject.SetActive(true);
            buttonRoomCreate.GetComponent<Button>().interactable = false;
            buttonRoomEnter.GetComponent<Button>().interactable = false;
            DisplayMessage("入室する部屋番号を入力してください");
        }

        // 部屋番号を入力し「入室」を選んだ時の処理
        public void PushButtonRoomSelect()
        {
            if(string.IsNullOrEmpty(textInputRoomNumber.text)){
                DisplayMessage("部屋番号が空です");
            }else{
                StartCoroutine(PushButtonRoomSelectCoroutine());
            }
        }
        // 部屋を選んだ時の処理 (コルーチン)
        public IEnumerator PushButtonRoomSelectCoroutine()
        {
            yield return StartCoroutine(RoomEnterCoroutine());
            if(isGameStart){
                Debug.Log("GAME START");
                SceneManager.LoadScene("GameScene");  
            }
        }
        // 部屋に入る時の処理をさらに条件分岐 (コルーチン)
        public IEnumerator RoomEnterCoroutine() {

            bool isConnecting = true;
            yield return StartCoroutine(dataManager.SelectRoomDataCoroutine(textInputRoomNumber.text));

            if(dataManager.RoomList.Count == 0){
                DisplayMessage("探している部屋が見つかりません");
                isConnecting = false;
            }else if(System.Convert.ToBoolean(dataManager.RoomList[0][RoomKey.DESTROYFLAG])){
                DisplayMessage("探している部屋は爆破済みです");
                isConnecting = false;            
            }else{
                DisplayMessage("部屋が見つかりました");
                            
                if(textInputRoomNumber.text == dataManager.myPlayer[0][PlayerKey.ENTERROOM] as string){
                    if(dataManager.myPlayer[0][PlayerKey.PHASE] as string == PhaseKey.GAME){
                        SceneManager.LoadScene("GameScene");
                    }else if(dataManager.myPlayer[0][PlayerKey.PHASE] as string == PhaseKey.PLAY){
                        SceneManager.LoadScene("PlayScene");
                    }
                }else{
                    // 初期化して GAMEシーンへ
                    yield return StartCoroutine(dataManager.InitMyPlayerCoroutine(textInputRoomNumber.text));
                    DisplayMessage("部屋に接続中…");

                    isGameStart = true;
                    isConnecting = false;
                }
            }

            while (isConnecting) { yield return null; }           
        }

        // 部屋選択時に「戻る」ボタンを押した時の処理
        public void PushButtonRoomEnterExit()
        {
            roomEnterObject.SetActive(false);
            buttonRoomCreate.GetComponent<Button>().interactable = true;
            buttonRoomEnter.GetComponent<Button>().interactable = true;
        }

        // 「成績を見る」を選んだ時の処理
        public void PushButtonGrades()
        {
            SceneManager.LoadScene("GradesScene");  
        }

        // ローカルに保存したユーザ情報を取得
        public string LocalSavedUserName
        {
            get
            {
                return PlayerPrefs.HasKey(PlayerPrefsKey.NCMB_USERNAME) ?
                    PlayerPrefs.GetString(PlayerPrefsKey.NCMB_USERNAME) : string.Empty;
            }
        }

        // Update is called once per frame
        void Update()
        {
            
        }
    }
}
