using UnityEngine;

[System.Serializable]
public class Pedido
{
    public string nome;
    public int recompensa;

    public Pedido(string nome, int recompensa)
    {
        this.nome = nome;
        this.recompensa = recompensa;
    }
}