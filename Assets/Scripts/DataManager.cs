using NCMB;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// NCMBObject操作のコルーチンラッパークラス//
/// </summary>

namespace Communication
{

    public class DataManager : MonoBehaviour
    {
        // 共通オブジェクト格納リスト
        public List<NCMBObject> myPlayer = new List<NCMBObject>();
        public List<NCMBObject> PlayerList = new List<NCMBObject>();
        public List<NCMBObject> RoomList = new List<NCMBObject>();
        public List<NCMBObject> CardList = new List<NCMBObject>();
        public List<NCMBObject> RelationList = new List<NCMBObject>();
        public List<NCMBObject> RelationFromList = new List<NCMBObject>();
        public List<NCMBObject> RelationToList = new List<NCMBObject>();
        public List<NCMBObject> AnswerList = new List<NCMBObject>();
        public List<NCMBObject> MasterList = new List<NCMBObject>();

        // 右上歯車ボタン(共通)
        public GameObject hagurumaObject;
        public GameObject exitObject;
        public Text textExit;
        private bool isHaguruma = false;
        private int backNumber;

        // 背景用
        private int timer = 0;
        private int angle = 0;
        private int[] vec_x = new int[10];
        private int[] vec_y = new int[10];        
        //public Text textTimer;
        public GameObject backImage;
        public GameObject backImage2;
        public GameObject[] backChara = new GameObject[10];
        public Sprite[] backCharaPicture = new Sprite[22];

        ///////////////////////////////////////////////////////////////////////////////////////
        // SELECT /////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////////////
        public IEnumerator LoginAndSelectMyPlayerCoroutine(string userName, string password)
        {
            bool isConnecting = true;

            NCMBUser.LogInAsync(userName, password, (NCMBException e) =>
            {
                if (e != null)
                {
                    Debug.Log(e);
                }

                Debug.Log("ログイン成功");

                //複数のNCMBObjectを取得するクエリを作成//
                NCMBQuery<NCMBObject> query  = new NCMBQuery<NCMBObject>("PlayerData");
                query.WhereEqualTo("MemberId", NCMBUser.CurrentUser.ObjectId as string);
                query.FindAsync((List<NCMBObject> _ncmbObjectList, NCMBException e) =>
                {
                    if (e != null)
                    {
                        Debug.Log(e);
                    }
                    else
                    {
                        Debug.Log("MyPlayerデータ取得成功");
                        myPlayer = _ncmbObjectList;
                        isConnecting = false;
                    }
                });
            });
            
            while (isConnecting) { yield return null; }
        }

        public IEnumerator SelectRoomDataCoroutine(string roomNumber)
        {
            bool isConnecting = true;

            //自分の所属する部屋データを取得するクエリを作成//
            NCMBQuery<NCMBObject> queryRoom  = new NCMBQuery<NCMBObject>("RoomData");
            queryRoom.WhereEqualTo("Name", roomNumber);
            queryRoom.FindAsync((List<NCMBObject> _ncmbObjectList, NCMBException e) =>
            {
                if (e != null)
                {
                    Debug.Log(e);
                }
                else
                {
                    Debug.Log("Roomデータ取得成功");
                    RoomList = _ncmbObjectList;
                    isConnecting = false;
                }
            });

            while (isConnecting) { yield return null; }
        }

        public IEnumerator SelectPlayerDataCoroutine()
        {
            bool isConnecting = true;

            NCMBQuery<NCMBObject> queryPlayer = new NCMBQuery<NCMBObject>("PlayerData");
            queryPlayer.WhereEqualTo("EnterRoom", RoomList[0]["Name"]  as string);
            queryPlayer.FindAsync((List<NCMBObject> _playerList, NCMBException e) =>
            {
                if(e != null)
                {
                    Debug.Log(e);
                }
                else
                {
                    //成功時の処理
                    PlayerList = _playerList;
                    Debug.Log("全プレイヤーデータを取得成功。人数は:" + PlayerList.Count);
                    isConnecting = false;
                }
            });

            while (isConnecting) { yield return null; }
        }

        public IEnumerator SelectCardDataCoroutine()
        {
            bool isConnecting = true;

            //カードを全取得
            NCMBQuery<NCMBObject> query = new NCMBQuery<NCMBObject>("CardData");
            query.FindAsync((List<NCMBObject> objList, NCMBException e) =>
            {
                if(e != null)
                {
                    Debug.Log(e);
                }
                else
                {
                    //成功時の処理
                    Debug.Log("全カードデータ取得成功");
                    CardList = objList;
                    isConnecting = false;
                }
            });

            while (isConnecting) { yield return null; }
        }

