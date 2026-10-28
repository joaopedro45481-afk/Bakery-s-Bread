using UnityEngine;

public class NPCData : MonoBehaviour
{
    [Header("Identificação")]
    public string npcID;
    public string npcName;

    [Header("Visual")]
    public Sprite portrait;

    [Header("Personalidade")]
    [TextArea]
    public string[] frasesPedido;
}