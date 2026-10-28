using System;

/// <summary>
/// Guarda e controla o estado de UMA task de cozinha em andamento.
/// NÃO é MonoBehaviour de propósito: nasce quando o jogador começa a cozinhar
/// (ex: CookingUI cria uma instância no OnAbrirMesa) e morre quando termina
/// ou é cancelada. Não precisa existir na cena o tempo todo.
///
/// Onde encaixa:
/// - InteractableTable/CookingUI: cria a instância passando a receita escolhida.
/// - DoughInteraction: chama AvancarParaSova() e AvancarParaFogao() conforme
///   o jogador interage com a massa.
/// - Stove: chama IniciarCozimento() quando recebe a massa, e o próprio
///   controller dispara FinalizarCozimento() sozinho via evento de tempo
///   (a coroutine de espera fica no Stove, que so avisa quando acabar).
/// - CookingUI: assina OnEstadoMudou para atualizar a tela sem checar estado
///   a cada frame.
/// </summary>
public class CookingTaskController
{
    public enum EstadoTask
    {
        AbrindoMassa,
        Sovando,
        LevandoAoFogao,
        Cozinhando,
        Concluida,
        Cancelada
    }

    public RecipeSO Receita { get; private set; }
    public EstadoTask EstadoAtual { get; private set; }

    /// <summary>Disparado toda vez que o estado muda. UI escuta isso.</summary>
    public event Action<EstadoTask> OnEstadoMudou;

    /// <summary>Disparado quando a task termina com sucesso, com o item resultante.</summary>
    public event Action<RecipeSO> OnTaskConcluida;

    public CookingTaskController(RecipeSO receita)
    {
        Receita = receita;
        EstadoAtual = EstadoTask.AbrindoMassa;
    }

    private void MudarEstado(EstadoTask novoEstado)
    {
        EstadoAtual = novoEstado;
        OnEstadoMudou?.Invoke(EstadoAtual);
    }

    /// <summary>Chamado pelo DoughInteraction quando o jogador termina de abrir a massa.</summary>
    public void AvancarParaSova()
    {
        if (EstadoAtual != EstadoTask.AbrindoMassa) return;
        MudarEstado(EstadoTask.Sovando);
    }

    /// <summary>Chamado pelo DoughInteraction quando a massa terminou de ser sovada.</summary>
    public void AvancarParaFogao()
    {
        if (EstadoAtual != EstadoTask.Sovando) return;
        MudarEstado(EstadoTask.LevandoAoFogao);
    }

    /// <summary>Chamado pelo Stove quando a massa é colocada nele.</summary>
    public void IniciarCozimento()
    {
        if (EstadoAtual != EstadoTask.LevandoAoFogao) return;
        MudarEstado(EstadoTask.Cozinhando);
    }

    /// <summary>Chamado pelo Stove quando a coroutine de espera termina.</summary>
    public void FinalizarCozimento()
    {
        if (EstadoAtual != EstadoTask.Cozinhando) return;
        MudarEstado(EstadoTask.Concluida);
        OnTaskConcluida?.Invoke(Receita);
    }

    public void Cancelar()
    {
        MudarEstado(EstadoTask.Cancelada);
    }
}