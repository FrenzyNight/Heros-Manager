[System.Serializable]
public class CookData
{
    public string CookID;
    public string NeedItem1ID;
    public int NeedItem1Amount;
    public string NeedItem2ID;
    public int NeedItem2Amount;
    public string NeedItem3ID;
    public int NeedItem3Amount;
    public string GetItemID;
    public int GetItemAmount;
    public float CookTime;
    public float GetTime;
}

[System.Serializable]
public class CutsceneData
{
    public string CutSceneID;
    public string OpenID;
    public int StringType;
    public string StringID;
    public float StringDelay;
    public float value1;
    public int ImageType;
    public string ImageResourceID;
    public float ImageDelay;
    public float value2;
    public int SoundType;
    public string SoundResourceID;
    public float SoundDelay;
    public float value3;
}


[System.Serializable]
public class BonfireData
{
    public string BonfireID;
    public string NeedItemID;
    public int NeedAmount;
    public float Time1;
    public float Time2;
    public float Time3;
    public float Hero1_Stress;
    public float Hero2_Stress;
    public float Hero3_Stress;
    public float Hero4_Stress;
}

[System.Serializable]
public class CollectSpaceData
{
    public string CollectSpaceID;
    public string MiniGameID1;
    public string MiniGameHelpStringID1;
    public string MiniGameID2;
    public string MiniGameHelpStringID2;
    public string FunctionID;
}

[System.Serializable]
public class CollectData
{
    public string CollectID;
    public string CollectSpaceID1;
    public string StringID1;
    public string CollectSpaceID2;
    public string StringID2;
    public string CollectSpaceID3;
    public string StringID3;
    public int RanPos;
    public string CollectSpaceID4;
    public string StringID4;
    public float StartTime;
    public float EndTime;
    public float ContinueTime;
}

[System.Serializable]
public class DefineData
{
    public int Index;
    public string DefineID;
    public float value1;
}

[System.Serializable]
public class FenceData
{
    public string FenceID;
    public string FenceGroupID;
    public int FenceLevel;
    public float InvadeProb;
    public string NeedItemID;
    public int Amount;
    public float NeedTime;
}

[System.Serializable]
public class FieldData
{
    public string FieldID;
    public string NeedItemID;
    public int NeedAmount;
    public float Time1;
    public float Time2;
    public int Repeat;
    public string GetItemID;
    public int GetAmount;
}

[System.Serializable]
public class HeroStateData
{
    public string HeroStateID;
    public string HeroStateGroupID;
    public string HeroStateStringID;
    public float FirstGive;
    public float Min;
    public float Max;
}

[System.Serializable]
public class HeroData
{
    public string HeroID;
    public string HeroStringID;
    public string HeroStateGroupID;
}

[System.Serializable]
public class InvadeData
{
    public string InvadeID;
    public string InvadeStringID;
    public int StealObject;
    public string ItemID1;
    public int RandMin1;
    public int RandMax1;
    public string ItemID2;
    public int RandMin2;
    public int RandMax2;
    public string ItemID3;
    public int RandMin3;
    public int RandMax3;
    public string ItemID4;
    public int RandMin4;
    public int RandMax4;
}

[System.Serializable]
public class ItemData
{
    public string ItemID;
    public string ItemStringID;
    public int FirstGive;
}

[System.Serializable]
public class JourneyData
{
    public string JourneyID;
    public int Item1Min;
    public int Item2Min;
    public string StageTitleStringID;
    public float MeatValue;
    public float WaterValue;
    public float HubValue;
    public float FoodValue;
    public float HStressValue;
    public float HPowerValue;
    public string JStateGroupID;
}

[System.Serializable]
public class JResultData
{
    public string JResultID;
    public string JResultGroupID;
    public string JRanStringID;
    public float Prob;
    public float Hero1_Stress;
    public float Hero1_Power;
    public float Hero1_Hp;
    public float Hero1_Exp;
    public float Hero2_Stress;
    public float Hero2_Power;
    public float Hero2_Hp;
    public float Hero2_Exp;
    public float Hero3_Stress;
    public float Hero3_Power;
    public float Hero3_Hp;
    public float Hero3_Exp;
    public float Hero4_Stress;
    public float Hero4_Power;
    public float Hero4_Hp;
    public float Hero4_Exp;
}

