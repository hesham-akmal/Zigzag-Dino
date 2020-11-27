using UnityEngine;

public class TrickTrigger : MonoBehaviour
{
    [SerializeField] private Animator animator = default;

    [SerializeField] private Transform PatternsParent = default;

    private void OnTriggerEnter(Collider other)
    {
        //Trigger flying tiles animation
        if (other.CompareTag("Player"))
            animator.Play("MainAnim");
    }

    private void OnEnable()
    {
        //Disable all patterns game objs
        foreach (Transform t in PatternsParent)
            t.gameObject.SetActive(false);

        //Enable only 1 pattern game obj
        PatternsParent.GetChild(Random.Range(0, PatternsParent.childCount)).gameObject.SetActive(true);
    }
}