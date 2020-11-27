using UnityEngine;

public class CactusTile : MonoBehaviour
{
    [SerializeField] private GameObject Cactus1 = default;

    [SerializeField] private GameObject Cactus2 = default;

    public void RandomizeDir()
    {
        if (GameSettings.RandomBool())
        {
            transform.rotation = Quaternion.Euler(0, 45, 180);
            Cactus1.SetActive(false);
            Cactus2.SetActive(true);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, -45, 0);
            Cactus1.SetActive(true);
            Cactus2.SetActive(false);
        }
    }
}