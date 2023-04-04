namespace Communication
{
    //PlayerPrefsのキー名定義//
    public static class PlayerPrefsKey
    {
        public static readonly string NCMB_USERNAME          = "NCMBUserName";
        public static readonly string NCMB_PASSWORD          = "NCMBPassword";
        public static readonly string GAMESETTING_UPDATEDATE = "GameSettingUpdateDate";
    }

    //会員管理のフィールド名定義//
    public static class NCMBUserKey
    {
        public static readonly string OBJECTID = "objectID";
        public static readonly string USERNAME = "UserName";
        public static readonly string NICKNAME = "NickName";
    }

    public static class InitDataKey
    {
        public static readonly int MINE = 1;
        public static readonly int ROOM = 2;
        public static readonly int PLAYER = 3;
        public static readonly int CARD = 3;
    }

    //プレイヤー管理のフィールド名定義//
    public static class PlayerKey
    {
        public static readonly string OBJECTID      = "objectID";
        public static readonly string NAME          = "Name";
        public static readonly string ENTERROOM     = "EnterRoom";
        public static readonly string ENTRYFLAG     = "EntryFlag";
        public static readonly string MEMBERID      = "MemberId";
        public static readonly string ORDER         = "Order";
        public static readonly string PHASE         = "Phase";
        public static readonly string THEME         = "Theme";
        public static readonly string VOTE          = "Vote";
        public static readonly string VOTE_1        = "Vote_1";
        public static readonly string VOTE_2        = "Vote_2";
        public static readonly string VOTE_3        = "Vote_3";
        public static readonly string VOTE_4        = "Vote_4";
        public static readonly string VOTE_5        = "Vote_5";
        public static readonly string VOTE_6        = "Vote_6";
        public static readonly string VOTE_7        = "Vote_7";
        public static readonly string VOTE_8        = "Vote_8";
        public static readonly string VOTEFLAG      = "VoteFlag";
        public static readonly string KICKFLAG      = "KickFlag";
        public static readonly int MAX_MEMBER_NUM   = 20;
        public static readonly int MAX_ENTRY_NUM    = 8;
    }

    //部屋管理のフィールド名定義//
    public static class RoomKey
    {
        public static readonly string NAME          = "Name";
        public static readonly string OWNER         = "Owner";
        public static readonly string PERFORMER     = "Performer";
        public static readonly string CARDNUMBER    = "CardNumber";
        public static readonly string ENTRYMEMBER   = "EntryMember";
        public static readonly string ROOMMEMBER    = "RoomMember";
        public static readonly string VOTEMEMBER    = "VoteMember";
        public static readonly string TURNNUMBER    = "TurnNumber";
        public static readonly string DESTROYFLAG   = "DestroyFlag";
        public static readonly string PHASE         = "Phase";
    }

    //フェーズ管理のフィールド名定義//
    public static class PhaseKey
    {
        public static readonly string START         = "Start";
        public static readonly string END           = "End";
        public static readonly string TOP           = "Top";
        public static readonly string LOBBY         = "Lobby";
        public static readonly string LOBBY_DESTROY = "LobbyDestroy";
        public static readonly string LOBBY_EXIT    = "LobbyExit";
        public static readonly string LOBBY_KICK    = "LobbyKick";
        public static readonly string GAME          = "Game";
        public static readonly string PLAY          = "Play";
        public static readonly string RESULT        = "Result";
    }

    //データストアのフィールド名定義//
    public static class NCMBDataStoreKey
    {
        //Default
        public static readonly string OBJECTID      = "objectId";
        public static readonly string CREATE_DATE   = "createDate";
        public static readonly string UPDATE_DATE   = "updateDate";

        //General
        public static readonly string USERNAME      = "UserName";
        public static readonly string NICKNAME      = "NickName";
        public static readonly string KILLCOUNT     = "KillCount";

        //GraveInfo
        public static readonly string CHECKCOUNTER  = "CheckCounter";
        public static readonly string MESSAGE       = "Message";
        public static readonly string CURSETYPE     = "CurseType";
        public static readonly string POSITION      = "Position";

        //PlayerInfo
        public static readonly string SCREENSHOT_FILENAME   = "ScreenShotFileName";
        public static readonly string EQUIPCARD_ID          = "EquipCardId";
        public static readonly string INSTALLSTION_OBJECTID = "InstallationObjectId";

        //GameSetting
        public static readonly string IS_SERVICE_ENABLE     = "IsServiceEnable";
        public static readonly string BANNERFILE_NAME       = "BannerFileName";
        public static readonly string ENEMY_DROPCOIN_NUM    = "EnemyDropCoinNum";
        public static readonly string GACHAPRICE            = "GachaPrice";
        public static readonly string TERMSOFUSE               = "TermsOfUse";

        //Announcements
        public static readonly string TITLE     = "Title";
        public static readonly string MAIN_TEXT = "MainText";
    }

    //データストアのクラス名定義//
    public static class NCMBDataStoreClass
    {
        public static readonly string GRAVEINFO_LIST    = "GraveInfoList";
        public static readonly string KILLRANKING       = "KillRanking";
        public static readonly string PLAYERINFO_LIST   = "PlayerInfoList";
        public static readonly string GAMESETTING       = "GameSetting";
        public static readonly string ANNOUNCEMENTS     = "Announcements";
    }

    public static class ActPlate
    {
        public static readonly int[] POS_X = new int[8] {-392,-392,-392,-392,192,192,192,192};
        public static readonly int[] POS_Y = new int[8] {160,33,-100,-235,160,33,-100,-235};
        public static readonly int WIDTH   = 200;
        public static readonly int HEIGHT  = 100;
    }
}