        public IEnumerator SelectRelationDataCoroutine()
        { 
            bool isConnecting = true;

            List<NCMBObject> master = new List<NCMBObject>();
            NCMBQuery<NCMBObject> queryMaster  = new NCMBQuery<NCMBObject>("RelationData");
            queryMaster.FindAsync((List<NCMBObject> _ncmbObjectList, NCMBException e) =>
            {
                if(e != null)
                {
                    Debug.Log(e);
                }
                else
                {
                    //成功時の処理
                    Debug.Log("マスターデータ取得成功");
                    RelationList = _ncmbObjectList;
                    isConnecting = false;
                } 
            });

            while (isConnecting) { yield return null; }
        }

        public IEnumerator SelectRelationFromDataCoroutine()
        { 
            bool isConnecting = true;
            
            NCMBQuery<NCMBObject> queryRelation  = new NCMBQuery<NCMBObject>("RelationData");
            queryRelation.WhereEqualTo("From", myPlayer[0]["Name"]  as string);
            queryRelation.FindAsync((List<NCMBObject> _ncmbObjectList, NCMBException e) =>
            {
                if(e != null)
                {
                    Debug.Log(e);
                }
                else
                {
                    //成功時の処理
                    Debug.Log("関係性データ(From)取得成功");
                    RelationFromList = _ncmbObjectList;
                    isConnecting = false;
                } 
            });

            while (isConnecting) { yield return null; }
        }

        public IEnumerator SelectRelationToDataCoroutine()
        { 
            bool isConnecting = true;
            
            NCMBQuery<NCMBObject> queryRelation  = new NCMBQuery<NCMBObject>("RelationData");
            queryRelation.WhereEqualTo("To", myPlayer[0]["Name"]  as string);
            queryRelation.FindAsync((List<NCMBObject> _ncmbObjectList, NCMBException e) =>
            {
                if(e != null)
                {
                    Debug.Log(e);
                }
                else
                {
                    //成功時の処理
                    Debug.Log("関係性データ(From)取得成功");
                    RelationToList = _ncmbObjectList;
                    isConnecting = false;
                } 
            });

            while (isConnecting) { yield return null; }
        }

        public IEnumerator SelectAnswerDataCoroutine()
        { 
            bool isConnecting = true;
            
            NCMBQuery<NCMBObject> queryRelation  = new NCMBQuery<NCMBObject>("AnswerData");
            queryRelation.WhereEqualTo("Name", myPlayer[0]["Name"]  as string);
            queryRelation.FindAsync((List<NCMBObject> _ncmbObjectList, NCMBException e) =>
            {
                if(e != null)
                {
                    Debug.Log(e);
                }
                else
                {
                    //成功時の処理
                    Debug.Log("解答データ取得成功");
                    AnswerList = _ncmbObjectList;
                    isConnecting = false;
                } 
            });

            while (isConnecting) { yield return null; }
        }

        public IEnumerator SelectMasterDataCoroutine()
        { 
            bool isConnecting = true;

            List<NCMBObject> master = new List<NCMBObject>();
            NCMBQuery<NCMBObject> queryMaster  = new NCMBQuery<NCMBObject>("MasterData");
            queryMaster.WhereEqualTo("GameNumber", "1");
            queryMaster.FindAsync((List<NCMBObject> _ncmbObjectList, NCMBException e) =>
            {
                if(e != null)
                {
                    Debug.Log(e);
                }
                else
                {
                    //成功時の処理
                    Debug.Log("マスターデータ取得成功");
                    MasterList = _ncmbObjectList;
                    isConnecting = false;
                } 
            });

            while (isConnecting) { yield return null; }
        }

        ///////////////////////////////////////////////////////////////////////////////////////
        // UPDATE /////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////////////
        public IEnumerator UpdateRoomCoroutine()
        {
            bool isConnecting = true;

            //カードを全取得
            RoomList[0].SaveAsync((NCMBException e) =>
            {
                if(e != null)
                {
                     Debug.Log(e);
                }
                else
                {
                    //成功時の処理
                    Debug.Log("部屋の更新に成功");
                    isConnecting = false;
                }
            });

            while (isConnecting) { yield return null; }
        }

