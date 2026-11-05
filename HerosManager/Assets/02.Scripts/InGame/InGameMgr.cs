using System;
using UnityEngine;

public enum State
{
    None,
    Camp,
    Gathering,
    Fence,
    MiniGame,
    Cutscene,
    Event,
    Adventure,
    Laundry
}

[DefaultExecutionOrder(-300)]
public class InGameMgr : Singleton<InGameMgr>
{
    public StageData stageData;
    public int stage;

    public Action NextStageAct;

    public State state = State.Camp;

    public Pause pause;

    public CutSceneManager cutSceneMgr;

    public bool IsComplete { get; private set; }
    bool hadGamepad;

    protected override void Awake()
    {
        base.Awake();
        if (_instance != this)
            return;

        Time.timeScale = 1f;
        LoadGameData.Instance.LoadTitleDatas();
        var saves = SaveDataManager.Instance;
        if (saves.saveGameData == null || saves.saveGameData.stage < 1 ||
            saves.saveGameData.items == null || saves.saveGameData.items.Length != 5)
        {
            if (!saves.LoadGameDatas())
                saves.NewGameData();
        }
        stage = saves.saveGameData.stage;
        LoadStageData();
        if (stage == 1 && GetComponent<MeadowCampaign>() == null)
            gameObject.AddComponent<MeadowCampaign>();
        if (stage == 1 && GetComponent<FirstRunGuide>() == null)
            gameObject.AddComponent<FirstRunGuide>();
    }

    void LoadStageData()
    {
        if (!LoadGameData.Instance.Catalog.TryGetStage(stage, out stageData))
        {
            Debug.LogError($"No configured stage for index {stage}.", this);
            enabled = false;
        }
    }

    public void NextStage()
    {
        if (IsComplete) return;
        if (!LoadGameData.Instance.Catalog.TryGetNextStage(stage, out var nextStage))
        {
            CompleteGame();
            return;
        }
        stage = nextStage.Index;
        stageData = nextStage;
        NextStageAct?.Invoke();
    }

    public void CompleteGame()
    {
        if (IsComplete) return;
        IsComplete = true;
        Clock.Instance.isStop = true;
        Debug.Log("The final configured stage is complete.");
        if (pause != null) pause.PauseBtn();
    }

    public void FinishMeadow()
    {
        IsComplete = true;
        Clock.Instance.isStop = true;
        state = State.None;
        Time.timeScale = 0f;
    }

    public bool SaveStageData()
    {
        SaveGameData savegameData = SaveDataManager.Instance.CaptureGameData();
        savegameData.stage = stage;
        savegameData.day = Clock.Instance.day;
        savegameData.items = ItemManager.Instance.CaptureItems();
        HeroStateManager.Instance.CaptureStats(savegameData);
        savegameData.fenceLevel = FenceMgr.Instance.fenceLevel;
        EventMgr.Instance.CapturePendingEvents(savegameData);
        savegameData.gems = CampGemWallet.Instance.GetAmount();
        if (MeadowCampaign.IsCreateInstance())
            savegameData.meadowOutcome = (int)MeadowCampaign.Instance.Outcome;
        return SaveDataManager.Instance.TrySaveGameSnapshot(savegameData);
    }

    void Update()
    {
        if (hadGamepad && !GamepadInput.Connected && !IsComplete && !TutorialMgr.Instance.isTutorial &&
            !(MeadowCampaign.IsCreateInstance() && MeadowCampaign.Instance.IsModalOpen))
            pause.PauseBtn();
        hadGamepad = GamepadInput.Connected;
        if (Input.GetKeyDown(KeyCode.Escape) || GamepadInput.Pressed(GamepadButton.Back) ||
            GamepadInput.Pressed(GamepadButton.Pause))
        {
            if (MeadowCampaign.IsCreateInstance() && MeadowCampaign.Instance.IsModalOpen)
            { MeadowCampaign.Instance.CloseHelp(); return; }
            if (TutorialMgr.Instance.isTutorial) { TutorialMgr.Instance.CloseTutorial(); return; }
            if (pause.optionMgr.BackPanel.activeInHierarchy) { pause.optionMgr.CloseBtn.onClick.Invoke(); return; }
            if (pause.IsOpen) { pause.ExitBtnEvent(); return; }
            if (GamepadInput.Pressed(GamepadButton.Pause)) { pause.PauseBtn(); return; }
            if (state == State.Camp || (state == State.None && AdventureManager.Instance.AdvPanel.activeInHierarchy))
            {
                pause.PauseBtn();
            }
            else if (state == State.Gathering)
            {
                GatheringManager.Instance.CloseGatheringPannel();
            }
            else if (state == State.MiniGame)
            {
                MiniGameMgr.Instance.CloseMiniGame();
            }
            else if (state == State.Fence)
            {
                FenceMgr.Instance.ClosePanel();
            }
            else if (state == State.Event)
            {
                EventMgr.Instance.ClosePanel();
            }
            else if(state == State.Laundry)
            {
                var laundry = FindAnyObjectByType<LaundryInteractionMgr>();
                if (laundry != null) laundry.ExitLaundryWindow();
            }
        }

        switch (state)
        {
            case State.Cutscene:
                if (DialogueInput.ConsumeAdvance())
                {
                    bool isLast = cutSceneMgr.NextCutscene();
                    if (isLast)
                    {

                    }
                }
                break;
        }
    }
}
