using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>A non-modal coach card and pointer. The card consumes clicks within its bounds.</summary>
public sealed class FirstRunGuideView : MonoBehaviour
{
    Canvas canvas;
    GameObject root, card, marker;
    RectTransform cardRect, markerRect;
    Text eyebrow, heading, description, note;
    Image progress;
    Button skip, collapse, reopen;
    CanvasGroup group;
    CanvasGroup markerGroup;
    Image cardImage;
    Text skipLabel, reopenLabel;
    bool expanded = true, visible = true;
    bool? compactLayout;
    string previousHeading;
    float fade = 1, targetProgress, displayedProgress;
    Transform target;
    readonly Vector3[] corners = new Vector3[4];

    public void Build(Font font, UnityAction onSkip)
    {
        root = new GameObject("First run coaching", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        root.transform.SetParent(transform, false);
        canvas = root.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 1400;
        var scaler = root.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;
        card = Box(root.transform, "Coach card", new Vector2(420, 510), GameUiStyle.Surface);
        cardImage = card.GetComponent<Image>();
        cardImage.raycastTarget = true;
        GameUiStyle.Shadow(cardImage);
        cardRect = (RectTransform)card.transform;
        cardRect.anchorMin = cardRect.anchorMax = new Vector2(0, 0.5f);
        cardRect.pivot = new Vector2(0, 0.5f);
        cardRect.anchoredPosition = new Vector2(24, -110);
        group = card.AddComponent<CanvasGroup>();
        eyebrow = Label(card.transform, font, 22, new Vector2(370, 35), new Vector2(0, 218));
        eyebrow.color = new Color(0.78f, 0.91f, 0.75f);
        heading = Label(card.transform, font, 30, new Vector2(370, 72), new Vector2(0, 158));
        heading.fontStyle = FontStyle.Bold;
        description = Label(card.transform, font, 24, new Vector2(370, 240), new Vector2(0, 3));
        description.alignment = TextAnchor.UpperLeft;
        description.lineSpacing = 1.12f;
        note = Label(card.transform, font, 18, new Vector2(370, 46), new Vector2(0, -144));
        note.color = new Color(0.7f, 0.82f, 0.83f);
        var track = Box(card.transform, "Progress", new Vector2(370, 5), new Color(0.2f, 0.3f, 0.32f));
        ((RectTransform)track.transform).anchoredPosition = new Vector2(0, -177);
        progress = Box(track.transform, "Fill", new Vector2(370, 5), new Color(0.6f, 0.86f, 0.62f)).GetComponent<Image>();
        skip = Button(card.transform, font, "실습 건너뛰기", new Vector2(230, 48), new Vector2(-70, -217), onSkip);
        skipLabel = skip.GetComponentInChildren<Text>();
        collapse = Button(card.transform, font, "접기", new Vector2(120, 48), new Vector2(125, -217), () => SetExpanded(false));
        reopen = Button(root.transform, font, "플레이 안내", new Vector2(200, 48), Vector2.zero, () => SetExpanded(true));
        reopenLabel = reopen.GetComponentInChildren<Text>();
        var reopenRect = (RectTransform)reopen.transform;
        reopenRect.anchorMin = reopenRect.anchorMax = new Vector2(0, 0.5f);
        reopenRect.pivot = new Vector2(0, 0.5f);
        reopenRect.anchoredPosition = new Vector2(24, 0);
        marker = Box(root.transform, "Current objective", new Vector2(90, 90), Color.clear);
        markerRect = (RectTransform)marker.transform;
        markerGroup = marker.AddComponent<CanvasGroup>();
        markerGroup.blocksRaycasts = false;
        for (int i = 0; i < 4; i++)
        {
            var edge = Box(marker.transform, "Edge", Vector2.zero, new Color(0.5f, 1f, 0.87f, 0.95f));
            var rect = (RectTransform)edge.transform;
            if (i < 2)
            {
                rect.anchorMin = new Vector2(0, i); rect.anchorMax = new Vector2(1, i);
                rect.sizeDelta = new Vector2(0, 4);
            }
            else
            {
                rect.anchorMin = new Vector2(i - 2, 0); rect.anchorMax = new Vector2(i - 2, 1);
                rect.sizeDelta = new Vector2(4, 0);
            }
        }
    }

    public void SetExpanded(bool value)
    {
        expanded = value;
        if (card != null) card.SetActive(expanded);
        if (reopen != null) reopen.gameObject.SetActive(!expanded);
    }

    public void SetVisible(bool value)
    {
        visible = value;
        if (root != null) root.SetActive(value);
    }

    public void SetContent(string section, string title, string body, string footer, float fraction,
        bool practice, bool saveFailed, Transform highlight, bool compact, string foldedLabel)
    {
        if (heading == null) return;
        if (previousHeading != section)
        {
            // Keep the old card readable while gently bringing attention to a changed task.
            if (previousHeading != null) fade = 0.55f;
            previousHeading = section;
        }
        eyebrow.text = section;
        heading.text = compact ? section + " · " + title : title;
        string content = compact ? body.Replace("\n\n", "\n") : body;
        description.text = saveFailed && compact ? "완료 상태를 저장하지 못했습니다. ‘저장 다시 시도’를 누르세요.\n" + content : content;
        note.text = footer;
        targetProgress = Mathf.Clamp01(fraction);
        progress.rectTransform.pivot = new Vector2(0, 0.5f);
        progress.rectTransform.anchoredPosition = new Vector2(-185, 0);
        skip.gameObject.SetActive(practice || saveFailed);
        skipLabel.text = saveFailed ? "저장 다시 시도" : "실습 건너뛰기";
        reopenLabel.text = saveFailed ? "저장 재시도 필요" : foldedLabel;
        target = highlight;
        ((RectTransform)collapse.transform).anchoredPosition = compact ? new Vector2(550, -32) : new Vector2(practice || saveFailed ? 125 : 0, -217);
        if (compactLayout != compact) ApplyLayout(compact);
    }

    void ApplyLayout(bool compact)
    {
        compactLayout = compact;
        // Gathering fills both columns. Use the free header strip instead of covering a game or area label.
        cardRect.anchorMin = cardRect.anchorMax = compact ? new Vector2(0.5f, 1) : new Vector2(0, 0.5f);
        cardRect.pivot = compact ? new Vector2(0.5f, 1) : new Vector2(0, 0.5f);
        cardRect.sizeDelta = compact ? new Vector2(1390, 174) : new Vector2(420, 510);
        cardRect.anchoredPosition = compact ? new Vector2(0, -12) : new Vector2(24, -110);
        cardImage.color = compact ? new Color(0.075f, 0.13f, 0.14f, 1) : GameUiStyle.Surface;
        eyebrow.gameObject.SetActive(!compact);
        note.gameObject.SetActive(!compact);
        progress.transform.parent.gameObject.SetActive(!compact);
        heading.rectTransform.sizeDelta = compact ? new Vector2(1000, 42) : new Vector2(370, 72);
        heading.rectTransform.anchoredPosition = compact ? new Vector2(-150, 56) : new Vector2(0, 158);
        heading.fontSize = compact ? 28 : 30;
        description.rectTransform.sizeDelta = compact ? new Vector2(1000, 100) : new Vector2(370, 240);
        description.rectTransform.anchoredPosition = compact ? new Vector2(-150, -16) : new Vector2(0, 3);
        description.fontSize = 24;
        description.resizeTextForBestFit = true;
        description.resizeTextMinSize = compact ? 20 : 21;
        description.resizeTextMaxSize = 24;
        ((RectTransform)skip.transform).anchoredPosition = compact ? new Vector2(550, 28) : new Vector2(-70, -217);
    }

    void LateUpdate()
    {
        if (root == null || !visible) return;
        fade = Mathf.MoveTowards(fade, 1f, Time.unscaledDeltaTime * 4f);
        group.alpha = compactLayout == true ? 1 : fade;
        displayedProgress = Mathf.MoveTowards(displayedProgress, targetProgress, Time.unscaledDeltaTime * 1.5f);
        progress.rectTransform.localScale = new Vector3(displayedProgress, 1, 1);
        // Folding only hides the explanation, not the next actionable target.
        bool show = target != null && target.gameObject.activeInHierarchy;
        marker.SetActive(show);
        if (!show) return;
        markerGroup.alpha = 0.8f + 0.2f * Mathf.Sin(Time.unscaledTime * 3f);
        Vector2 position, size;
        if (target is RectTransform rect)
        {
            var owner = target.GetComponentInParent<Canvas>();
            var camera = owner == null || owner.renderMode == RenderMode.ScreenSpaceOverlay ? null : owner.worldCamera != null ? owner.worldCamera : Camera.main;
            rect.GetWorldCorners(corners);
            Vector2 a = RectTransformUtility.WorldToScreenPoint(camera, corners[0]);
            Vector2 b = RectTransformUtility.WorldToScreenPoint(camera, corners[2]);
            position = (a + b) * 0.5f;
            size = new Vector2(Mathf.Abs(b.x - a.x), Mathf.Abs(b.y - a.y)) / canvas.scaleFactor + Vector2.one * 12;
        }
        else
        {
            if (Camera.main == null) { marker.SetActive(false); return; }
            // Authored camp roots sit at the origin; their trigger centers locate the actual work sites.
            Vector3 world = target.position;
            foreach (var collider in target.GetComponentsInChildren<Collider>())
                if (collider.CompareTag("CampObject")) { world = collider.bounds.center; break; }
            Vector3 screen = Camera.main.WorldToScreenPoint(world + Vector3.up * 0.7f);
            if (screen.z < 0) { marker.SetActive(false); return; }
            position = screen;
            size = new Vector2(110, 110);
        }
        markerRect.position = position;
        markerRect.sizeDelta = size;
    }

    static GameObject Box(Transform parent, string name, Vector2 size, Color color)
    {
        var obj = new GameObject(name, typeof(RectTransform), typeof(Image));
        obj.transform.SetParent(parent, false);
        obj.GetComponent<RectTransform>().sizeDelta = size;
        var image = obj.GetComponent<Image>();
        image.color = color;
        image.raycastTarget = false;
        return obj;
    }

    static Text Label(Transform parent, Font font, int size, Vector2 dimensions, Vector2 position)
    {
        var obj = new GameObject("Text", typeof(RectTransform), typeof(Text));
        obj.transform.SetParent(parent, false);
        var text = obj.GetComponent<Text>();
        text.font = font;
        text.fontSize = size;
        text.color = new Color(0.95f, 0.95f, 0.88f);
        text.alignment = TextAnchor.MiddleLeft;
        text.raycastTarget = false;
        text.rectTransform.sizeDelta = dimensions;
        text.rectTransform.anchoredPosition = position;
        return text;
    }

    static Button Button(Transform parent, Font font, string label, Vector2 size, Vector2 position, UnityAction action)
    {
        var obj = Box(parent, label, size, new Color(0.24f, 0.37f, 0.36f));
        obj.GetComponent<RectTransform>().anchoredPosition = position;
        obj.GetComponent<Image>().raycastTarget = true;
        var button = obj.AddComponent<Button>();
        var colors = button.colors;
        colors.highlightedColor = colors.selectedColor = new Color(1, 0.88f, 0.55f);
        button.colors = colors;
        var text = Label(obj.transform, font, 21, size - new Vector2(12, 4), Vector2.zero);
        text.alignment = TextAnchor.MiddleCenter;
        text.text = label;
        button.onClick.AddListener(action);
        return button;
    }
}