        public IEnumerator UpdateMyPlayerCoroutine()
        {
            bool isConnecting = true;

            myPlayer[0].SaveAsync((NCMBException e) =>
            {
                if(e != null)
                {
                     Debug.Log(e);
                }
                else
                {
                    //成功時の処理
                    isConnecting = false;
                }
            });

            while (isConnecting) { yield return null; }
        }

        public IEnumerator UpdatePlayerCoroutine(int i)
        {
            bool isConnecting = true;

            PlayerList[i].SaveAsync((NCMBException e) =>
            {
                if(e != null)
                {
                     Debug.Log(e);
                }
                else
                {
                    //成功時の処理
                    isConnecting = false;
                }
            });

            while (isConnecting) { yield return null; }
        }

        public IEnumerator UpdateRelationCoroutine(int i, bool check)
        {
            bool isConnecting = true;

            if(check){
                int goodNumber = Int32.Parse(RelationList[i]["GoodNumber"] as string)+1;
                RelationList[i]["GoodNumber"] = goodNumber.ToString();;
            }else{
                int badNumber = Int32.Parse(RelationList[i]["BadNumber"] as string)+1;
                RelationList[i]["BadNumber"] = badNumber.ToString();;
            }

            RelationList[i].SaveAsync((NCMBException e) =>
            {
                if(e != null)
                {
                     Debug.Log(e);
                }
                else
                {
                    //成功時の処理
                    isConnecting = false;
                }
            });

            while (isConnecting) { yield return null; }
        }

        public IEnumerator UpdateMasterCoroutine()
        {
            bool isConnecting = true;

            MasterList[0].SaveAsync((NCMBException e) =>
            {
                if(e != null)
                {
                     Debug.Log(e);
                }
                else
                {
                    //成功時の処理
                    isConnecting = false;
                }
            });

            while (isConnecting) { yield return null; }
        }

        ///////////////////////////////////////////////////////////////////////////////////////
        // INSERT /////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////////////
        public IEnumerator InsertRoomDataCoroutine(string createRoomNumber)
        {
            bool isConnecting = true;

            NCMBObject obj = new NCMBObject("RoomData");
            obj.Add(RoomKey.NAME, createRoomNumber);
            obj.Add(RoomKey.OWNER, NCMBUser.CurrentUser[NCMBUserKey.NICKNAME]);
            obj.Add(RoomKey.PERFORMER, NCMBUser.CurrentUser[NCMBUserKey.NICKNAME]);
            obj.Add(RoomKey.CARDNUMBER, 0);
            obj.Add(RoomKey.ROOMMEMBER, NCMBUser.CurrentUser[NCMBUserKey.NICKNAME]);
            obj.Add(RoomKey.ENTRYMEMBER, "");
            obj.Add(RoomKey.VOTEMEMBER, "");
            obj.Add(RoomKey.DESTROYFLAG, false);
            obj.Add(RoomKey.PHASE, PhaseKey.START);
            obj.SaveAsync((NCMBException e) =>
            {
                if(e != null)
                {
                    Debug.Log(e);
                }
                else
                {
                    //成功時の処理
                    isConnecting = false;
                }
            });

            while (isConnecting) { yield return null; }
        }

        public IEnumerator InsertAnswerDataCoroutine(string createRoomNumber)
        {
            bool isConnecting = true;

            NCMBObject obj = new NCMBObject("AnswerData");
            obj.Add(RoomKey.NAME, createRoomNumber);
            obj.Add(RoomKey.OWNER, NCMBUser.CurrentUser[NCMBUserKey.NICKNAME]);
            obj.Add(RoomKey.PERFORMER, NCMBUser.CurrentUser[NCMBUserKey.NICKNAME]);
            obj.Add(RoomKey.CARDNUMBER, 0);
            obj.Add(RoomKey.ROOMMEMBER, NCMBUser.CurrentUser[NCMBUserKey.NICKNAME]);
            obj.Add(RoomKey.ENTRYMEMBER, "");
            obj.Add(RoomKey.VOTEMEMBER, "");
            obj.Add(RoomKey.DESTROYFLAG, false);
            obj.Add(RoomKey.PHASE, PhaseKey.START);
            obj.SaveAsync((NCMBException e) =>
            {
                if(e != null)
                {
                    Debug.Log(e);
                }
                else
                {
                    //成功時の処理
                    isConnecting = false;
                }
            });

            while (isConnecting) { yield return null; }
        }

