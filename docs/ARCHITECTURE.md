# 프로젝트 구조

## 디렉터리

| 경로 | 역할 |
| --- | --- |
| `HerosManager` | 실제 게임 Unity 프로젝트 |
| `HerosManager/Assets/01.Scenes` | Title, Intro, InGame, MapTest 씬 |
| `HerosManager/Assets/02.Scripts` | 자체 게임 코드 |
| `HerosManager/Assets/03.Prefabs` | UI, 캠프, 미니게임 프리팹 및 일부 외부 에셋 |
| `HerosManager/Assets/04.Images`, `05.Animations`, `06.Metarials`, `07.Sounds`, `09.Fonts` | 게임 리소스. 기존 경로 철자 보존 |
| `HerosManager/Assets/Resources/Json` | 실행 중 사용하는 24개 JSON 테이블 |
| `HerosManager/Assets/Editor` | 프로젝트·저장·플레이 검증 및 Windows 빌드 도구 |
| `HerosManager/Assets/Plugins`, `UniGLTF` | 기존 DOTween/UniGLTF 등의 외부 코드 |
| `GameDesign` | 기획 CSV, JSON 사본, 표 설명 자료 |
| `Art` | 원본 아트 자료 |
| `ff` | 과거 별도 샘플 Unity 프로젝트. 게임 진입점 아님 |

자체 게임 코드는 `02.Scripts`와 루트 `TutorialMgr.cs`로 구성됩니다.
전체 C#에는 외부 에셋 코드도 포함되므로 자체 게임 코드와 구분합니다.

## 실행 흐름

`Title → Intro → InGame`이 주 흐름입니다. `MapTest`는 기존 빌드 목록에 남아 있는 테스트 씬입니다.

- `LoadGameData`: 리소스 테이블을 한 번 로딩하고 ID 누락·중복을 검증합니다. 씬 전환 후 유지합니다.
- `GameDataModels`, `GameDataSerialization`, `GameDataCatalog`, `GameDataRules`: 테이블 형식·역직렬화·조회 인덱스·참조 검증을 분리합니다. `Catalog`의 반환 목록과 테이블 행은 읽기 전용으로 취급합니다.
- `SaveDataManager`: 게임·옵션 스냅샷을 관리하고 `JsonSaveStore`에 디스크 처리를 위임합니다. 씬 전환 후 유지합니다.
- `Singleton<T>`: 씬 서비스 검색, 한 번 초기화, 중복 인스턴스 제거, 소유 인스턴스 해제, 플레이 세션 정적 상태 초기화를 담당합니다.
- `InGameMgr`: 현재 스테이지·화면 상태와 일 단위 저장을 담당합니다.
- `ItemManager`, `HeroStateManager`: 자원 및 영웅 상태의 UI·스냅샷 변환을 담당합니다.
- `Clock`: 하루 진행과 모험 복귀, 채집 종료, 다음 날·스테이지 전환을 담당합니다.
- `MeadowCampaign`/`MeadowCampaignRules`: 초원·사막 각각 4일의 승패, 일일 결산, 회복 구매, 결과 화면과 재시작을 담당합니다. 기존 클래스명과 `meadowOutcome` 저장 필드명은 호환성을 위해 유지합니다. `Clock`의 일일 정산은 한 번만 적용됩니다.
- `CampaignProgressionRules`: 초원 승리 스냅샷을 사막 1일로 복제하고 이동 보급·생존자 회복을 적용합니다. 결과 저장에 성공한 후 장면을 다시 엽니다. 마법 숲은 플레이 완료 경로에 포함하지 않습니다.
- `MeadowRules`: 용사도 성장, 행동 가능 여부, 초원·사막 원정 보상·음식 회복의 순수 규칙입니다.
- `CampGemWallet`: 기존 5자원과 분리한 원석 잔액을 저장 스냅샷에서 복원합니다.
- `GamepadInput`/`WindowsGamepadSource`/`GamepadUiController`: XInput 프레임 수집, 키보드 UI 중복 방지, 화면 탐색, 진동과 연결 생명주기를 담당합니다. 가상 입력 소스를 주입해 검증합니다.
- `GameProgression`: 실제 설정된 스테이지/날짜 순서로 다음 날을 계산합니다. 일수나 연속 번호를 가정하지 않습니다.
- `InventoryRules`/`ItemCost`: 비용 합산·검증·차감의 순수 규칙입니다. 화면에서는 `ItemManager.TrySpend`로 여러 비용을 한 번에 지불합니다.
- `EventChoiceRules`: 선택지의 비용 sentinel과 성공/실패 보상을 해석합니다. `EventPanel`은 검증 후 한 번 적용하고 결과를 표시합니다.
- `WeightedSelection`: 주입받은 난수 표본으로 가중 후보를 선택합니다. 합이 1이 아니어도 마지막 후보까지 도달합니다.
- `GatheringManager → MiniGameMgr → MiniGameSetMgr`: 채집 장소 선택, 미니게임 세션·임시 보상, 각 미니게임 실행으로 책임을 구분합니다.
- `TutorialMgr`: 대사·창 튜토리얼의 페이지 이동 및 게임 시계 정지 복원을 담당합니다.
- `FirstRunGuide`/`FirstRunGuideView`: 초원 첫날의 실제 행동을 관찰하는 실습 단계와 비차단 카드·작업 위치 강조를 담당합니다. 초원의 구형 설명창은 이 안내로 대체하고 사막에서는 완료한 초원 설명을 반복하지 않습니다. 사막은 지역별 목표·도움말·채집 조작 안내를 제공합니다. 실습은 `Clock`만 보류하며 실제 작업 비용과 보상을 사용합니다.
- `DialogueReveal`/`DialogueInput`/`DialogueEffects`: 프레임 독립 대사 출력, 한 번의 의도적 확인 입력, 타이핑음 간격과 대기 효과를 공유합니다.
- `GameUiStyle`: 기존 아트를 유지하는 실행 시 UI 색상·텍스트·버튼 상태 규칙입니다.

