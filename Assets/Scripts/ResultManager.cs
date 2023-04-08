using NCMB;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using TMPro;

namespace Communication
{
    public class ResultManager : MonoBehaviour
    {
        public GameObject buttonGameEnd;
        public GameObject buttonMessage;
        public GameObject buttonMessageText;
        public GameObject buttonYourMessage;
        public GameObject buttonYourMessageText;
        public GameObject buttonCorrectMessage;
        public GameObject buttonCorrectMessageText;
        public GameObject buttonResult;
        public GameObject gameEndObject;
        public GameObject kadaiObject;
        public GameObject answerObject;
        public GameObject correctObject;
        public GameObject memberObject;
        public GameObject flameObject;
        public GameObject[] buttonPlayerName = new GameObject[PlayerKey.MAX_MEMBER_NUM];
        public GameObject[] buttonRsultName = new GameObject[PlayerKey.MAX_ENTRY_NUM];
        public GameObject[] imageMarker = new GameObject[PlayerKey.MAX_ENTRY_NUM];
        public Text[] textMarker = new Text[PlayerKey.MAX_ENTRY_NUM];

        private string[] entryMemberArr = new string[PlayerKey.MAX_ENTRY_NUM];
        private string[] actNumberArr = new string[PlayerKey.MAX_ENTRY_NUM];

        private string userName;
        private string password;
        private string nickName;

        private string roomPlayerStr;
        private string entryPlayerStr;
        private string myTheme;
        private string myOrder;
        private bool flagResult = false;
        public Text textStatus;
        public Text[] textPlayerName = new Text[PlayerKey.MAX_MEMBER_NUM];
        public Text[] textResultName = new Text[PlayerKey.MAX_ENTRY_NUM];
        public Text[] textPlayerRate = new Text[PlayerKey.MAX_ENTRY_NUM];
        public Text textMyRate;
        public Text textOurRate;
        public Text[] textPlayerVote = new Text[PlayerKey.MAX_MEMBER_NUM];
        private bool[] isOpen = new bool[PlayerKey.MAX_ENTRY_NUM];
        private int[] rotate = new int[8]{0,0,0,0,0,0,0,0};
        public TextMeshProUGUI[] kadaiText = new TextMeshProUGUI[9];
        private bool flagStart = false;
        private int playerNumber = 0;
        private int entryNumber = 0;
        private int cardNumber = 0;
        private int showNumber = 0;
        private string roomNumber;

        private int[] correctNumber = new int[8]{0,0,0,0,0,0,0,0};
        private int[] inCorrectNumber = new int[8]{0,0,0,0,0,0,0,0};

        private string beforePerformer;
        private string afterPerformer;
        private string correctStr = "";

        private List<string> fromList = new List<string>();
        private List<string> toList = new List<string>();
        private List<bool> checkList = new List<bool>();
        
        private float time = 0f;
        private int backTimer=0;
        private DataManager dataManager;
        public static TextMeshProUGUI[] playKadaiText = new TextMeshProUGUI[9];
        private string[] entryPlayerArr;
        private bool isOwner = false;
        private bool isResult = false;
        private bool isEnd = false;

        // Start is called before the first frame update
        void Start()
        {
            // DBアクセス用クラス宣言
            dataManager = new DataManager();

            // ローカルからプレイヤー情報取得
            userName = PlayerPrefs.GetString(PlayerPrefsKey.NCMB_USERNAME);
            password = PlayerPrefs.GetString(PlayerPrefsKey.NCMB_PASSWORD);
           // DBから各種データ取得処理
            //memberFlameObject.transform.localPosition = new Vector3(0.0f,320.0f,0.0f);
            StartCoroutine(InitCoroutine());
        }