        public IEnumerator InsertRelationDataCoroutine(string from, string to, bool check)
        {
            bool isConnecting = true;

            NCMBObject obj = new NCMBObject("RelationData");
            obj.Add("From", from);
            obj.Add("To", to);
            if(check){
                obj.Add("GoodNumber", "1");
                obj.Add("BadNumber", "0");
            }else{
                obj.Add("GoodNumber", "0");
                obj.Add("BadNumber", "1");
            }
            obj.SaveAsync((NCMBException e) =>
            {
                if(e != null)
                {
                    Debug.Log(e);
                }
                else
                {
                    //成功時の処理
                    isConnecting = false;
                }
            });

            while (isConnecting) { yield return null; }
        }

        ///////////////////////////////////////////////////////////////////////////////////////
        // OTHER //////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////////////

        public IEnumerator InitMyPlayerCoroutine(string roomNumber)
        {
            bool isConnecting = true;
            //Debug.Log("作成された部屋番号は：" + roomNumber);
            myPlayer[0][PlayerKey.PHASE]     = "Game";
            myPlayer[0][PlayerKey.ORDER]     = "0";
            myPlayer[0][PlayerKey.THEME]     = "0";
            myPlayer[0][PlayerKey.VOTE_1]    = "";
            myPlayer[0][PlayerKey.VOTE_2]    = "";
            myPlayer[0][PlayerKey.VOTE_3]    = "";
            myPlayer[0][PlayerKey.VOTE_4]    = "";
            myPlayer[0][PlayerKey.VOTE_5]    = "";
            myPlayer[0][PlayerKey.VOTE_6]    = "";
            myPlayer[0][PlayerKey.VOTE_7]    = "";
            myPlayer[0][PlayerKey.VOTE_8]    = "";
            myPlayer[0][PlayerKey.ENTRYFLAG] = false;
            myPlayer[0][PlayerKey.VOTEFLAG]  = false;
            myPlayer[0][PlayerKey.ENTERROOM] = roomNumber;
            
            myPlayer[0].SaveAsync((NCMBException e) =>
            {
                if(e != null)
                {
                     Debug.Log(e);
                }
                else
                {
                    //成功時の処理
                    isConnecting = false;
                }
            });

            while (isConnecting) { yield return null; }
        }

        public IEnumerator InitPlayerCoroutine(int number, string roomNumber)
        {
            bool isConnecting = true;
            Debug.Log("作成された部屋番号は：" + roomNumber);
            PlayerList[number][PlayerKey.PHASE]     = "Game";
            PlayerList[number][PlayerKey.ORDER]     = "0";
            PlayerList[number][PlayerKey.THEME]     = "0";
            PlayerList[number][PlayerKey.VOTE_1]    = "";
            PlayerList[number][PlayerKey.VOTE_2]    = "";
            PlayerList[number][PlayerKey.VOTE_3]    = "";
            PlayerList[number][PlayerKey.VOTE_4]    = "";
            PlayerList[number][PlayerKey.VOTE_5]    = "";
            PlayerList[number][PlayerKey.VOTE_6]    = "";
            PlayerList[number][PlayerKey.VOTE_7]    = "";
            PlayerList[number][PlayerKey.VOTE_8]    = "";
            PlayerList[number][PlayerKey.ENTRYFLAG] = false;
            PlayerList[number][PlayerKey.VOTEFLAG]  = false;
            PlayerList[number][PlayerKey.ENTERROOM] = roomNumber;
            
            PlayerList[number].SaveAsync((NCMBException e) =>
            {
                if(e != null)
                {
                     Debug.Log(e);
                }
                else
                {
                    //成功時の処理
                    isConnecting = false;
                }
            });

            while (isConnecting) { yield return null; }
        }

        private void SetBackCharaSprite()
        {
            int[] spr_num = new int[22]{0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21};
            for (int i = 0; i < spr_num.Length; i++)
            {
                int temp = spr_num[i]; 
                int randomIndex = UnityEngine.Random.Range(0, spr_num.Length); 
                spr_num[i] = spr_num[randomIndex]; 
                spr_num[randomIndex] = temp; 
            }

            for(int i=0;i<10;i++){
                backChara[i].GetComponent<Image>().sprite = backCharaPicture[spr_num[i]];
            }

            for(int i=0; i<10; i++){
                    int random_x = UnityEngine.Random.Range(-1, 2);
                    int random_y = UnityEngine.Random.Range(0, 2); 
                    
                    vec_x[i] = random_x;
                    if(random_y == 0){
                        vec_y[i] = 1;
                    }else{
                        vec_y[i] = -1;
                    }
                }
        }

