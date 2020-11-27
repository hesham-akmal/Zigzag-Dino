using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class CFX_AutoDestructShuriken : MonoBehaviour
{
    private ParticleSystem ps;

    private void Start()
    {
        ps = GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        CheckIfAlive();
    }

    private void CheckIfAlive()
    {
        if (ps != null && !ps.IsAlive(true))
            Lean.Pool.LeanPool.Despawn(gameObject);
    }
}