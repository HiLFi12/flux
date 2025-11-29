using UnityEngine;

public class VisualSpriteKeeper : MonoBehaviour
{
    [SerializeField] private SpriteRenderer visual;
    private Vector3 baselineLocalScale = Vector3.one;
    private Vector2 baselineSpriteSize = Vector2.one;
    private bool initialized = false;

    public void Initialize(SpriteRenderer visualRenderer)
    {
        if (visualRenderer == null) return;
        visual = visualRenderer;
        baselineLocalScale = visual.transform.localScale;
        baselineSpriteSize = visual.sprite != null ? visual.sprite.bounds.size : Vector2.one;
        initialized = true;
    }

    public void SetSprite(Sprite newSprite)
    {
        if (!initialized && visual != null) Initialize(visual);
        if (visual == null)
        {
            return;
        }

        if (newSprite == null)
        {
            visual.sprite = null;
            return;
        }

        Vector2 newSize = newSprite.bounds.size;
        if (newSize.x <= 0f || newSize.y <= 0f)
        {
            visual.sprite = newSprite;
            return;
        }

        // Calculate baseline world size (sprite size * baseline local scale)
        Vector2 baselineWorldSize = new Vector2(baselineSpriteSize.x * baselineLocalScale.x, baselineSpriteSize.y * baselineLocalScale.y);

        // Compute target local scale so that new sprite occupies same world size
        Vector3 targetLocal = visual.transform.localScale;
        targetLocal.x = baselineWorldSize.x / newSize.x;
        targetLocal.y = baselineWorldSize.y / newSize.y;

        visual.sprite = newSprite;
        visual.transform.localScale = targetLocal;

        // Update baseline to reflect new sprite and scale
        baselineSpriteSize = newSize;
        baselineLocalScale = visual.transform.localScale;
        initialized = true;
    }

    public void RefreshBaseline()
    {
        if (visual == null) return;
        baselineLocalScale = visual.transform.localScale;
        baselineSpriteSize = visual.sprite != null ? visual.sprite.bounds.size : Vector2.one;
        initialized = true;
    }
}