        // Start is called before the first frame update
        void Start()
        {
            backNumber = UnityEngine.Random.Range(0, 2);
            SetBackCharaSprite();
        }

        // Update is called once per frame
        void Update(){
            timer++;
            
            //textTimer.GetComponent<Text>().text = timer.ToString();

            backImage.transform.localPosition = new Vector3(-timer/5,0,0);
            backImage2.transform.localPosition = new Vector3(-timer/5+800,0,0);
            //for(int i=0; i<3; i++){

            int p = timer % 200;
            if(p < 50){
                angle = p/5; //1～10
            }else if(p < 150){
                angle = 20-p/5; //10～-10(10～30)
            }else{
                angle = p/5-40; //-10～0(30～40)
            }

            if(backNumber == 0){
                backChara[0].transform.localPosition = new Vector3(-305+vec_x[0]*angle,510+vec_y[0]*angle,0);
                backChara[1].transform.localPosition = new Vector3(215+vec_x[1]*angle,440+vec_y[1]*angle,0);
                backChara[2].transform.localPosition = new Vector3(-260+vec_x[2]*angle,85+vec_y[2]*angle,0);
                backChara[3].transform.localPosition = new Vector3(-90+vec_x[3]*angle,-20+vec_y[3]*angle,0);
                backChara[4].transform.localPosition = new Vector3(90+vec_x[4]*angle,170+vec_y[4]*angle,0);
                backChara[5].transform.localPosition = new Vector3(-290+vec_x[5]*angle,-525+vec_y[5]*angle,0);
                backChara[6].transform.localPosition = new Vector3(300+vec_x[6]*angle,-55+vec_y[6]*angle,0);
                backChara[7].transform.localPosition = new Vector3(-240+vec_x[7]*angle,-350+vec_y[7]*angle,0);
                backChara[8].transform.localPosition = new Vector3(-20+vec_x[8]*angle,-615+vec_y[8]*angle,0);
                backChara[9].transform.localPosition = new Vector3(275+vec_x[9]*angle,-510+vec_y[9]*angle,0); 
            }else if(backNumber == 1){
                backChara[0].transform.localPosition = new Vector3(timer/3 - 550, -timer/9 + 700,0);
                backChara[1].transform.localPosition = new Vector3(timer/2 - 1200,-timer/3 + +200,0);
                backChara[2].transform.localPosition = new Vector3(-timer/24 + 700, timer/2 - 100,0);
                backChara[3].transform.localPosition = new Vector3(-timer/4 + 420,-timer/3 + 400,0);
                backChara[4].transform.localPosition = new Vector3(-timer/2 + 1200,timer/3 - 800,0);
                backChara[5].transform.localPosition = new Vector3(timer/20 - 400, timer/2 - 800,0);
                backChara[6].transform.localPosition = new Vector3(timer/4 - 100,-timer/2 + 1200,0);
                backChara[7].transform.localPosition = new Vector3(-timer/3 + 700,-timer/5 + 400,0);
                backChara[8].transform.localPosition = new Vector3(timer/10 + 100, timer/4 - 1000,0);
                backChara[9].transform.localPosition = new Vector3(timer/3 - 900,timer/2 - 600,0);

                backChara[0].transform.Rotate(0,0,0.1f);
                backChara[1].transform.Rotate(0,0,-0.2f);
                backChara[2].transform.Rotate(0,0,0.4f);
                backChara[3].transform.Rotate(0,0,-0.4f);
                backChara[4].transform.Rotate(0,0,-0.4f);
                backChara[5].transform.Rotate(0,0,-0.1f);
                backChara[6].transform.Rotate(0,0,-0.2f);
                backChara[7].transform.Rotate(0,0,0.2f);
                backChara[8].transform.Rotate(0,0,-0.1f);
                backChara[9].transform.Rotate(0,0,-0.4f);
            }

            if(timer > 4000){
                timer = 0;
                if(backNumber == 1){
                    SetBackCharaSprite();
                }
            }
        }
    }
}