        private IEnumerator InitCoroutine(){
            // Login & Myプレイヤーデータ取得
            yield return StartCoroutine(dataManager.LoginAndSelectMyPlayerCoroutine(userName,password));
            InitData(InitDataKey.MINE);

            // RoomData取得
            yield return StartCoroutine(dataManager.SelectRoomDataCoroutine(dataManager.myPlayer[0][PlayerKey.ENTERROOM]  as string));
            InitData(InitDataKey.ROOM);
            

            // 全プレイヤーデータ取得
            yield return StartCoroutine(dataManager.SelectPlayerDataCoroutine());
            InitData(InitDataKey.PLAYER);

            // 全カードデータ取得
            yield return StartCoroutine(dataManager.SelectCardDataCoroutine());
            cardNumber = Convert.ToInt32(dataManager.RoomList[0]["CardNumber"]);
            SetKadiText(cardNumber);

            //if(isOwner){

                // マスターデータ・関係性データ取得
                yield return StartCoroutine(dataManager.SelectMasterDataCoroutine());
                yield return StartCoroutine(dataManager.SelectRelationDataCoroutine());

                int playNumber = Int32.Parse(dataManager.MasterList[0]["PlayNumber"] as string);
                playNumber++;

                string[] correctUser = new string[8]{"","","","","","","",""};
                string[] inCorrectUser = new string[8]{"","","","","","","",""};
                int myCorrect = 0;
                int myInCorrect = 0; 

                for(int i=0; i<playerNumber; i++){
                    string[] answer = new string[entryNumber];
                    bool isMine = false;

                    if(dataManager.PlayerList[i][PlayerKey.NAME] as string == dataManager.myPlayer[0][PlayerKey.NAME] as string){
                        isMine = true;
                    }

                    for(int j=0; j<entryNumber; j++){
                        string voteStr = "Vote_" + (j+1);
                        answer[j] = dataManager.PlayerList[i][voteStr] as string;

                        if(answer[j] == ""){
                                            
                        }else if(answer[j] == actNumberArr[j]){
                            fromList.Add(dataManager.PlayerList[i][PlayerKey.NAME] as string);
                            toList.Add(entryMemberArr[j]);
                            checkList.Add(true);

                            if(correctUser[j] == ""){
                                correctNumber[j]++;
                                correctUser[j] = dataManager.PlayerList[i][PlayerKey.NAME] as string;
                            }else{
                                correctNumber[j]++;
                                correctUser[j] += "," + dataManager.PlayerList[i][PlayerKey.NAME] as string;
                            }

                            if(isMine){
                                myCorrect++;
                            }
                        }else{
                            fromList.Add(dataManager.PlayerList[i][PlayerKey.NAME] as string);
                            toList.Add(entryMemberArr[j]);
                            checkList.Add(false);

                            if(inCorrectUser[j] == ""){
                                inCorrectNumber[j]++;
                                inCorrectUser[j] = dataManager.PlayerList[i][PlayerKey.NAME] as string;
                            }else{
                                inCorrectNumber[j]++;
                                inCorrectUser[j] += "," + dataManager.PlayerList[i][PlayerKey.NAME] as string;
                            }

                            if(isMine){
                                myInCorrect++;
                            }
                        }
                    }
                }

            textMyRate.GetComponent<Text>().text = (myCorrect * 100 / (myCorrect + myInCorrect)).ToString() + "%";

            if(isOwner){
                for(int i=0; i<entryNumber; i++){

                    string tex = "Sub" + actNumberArr[i];
                    NCMBObject obj = new NCMBObject("AnswerData");
                    obj.Add("Name", entryMemberArr[i]);
                    obj.Add("PlayNumber", playNumber);
                    obj.Add("Kadai", dataManager.CardList[cardNumber][tex]);
                    obj.Add("CorrectUser", correctUser[i]);
                    obj.Add("InCorrectUser", inCorrectUser[i]);
                    obj.Add("CorrectRate", correctNumber[i] * 100 / (correctNumber[i] + inCorrectNumber[i]));
                    obj.SaveAsync((NCMBException e) =>
                    {
                            
                    });                                
                }

                
                for(int i=0; i<fromList.Count; i++){
                    bool isInsert = true;
                    for(int j=0; j<dataManager.RelationList.Count; j++){
                        if(fromList[i] == dataManager.RelationList[j]["From"] as string){
                            if(toList[i] == dataManager.RelationList[j]["To"] as string){
                                isInsert = false;
                                yield return StartCoroutine(dataManager.UpdateRelationCoroutine(j, checkList[i]));
                                break;
                            }
                        }
                    }

                    if(isInsert){
                        yield return StartCoroutine(dataManager.InsertRelationDataCoroutine(fromList[i], toList[i], checkList[i]));
                    }
                }           

                dataManager.MasterList[0]["PlayNumber"] = playNumber.ToString();
                yield return StartCoroutine(dataManager.UpdateMasterCoroutine());
            }                      
            //}

            // データ取得完了すればボタンを有効化
            flagStart = true;
        }

