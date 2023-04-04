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
    public class GameManager : MonoBehaviour
    {
        public GameObject panelWalls;
        public GameObject buttonCardSelect;
        public GameObject buttonMemberSelect;
        public GameObject buttonMemberDecide;
        public GameObject buttonMemberObeject;
        public GameObject buttonCardDecide;
        public GameObject buttonCardBack;
        public GameObject buttonGameStart;
        public GameObject buttonMessage;
        public GameObject buttonMessageText;
        public GameObject cardObject;
        public GameObject kadaiObject;
        public GameObject memberObject;
        public GameObject[] buttonPlayerName = new GameObject[PlayerKey.MAX_MEMBER_NUM];

        private int wallNo;
        private bool haveSavedUser;
        public static string userName;
        public static string password;
        public static string nickName;

        //オブジェクトと結びつける
        public Text textNickname;
        public Text textRoomNumber;
        public Text textEntryNumber;
        public Text textCardSelect;
        
        private string roomPlayerStr;
        private string entryPlayerStr;
        private string[] turnNumberArr = new string[8] {"A","B","C","D","E","F","G","H"};
        private bool[] flagEntry = new bool[20];
        public Text[] textPlayerName = new Text[20];
        public Sprite[] nameplatePicture = new Sprite[2];
        public TextMeshProUGUI[] cardText = new TextMeshProUGUI[9];
        public TextMeshProUGUI[] kadaiText = new TextMeshProUGUI[9];

        private bool isOwner = false;
        private bool flagStart = false;
        private bool flagCard = false;
        private bool flagMember = false;
        public int playerNumber = 0;
        public static string roomNumber;
        public static int cardNumber = 0;
        public static int entryNumber = 0;

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

            // データ取得完了までボタンを無効化
            memberObject.transform.position = new Vector3(0.0f,-2.1f,0.0f);
            buttonGameStart.SetActive(false);
            buttonCardSelect.GetComponent<Button>().interactable = false;
            buttonMemberSelect.GetComponent<Button>().interactable = false;

            // DBから各種データ取得処理
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
            InitData(InitDataKey.CARD);

            // 初期アップデート(以降、毎フレームで呼ばれる)
            yield return StartCoroutine(UpdateCoroutine());

            // データ取得完了すればボタンを有効化
            buttonCardSelect.GetComponent<Button>().interactable = true;
            if(isOwner){
                buttonMemberSelect.GetComponent<Button>().interactable = true;
            }
            SetMemberObjectPosition();
            flagStart = true;
        }

        // 画面下に文字を表示
        private void DisplayMessage(string mes)
        {
            buttonMessage.SetActive(true);
            buttonMessageText.GetComponent<Text>().text = mes;
        }

        //「カードを選ぶ」ボタンを押したときの処理
        public void PushbuttonCardSelect()
        {
            flagCard = true;
            buttonCardSelect.GetComponent<Button>().interactable = false;
            buttonMemberSelect.GetComponent<Button>().interactable = false;
            buttonGameStart.SetActive(false);
            kadaiObject.SetActive(false);
            cardObject.SetActive(true);
            if(!isOwner){
                buttonCardDecide.GetComponent<Button>().interactable = false;
            }else{
                buttonCardDecide.GetComponent<Button>().interactable = true;
            }
            if(isOwner){
                DisplayMessage("ゲームのお題となるカードを選んでください");
            }
        }

        // 「カード右矢印」ボタンを押したときの処理
        public void PushButtonCardRight()
        {
            cardNumber++;
            if(cardNumber >= dataManager.CardList.Count){
                cardNumber = 0;
            }
            SetCardText(cardNumber);
        }

        // 「カード左矢印」ボタンを押したときの処理
        public void PushButtonCardLeft()
        {
            cardNumber--;
            if(cardNumber < 0){
                cardNumber = dataManager.CardList.Count - 1;
            }
            SetCardText(cardNumber);
        }

        //「カードを戻る」ボタンを押したときの処理
        public void PushButtonCardBack()
        {
            flagCard = false;
            buttonCardSelect.GetComponent<Button>().interactable = true;
            if(isOwner){
                buttonMemberSelect.GetComponent<Button>().interactable = true;
            }

            kadaiObject.SetActive(true);
            cardObject.SetActive(false);
            buttonMessage.SetActive(false);
            CheckEntryMebmer();
        }

        //「カードを決定する」ボタンを押したときの処理
        public void PushButtonCardDecide()
        {
            flagCard = false;
            buttonCardSelect.GetComponent<Button>().interactable = true;
            if(isOwner){
                buttonMemberSelect.GetComponent<Button>().interactable = true;
            }
            kadaiObject.SetActive(true);
            cardObject.SetActive(false);
            buttonMessage.SetActive(false);
            SetKadiText(cardNumber);
            dataManager.RoomList[0][RoomKey.CARDNUMBER] = cardNumber;
            StartCoroutine(dataManager.UpdateRoomCoroutine());
            CheckEntryMebmer();
        }

        //「カードをランダムに選ぶ」ボタンを押したときの処理
        public void PushButtonCardRandom()
        {
            flagCard = false;
            buttonCardSelect.GetComponent<Button>().interactable = true;
            if(isOwner){
                buttonMemberSelect.GetComponent<Button>().interactable = true;
            }
            kadaiObject.SetActive(true);
            cardObject.SetActive(false);
            buttonMessage.SetActive(false);
            cardNumber = dataManager.CardList.Count;
            dataManager.RoomList[0][RoomKey.CARDNUMBER] = cardNumber;
            StartCoroutine(dataManager.UpdateRoomCoroutine());
            CheckEntryMebmer();
        }

        // プレイヤーのネームプレートを押したときの処理
        public void PushButtonPlayerName(int num)
        {
            if(flagMember){
                //Buttonコンポーネントを取得
                Button btn = buttonPlayerName[num].GetComponent<Button>();

                if(flagEntry[num]){
                    flagEntry[num] = false;
                    buttonPlayerName[num].GetComponent<Image>().sprite = nameplatePicture[0];
                    btn.image.color = Color.white;
                }else{
                    int player_num = 0;
                    for(int i=0; i<PlayerKey.MAX_MEMBER_NUM; i++){
                        if(flagEntry[i]){
                            player_num ++;
                        }
                    }

                    //Debug.Log("プレイヤー数：" + player_num);

                    if(player_num < PlayerKey.MAX_ENTRY_NUM){
                        flagEntry[num] = true;
                        buttonPlayerName[num].GetComponent<Image>().sprite = nameplatePicture[1];
                        btn.image.color = Color.yellow;
                    }
                }

                entryPlayerStr = "";
                for(int i=0; i<PlayerKey.MAX_MEMBER_NUM; i++){
                    if(flagEntry[i]){
                        if(entryPlayerStr != ""){
                            entryPlayerStr += ",";
                        }
                        entryPlayerStr += textPlayerName[i].text;
                    } 
                }
            }
        }

        // 「メンバーを選ぶ」ボタンを押した時の処理
        public void PushbuttonMemberSelect()
        {
            flagMember = true;
            buttonCardSelect.GetComponent<Button>().interactable = false;
            buttonMemberSelect.GetComponent<Button>().interactable = false;
            cardObject.SetActive(false);
            buttonMemberObeject.SetActive(true);
            buttonGameStart.SetActive(false);
            flagStart = false;
            Vector3 tmp = memberObject.transform.position;
            memberObject.transform.position = new Vector3(tmp.x, 5, tmp.z);
            DisplayMessage("ゲームに参加するメンバーを選んでください");
        }

        // 「メンバーを決定する」ボタンを押した時の処理
        public void PushbuttonMemberDecide()
        {
            flagMember = false;
            buttonCardSelect.GetComponent<Button>().interactable = true;
            buttonMemberSelect.GetComponent<Button>().interactable = true;
            kadaiObject.SetActive(true);
            buttonMemberObeject.SetActive(false);
            Vector3 tmp = memberObject.transform.position;
            SetMemberObjectPosition();
            buttonMessage.SetActive(false);
            buttonGameStart.SetActive(false);
            dataManager.RoomList[0][RoomKey.ENTRYMEMBER] = entryPlayerStr;
            dataManager.UpdateRoomCoroutine();
            StartCoroutine(UpdateCoroutine());
            CheckEntryMebmer();
        }

        //UnityAction<NCMBException> errorCallback
        public void PushButtonRoomSelect()
        {
            buttonCardSelect.GetComponent<Button>().interactable = false;
            buttonMemberSelect.GetComponent<Button>().interactable = false;
            kadaiObject.SetActive(true);
        }

        // 「ゲーム開始」ボタンを押したときの処理
        public void PushButtonGameStart()
        {
            StartCoroutine(PushButtonGameStartCoroutine());
        }
        public IEnumerator PushButtonGameStartCoroutine()
        {
            dataManager.RoomList[0][RoomKey.PHASE] = PhaseKey.PLAY;
            dataManager.RoomList[0][RoomKey.TURNNUMBER] = "0";
            if(cardNumber == dataManager.CardList.Count){
                cardNumber = UnityEngine.Random.Range(0, dataManager.CardList.Count);
                dataManager.RoomList[0][RoomKey.CARDNUMBER] = cardNumber; 
            }
            yield return StartCoroutine(dataManager.UpdateRoomCoroutine());

            for(int i=0; i<playerNumber; i++){
                dataManager.PlayerList[i][PlayerKey.ORDER]     = "0";
                dataManager.PlayerList[i][PlayerKey.THEME]     = "0";
                dataManager.PlayerList[i][PlayerKey.VOTE]      = "";
                dataManager.PlayerList[i][PlayerKey.VOTEFLAG]  = false;
                dataManager.PlayerList[i][PlayerKey.ENTRYFLAG] = false;
                yield return StartCoroutine(dataManager.UpdatePlayerCoroutine(i));
            }

            yield return StartCoroutine(UpdateCoroutine());

            entryPlayerStr = dataManager.RoomList[0]["EntryMember"] as string;
            string[] entryPlayerArr = new string[entryPlayerStr.Split(',').Length];
            entryPlayerArr = entryPlayerStr.Split(',');
                                    
            for(int i = 0; i < entryPlayerArr.Length; i++)
            {
                string temp = entryPlayerArr[i]; 
                int randomIndex = UnityEngine.Random.Range(0, entryPlayerArr.Length); 
                entryPlayerArr[i] = entryPlayerArr[randomIndex]; 
                entryPlayerArr[randomIndex] = temp; 
            }
            for (int i = 0; i < entryPlayerArr.Length; i++)
            {
                string temp = turnNumberArr[i]; 
                int randomIndex = UnityEngine.Random.Range(0, 7); 
                turnNumberArr[i] = turnNumberArr[randomIndex]; 
                turnNumberArr[randomIndex] = temp; 
            }

            yield return StartCoroutine(PlayerEntryCoroutine(entryPlayerArr));

            // PLAYシーンへ
            SceneManager.LoadScene("PlayScene");
        }                   

        // Update is called once per frame
        void Update()
        {
            if(flagStart){
                time+=Time.deltaTime;
                if(time>=3f){
                    time = 0f;
                    StartCoroutine(UpdateCoroutine());
                }
            }
        }

        // 随時アップデートされる処理をコルーチン化
        public IEnumerator UpdateCoroutine(){
            if(isOwner){
                yield return UpdateRoomMemberCoroutine();
                yield return UpdateRoomInfoCoroutine();
            }else{
                yield return UpdateRoomInfoCoroutine();
            }
        }
        
        ///////////////////////////////////////////////////////////////////////////////
        /// OTTHER  ///////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////

        // 初期化処理
        public void InitData(int session){

            if(session == InitDataKey.MINE){
                textNickname.GetComponent<Text>().text = NCMBUser.CurrentUser[NCMBUserKey.NICKNAME] as string;
                textRoomNumber.GetComponent<Text>().text = dataManager.myPlayer[0][PlayerKey.ENTERROOM] as string;
            }
            else if(session == InitDataKey.ROOM){
                if(dataManager.myPlayer[0][PlayerKey.NAME] as string == dataManager.RoomList[0][RoomKey.OWNER] as string){
                    isOwner = true;
                    CheckEntryMebmer();
                }
            }
            else if(session == InitDataKey.PLAYER){
                playerNumber = dataManager.PlayerList.Count;
                roomPlayerStr = "";

                for(int i=0; i<PlayerKey.MAX_MEMBER_NUM; i++){
                    if(i < playerNumber){
                        buttonPlayerName[i].SetActive(true);
                        //Debug.Log("PlayerList[" + i + "][NAME]：" + PlayerList[i]["Name"]);
                        textPlayerName[i].text = dataManager.PlayerList[i][PlayerKey.NAME] as string;
                                                    
                        if(roomPlayerStr != ""){
                            roomPlayerStr += ",";
                        }else{
                            roomPlayerStr += textPlayerName[i].text;
                        }                      
                    }else{
                        buttonPlayerName[i].SetActive(false);
                    }
                } 
            }
            else if(session == InitDataKey.CARD){
                cardNumber = Convert.ToInt32(dataManager.RoomList[0]["CardNumber"]);
                SetCardText(cardNumber);
                SetKadiText(cardNumber);

                buttonCardSelect.GetComponent<Button>().interactable = true;
                buttonMemberSelect.GetComponent<Button>().interactable = true;
                kadaiObject.SetActive(true);

                if(!isOwner){
                    buttonMemberSelect.GetComponent<Button>().interactable = false;
                    textCardSelect.GetComponent<Text>().text = "お題一覧を見る";
                }
            }
        }

        // カード選択時のカード内容を設定
        private void SetCardText(int number)
        {
            if(number == dataManager.CardList.Count){
                cardText[0].text = "？？";
                cardText[1].text = "？？";
                cardText[2].text = "？？";
                cardText[3].text = "？？";
                cardText[4].text = "？？";
                cardText[5].text = "？？";
                cardText[6].text = "？？";
                cardText[7].text = "？？";
                cardText[8].text = "？？";
            }else{
                cardText[0].text = dataManager.CardList[number]["Main"].ToString();
                cardText[1].text = dataManager.CardList[number]["SubA"].ToString();
                cardText[2].text = dataManager.CardList[number]["SubB"].ToString();
                cardText[3].text = dataManager.CardList[number]["SubC"].ToString();
                cardText[4].text = dataManager.CardList[number]["SubD"].ToString();
                cardText[5].text = dataManager.CardList[number]["SubE"].ToString();
                cardText[6].text = dataManager.CardList[number]["SubF"].ToString();
                cardText[7].text = dataManager.CardList[number]["SubG"].ToString();
                cardText[8].text = dataManager.CardList[number]["SubH"].ToString();
            }
        }

        // 画面真ん中に配置される課題カード内容を設定
        private void SetKadiText(int number)
        {
            if(number == dataManager.CardList.Count){
                kadaiText[0].text = "？？";
                kadaiText[1].text = "？？";
                kadaiText[2].text = "？？";
                kadaiText[3].text = "？？";
                kadaiText[4].text = "？？";
                kadaiText[5].text = "？？";
                kadaiText[6].text = "？？";
                kadaiText[7].text = "？？";
                kadaiText[8].text = "？？";
            }else{
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
        }

        // 現状のエントリーメンバーを更新し、2名以上なら「ゲーム開始」ボタンを有効にする
        private void CheckEntryMebmer()
        {
            if(isOwner){
                buttonGameStart.SetActive(true);
            }
            entryPlayerStr = dataManager.RoomList[0]["EntryMember"] as string;
            string[] entryPlayerArr = new string[entryPlayerStr.Split(',').Length];
            entryPlayerArr = entryPlayerStr.Split(',');
            if(entryPlayerArr.Length >= 2){
                buttonGameStart.GetComponent<Button>().interactable = true;
            }else{
                buttonGameStart.GetComponent<Button>().interactable = false;
            }
        }

        // メンバーの枠位置を更新
        private void SetMemberObjectPosition(){
            int numa = (playerNumber +3) / 4;
            memberObject.transform.position = new Vector3(0.0f,-2.1f + (0.45f * numa),0.0f);
        }

        ///////////////////////////////////////////////////////////////////////////////
        /// ここからコルーチン処理
        ///////////////////////////////////////////////////////////////////////////////

        // 全プレイヤーの情報を取得し、部屋メンバー情報を更新 (Owner用)
        public IEnumerator UpdateRoomMemberCoroutine()
        {
            //Debug.Log("SelectPlayerDataCoroutine Before");
            yield return dataManager.SelectPlayerDataCoroutine();
            //Debug.Log("SelectPlayerDataCoroutine After");
            playerNumber = dataManager.PlayerList.Count;

            roomPlayerStr = "";
            //Debug.Log("プレイヤー人数：" + playerNumber);
            for(int i=0; i<PlayerKey.MAX_MEMBER_NUM;i++){

                if(i < playerNumber){
                    buttonPlayerName[i].SetActive(true);
                    //Debug.Log("PlayerName[" + i + "]：" + dataManager.PlayerList[i]["Name"] as string);
                    textPlayerName[i].text = dataManager.PlayerList[i][PlayerKey.NAME] as string;
                                                    
                    if(roomPlayerStr != ""){
                        roomPlayerStr += ",";
                    }
                            
                    roomPlayerStr += textPlayerName[i].text;                      
                }else{
                    buttonPlayerName[i].SetActive(false);
                }
            }
            dataManager.RoomList[0][RoomKey.ROOMMEMBER] = roomPlayerStr;

            //Debug.Log("UpdateRoomCoroutine Before");
            yield return StartCoroutine(dataManager.UpdateRoomCoroutine());
            //Debug.Log("UpdateRoomCoroutine After");

            //Debug.Log("UpdateRoomMemberCoroutine End");
        }

        // 部屋メンバー情報をルームデータから取得
        public IEnumerator UpdateRoomInfoCoroutine()
        {
            Debug.Log("UpdateRoomInfoCoroutine Before");
            yield return StartCoroutine(dataManager.SelectRoomDataCoroutine(dataManager.myPlayer[0][PlayerKey.ENTERROOM] as string));
            Debug.Log("UpdateRoomInfoCoroutine After");

            entryPlayerStr = dataManager.RoomList[0][RoomKey.ENTRYMEMBER] as string;

            string[] entryPlayerArr = new string[PlayerKey.MAX_MEMBER_NUM];
            int length;
            if(entryPlayerStr == "" || entryPlayerStr == null)
            {
                Array.Clear(entryPlayerArr, 0, entryPlayerArr.Length);
                length = 0;
                textEntryNumber.GetComponent<Text>().text = "0";
            }
            else
            {
                //Debug.Log("entryPlayerStr：" + entryPlayerStr);
                entryPlayerArr =  entryPlayerStr.Split(',');
                length = entryPlayerArr.Length;
                textEntryNumber.GetComponent<Text>().text = entryPlayerArr.Length.ToString();
            }

            for(int i=0; i<playerNumber; i++){
                Button btn = buttonPlayerName[i].GetComponent<Button>();
                flagEntry[i] = false;
                textPlayerName[i].text = dataManager.PlayerList[i][PlayerKey.NAME] as string;
                btn.image.color = Color.white;

                for(int j=0; j<entryPlayerArr.Length; j++){
                    if(textPlayerName[i].text == entryPlayerArr[j]){
                        flagEntry[i] = true;
                        buttonPlayerName[i].GetComponent<Image>().sprite = nameplatePicture[1];
                        btn.image.color = Color.yellow;
                    }
                }
            }

            if(!flagCard){
                cardNumber = Convert.ToInt32(dataManager.RoomList[0]["CardNumber"]);
                SetCardText(cardNumber);
                SetKadiText(cardNumber);
            }

            if(!isOwner){
                //Debug.Log("RoomList_Phase：" + dataManager.RoomList[0]["Phase"]);
                if(dataManager.RoomList[0][RoomKey.PHASE] as string == PhaseKey.PLAY){
                    //Debug.Log("GO PLAY Scene!");
                    SceneManager.LoadScene("PlayScene");
                }
            }

            Debug.Log("UpdateRoomInfoCoroutine End");
        }

        // メンバー決定画面で各メンバープレートを押したときの処理
        public IEnumerator PlayerEntryCoroutine(string[] entryPlayerArr)
        {
            int number = 0;

            for (int i = 0; i < entryPlayerArr.Length; i++){
                for(int j=0; j<playerNumber; j++){
                    if(entryPlayerArr[i] == dataManager.PlayerList[j][PlayerKey.NAME] as string){
                        //Debug.Log("player[" + i + "]：" + entryPlayerArr[i]);
                        dataManager.PlayerList[j][PlayerKey.ORDER] = (i+1).ToString();
                        if(i==0){
                            dataManager.PlayerList[j][PlayerKey.VOTEFLAG] = true;
                        }
                        dataManager.PlayerList[j][PlayerKey.THEME] = turnNumberArr[i];
                        dataManager.PlayerList[j][PlayerKey.PHASE] = PhaseKey.PLAY;
                        dataManager.PlayerList[j][PlayerKey.ENTRYFLAG] = true;
                        yield return StartCoroutine(dataManager.UpdatePlayerCoroutine(j));
                        number++;
                    }
                }
            }

            while (number < entryPlayerArr.Length) { yield return null; }
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
