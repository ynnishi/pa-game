using NCMB;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

namespace Communication
{
    public class TopManager : MonoBehaviour
    {
        /* オブジェクト一覧(基本的にUnity状のオブジェクトと同じ) */
        public GameObject buttonStart;
        public GameObject buttonNext;
        public GameObject buttonCard;
        public GameObject buttonMessage;
        public GameObject buttonMessageText;
        public GameObject nickNameObject;
        public GameObject cardObject;
        public GameObject backImage;
        public GameObject backImage2;

        // ローカルにユーザ情報が登録されてるかどうかのフラグ
        private bool haveSavedUser;

        //課題作成用
        public InputField inputField;
        public Text text;
        public InputField[] cardField = new InputField[10];

        // データクラス接続用オブジェクト
        private DataManager dataManager;

        // Start is called before the first frame update
        void Start()
        {
            // ローカルにユーザ登録があるかのフラグ
            haveSavedUser = false;

            // まずはログアウト
            try {
                NCMBUser.LogOutAsync ();
            } catch (NCMBException e) {
                UnityEngine.Debug.Log ("エラー: " + e.ErrorMessage);
            }

            // 課題入力用のフィールドを初期化
            inputField = inputField.GetComponent<InputField> ();
            for(int i=0;i<10;i++){
                cardField[0] = inputField.GetComponent<InputField> ();
            }
            text = text.GetComponent<Text> ();
            
            // ユーザログイン確認
            CheckSavedUserNameExist();
        }

        // ユーザが登録されているか確認し、ボタン処理を分岐
        private void CheckSavedUserNameExist()
        {
            //ローカルにオートログイン用のIDが保存されているか調べる//
            if (string.IsNullOrEmpty(LocalSavedUserName))
            {
                //Debug.Log("ない！！！！！");
                //初めからボタンを表示//
                Debug.Log("empty!");
                buttonStart.SetActive(true);
                buttonNext.SetActive(false);
            }
            else
            {
                //Debug.Log("ある！！！！！");
                //続きからボタンを表示//
                Debug.Log("exist!");
                buttonStart.SetActive(false);
                buttonNext.SetActive(true);
                haveSavedUser = true;
            }
        }

        // 画面下に文字を表示
        private void DisplayMessage(string mes)
        {
            buttonMessage.SetActive(true);
            buttonMessageText.GetComponent<Text>().text = mes;
        }

        // テキストにinputFieldの内容を反映
        public void InputText(){ 
            text.text = inputField.text;
        }

        // 「START」のボタン押下時の処理
        public void PushButtonRegisterStart()
        {
            Debug.Log("STARTを推したよ");

            if(haveSavedUser == false){
                // ユーザ登録がない場合の処理
                nickNameObject.SetActive(true);
                buttonStart.GetComponent<Button>().interactable = false;
                buttonCard.GetComponent<Button>().interactable = false;
                DisplayMessage("新規に登録するユーザ名を入力してください");
            }else{
                // ユーザ登録がある場合次のシーンへ
                SceneManager.LoadScene("LobbyScene");
            }
        }

        // 「登録」ボタン押下時の処理
        public void PushButtonRegister()
        {
            Debug.Log("登録を推したよ");

            if(string.IsNullOrEmpty(text.text)){
                DisplayMessage("ニックネームの入力が空です");
            }else if(haveSavedUser == false){
                StartCoroutine(UserRegisterSequence());
            }
        }

        // 「カード設定」ボタン押下時の処理
        public void PushButtonCard()
        {
            cardObject.SetActive(true);
            buttonStart.GetComponent<Button>().interactable = false;
            buttonCard.GetComponent<Button>().interactable = false;
        }

        // 「登録(カード入力画面)」ボタン押下時の処理
        public void PushButtonCardRgister()
        {

            NCMBObject obj = new NCMBObject("CardData");
            obj.Add("Main", cardField[9].text);
            obj.Add("SubA", cardField[1].text);
            obj.Add("SubB", cardField[2].text);
            obj.Add("SubC", cardField[3].text);
            obj.Add("SubD", cardField[4].text);
            obj.Add("SubE", cardField[5].text);
            obj.Add("SubF", cardField[6].text);
            obj.Add("SubG", cardField[7].text);
            obj.Add("SubH", cardField[8].text);

            //複数のNCMBObjectを取得するクエリを作成//
            NCMBQuery<NCMBObject> query = new NCMBQuery<NCMBObject>("CardData");

            query.FindAsync((List<NCMBObject> objList, NCMBException e) =>
            {
                if(e != null)
                {
                    //エラー処理
                }
                else
                {
                    //Debug.Log("カウント：" + objList.Count);
                    obj.Add("Number", (int)objList.Count);

                    obj.SaveAsync((NCMBException e) =>
                    {
                        if(e != null) {
                            //エラー結果
                            Debug.Log("保存失敗、通信環境を確認してください。");
                        }
                        else
                        {
                            //成功時の処理
                            Debug.Log("保存成功！");
                        }
                    });
                }
            });
        }