초기화 순서는 `InGameMgr(-300) → ItemManager/HeroStateManager(-200) → Clock(100)`입니다.
기존 씬 오브젝트, 컴포넌트 이름, 공개 직렬화 필드 및 `.meta` GUID는 유지합니다.

## 저장 계약

게임은 하루 경계의 스냅샷을 저장합니다. 중간 컷신·진행 중 미니게임·캠프 타이머 전체를 복구하는 저장은 아닙니다.
자원 순서는 나무·물·고기·약초·음식이며, 영웅 배열은 스트레스·전투력·체력·누적 용사도 순서입니다.
영웅 배열이 없는 구형 저장은 테이블 기본값을 사용합니다.
`gems`는 원석 잔액, `meadowOutcome`은 0=진행 중/1=승리/2=패배입니다. 구형 저장의 누락 값은 각각 0으로 읽습니다. 옵션의 `disableVibration` 누락은 진동 사용을 뜻합니다.
`onboardingComplete`는 해당 원정의 첫날 실습 완료/건너뛰기입니다. 중간에 이 값만 저장할 때는 기존 아침 스냅샷을 복제하고 메타데이터만 바꿉니다. 새 원정에서는 다시 false로 시작합니다.
다음 날 시작 시 남은 이벤트 큐도 저장합니다. `hasEventSnapshot`이 없는 구형 저장은 초기 큐를 사용하고, 새 저장의 빈 `pendingEventIds`는 소진된 큐를 뜻합니다.

실행 파일은 `Application.persistentDataPath`의 `SaveGameData.Json`, `SaveOptionData.Json`을 사용합니다. 에디터 Play는 프로젝트의 `Library/EditorSaveData`를 사용하며 실행 파일의 저장을 읽거나 쓰지 않습니다. 에디터 프로필은 Library 삭제 시 함께 삭제되므로 실제 플레이 기록은 실행 파일에서 관리합니다.
실행 파일에서는 기존 `Assets` 위치 저장을 현대 저장이 없을 때만 복사하며 원본을 남깁니다. 격리된 프로필과 에디터에서는 이 마이그레이션을 수행하지 않습니다.
새 게임은 스냅샷 전체를 교체합니다. 저장 호출 뒤 화면을 전환하려면 `TrySaveGameDatas` 반환값을 확인합니다.
새 코드에서는 `CaptureGameData()`의 복제본을 수정한 뒤 `TrySaveGameSnapshot(snapshot)`을 호출합니다. 성공 전에는 현재 메모리 스냅샷을 바꾸지 않습니다. 새 게임 역시 `CreateNewGameData()`의 결과를 저장에 성공한 뒤 반영합니다.
임시 파일 작성·flush 후 원자적으로 교체하고 이전 정상 저장은 `.bak`에 남깁니다.

## 검증 저장의 안전 경계

