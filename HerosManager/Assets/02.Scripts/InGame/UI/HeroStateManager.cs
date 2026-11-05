[UnityEngine.DefaultExecutionOrder(-200)]
public class HeroStateManager : Singleton<HeroStateManager>
{
    public HeroState[] heroStates;

    protected override void Awake()
    {
        base.Awake();
        if (_instance != this)
            return;
        if (heroStates == null || heroStates.Length == 0 ||
            System.Array.Exists(heroStates, hero => hero == null))
            heroStates = GetComponentsInChildren<HeroState>(true);
    }

    void Start()
    {
        Setup();
    }

    void Setup()
    {
        //0-레이널드 1-아시스 2-엘리스 3-슈아
        for (int i = 0; i < heroStates.Length && i < 4; i++)
        {
            string key = "Hero_" + (i + 1);
            HeroData heroData = LoadGameData.Instance.heroDatas[key];

            heroStates[i].Setup(heroData);
            heroStates[i].RestoreStats(GetSavedStats(SaveDataManager.Instance.saveGameData, i));
        }
    }

    static float[] GetSavedStats(SaveGameData data, int index)
    {
        switch (index)
        {
            case 0: return data.hero1Stat;
            case 1: return data.hero2Stat;
            case 2: return data.hero3Stat;
            case 3: return data.hero4Stat;
            default: return null;
        }
    }

    public void CaptureStats(SaveGameData data)
    {
        if (heroStates == null)
            return;
        if (heroStates.Length > 0 && heroStates[0] != null) data.hero1Stat = heroStates[0].CaptureStats();
        if (heroStates.Length > 1 && heroStates[1] != null) data.hero2Stat = heroStates[1].CaptureStats();
        if (heroStates.Length > 2 && heroStates[2] != null) data.hero3Stat = heroStates[2].CaptureStats();
        if (heroStates.Length > 3 && heroStates[3] != null) data.hero4Stat = heroStates[3].CaptureStats();
    }

    public HeroState GetHeroStatInfo(string _code)
    {
        HeroState heroStat = null;

        switch (_code)
        {
            case "Hero_1":
                heroStat = heroStates[0];
                break;
            case "Hero_2":
                heroStat = heroStates[1];
                break;
            case "Hero_3":
                heroStat = heroStates[2];
                break;
            case "Hero_4":
                heroStat = heroStates[3];
                break;
        }

        return heroStat;
    }

    public void SlideAll(bool _isSlide)
    {
        for (int i = 0; i < heroStates.Length; i++)
        {
            heroStates[i].isSlide = _isSlide;
            heroStates[i].Slide();
        }
    }
}
