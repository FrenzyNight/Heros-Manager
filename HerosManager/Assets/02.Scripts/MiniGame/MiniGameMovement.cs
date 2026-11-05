using UnityEngine;

public static class MiniGameMovement
{
    public static Vector2 Direction(bool up, bool left, bool down, bool right)
    {
        return new Vector2(MiniGameRules.HorizontalInput(left, right),
            MiniGameRules.VerticalInput(up, down)).normalized;
    }

    public static void FaceHorizontal(Transform transform, float horizontal)
    {
        if (Mathf.Approximately(horizontal, 0f))
            return;
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * (horizontal > 0f ? 1f : -1f);
        transform.localScale = scale;
    }
}