        public void InitData(int session){

            if(session == InitDataKey.MINE){
                //textNickname.GetComponent<Text>().text = NCMBUser.CurrentUser[NCMBUserKey.NICKNAME] as string;
                //textRoomNumber.GetComponent<Text>().text = dataManager.dataManager.myPlayer[0][PlayerKey.ENTERROOM] as string;
            }
            else if(session == InitDataKey.ROOM){

                // 部屋主かどうかを判定
                if(dataManager.myPlayer[0][PlayerKey.NAME] as string == dataManager.RoomList[0][RoomKey.OWNER] as string){
                    isOwner = true;
                }                

                // 演技者を配列に格納
                entryPlayerStr = dataManager.RoomList[0]["EntryMember"] as string;
                entryPlayerArr =  entryPlayerStr.Split(',');
                entryNumber = entryPlayerArr.Length;
                
                // 自分の投票結果をテキストに入力
                string a = "あなたの投票：";
                for(int j=0; j<entryNumber; j++){
                    string voteStr = "Vote_" + (j+1);
                    if(dataManager.myPlayer[0][voteStr] == ""){
                        a += " ×";
                    }else{
                        a += " " + dataManager.myPlayer[0][voteStr];
                    }
                }

                DisplayYourMessage(a);
            }
            else if(session == InitDataKey.PLAYER){
                playerNumber = dataManager.PlayerList.Count;

                for(int i=0; i<PlayerKey.MAX_MEMBER_NUM;i++){
                    if(i < playerNumber){
                        buttonPlayerName[i].SetActive(true);
                        textPlayerName[i].text = dataManager.PlayerList[i]["Name"] as string;
                                                
                        if(roomPlayerStr != ""){
                            roomPlayerStr += ",";
                        }else{
                            roomPlayerStr += textPlayerName[i].text;
                        }                      
                    }else{
                        buttonPlayerName[i].SetActive(false);
                    }
                }

                //参加してるかどうかを付ける
                for(int i=0; i<PlayerKey.MAX_MEMBER_NUM; i++){
                                            
                    buttonPlayerName[i].SetActive(false);
                    Button btn = buttonPlayerName[i].GetComponent<Button>();
                    btn.image.color = Color.white;

                    for(int j=0; j<entryPlayerArr.Length; j++){
                        if(textPlayerName[i].text == entryPlayerArr[j]){
                            for(int k=0; k<dataManager.PlayerList.Count; k++){
                                if(textPlayerName[i].text == dataManager.PlayerList[k]["Name"] as string){
                                    int number = Int32.Parse(dataManager.PlayerList[k]["Order"] as string);
                                    //Debug.Log("順番：" + number);
                                    Debug.Log("dataManager.PlayerList[Theme]：" + dataManager.PlayerList[k]["Theme"]);
                                    Debug.Log("[number-1]：" + (number-1));
                                    entryMemberArr[number-1] = dataManager.PlayerList[k]["Name"] as string;
                                    actNumberArr[number-1] = dataManager.PlayerList[k]["Theme"] as string;
                                }
                            }
                        }
                    }
                }

                for(int i=0; i<dataManager.PlayerList.Count; i++){
                    for(int j=0; j<entryNumber; j++){
                        if(dataManager.PlayerList[i][PlayerKey.NAME] as string == entryMemberArr[j]){
                            buttonPlayerName[i].SetActive(true);
                            if(j<4){
                                buttonPlayerName[i].transform.localPosition = new Vector3(-273.0f,215.0f-(130*j),0.0f);
                            }else if(j<8){
                                buttonPlayerName[i].transform.localPosition = new Vector3(312.0f,215.0f-(130*(j-4)),0.0f);
                            }
                        }
                    }
                }

                for(int i=0; i<PlayerKey.MAX_ENTRY_NUM; i++){
                    if(i < entryNumber){
                        buttonRsultName[i].SetActive(true);
                    }else{
                        buttonRsultName[i].SetActive(false);
                    }
                }
                

                for(int i=0; i<entryNumber; i++){
                    if(actNumberArr[i] == "A"){
                        imageMarker[0].transform.localPosition = new Vector3(ActPlate.POS_X[i]+100,ActPlate.POS_Y[i]+30,0.0f);
                        imageMarker[0].GetComponent<Image>().color = Color.yellow;
                        textMarker[0].GetComponent<Text>().text = "";
                    }else if(actNumberArr[i] == "B"){
                        imageMarker[1].transform.localPosition = new Vector3(ActPlate.POS_X[i]+100,ActPlate.POS_Y[i]+30,0.0f);
                        imageMarker[1].GetComponent<Image>().color = Color.yellow;
                        textMarker[1].GetComponent<Text>().text = "";
                    }else if(actNumberArr[i] == "C"){
                        imageMarker[2].transform.localPosition = new Vector3(ActPlate.POS_X[i]+100,ActPlate.POS_Y[i]+30,0.0f);
                        imageMarker[2].GetComponent<Image>().color = Color.yellow;
                        textMarker[2].GetComponent<Text>().text = "";
                    }else if(actNumberArr[i] == "D"){
                        imageMarker[3].transform.localPosition = new Vector3(ActPlate.POS_X[i]+100,ActPlate.POS_Y[i]+30,0.0f);
                        imageMarker[3].GetComponent<Image>().color = Color.yellow;
                        textMarker[3].GetComponent<Text>().text = "";
                    }else if(actNumberArr[i] == "E"){
                        imageMarker[4].transform.localPosition = new Vector3(ActPlate.POS_X[i]+100,ActPlate.POS_Y[i]+30,0.0f);
                        imageMarker[4].GetComponent<Image>().color = Color.yellow;
                        textMarker[4].GetComponent<Text>().text = "";
                    }else if(actNumberArr[i] == "F"){
                        imageMarker[5].transform.localPosition = new Vector3(ActPlate.POS_X[i]+100,ActPlate.POS_Y[i]+30,0.0f);
                        imageMarker[5].GetComponent<Image>().color = Color.yellow;
                        textMarker[5].GetComponent<Text>().text = "";
                    }else if(actNumberArr[i] == "G"){
                        imageMarker[6].transform.localPosition = new Vector3(ActPlate.POS_X[i]+100,ActPlate.POS_Y[i]+30,0.0f);
                        imageMarker[6].GetComponent<Image>().color = Color.yellow;
                        textMarker[6].GetComponent<Text>().text = "";
                    }else if(actNumberArr[i] == "H"){
                        imageMarker[7].transform.localPosition = new Vector3(ActPlate.POS_X[i]+100,ActPlate.POS_Y[i]+30,0.0f);
                        imageMarker[7].GetComponent<Image>().color = Color.yellow;
                        textMarker[7].GetComponent<Text>().text = "";
                    }
                }
            }
        }

