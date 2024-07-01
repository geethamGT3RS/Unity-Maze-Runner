using UnityEngine;

public class ViewportSwitcher : MonoBehaviour
{
    public Camera mainCamera;
    public Camera[] smallCameras;
    private Camera currentMainCamera;
    private Rect[] originalRects;

    void Start()
    {
        originalRects = new Rect[smallCameras.Length + 1];
        originalRects[0] = mainCamera.rect;
        for (int i = 0; i < smallCameras.Length; i++)
        {
            originalRects[i + 1] = smallCameras[i].rect;
        }
        currentMainCamera = mainCamera;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            for (int i = 0; i < smallCameras.Length; i++)
            {
                if (IsMouseInCameraViewport(smallCameras[i], Input.mousePosition))
                {
                    SwapViewports(smallCameras[i], i + 1);
                    break;
                }
            }
        }
    }

    bool IsMouseInCameraViewport(Camera camera, Vector3 mousePosition)
    {
        Rect viewportRect = camera.pixelRect;
        return viewportRect.Contains(mousePosition);
    }

    void SwapViewports(Camera smallCamera, int smallCameraIndex)
    {
        Rect tempRect = smallCamera.rect;
        smallCamera.rect = currentMainCamera.rect;
        currentMainCamera.rect = tempRect;
        originalRects[smallCameraIndex] = currentMainCamera.rect;
        originalRects[0] = smallCamera.rect;
        currentMainCamera = smallCamera;
    }
}
