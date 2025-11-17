using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Muestra una imagen de tecla (UI) cuando el jugador entra al Trigger del objeto.
/// - Asigna el Sprite de la tecla en <see cref="keySprite"/> (por ejemplo, un ícono de la tecla E).
/// - Opcional: restringe por estado del jugador (Solid/Liquid/Gas) con las casillas.
/// - La imagen se instancia en un Canvas en modo World-Space, como hijo del objeto.
/// - Requiere un Collider2D configurado como isTrigger en el mismo GameObject.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class InteractPromptUI : MonoBehaviour
{
    [Header("Sprite de la tecla")]
    [Tooltip("Sprite que se mostrará como prompt (p.ej. ícono de la tecla E).")]
    public Sprite keySprite;

    [Header("Restricción por estado (opcional)")]
    public bool restrictByState = false;
    public bool allowSolid = true;
    public bool allowLiquid = true;
    public bool allowGas = true;

    [Header("Apariencia en mundo")]
    [Tooltip("Desplazamiento en el mundo respecto a este objeto.")]
    public Vector3 worldOffset = new Vector3(0f, 1f, 0f);
    [Tooltip("Escala de la IMAGEN (no del Canvas), para ajustarla manualmente desde el Inspector.")]
    public Vector2 imageScale = new Vector2(1f, 1f);

    [Header("Opcional")]
    [Tooltip("Si se asigna, se usará este Transform como ancla para posicionar el prompt.")]
    public Transform followTarget;

    private PlayerMovement _playerInside;
    private Canvas _canvasInstance;
    private Image _imageInstance;

    private void Awake()
    {
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
        if (followTarget == null) followTarget = this.transform;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var pm = other.GetComponent<PlayerMovement>();
        if (pm != null)
        {
            _playerInside = pm;
            UpdateVisibility();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var pm = other.GetComponent<PlayerMovement>();
        if (pm != null && pm == _playerInside)
        {
            _playerInside = null;
            Hide();
        }
    }

    private void Update()
    {
        // Seguir posición del objeto
        if (_canvasInstance != null)
        {
            _canvasInstance.transform.position = (followTarget != null ? followTarget.position : transform.position) + worldOffset;
        }

        if (_playerInside != null)
        {
            // Si hay restricción por estado, conmutar visibilidad según estado actual
            UpdateVisibility();
        }
    }

    private void UpdateVisibility()
    {
        if (_playerInside == null)
        {
            Hide();
            return;
        }

        if (restrictByState)
        {
            bool allow = false;
            switch (_playerInside.currentState)
            {
                case PlayerMovement.PlayerState.Solid: allow = allowSolid; break;
                case PlayerMovement.PlayerState.Liquid: allow = allowLiquid; break;
                case PlayerMovement.PlayerState.Gas: allow = allowGas; break;
            }
            if (!allow)
            {
                Hide();
                return;
            }
        }

        Show();
    }

    private void Show()
    {
        if (keySprite == null)
        {
            // No hay sprite asignado, no mostramos nada.
            return;
        }
        if (_canvasInstance == null)
        {
            // Crear Canvas en World-Space
            var go = new GameObject("InteractPromptCanvas");
            go.transform.SetParent(this.transform);
            _canvasInstance = go.AddComponent<Canvas>();
            _canvasInstance.renderMode = RenderMode.WorldSpace;
            _canvasInstance.sortingOrder = 1000; // encima de la mayoría de sprites

            var scaler = go.AddComponent<CanvasScaler>();
            scaler.dynamicPixelsPerUnit = 100f;

            go.AddComponent<GraphicRaycaster>();

            // Mantener el Canvas con escala 1; la escala se controlará desde la IMAGEN (no el Canvas)
            _canvasInstance.transform.localScale = Vector3.one;
            _canvasInstance.transform.position = (followTarget != null ? followTarget.position : transform.position) + worldOffset;
            _canvasInstance.transform.rotation = Quaternion.identity;

            // Crear Image hijo
            var imgGO = new GameObject("PromptImage");
            imgGO.transform.SetParent(_canvasInstance.transform);
            _imageInstance = imgGO.AddComponent<Image>();
            _imageInstance.raycastTarget = false;

            var rt = _imageInstance.rectTransform;
            rt.sizeDelta = new Vector2(256, 256); // tamaño base de la imagen
            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = Vector2.zero;
            // Aplicar escala inicial de la imagen
            rt.localScale = new Vector3(imageScale.x, imageScale.y, 1f);
        }

        _imageInstance.sprite = keySprite;
        // Asegurar que la escala de la imagen refleje lo configurado en el Inspector
        if (_imageInstance != null)
        {
            var rt = _imageInstance.rectTransform;
            rt.localScale = new Vector3(imageScale.x, imageScale.y, 1f);
        }
        if (!_canvasInstance.gameObject.activeSelf)
            _canvasInstance.gameObject.SetActive(true);
    }

    private void Hide()
    {
        if (_canvasInstance != null)
            _canvasInstance.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        Hide();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        // Reflejar cambios de escala inmediatamente en el editor
        if (_imageInstance != null)
        {
            var rt = _imageInstance.rectTransform;
            rt.localScale = new Vector3(imageScale.x, imageScale.y, 1f);
        }
    }
#endif
}