        // 
        private void SetKadiText(int number)
        {
            kadaiText[0].text = dataManager.CardList[number]["Main"].ToString();
            kadaiText[1].text = dataManager.CardList[number]["SubA"].ToString();
            kadaiText[2].text = dataManager.CardList[number]["SubB"].ToString();
            kadaiText[3].text = dataManager.CardList[number]["SubC"].ToString();
            kadaiText[4].text = dataManager.CardList[number]["SubD"].ToString();
            kadaiText[5].text = dataManager.CardList[number]["SubE"].ToString();
            kadaiText[6].text = dataManager.CardList[number]["SubF"].ToString();
            kadaiText[7].text = dataManager.CardList[number]["SubG"].ToString();
            kadaiText[8].text = dataManager.CardList[number]["SubH"].ToString();
        }

        private void DisplayMessage(string mes)
        {
            buttonMessage.SetActive(true);
            buttonMessageText.GetComponent<Text>().text = mes;
        }

        private void DisplayYourMessage(string mes)
        {
            buttonYourMessage.SetActive(true);
            buttonYourMessageText.GetComponent<Text>().text = mes;
        }

        private void DisplayCorrectMessage(string mes)
        {
            buttonCorrectMessage.SetActive(true);
            buttonCorrectMessageText.GetComponent<Text>().text = mes;
        }

