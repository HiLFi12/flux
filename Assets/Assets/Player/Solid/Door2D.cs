using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
public class Door2D : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Collider2D doorCollider;
    [SerializeField] private SpriteRenderer visual;

    [Header("Colors")]
    [SerializeField] private Color closedColor = Color.red;
    [SerializeField] private Color openColor = Color.green;

    [Header("State")]
    [SerializeField] private bool isOpen = false;

    [Header("Interaction")]
    [Tooltip("Nombre de la escena a cargar")]
    [SerializeField] private string nextSceneName = "";
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    private readonly HashSet<PlayerMovement> playersInRange = new();

    private void Reset()
    {
        doorCollider = GetComponent<Collider2D>();
        visual = GetComponentInChildren<SpriteRenderer>();
    }

    private void Awake()
    {
        if (doorCollider == null) doorCollider = GetComponent<Collider2D>();
        if (visual == null) visual = GetComponentInChildren<SpriteRenderer>(true);
        ApplyState(isOpen);
    }

    private void Update()
    {
        if (!isOpen || playersInRange.Count == 0) return;
        if (Input.GetKeyDown(interactKey))
        {
            TryChangeScene();
        }
    }

    public void SetOpen(bool open)
    {
        if (isOpen == open) return;
        isOpen = open;
        ApplyState(isOpen);
    }

    private void ApplyState(bool open)
    {
        if (doorCollider != null)
        {
            doorCollider.enabled = true;
            doorCollider.isTrigger = open;
        }

        if (visual != null)
            visual.color = open ? openColor : closedColor;

        if (!open)
            playersInRange.Clear();
    }

    private void TryChangeScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isOpen) return;
        var player = other.GetComponentInParent<PlayerMovement>();
        if (player != null) playersInRange.Add(player);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!isOpen) return;
        var player = other.GetComponentInParent<PlayerMovement>();
        if (player != null) playersInRange.Add(player);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var player = other.GetComponentInParent<PlayerMovement>();
        if (player != null) playersInRange.Remove(player);
    }
}