[System.Serializable]
public class JStateData
{
    public string JStateID;
    public string JStateGroupID;
    public string JStateStringID;
    public float ValueMin;
    public string JResultGroupID;
}

[System.Serializable]
public class MiniGameData
{
    public string ObjectID;
    public string MinigameID;
    public string GetItemID1;
    public int GetAmount1;
    public string GetItemID2;
    public int GetAmount2;
    public int GetJemAmount;
    public float Probability;
    public float Stun;
    public float Invincibility;
    public float Velocity;
    public float CoolTime;
    public int GameEnvironment;
    public float value1;
    public float value2;
    public float value3;
    public float value4;
    public float value5;
    public float value6;
}

[System.Serializable]
public class StageDayData
{
    public string StageDayID;
    public string StageDayGroupID;
    public int Day;
    public string InvadeID;
    public string JourneyID;
}

[System.Serializable]
public class EventData
{
    public string EventID;
    public int opentype;
    public int EventType;
    public string H1ImageName;
    public string H2ImageName;
    public int ChoiceNum;
    public string EventStringID;
    public string Choice1ID;
    public string Choice2ID;
    public string Choice3ID;
}

[System.Serializable]
public class ChoiceData
{
    public string ChoiceID;
    public string ChoiceStringID;
    public string NeeditemID;
    public int NeedAmount;
    public float RewardProb;
    public int RewardType;
    public string SuccessRewardStringID;
    public string Hero1ID;
    public string H1Reward1Type;
    public int H1Reward1Amount;
    public string H1Reward2Type;
    public int H1Reward2Amount;
    public string Hero2ID;
    public string H2Reward1Type;
    public int H2Reward1Amount;
    public string H2Reward2Type;
    public int H2Reward2Amount;
    public string FailRewardStringID;
    public string FailHero1ID;
    public string FailH1Reward1Type;
    public int FailH1Reward1Amount;
    public string FailH1Reward2Type;
    public int FailH1Reward2Amount;
    public string FailHero2ID;
    public string FailH2Reward1Type;
    public int FailH2Reward1Amount;
    public string FailH2Reward2Type;
    public int FailH2Reward2Amount;
    public string AddEventID;
}



[System.Serializable]
public class StageData
{
    public string StageID;
    public string StageDayGroupID;
    public int Index;
    public string StageStringID;
    public string BonfireID;
    public string FenceGroupID;
    public string CollectID;
    public string FieldID;
}

[System.Serializable]
public class JemData
{
    public string JemID;
    public string JemImageID;
    public string JemNameString;
    public string JemExplainString;
    public int JemType;
    public int StartLevel;
    public int MaxLevel;
    public float Value1;
    public float Value2;
    public float LevelUpAmount;
    public string CutsceneOpenID;
}

[System.Serializable]
public class LaundryData
{
    public string LaundryID;
    public string StageID;
    public int NeedWaterAmount;
    public int L1_GermCount;
    public float L1_GermTime;
    public float L1_JenTimeMin;
    public float L1_JenTimeMax;
    public int L2_RinseCount;
    public float L2_DelayTime;
    public float L3_MinTime;
    public float L3_MaxTime;
    public float L3_TotalTime;
    public int Hero1_Stress;
    public int Hero2_Stress;
    public int Hero3_Stress;
    public int Hero4_Stress;
}

[System.Serializable]
public class TutorialData
{
    public string TutorialID;
    public string OpenID;
    public int TutorialType;
    public int Index;
    public int ImageType;
    public string ImageID;
    public string StringID;
}

[System.Serializable]
public class StringData
{
    public string StringID;
    public string KOR;
    public string ENG;
}
