using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class HUD : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textoMoedas;
    [SerializeField] private PlayerMove player;
    [SerializeField] public GameObject menu;

    void Update()
    {
        textoMoedas.text = "$: " + player.moedas;
    }

    public void fechar_menu()
    {
        menu.SetActive(!menu.activeSelf);
    }
}