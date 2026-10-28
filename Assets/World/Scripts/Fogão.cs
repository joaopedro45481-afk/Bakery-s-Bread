using UnityEngine;
using UnityEngine.InputSystem;

public class NewMonoBehaviourScript : MonoBehaviour
{
    [Header("Interação")]
    [SerializeField] private bool podeCozinhar;

    public GameObject painel;

    void Start()
    {
        painel.SetActive(false);
    }

    void Update()
    {
        if (podeCozinhar && Keyboard.current.eKey.wasPressedThisFrame)
        {
            painel.SetActive(!painel.activeSelf);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            podeCozinhar = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            podeCozinhar = false;
            painel.SetActive(false); // Fecha ao sair da área
        }
    }
}