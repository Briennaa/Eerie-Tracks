using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer mapRenderer;
    private float leftLimit, rightLimit, bottomLimit, upperLimit;

    [SerializeField] private float zoomMin;
    [SerializeField] private float zoomMax;


    private Camera cam;
    private bool moveAllowed;
    private Vector3 touchPos;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        Application.targetFrameRate = 60;


        //Get map limits based on sprite renderer
        leftLimit = (mapRenderer.transform.position.x - mapRenderer.bounds.size.x) / 2f;
        rightLimit = (mapRenderer.transform.position.x + mapRenderer.bounds.size.x) / 2f;

        bottomLimit = (mapRenderer.transform.position.y - mapRenderer.bounds.size.y) / 2f;
        upperLimit = (mapRenderer.transform.position.y + mapRenderer.bounds.size.y) / 2f;

    }


    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0 )
        {

            if (Input.touchCount == 2)
            {
                Touch touchZero = Input.GetTouch(0);
                Touch touchOne = Input.GetTouch(1);

                if (EventSystem.current.IsPointerOverGameObject(touchOne.fingerId) || EventSystem.current.IsPointerOverGameObject(touchZero.fingerId))
                {
                    return;
                }

                Vector2 touchZeroLastPos = touchZero.position - touchZero.deltaPosition;
                Vector2 touchOneLastPos = touchOne.position - touchOne.deltaPosition;

                float disTouch = (touchZeroLastPos - touchOneLastPos).magnitude;
                float currentDisTouch = (touchZero.position - touchOne.position).magnitude;

                float difference = currentDisTouch - disTouch;

                Zoom(difference * 0.01f);
            }
            else
            {
                Touch touch = Input.GetTouch(0);

                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                        {
                            moveAllowed = false;
                        } else
                        {
                            moveAllowed = true;
                        }

                        touchPos = cam.ScreenToWorldPoint(touch.position);
                        break;

                    case TouchPhase.Moved:
                        if (moveAllowed)
                        {
                            Vector3 direction = touchPos - cam.ScreenToWorldPoint(touch.position);
                            cam.transform.position += direction;
                            cam.transform.position = ClampCamera(transform.position);
                        }
                        break;
                }
            }          
        }
    }


        private void Zoom(float increment)
    {
        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize - increment, zoomMin, zoomMax);
        cam.transform.position = ClampCamera(cam.transform.position);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(new Vector3(
            (rightLimit - Mathf.Abs(leftLimit)),
            (upperLimit - Mathf.Abs(bottomLimit))),
            new Vector3(rightLimit - leftLimit, upperLimit - bottomLimit));
    }


    private Vector3 ClampCamera(Vector3 targetPosition)
    {
        float camHeight = cam.orthographicSize;
        float camWidth = cam.orthographicSize * cam.aspect;

        float minX = leftLimit + camWidth;
        float maxX = rightLimit - camWidth;

        float minY = bottomLimit + camHeight;
        float maxY = upperLimit - camHeight;

        float newX = Mathf.Clamp(targetPosition.x, minX, maxX);
        float newY = Mathf.Clamp(targetPosition.y, minY, maxY);

        return new Vector3(newX, newY, targetPosition.z);
    }
}