- `ValidationSaveSession`이 검증마다 `Library/ValidationSaves/<GUID>`를 만들고 소유 표식을 기록합니다. 경로와 상태는 `SessionState`에 보관하여 도메인 재로드 여부와 초기화 순서에 의존하지 않습니다.
- 동기 검사는 `using var session = ValidationSaveSession.Open(...)`, Play 검사는 `Begin(prefix)`로 시작합니다. Play 검사에서 호출하는 보조 검사는 `RequireActive()`로 세션을 요구합니다.
- `SaveStoragePaths`의 경로 누락·변조·소유 표식 손실·종료 상태는 오류입니다. 배치와 `-executeMethod`는 활성 검증 세션 없이 일반 프로필로 돌아가지 않습니다. 잘못된 `--save-directory` 인수도 기본 경로로 조용히 대체하지 않습니다.
- `JsonSaveStore`의 읽기·쓰기에서 원본과 백업의 접근을 먼저 검사합니다. 검증 폴더 밖 경로, 경로 별칭에 의한 이탈, junction/reparse point를 거부합니다. 에디터의 실제 플레이어 저장 접근은 세션과 관계없이 차단합니다. 옵션 기본값 저장과 레거시 복구도 이 계층을 통과합니다.
- 정상 종료·수동 Stop 후에는 세션을 봉인하여 늦은 콜백의 저장을 거부합니다. 수동 중단된 검사에는 실패 상태를 남깁니다. 다음 검사는 새 프로필을 사용하며 이전 실행자의 콜백은 소유권이 바뀌면 해제됩니다.
- 종료 시 플레이어·에디터·레거시 저장의 파일 목록, SHA256, 수정 시간을 비교합니다. 이상이 있어도 자동 복원으로 증거를 덮지 않습니다. 검증 산출물은 자동 재귀 삭제하지 않고 Library에 남깁니다.
- 검증 정리가 끝난 뒤 수동으로 Play를 시작하면 에디터 전용 프로필로 돌아갑니다. 편집 모드에서 명시적으로 전환할 때는 `Tools > Heros Manager > Use Editor Save Profile After Validation`을 사용할 수 있습니다.

`tools/Validate-Unity.ps1 -Check Isolation`은 가짜 보호 파일로 경로 이탈·경로 변조·소유권 손실·봉인 후 쓰기·Windows junction을 검사합니다. 실제 저장 파일에는 쓰기 거부 실험조차 하지 않습니다. `-Check IsolationPlay`는 실제 Title의 게임·옵션 저장을 도메인 재로드 on/off 각각에서 실행하고 수동 중단·늦은 콜백·새 프로필 재실행을 검사합니다. 실행 도구는 같은 프로젝트의 열린 Unity를 거부하고 20분 상한을 둡니다.

이는 자체 저장 API와 현재 검증 진입점을 보호하는 장치입니다. 새 검증에서 임의의 `File.Write*`로 사용자 경로를 직접 쓰는 코드는 추가하지 않습니다. 원시 파일 I/O는 소유 검증 폴더의 fixture와 Logs 산출물로 제한합니다.

## 미니게임 확장

새 매니저는 `MiniGameSetMgr`를 상속하고 `SetUp`에서 `base.SetUp()`을 호출합니다.
`SetUp`, `StartGame`, `OnDrawGizmos`를 재정의할 때 `override`를 사용합니다.
생성물은 `TrackSpawn`으로 등록합니다. `StopGame`/비활성화 시 코루틴·예약 호출·생성물을 정리합니다.
일반 생성은 `Spawn`을 사용합니다. 생성 인스턴스의 매니저 참조를 연결하며 프리팹 원본은 변경하지 않습니다. `MiniGameReferences`는 해당 게임 계층 안에서 참조를 찾습니다.
보상은 `AddItem → MiniGameMgr`의 열린 세션에 적립하며 종료 시 한 번 지급합니다.
`MiniGameRewardLedger`는 보상 누적·취소·단일 지급을 담당합니다. `MiniGameRules`의 유한 미로 생성과 스폰/확률 경계 규칙은 화면 없이 검사할 수 있습니다.
프리팹과 `CollectSpaceTable`, `MiniGameTable`, `MiniGameMgr.miniGames` 연결을 함께 확인합니다.

## 변경 원칙

씬에 연결된 MonoBehaviour를 바꿀 때 클래스·파일명·직렬화 필드명을 먼저 보존합니다.
의미가 같은 새 타입으로 옮기려면 별도 직렬화 마이그레이션과 씬 검증을 같이 작성합니다.
외부 에셋 소스는 Unity API 호환 수정 외의 자체 구조개편 대상에서 제외합니다.
`Resources/Json`이 현재 실행 기준이며 `GameDesign` 사본은 자동 동기화되지 않습니다.
새 라이브러리 추가 없이 기존 Unity API와 도구를 사용합니다.
