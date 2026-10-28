using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Fica no objeto do fogão na cena. Depois que o jogador termina de sovar
/// e abrir a massa, o CookingUI só GUARDA a task aqui como "pendente" —
/// o cozimento só começa de verdade quando o jogador anda até esse fogão
/// no mundo e aperta E, igual à interação da mesa.
///
/// Onde encaixa:
/// - CookingUI chama GuardarMassaPendente() quando a massa termina de ser
///   moldada (estado LevandoAoFogao), e fecha a tela.
/// - Esse script detecta o jogador (trigger, igual InteractableTable) e,
///   ao apertar E com uma massa pendente guardada, inicia o cozimento de
///   verdade e roda o timer.
/// - Ao terminar a espera, chama controller.FinalizarCozimento(), que
///   dispara OnTaskConcluida — o CookingUI escuta isso e entrega o item
///   ao inventário (mesmo com a tela já fechada há tempo).
///
/// Precisa de um Collider2D marcado como Is Trigger nesse objeto, igual
/// à mesa.
/// </summary>
public class Stove : MonoBehaviour
{
    private bool jogadorPerto = false;
    private Coroutine cozimentoAtivo;

    private CookingTaskController controllerPendente;
    private RecipeSO receitaPendente;

    private void Update()
    {
        if (jogadorPerto && controllerPendente != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            IniciarCozimentoComPendente();
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

    /// <summary>Chamado pelo CookingUI quando a massa termina de ser moldada.</summary>
    public void GuardarMassaPendente(CookingTaskController controller, RecipeSO receita)
    {
        controllerPendente = controller;
        receitaPendente = receita;
        Debug.Log("Fogão: massa pendente guardada. Aperte E perto do fogão pra cozinhar.");
    }

    private void IniciarCozimentoComPendente()
    {
        CookingTaskController controller = controllerPendente;
        RecipeSO receita = receitaPendente;
        controllerPendente = null;
        receitaPendente = null;

        Debug.Log($"Fogão recebeu a massa. Vai esperar {receita.tempoDeForno} segundos.");
        controller.IniciarCozimento();
        cozimentoAtivo = StartCoroutine(Cozinhar(controller, receita.tempoDeForno));
    }

    private IEnumerator Cozinhar(CookingTaskController controller, float tempo)
    {
        yield return new WaitForSeconds(tempo);
        Debug.Log("Cozimento terminou, chamando FinalizarCozimento().");
        controller.FinalizarCozimento();
        cozimentoAtivo = null;
    }

    public void CancelarCozimento(CookingTaskController controller)
    {
        if (cozimentoAtivo != null)
        {
            StopCoroutine(cozimentoAtivo);
            cozimentoAtivo = null;
        }
        controller.Cancelar();
    }
}