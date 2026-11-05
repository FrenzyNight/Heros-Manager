# Heros-Manager

용사관리사

장르     : 어드밴처   
개발기간 : 2021.10 ~ 현재   
사용도구 : Unity 6000.4.7f1 / C# (원본: 2019.4.13f1)
개발인원 : 8인 (프로그래밍 2인)   
담당업무 : 미니게임 파트 및 인게임 로직   
   
플레이 영상 : https://youtu.be/PaLmw5Qeb40

C2BG 게임제작동아리 연합발표회 대상 : https://www.inven.co.kr/webzine/news/?news=276176

## 개발 시작

Unity Hub에서 이 저장소의 **`HerosManager` 하위 폴더**를 Unity **6000.4.7f1**로 엽니다.
시작 씬은 `Assets/01.Scenes/Title.unity`입니다. `InGame.unity` 직접 실행도 데이터를 초기화합니다.
`ff`는 별도의 과거 샘플 프로젝트이며 이번 이식 대상이 아닙니다.

- [프로젝트 구조와 개발 규칙](docs/ARCHITECTURE.md)
- [Unity 6 이식 내역과 검증](docs/UNITY6_MIGRATION.md)
- [유지보수 리팩터링과 회귀 검사](docs/REFACTORING.md)

Unity를 닫은 뒤 저장소 루트의 PowerShell에서 실행합니다.

```powershell
./tools/Validate-Unity.ps1 -Check Project
./tools/Validate-Unity.ps1 -Check Rules
./tools/Validate-Unity.ps1 -Check Save
./tools/Validate-Unity.ps1 -Check Play
./tools/Validate-Unity.ps1 -Check Build
```

검증 로그는 `HerosManager/Logs`, Windows 빌드는 `HerosManager/Builds/Windows`에 생성됩니다.
에디터 메뉴 `Tools > Heros Manager`에서도 프로젝트 검사와 빌드를 실행할 수 있습니다.
