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
    public class GradesManager : MonoBehaviour
    {
        public GameObject gradesObject;
        public GameObject historyObject;
        public GameObject buttonHistory;

        // ★追加
        private string userName;
        private string password;

        //オブジェクトと結びつける
        public Text textNickname;
        public Text textAccuracyRate;
        public Text textGuessedRate;
        public Text textAccuracyTop;
        public Text textGuessedTop;
        public Text textAccuracyWorst;
        public Text textGuessedWorst;

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
            buttonHistory.GetComponent<Button>().interactable = false;
            gradesObject.SetActive(true);
            historyObject.SetActive(false);

            // ログイン処理、マイプレイヤー取得
            StartCoroutine(InitCoroutine());
        }
        // 初期データ取得をコルーチンにて処理
        public IEnumerator InitCoroutine()
        {
            yield return StartCoroutine(dataManager.LoginAndSelectMyPlayerCoroutine(userName, password));
            textNickname.GetComponent<Text>().text = NCMBUser.CurrentUser[NCMBUserKey.NICKNAME] as string;

            yield return StartCoroutine(dataManager.SelectRelationFromDataCoroutine());

            int goodNumber = 0;
            int badNumber = 0;
            int maxRate = 0;
            string maxRateUser = "";
            int worstRate = 100;
            string worstRateUser = "";

            for(int i=0; i<dataManager.RelationFromList.Count; i++){
                int good = Int32.Parse(dataManager.RelationFromList[i]["GoodNumber"] as string);
                goodNumber += good;
                int bad = Int32.Parse(dataManager.RelationFromList[i]["BadNumber"] as string);
                badNumber += bad;
                int rate = (good * 100 / (good + bad));

                if(rate > maxRate){
                    maxRate = rate;
                    maxRateUser = dataManager.RelationFromList[i]["To"] as string;
                }else if(rate >= maxRate){
                    maxRateUser += "," + dataManager.RelationFromList[i]["To"] as string;
                }

                if(rate < worstRate){
                    worstRate = rate;
                    worstRateUser = dataManager.RelationFromList[i]["To"] as string;
                }else if(rate <= worstRate){
                    worstRateUser += "," + dataManager.RelationFromList[i]["To"] as string;
                }
            }

            textAccuracyRate.GetComponent<Text>().text = (goodNumber * 100 / (goodNumber + badNumber)).ToString() + "%";;
            textAccuracyTop.GetComponent<Text>().text = maxRateUser;
            textAccuracyWorst.GetComponent<Text>().text = maxRateUser;

            yield return StartCoroutine(dataManager.SelectRelationToDataCoroutine());

            goodNumber = 0;
            badNumber = 0;
            maxRate = 0;
            maxRateUser = "";
            worstRate = 100;
            worstRateUser = "";

            for(int i=0; i<dataManager.RelationToList.Count; i++){
                int good = Int32.Parse(dataManager.RelationToList[i]["GoodNumber"] as string);
                goodNumber += good;
                //Debug.Log("GoodNumber:" + good);
                int bad = Int32.Parse(dataManager.RelationToList[i]["BadNumber"] as string);
                badNumber += bad;
                //Debug.Log("BadNumber:" + bad);
                int rate = (good * 100 / (good + bad));

                if(rate > maxRate){
                    maxRate = rate;
                    maxRateUser = dataManager.RelationToList[i]["From"] as string;
                }else if(rate >= maxRate){
                    maxRateUser += "," + dataManager.RelationToList[i]["From"] as string;
                }

                if(rate < worstRate){
                    worstRate = rate;
                    worstRateUser = dataManager.RelationToList[i]["From"] as string;
                }else if(rate <= worstRate){
                    worstRateUser += "," + dataManager.RelationToList[i]["From"] as string;
                }
            }

            textGuessedRate.GetComponent<Text>().text = (goodNumber * 100 / (goodNumber + badNumber)).ToString() + "%";;
            textGuessedTop.GetComponent<Text>().text = maxRateUser;
            textGuessedWorst.GetComponent<Text>().text = maxRateUser;

            yield return StartCoroutine(dataManager.SelectAnswerDataCoroutine());

            // データ取得完了すればボタンを有効化
            buttonHistory.GetComponent<Button>().interactable = true;
        }        

        // 「成績」ボタンを選んだ時の処理 
        public void PushButtonGrades()
        {
            gradesObject.SetActive(true);
            historyObject.SetActive(false);
        }

        // 「歴史」ボタンを選んだ時の処理 
        public void PushButtonHistory()
        {
            gradesObject.SetActive(false);
            historyObject.SetActive(true);
        }

        // 「✕」を選んだ時の処理
        public void PushButtonClose()
        {
            SceneManager.LoadScene("LobbyScene");  
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