        public void pushButtonMarker(int i)
        {
            isOpen[i] = true;
        }

        public void PushButtonResult()
        {
            gameEndObject.SetActive(true);
            for(int i=0; i<entryNumber; i++){
                textResultName[i].GetComponent<Text>().text = entryMemberArr[i];
                textPlayerRate[i].GetComponent<Text>().text = correctNumber[i] + "/" + (correctNumber[i] + inCorrectNumber[i]);

                if(entryMemberArr[i] == dataManager.myPlayer[0][PlayerKey.NAME] as string){
                    textOurRate.GetComponent<Text>().text = (correctNumber[i] * 100 / (correctNumber[i] + inCorrectNumber[i])).ToString()  + "%";
                }
            }
        }

        public void PushButtonGameEnd()
        {
            GameEndCoroutine();
            SceneManager.LoadScene("GameScene");
        }

        private IEnumerator GameEndCoroutine()
        {
            if(isOwner){
                for(int i=0; i<playerNumber; i++){
                    yield return StartCoroutine(dataManager.InitPlayerCoroutine(i,dataManager.PlayerList[i][PlayerKey.ENTERROOM] as string));
                }
            }

            dataManager.RoomList[0]["TurnNumber"] = "0";
            dataManager.RoomList[0]["Performer"] = "";
            dataManager.RoomList[0]["Phase"] = "GAME";
            yield return StartCoroutine(dataManager.UpdateRoomCoroutine());
        }

        // Update is called once per frame
        void Update()
        {
            if(flagStart){
                time+=Time.deltaTime;

                if(!flagResult){
                    if(time >= 2.0f){
                        flagResult = true;
                    }else if(time >= 1.5f){
                        DisplayMessage("タップして正解を確認！");
                    }else if(time >= 1.0f){
                        textStatus.GetComponent<Text>().text = "結果発表";
                    }else if(time >= 0.75f){
                        textStatus.GetComponent<Text>().text = "結果発";
                    }else if(time >= 0.5f){
                        textStatus.GetComponent<Text>().text = "結果";
                    }else if(time >= 0.25f){
                        textStatus.GetComponent<Text>().text = "結";
                    }
                }

                int openNumber = 0;

                for(int i=0; i<PlayerKey.MAX_ENTRY_NUM; i++){
                    if(isOpen[i]){     
                        rotate[i]+=1;
                        if(rotate[i] == 90){
                            //Debug.Log("actNumberArr[i]:" + actNumberArr[i]);
                            string str = "";
                            if(i==0) str = "A";
                            else if(i==1) str = "B";
                            else if(i==2) str = "C";
                            else if(i==3) str = "D";
                            else if(i==4) str = "E";
                            else if(i==5) str = "F";
                            else if(i==6) str = "G";
                            else if(i==7) str = "H";

                            textMarker[i].GetComponent<Text>().text = str;
                            imageMarker[i].transform.Rotate(0,180,0);
                        }else if(rotate[i] <= 180){
                            imageMarker[i].transform.Rotate(0,1,0);
                        }
                        openNumber++;
                    }
                }

                if(openNumber == entryNumber){
                    if(!isResult){
                        isResult = true;
                        buttonResult.SetActive(true);
                        DisplayMessage("成績画面へ => ");
                    }
                }
            }
        }
    }
}
