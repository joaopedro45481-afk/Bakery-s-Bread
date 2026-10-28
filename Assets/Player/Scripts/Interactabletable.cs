using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Fica no objeto da mesa na cena. Ao apertar E perto dela, abre a tela
/// de SELEÇÃO de receitas (se a mesa tiver mais de uma) — não abre o
/// CookingUI diretamente mais.
///
/// Onde encaixa:
/// - É o ponto de entrada do fluxo: jogador aperta E -> abre
///   RecipeSelectionUI.AbrirSelecao(receitasDestaMesa) -> jogador escolhe
///   -> aí sim abre o CookingUI com a receita escolhida.
/// </summary>
public class InteractableTable : MonoBehaviour
{
    [SerializeField] private RecipeSelectionUI recipeSelectionUI;
    [SerializeField] private RecipeSO[] receitasDestaMesa;

    private bool jogadorPerto = false;

    private void Update()
    {
        if (jogadorPerto && Keyboard.current.eKey.wasPressedThisFrame)
        {
            recipeSelectionUI.AbrirSelecao(receitasDestaMesa);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) jogadorPerto = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) jogadorPerto = false;
    }
}