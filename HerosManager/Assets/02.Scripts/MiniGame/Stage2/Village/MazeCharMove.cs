using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeCharMove : MonoBehaviour
{
    private MazeManager MM;
    private RectTransform rt;

    private bool isMove;
    private float x, y;
    private int direction;
    private bool isW, isA, isS, isD;
    public Vector2 firstPoint;
    private int treasureFrame = -1;

    void Awake()
    {
        rt = GetComponent<RectTransform>();
        firstPoint = rt.anchoredPosition;
    }

    void OnEnable() => ResetPosition();

    public void ResetPosition()
    {
        if (rt == null) return;
        rt.anchoredPosition = firstPoint;
        isMove = false;
        x = y = 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        MM = MiniGameReferences.Manager<MazeManager>(this);

        rt = gameObject.GetComponent<RectTransform>();
        
        firstPoint = rt.anchoredPosition;
        isMove = false;
        isW = false;
        isA = false;
        isS = false;
        isD = false;

    }

    // Update is called once per frame
    void Update()
    {
        if (!GamepadUiController.MiniGameInputAllowed)
        {
            isMove = false;
            return;
        }
        isW = Input.GetKey(KeyCode.UpArrow) || GamepadInput.Aim.y > 0.4f;
        isA = Input.GetKey(KeyCode.LeftArrow) || GamepadInput.Aim.x < -0.4f;
        isS = Input.GetKey(KeyCode.DownArrow) || GamepadInput.Aim.y < -0.4f;
        isD = Input.GetKey(KeyCode.RightArrow) || GamepadInput.Aim.x > 0.4f;
        isMove = MiniGameMovement.Direction(isW, isA, isS, isD) != Vector2.zero;
        SetDirection();
    }

    void FixedUpdate()
    {
        if(isMove)
        {
            rt.anchoredPosition = new Vector2(rt.anchoredPosition.x + (x * Time.deltaTime) , rt.anchoredPosition.y + (y * Time.deltaTime));
        }
    }

    void SetDirection()
    {
        Vector2 velocity = MiniGameMovement.Direction(isW, isA, isS, isD) * MM.realMazeCharSpeed;
        x = velocity.x;
        y = velocity.y;
        MiniGameMovement.FaceHorizontal(transform, x);
    }

    void OnTriggerEnter2D(Collider2D c)
    {
        if(c.CompareTag("Treasure") && treasureFrame != Time.frameCount)
        {
            treasureFrame = Time.frameCount;
            GetTreasure();
        }
    }

    void GetTreasure()
    {
        rt.anchoredPosition = firstPoint;
        MM.CollectTreasure();

    }
}
