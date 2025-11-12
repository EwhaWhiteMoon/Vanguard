// 작성자 : 김도건
// 마지막 수정 : 2025.10.11.
// 싱글톤 패턴을 이용하는 MonoBehaviour의 반복 작성을 방지하고, 가독성을 상승시키기 위한 Abstract 클래스

using UnityEngine;

public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<T>();
                if (_instance == null)
                {
                    Debug.LogError(typeof(T).Name + " could not find an instance of type " + typeof(T).Name);
                    return null;
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = GetComponent<T>();
        DontDestroyOnLoad(gameObject); //Scene 전환 시 사라지지 않음.
    }
}