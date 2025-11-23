using UnityEngine;

public class SafeAnimatorLoader : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void InitAnimator(UnitData data)
    {
        if (animator == null)
        {
            Debug.LogError("[SafeAnimatorLoader] Animator 컴포넌트를 찾을 수 없습니다.");
            return;
        }

        if (data == null)
        {
            Debug.LogError("[SafeAnimatorLoader] UnitData가 null입니다.");
            return;
        }

        string aniCtrName = data.AnimatorControllerName;

        if (string.IsNullOrEmpty(aniCtrName))
        {
            Debug.LogError("[SafeAnimatorLoader] AnimatorControllerName 비어있습니다.");
            return;
        }

        string path = $"Ani_Controllers/{aniCtrName}";
        var controller = Resources.Load<RuntimeAnimatorController>(path);

        if (controller == null)
        {
            Debug.LogError($"[SafeAnimatorLoader] 로드 실패: Resources/{path}.controller");
            return;
        }

        animator.runtimeAnimatorController = controller;
    }
}