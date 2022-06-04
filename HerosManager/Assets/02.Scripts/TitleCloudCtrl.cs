using UnityEngine;

public class TitleCloudCtrl : MonoBehaviour
{
    [Range(0, 200)]
    public float speed;

    public float startX;
    public float endX;

    void Update()
    {
        this.transform.Translate(Vector2.right * Time.deltaTime * speed);
        if (this.transform.localPosition.x > endX)
        {
            Vector2 pos = this.transform.localPosition;
            pos.x = startX;
            this.transform.localPosition = pos;
        }
    }
}
