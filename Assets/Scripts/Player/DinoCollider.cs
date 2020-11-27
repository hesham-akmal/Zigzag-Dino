using UnityEngine;

public class DinoCollider : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Cactus"))
        {
            other.GetComponent<Animator>().Play("CactusBounce");
            PlayerManager.Instance.OnTriggerWithCactus();
        }
        else if (other.CompareTag("TileJump") && PlayerManager.Instance.AutoNavigate)
            PlayerManager.Instance.Jump();
        else if (other.CompareTag("Coin"))
            PlayerManager.Instance.CollectCoin(other.gameObject);
    }
}