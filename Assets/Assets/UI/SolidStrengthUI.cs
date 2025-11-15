using UnityEngine;
using UnityEngine.UI;

public class SolidStrengthUI : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private PlayerMovement player;
    [SerializeField] private Slider slider;           
    [SerializeField] private CanvasGroup group;

    private void Awake()
    {
        if (player == null) player = FindObjectOfType<PlayerMovement>();
        if (group == null)
        {
            group = GetComponent<CanvasGroup>();
            if (group == null) group = gameObject.AddComponent<CanvasGroup>();
        }
        Show(false); 
    }

    private void OnEnable()
    {
        if (player == null) return;
        player.OnStateChanged += HandleStateChanged;
        player.OnSolidStrengthChanged += HandleSolidStrengthChanged;

        HandleStateChanged(player.currentState);
        HandleSolidStrengthChanged(player.SolidStrength, player.solidMaxStrength);
    }

    private void OnDisable()
    {
        if (player == null) return;
        player.OnStateChanged -= HandleStateChanged;
        player.OnSolidStrengthChanged -= HandleSolidStrengthChanged;
    }

    private void HandleStateChanged(PlayerMovement.PlayerState state)
    {
        // Mostrar solo si el jugador está en estado sólido
        Show(state == PlayerMovement.PlayerState.Solid);
    }

    private void HandleSolidStrengthChanged(float current, float max)
    {
        if (slider == null) return;
        slider.maxValue = max;
        slider.value = current;
    }

    private void Show(bool visible)
    {
        group.alpha = visible ? 1f : 0f;
        group.blocksRaycasts = visible;
        group.interactable = visible;
    }
}
