using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    protected static T _instance;
    private static int session = -1;
    private int initializedSession = -1;
    /// <summary>
    /// 싱글톤이 생성되어있는지 확인하고 생성되어있으면 리턴.
    /// 생성되어 있지 않으면 생성 후 리턴.
    /// </summary>
    public static T Instance
    {
        get
        {
            ResetSession();
            if (SingletonRuntime.IsQuitting)
                return null;
            if (_instance == null)
            {
                _instance = FindAnyObjectByType<T>(FindObjectsInactive.Include);
                if (_instance == null)
                {
                    _instance = new GameObject(typeof(T).ToString(), typeof(T)).GetComponent<T>();
                    if (_instance == null)
                    {
                        // 인스턴스를 생성하지 못했을때 에러.
                        Debug.LogError("Singleton::Problem during the creation of " + typeof(T).ToString());
                    }
                }
            }
            _instance.InitializeOnce();
            return _instance;
        }
    }

    public static bool IsCreateInstance()
    {
        ResetSession();
        return _instance != null;
    }

    private static void ResetSession()
    {
        if (session == SingletonRuntime.Session)
            return;
        session = SingletonRuntime.Session;
        _instance = null;
    }

    protected virtual void Awake()
    {
        ResetSession();
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = (T)this;
        InitializeOnce();
    }

    private void InitializeOnce()
    {
        if (initializedSession == SingletonRuntime.Session)
            return;
        initializedSession = SingletonRuntime.Session;
        Init();
    }

    /// <summary>
    /// 싱글톤이 생성될때.
    /// </summary>
    public virtual void Init() { }
    /// <summary>
    /// 싱글톤 해제될때.
    /// </summary>
    public virtual void Destory() { }

    /// <summary>
    /// GameObject 해제.
    /// </summary>
    protected virtual void OnDestroy()
    {
        if (_instance != this)
            return;
        _instance = null;
        Destory();
    }
    /// <summary>
    /// 어플리케이션 종료.
    /// </summary>
    protected virtual void OnApplicationQuit()
    {
        SingletonRuntime.IsQuitting = true;
    }
}

internal static class SingletonRuntime
{
    internal static int Session { get; private set; }
    internal static bool IsQuitting { get; set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Reset()
    {
        Session++;
        IsQuitting = false;
        Time.timeScale = 1f;
    }
}
