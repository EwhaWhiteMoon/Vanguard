using UnityEngine;

public class UIBillboard : MonoBehaviour
{
    private Camera cam;

    private void Start()
    {
        cam = Camera.main;
    }

    private void LateUpdate()
    {
        if (cam == null) return;

        // UI가 카메라를 바라보게 만들기
        transform.LookAt(transform.position + cam.transform.forward);
    }
}
