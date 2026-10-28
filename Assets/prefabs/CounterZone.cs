using UnityEngine;

public class CounterZone : MonoBehaviour
{
    public bool playerOnCounter = false;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            playerOnCounter = true;
        }
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            playerOnCounter = false;
        }
    }
}