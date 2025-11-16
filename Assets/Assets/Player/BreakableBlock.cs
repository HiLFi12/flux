using UnityEngine;

public class BreakableBlock : MonoBehaviour
{
    public void TriggerBreak()
    {
        Destroy(gameObject);
    }
}
