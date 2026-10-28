using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class DialogueManager : MonoBehaviour
{
    [Header("Referências - Balão")]
    public GameObject dialogueBox;
    public TMP_Text dialogueText;

    [Header("Indicador de continuar")]
    [Tooltip("Texto/ícone que aparece quando há mais páginas (ex: '▼' ou '[E]')")]
    public GameObject continueIndicator;

    [Header("Configuração")]
    [Tooltip("Quantos caracteres cabem por página do balão")]
    public int caracteresPorPagina = 120;

    private bool dialogueActive = false;
    private CustomerOrder currentCustomer;

    private List<string> paginas = new List<string>();
    private int paginaAtual = 0;

    void Start()
    {
        dialogueBox.SetActive(false);

        if (continueIndicator != null)
            continueIndicator.SetActive(false);
    }

    public void ShowDialogue(string text, CustomerOrder customer)
    {
        dialogueBox.SetActive(true);

        dialogueActive = true;
        currentCustomer = customer;

        // Divide a fala em páginas
        paginas = DividirEmPaginas(text);
        paginaAtual = 0;

        dialogueText.text = paginas[paginaAtual];
        AtualizarIndicador();
    }

    public void HideDialogue()
    {
        dialogueBox.SetActive(false);
        dialogueActive = false;
        currentCustomer = null;

        paginas.Clear();
        paginaAtual = 0;

        if (continueIndicator != null)
            continueIndicator.SetActive(false);
    }

    void Update()
    {
        if (!dialogueActive)
            return;

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            // Ainda tem páginas pra mostrar
            if (paginaAtual < paginas.Count - 1)
            {
                paginaAtual++;
                dialogueText.text = paginas[paginaAtual];
                AtualizarIndicador();
            }
            else
            {
                // Última página → confirma o pedido e fecha
                if (currentCustomer != null)
                    currentCustomer.ConfirmOrder();

                HideDialogue();
            }
        }
    }

    // ========== Indicador ==========

    private void AtualizarIndicador()
    {
        if (continueIndicator == null)
            return;

        // Mostra o indicador só se ainda tiver próxima página
        bool temMaisPagina = paginaAtual < paginas.Count - 1;
        continueIndicator.SetActive(temMaisPagina);
    }

    // ========== Divisão em páginas ==========

    private List<string> DividirEmPaginas(string texto)
    {
        List<string> resultado = new List<string>();

        if (string.IsNullOrEmpty(texto))
        {
            resultado.Add("");
            return resultado;
        }

        if (texto.Length <= caracteresPorPagina)
        {
            resultado.Add(texto);
            return resultado;
        }

        // Quebra por palavras, sem cortar no meio de uma palavra
        string[] palavras = texto.Split(' ');
        string pagina = "";

        foreach (string palavra in palavras)
        {
            // +1 por causa do espaço
            if (pagina.Length + palavra.Length + 1 > caracteresPorPagina)
            {
                resultado.Add(pagina.Trim());
                pagina = palavra + " ";
            }
            else
            {
                pagina += palavra + " ";
            }
        }

        if (pagina.Trim().Length > 0)
            resultado.Add(pagina.Trim());

        return resultado;
    }
}