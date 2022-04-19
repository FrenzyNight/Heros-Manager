public class HeroStateManager : Singleton<HeroStateManager>
{
    public HeroState[] heroStates;

    void Start()
    {
        heroStates = this.GetComponentsInChildren<HeroState>();

        Setup();
    }

    void Setup()
    {
        //0-레이널드 1-아시스 2-엘리스 3-슈아
        for (int i = 0; i < heroStates.Length; i++)
        {
            string key = "Hero_" + (i + 1);
            HeroData heroData = LoadGameData.Instance.heroDatas[key];

            heroStates[i].Setup(heroData);
        }
    }

    HeroState GetHeroStatInfo(string _code)
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