        // ローカルにユーザ登録する処理
        public void SaveLocalUserNameAndPassword(string userName, string password)
        {
            PlayerPrefs.SetString(PlayerPrefsKey.NCMB_USERNAME, userName);
            PlayerPrefs.SetString(PlayerPrefsKey.NCMB_PASSWORD, password);
            PlayerPrefs.Save();
        }

        // ローカルに登録されたユーザ名を取得
        public string LocalSavedUserName
        {
            get
            {
                Debug.Log("Debug Key:" + PlayerPrefs.HasKey(PlayerPrefsKey.NCMB_USERNAME));

                return PlayerPrefs.HasKey(PlayerPrefsKey.NCMB_USERNAME) ?
                    PlayerPrefs.GetString(PlayerPrefsKey.NCMB_USERNAME) : string.Empty;
            }
        }

        // Update is called once per frame
        void Update()
        {
           
        }

        ///////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////
        // ここからコルーチン処理 ///////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////

        public IEnumerator UserRegisterSequence()
        {
            bool isConnecting = true;
            NCMBUser user = new NCMBUser();

            string generatedPassword = Utility.GenerateRandomAlphanumeric(8, true);
            user.Password = generatedPassword;

                //ID重複の際に成功するまで繰り返す//
                //while (!isSuccess)
                //{
                string generatedUserName = Utility.GenerateRandomAlphanumeric(8, true);
                user.UserName = generatedUserName;

                user.Add(NCMBUserKey.NICKNAME, text.text);                   

                user.SignUpAsync((NCMBException e) =>
                {
                    if (e != null)
                    {
                    //userNameが衝突した場合は処理を繰り返すため、エラー終了しない//
                    if (e.ErrorCode != NCMBException.DUPPLICATION_ERROR)
                        {
                            //errorCallback(e);
                        }
                    }
                    else
                    {
                        //ログインに成功したら生成したID・パスをローカルに保存する//
                        Debug.Log("generatedUserName：" + generatedUserName);
                        Debug.Log("generatedUserName：" + generatedPassword);
                        Debug.Log("nicName：" + text.text);
                        nickNameObject.SetActive(false);
                        buttonStart.GetComponent<Button>().interactable = true;
                        haveSavedUser = true;
                        SaveLocalUserNameAndPassword(generatedUserName,generatedPassword);
                        DisplayMessage("ユーザ登録が完了しました");

                        NCMBUser.LogInAsync(generatedUserName, generatedPassword, (NCMBException e) =>
                        {
                            if (e != null)
                            {
                                Debug.Log(e);
                            }

                            Debug.Log("ログイン成功");

                            NCMBObject obj = new NCMBObject("PlayerData");
                            obj.Add(PlayerKey.NAME, NCMBUser.CurrentUser[NCMBUserKey.NICKNAME] as string);
                            obj.Add(PlayerKey.MEMBERID, NCMBUser.CurrentUser.ObjectId as string);
                            obj.Add(PlayerKey.PHASE, "Lobby");
                            obj.Add(PlayerKey.ORDER, "0");
                            obj.Add(PlayerKey.THEME, "0");
                            obj.Add(PlayerKey.VOTE_1, "");
                            obj.Add(PlayerKey.VOTE_2, "");
                            obj.Add(PlayerKey.VOTE_3, "");
                            obj.Add(PlayerKey.VOTE_4, "");
                            obj.Add(PlayerKey.VOTE_5, "");
                            obj.Add(PlayerKey.VOTE_6, "");
                            obj.Add(PlayerKey.VOTE_7, "");
                            obj.Add(PlayerKey.VOTE_8, "");
                            obj.Add(PlayerKey.ENTERROOM, "0000");
                            obj.Add(PlayerKey.ENTRYFLAG, false);
                            obj.Add(PlayerKey.VOTEFLAG, false);
                            obj.Add(PlayerKey.KICKFLAG, false);
                            obj.SaveAsync((NCMBException e) =>
                            {
                                if(e != null) {
                                    //エラー結果
                                    Debug.Log("保存失敗、通信環境を確認してください。");
                                }
                                else
                                {
                                    //成功時の処理
                                    Debug.Log("保存成功！");
                                }

                                isConnecting = false;
                            });
                        });
                    }
                });
            
            while (isConnecting) { yield return null; }
        }
    }
}
