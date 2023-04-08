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
    public class PlayManager : MonoBehaviour
    {
        public GameObject panelWalls;
        public GameObject buttonMemberObeject;
        public GameObject buttonMessage;
        public GameObject buttonMessageText;
        public GameObject buttonYourMessage;
        public GameObject buttonYourMessageText;
        public GameObject kadaiObject;
        public GameObject memberFlameObject;
        public GameObject kickObject;
        public GameObject[] ImagePerformer = new GameObject[PlayerKey.MAX_ENTRY_NUM];
        public GameObject[] ImageMaker = new GameObject[PlayerKey.MAX_ENTRY_NUM];
        public GameObject[] buttonPlayerName = new GameObject[PlayerKey.MAX_MEMBER_NUM];

        private static int turnNumber = 0;
        private static string selectPerform = "";
        private static bool isSelect;
        private static bool isPerformer;
        private static bool[] isSet = new bool[8] {false,false,false,false,false,false,false,false};

        private int entryNumber = 0;
        private string[] entryMemberArr = new string[PlayerKey.MAX_ENTRY_NUM];

        private string userName;
        private string password;
        private string nickName;
        private string myTheme;
        private string myOrder;

        //オブジェクトと結びつける
        public Text textNickname;
        public Text textRoomNumber;
        public Text textEntryNumber;

        private string roomPlayerStr;
        private string entryPlayerStr;
        private bool[] flagEntry = new bool[PlayerKey.MAX_MEMBER_NUM];
        public Text[] textPlayerName = new Text[PlayerKey.MAX_MEMBER_NUM];
        public Text textKickName;
        public Sprite[] nameplatePicture = new Sprite[3];
        public TextMeshProUGUI[] kadaiText = new TextMeshProUGUI[9];
        private bool flagStart = false;
        private int playerNumber = 0;
        private int cardNumber = 0;
        private int kickNumber = 0;
        private string roomNumber;

        public static TextMeshProUGUI[] playKadaiText = new TextMeshProUGUI[9];
        private bool isOwner = false;
        private bool isEnd = false;
        private bool isUp = false;

        // 右上歯車ボタン(共通)
        public GameObject hagurumaObject;
        public GameObject exitObject;
        public Text textExit;
        private bool isHaguruma = false;

        private float time = 0f;
        private DataManager dataManager;

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

        IEnumerator InitCoroutine(){
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

            // 初期アップデート(以降、毎フレームで呼ばれる)
            //yield return StartCoroutine(UpdateCoroutine());

            // データ取得完了すればボタンを有効化
            flagStart = true;
        }

        public void InitData(int session){

            if(session == InitDataKey.MINE){
                textNickname.GetComponent<Text>().text = NCMBUser.CurrentUser[NCMBUserKey.NICKNAME] as string;
                textRoomNumber.GetComponent<Text>().text = dataManager.myPlayer[0][PlayerKey.ENTERROOM] as string;
                myOrder = dataManager.myPlayer[0]["Order"] as string;
                myTheme = dataManager.myPlayer[0]["Theme"] as string;
                if(myTheme == "0"){
                    DisplayYourMessage("あなたは傍観者です");
                }else{
                    DisplayYourMessage("あなたの演目は[" + myTheme + "]です");
                }
            }
            else if(session == InitDataKey.ROOM){
                turnNumber = Int32.Parse(dataManager.RoomList[0]["TurnNumber"] as string);
                SetPerformerColor();
                if(turnNumber != 0){
                    SetMarker();
                }
                if(dataManager.myPlayer[0][PlayerKey.NAME] as string == dataManager.RoomList[0][RoomKey.OWNER] as string){
                    isOwner = true;
                }
            }
            else if(session == InitDataKey.PLAYER){
                playerNumber = dataManager.PlayerList.Count;
                roomPlayerStr = "";

                for(int i=0; i<PlayerKey.MAX_MEMBER_NUM; i++){
                    if(i < playerNumber){
                        buttonPlayerName[i].SetActive(true);
                        textPlayerName[i].text = dataManager.PlayerList[i]["Name"] as string;                          
                        if(roomPlayerStr != ""){
                            roomPlayerStr += ",";
                        }else{
                            roomPlayerStr += textPlayerName[i].text;
                        }                      
                    }
                }

                entryPlayerStr = dataManager.RoomList[0]["EntryMember"] as string;
                string[] entryPlayerArr =  entryPlayerStr.Split(',');
                entryNumber = entryPlayerArr.Length;
                textEntryNumber.GetComponent<Text>().text = entryPlayerArr.Length.ToString();
                //Debug.Log("entryNumber：" + entryNumber);

                //参加してるかどうかを付ける
                for(int i=0; i<PlayerKey.MAX_MEMBER_NUM; i++){
                    Button btn = buttonPlayerName[i].GetComponent<Button>();
                    flagEntry[i] = false;
                    btn.image.color = Color.white;

                    for(int j=0; j<entryNumber; j++){
                        //Debug.Log("textPlayerName：" + textPlayerName[i].text + ",entryPlayerArr：" + entryPlayerArr[j]);
                        if(textPlayerName[i].text == entryPlayerArr[j]){
                                flagEntry[i] = true;
                                btn.image.color = Color.yellow;

                                for(int k=0; k<dataManager.PlayerList.Count; k++){
                                    if(textPlayerName[i].text == dataManager.PlayerList[k]["Name"] as string){
                                        int number = Int32.Parse(dataManager.PlayerList[k]["Order"] as string);
                                        //Debug.Log("順番：" + number);
                                        entryMemberArr[number-1] = dataManager.PlayerList[k]["Name"] as string;

                                        if(number == 1){
                                            buttonPlayerName[i].transform.localPosition = new Vector3(-290.0f,170.0f,0.0f);
                                        }else if(number == 2){
                                            buttonPlayerName[i].transform.localPosition = new Vector3(-290.0f,40.0f,0.0f);
                                        }else if(number == 3){
                                            buttonPlayerName[i].transform.localPosition = new Vector3(-290.0f,-90.0f,0.0f);
                                        }else if(number == 4){
                                            buttonPlayerName[i].transform.localPosition = new Vector3(-290.0f,-220.0f,0.0f);
                                        }else if(number == 5){
                                            buttonPlayerName[i].transform.localPosition = new Vector3(290.0f,170.0f,0.0f);
                                        }else if(number == 6){
                                            buttonPlayerName[i].transform.localPosition = new Vector3(290.0f,40.0f,0.0f);
                                        }else if(number == 7){
                                            buttonPlayerName[i].transform.localPosition = new Vector3(290.0f,-90.0f,0.0f);
                                        }else if(number == 8){
                                            buttonPlayerName[i].transform.localPosition = new Vector3(290.0f,-220.0f,0.0f);
                                        }

                                        if(dataManager.PlayerList[k]["Name"] as string == dataManager.myPlayer[0]["Name"] as string){
                                        int myNumber = Int32.Parse(dataManager.myPlayer[0]["Order"] as string) -1;
                                        int mkNumber = 0;
                                        if(dataManager.myPlayer[0]["Theme"] as string == "A"){
                                            mkNumber = 0;
                                        }else if(dataManager.myPlayer[0]["Theme"] as string == "B"){
                                            mkNumber = 1;
                                        }else if(dataManager.myPlayer[0]["Theme"] as string == "C"){
                                            mkNumber = 2;
                                        }else if(dataManager.myPlayer[0]["Theme"] as string == "D"){
                                            mkNumber = 3;
                                        }else if(dataManager.myPlayer[0]["Theme"] as string == "E"){
                                            mkNumber = 4;
                                        }else if(dataManager.myPlayer[0]["Theme"] as string == "F"){
                                            mkNumber = 5;
                                        }else if(dataManager.myPlayer[0]["Theme"] as string == "G"){
                                            mkNumber = 6;
                                        }else if(dataManager.myPlayer[0]["Theme"] as string == "H"){
                                            mkNumber = 7;
                                        }
                                                                
                                        ImageMaker[mkNumber].transform.localPosition = new Vector3(ActPlate.POS_X[myNumber]-1000,ActPlate.POS_Y[myNumber]-1000,0.0f);
                                        ImageMaker[mkNumber].GetComponent<Image>().color = Color.red;
                                        isSet[mkNumber] = true;
                                    }
                                }
                            }
                        }
                    }
                }
                if(turnNumber >= entryNumber){
                    DisplayMessage("演技終了です！お疲れ様！");
                }else{
                    DisplayMessage(entryMemberArr[turnNumber] + "の演技ターンです");
                }
            }
        }

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

        private void SetPerformerColor()
        {
            for(int i=0; i<8; i++){
                if(i == turnNumber){
                    ImagePerformer[i].GetComponent<Image>().color = new Color32 (255, 180, 240, 255);
                }else{
                    ImagePerformer[i].GetComponent<Image>().color = new Color32 (255, 255, 255, 255);
                }
            }
        }

        private void SetMarker()
        {
            for(int i=0; i<turnNumber; i++){
                string VoteStr = "Vote_" + (i+1);

                if(dataManager.myPlayer[0][VoteStr] as string == "A"){
                    ImageMaker[0].transform.localPosition = new Vector3(ActPlate.POS_X[i]+170,ActPlate.POS_Y[i]+80,0.0f);
                    ImageMaker[0].GetComponent<Image>().color = Color.red;
                    isSet[0] = true;
                }else if(dataManager.myPlayer[0][VoteStr] as string == "B"){
                    ImageMaker[1].transform.localPosition = new Vector3(ActPlate.POS_X[i]+170,ActPlate.POS_Y[i]+80,0.0f);
                    ImageMaker[1].GetComponent<Image>().color = Color.red;
                    isSet[1] = true;
                }else if(dataManager.myPlayer[0][VoteStr] as string == "C"){
                    ImageMaker[2].transform.localPosition = new Vector3(ActPlate.POS_X[i]+170,ActPlate.POS_Y[i]+80,0.0f);
                    ImageMaker[2].GetComponent<Image>().color = Color.red;
                    isSet[2] = true;
                }else if(dataManager.myPlayer[0][VoteStr] as string == "D"){
                    ImageMaker[3].transform.localPosition = new Vector3(ActPlate.POS_X[i]+170,ActPlate.POS_Y[i]+80,0.0f);
                    ImageMaker[3].GetComponent<Image>().color = Color.red;
                    isSet[3] = true;
                }else if(dataManager.myPlayer[0][VoteStr] as string == "E"){
                    ImageMaker[4].transform.localPosition = new Vector3(ActPlate.POS_X[i]+170,ActPlate.POS_Y[i]+80,0.0f);
                    ImageMaker[4].GetComponent<Image>().color = Color.red;
                    isSet[4] = true;
                }else if(dataManager.myPlayer[0][VoteStr] as string == "F"){
                    ImageMaker[5].transform.localPosition = new Vector3(ActPlate.POS_X[i]+170,ActPlate.POS_Y[i]+80,0.0f);
                    ImageMaker[5].GetComponent<Image>().color = Color.red;
                    isSet[5] = true;
                }else if(dataManager.myPlayer[0][VoteStr] as string == "G"){
                    ImageMaker[6].transform.localPosition = new Vector3(ActPlate.POS_X[i]+170,ActPlate.POS_Y[i]+80,0.0f);
                    ImageMaker[6].GetComponent<Image>().color = Color.red;
                    isSet[6] = true;
                }else if(dataManager.myPlayer[0][VoteStr] as string == "H"){
                    ImageMaker[7].transform.localPosition = new Vector3(ActPlate.POS_X[i]+170,ActPlate.POS_Y[i]+80,0.0f);
                    ImageMaker[7].GetComponent<Image>().color = Color.red;
                    isSet[7] = true;
                }
            }
        }

        public static bool CheckMarkerSet(string name)
        {
            if(name == "ImageMarkerA" && isSet[0]){
                return true;
            }else if(name == "ImageMarkerB" && isSet[1]){
                return true;
            }else if(name == "ImageMarkerC" && isSet[2]){
                return true;
            }else if(name == "ImageMarkerD" && isSet[3]){
                return true;
            }else if(name == "ImageMarkerE" && isSet[4]){
                return true;
            }else if(name == "ImageMarkerF" && isSet[5]){
                return true;
            }else if(name == "ImageMarkerG" && isSet[6]){
                return true;
            }else if(name == "ImageMarkerH" && isSet[7]){
                return true;
            }
            return false;
        }

        public static bool CheckMarker(float x, float y, string name)
        {
            if(isSelect){
                Debug.Log(isSelect);
                return false;
            }
            if(isPerformer){
                Debug.Log(isPerformer);
                return false;
            }

            //Debug.Log("x,y = " + x + "," + y + "：" + name);
            for(int i=turnNumber; i<turnNumber+1; i++){
                if(ActPlate.POS_X[i] < x && x < ActPlate.POS_X[i] + ActPlate.WIDTH){
                    if(ActPlate.POS_Y[i] < y && y < ActPlate.POS_Y[i] + ActPlate.HEIGHT){
                        Debug.Log("今ボックス[" + i + "]の中にいる");
                        selectPerform = name.Replace("ImageMarker", "");
                        return true;
                    }
                }
            }

            return false;
        }


        public void PushButtonSkip()
        {
            isUp = false;
            memberFlameObject.transform.localPosition = new Vector3(0.0f,0.0f,0.0f);

            int num = 0;
            for(int i=0; i<PlayerKey.MAX_MEMBER_NUM; i++){
                if(!flagEntry[i]){
                    int x = num%4 + 1;
                    int y = num/4 + 1;
                    buttonPlayerName[i].transform.localPosition = new Vector3(0, -1000f, 0.0f);
                    num++;
                }
            }

            isSelect = false;
            StartCoroutine(SkipTurnCoroutine());
        }

        public void PushButtonShowMember()
        {
            if(!isUp){
                isUp = true;
                memberFlameObject.transform.localPosition = new Vector3(0.0f,320.0f,0.0f);

                int num = 0;
                for(int i=0; i<PlayerKey.MAX_MEMBER_NUM; i++){
                    if(!flagEntry[i]){
                        int x = num%4 + 1;
                        int y = num/4 + 1;
                        if(i < playerNumber){
                            if(System.Convert.ToBoolean(dataManager.PlayerList[i][PlayerKey.VOTEFLAG])){
                                buttonPlayerName[i].GetComponent<Image>().sprite = nameplatePicture[1];
                            }else{
                                buttonPlayerName[i].GetComponent<Image>().sprite = nameplatePicture[0];
                            }
                        }
                        buttonPlayerName[i].transform.localPosition = new Vector3(-440f + 180 * x, -320f - 60 * y,0.0f);
                        num++;
                    }
                }
            }else{
                isUp = false;
                memberFlameObject.transform.localPosition = new Vector3(0.0f,0.0f,0.0f);

                int num = 0;
                for(int i=0; i<PlayerKey.MAX_MEMBER_NUM; i++){
                    if(!flagEntry[i]){
                        int x = num%4 + 1;
                        int y = num/4 + 1;
                        buttonPlayerName[i].transform.localPosition = new Vector3(0, -1000f, 0.0f);
                        num++;
                    }
                }
            }
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

        public IEnumerator UpdateRoomMemberCoroutine()
        {
            yield return StartCoroutine(dataManager.SelectPlayerDataCoroutine());
            playerNumber = dataManager.PlayerList.Count;

            string voteMemberStr = ""; 
            string roomMemberStr = ""; 
            for(int i=0; i<playerNumber; i++){
                string voteStr = "Vote_" + (turnNumber+1);
                if(dataManager.PlayerList[i][voteStr] as string != ""){
                    if(voteMemberStr == ""){
                        voteMemberStr = dataManager.PlayerList[i][PlayerKey.NAME] as string;
                    }else{
                        voteMemberStr += "," + dataManager.PlayerList[i][PlayerKey.NAME] as string;
                    }   
                }

                if(roomMemberStr == ""){
                    roomMemberStr = dataManager.PlayerList[i][PlayerKey.NAME] as string;
                }else{
                    roomMemberStr += "," + dataManager.PlayerList[i][PlayerKey.NAME] as string;
                } 
            }

            dataManager.RoomList[0][RoomKey.ROOMMEMBER] = roomMemberStr;
            dataManager.RoomList[0][RoomKey.VOTEMEMBER] = voteMemberStr;
            yield return StartCoroutine(dataManager.UpdateRoomCoroutine());

            string[] voteMemberArr =  voteMemberStr.Split(',');
            //Debug.Log("turnNumber:" + turnNumber + ", voteMemberStr:" + voteMemberStr);
            //Debug.Log("playerNumber：" + playerNumber);
            //Debug.Log("voteMemberArr：" + voteMemberArr.Length + ", playerNumber：" + playerNumber);
            if(voteMemberStr == ""){
                //
            }else if((voteMemberArr.Length + 1) < playerNumber){
                //Debug.Log("voteMemberArr：" + voteMemberArr.Length + ", playerNumber：" + playerNumber);
            }else{
                isSelect = false;
                StartCoroutine(SkipTurnCoroutine());
            }      
        }

        public IEnumerator UpdateRoomInfoCoroutine()
        {
            yield return StartCoroutine(dataManager.SelectRoomDataCoroutine(dataManager.myPlayer[0][PlayerKey.ENTERROOM] as string));

            string[] voteMemberArr = (dataManager.RoomList[0][RoomKey.VOTEMEMBER] as string).Split(',');
            string[] roomMemberArr = (dataManager.RoomList[0][RoomKey.ROOMMEMBER] as string).Split(',');
            playerNumber = roomMemberArr.Length;
            
            for(int i=0; i<PlayerKey.MAX_MEMBER_NUM; i++){
                if(i < playerNumber){
                    buttonPlayerName[i].SetActive(true);
                    buttonPlayerName[i].GetComponent<Image>().sprite = nameplatePicture[0];
                    textPlayerName[i].text = roomMemberArr[i]; 

                    for(int j=0; j<entryMemberArr.Length; j++){
                        if(roomMemberArr[i] == entryMemberArr[j]){
                            if(entryMemberArr[turnNumber] == textPlayerName[i].text){
                                buttonPlayerName[i].GetComponent<Image>().sprite = nameplatePicture[2];
                            }
                        }
                    }

                    for(int k=0; k<voteMemberArr.Length; k++){
                        if(roomMemberArr[i] == voteMemberArr[k]){
                            buttonPlayerName[i].GetComponent<Image>().sprite = nameplatePicture[1];
                        }
                    }
                }else{
                    buttonPlayerName[i].SetActive(false);
                }
            }
              
            turnNumber = Int32.Parse(dataManager.RoomList[0]["TurnNumber"] as string);
            selectPerform = "";
            if(turnNumber < 8){ 
                if((turnNumber+1) == Int32.Parse(dataManager.myPlayer[0]["Order"] as string)){
                    isSelect = true;
                }else{
                    isSelect = false;
                }
                DisplayMessage(entryMemberArr[turnNumber] + "の演技ターンです");
                dataManager.RoomList[0]["Performer"] = entryMemberArr[turnNumber];
            }else{
                isSelect = false;
                DisplayMessage("演技終了です！お疲れ様！");
                dataManager.RoomList[0]["Performer"] = "END";
            }
            SetPerformerColor();

            for(int i=0; i<PlayerKey.MAX_MEMBER_NUM; i++){
                for(int j=0; j<entryMemberArr.Length; j++){
                    if(textPlayerName[i].text == entryMemberArr[j]){
                        if(entryMemberArr[turnNumber] == textPlayerName[i].text){
                            buttonPlayerName[i].GetComponent<Image>().sprite = nameplatePicture[2];
                        }
                    }
                }
            }

            if(!isOwner){
                //Debug.Log("RoomList_Phase：" + dataManager.RoomList[0]["Phase"]);
                if(dataManager.RoomList[0]["Phase"] as string == "RESULT"){
                    //Debug.Log("GO RESULT Scene!");
                    SceneManager.LoadScene("ResultScene");
                }
            }

        }

        public IEnumerator SkipTurnCoroutine()
        {
            turnNumber++;
            selectPerform = "";
            entryPlayerStr = dataManager.RoomList[0]["EntryMember"] as string;
            string[] entryPlayerArr =  entryPlayerStr.Split(',');
                    
            if(turnNumber < entryPlayerArr.Length){ 
                DisplayMessage(entryMemberArr[turnNumber] + "の演技ターンです");
                dataManager.RoomList[0]["Performer"] = entryMemberArr[turnNumber];
            }else{
                DisplayMessage("演技終了です！お疲れ様！");
                dataManager.RoomList[0]["Performer"] = "END";
                //turnNumber = 8;
                time = 0;
                isEnd = true;
            }

            SetPerformerColor();

            dataManager.RoomList[0]["TurnNumber"] = turnNumber.ToString();
            yield return StartCoroutine(dataManager.UpdateRoomCoroutine());      
        }

        // Update is called once per frame
        void Update()
        {
            if(flagStart){
                time+=Time.deltaTime;

                if(time>=3f){
                    time = 0f;

                    if(Int32.Parse(dataManager.myPlayer[0]["Order"] as string) == (turnNumber+1)){
                        isPerformer = true;
                    }else{
                        isPerformer = false;
                    }

                    StartCoroutine(UpdateCoroutine());

                    if(isEnd){
                        dataManager.RoomList[0]["Phase"] = "RESULT";
                        dataManager.RoomList[0].SaveAsync((NCMBException e) =>
                        {
                            SceneManager.LoadScene("ResultScene");
                        });
                    }
                }

                if(!isSelect && !string.IsNullOrEmpty(selectPerform)){

                    isSelect = true;

                    //自プレイヤーデータを取得するクエリを作成//
                    NCMBQuery<NCMBObject> queryMyPlayer  = new NCMBQuery<NCMBObject>("PlayerData");
                    queryMyPlayer.WhereEqualTo("MemberId", NCMBUser.CurrentUser.ObjectId as string);
                    queryMyPlayer.FindAsync((List<NCMBObject> _ncmbObjectList, NCMBException e) =>
                    {
                        if (e != null)
                        {
                            Debug.Log(e);
                        }
                        else
                        {
                            //Debug.Log("取得成功");
                            dataManager.myPlayer = _ncmbObjectList;
                            string voteStr = "Vote_" + (turnNumber+1);
                            dataManager.myPlayer[0][voteStr] = selectPerform;
                            dataManager.myPlayer[0][PlayerKey.VOTEFLAG] = true;
                            dataManager.myPlayer[0].SaveAsync((NCMBException e) =>
                            {
                                
                            });
                        }
                    });
                }
            }
        }

        public IEnumerator UpdateCoroutine(){
            if(isOwner){
                yield return StartCoroutine(UpdateRoomMemberCoroutine());
            }
            yield return StartCoroutine(UpdateRoomInfoCoroutine());
        }

        // プレイヤーのネームプレートを押したときの処理
        public void PushButtonPlayerName(int num)
        {
            kickNumber = num;
            textKickName.GetComponent<Text>().text = dataManager.PlayerList[num][PlayerKey.NAME] as string + "を";
            kickObject.SetActive(true);
        }
        public void PushButtonNo()
        {
            kickNumber = 0;
            kickObject.SetActive(false);
        }
        public void PushButtonYes()
        {
            StartCoroutine(PushButtonYesCoroutine());
        }
        public IEnumerator PushButtonYesCoroutine()
        {
            dataManager.PlayerList[kickNumber][PlayerKey.ENTERROOM] = "";
            dataManager.PlayerList[kickNumber][PlayerKey.PHASE] = PhaseKey.LOBBY_KICK;
            yield return dataManager.UpdatePlayerCoroutine(kickNumber);
            
            kickNumber = 0;
            kickObject.SetActive(false);
        }

        public void PushHagrumaButton()
        {
            if(!isHaguruma){
                isHaguruma = true;
                exitObject.SetActive(true);

                if(isOwner){
                    textExit.text = "部屋を壊す";
                }else{
                    textExit.text = "部屋から出る";
                }
            }else{
                isHaguruma = false;
                exitObject.SetActive(false);
            }
        }

        public void PushExitButton()
        {
            StartCoroutine(PushExitButtonCoroutine());
        }
        public IEnumerator PushExitButtonCoroutine()
        {
            if(isOwner){
                dataManager.myPlayer[0][PlayerKey.PHASE] = PhaseKey.LOBBY_DESTROY;
                dataManager.RoomList[0][RoomKey.DESTROYFLAG] = true; 
            }else{
                dataManager.myPlayer[0][PlayerKey.PHASE] = PhaseKey.LOBBY_EXIT;
            }
            dataManager.myPlayer[0][PlayerKey.ENTERROOM] = "";
            yield return StartCoroutine(dataManager.UpdateMyPlayerCoroutine());
            yield return StartCoroutine(dataManager.UpdateRoomCoroutine());

            // 部屋から出ることになるのでLOBBYシーンへ
            SceneManager.LoadScene("LobbyScene");
        }
    }
}
