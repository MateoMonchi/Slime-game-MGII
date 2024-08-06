using UnityEngine;

public class GameLayer : MonoBehaviour
{
    [SerializeField] LayerMask solidObjectsLayer;
    [SerializeField] LayerMask interactuableLayer;
    [SerializeField] LayerMask grassLayer;
    [SerializeField] LayerMask fovLayer;
    [SerializeField] LayerMask playerLayer;
    public static GameLayer Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public LayerMask PlayerLayer => playerLayer;
    public LayerMask FovLayer => fovLayer;
    public LayerMask SolidLayer => solidObjectsLayer;

    public LayerMask InteractuableLayer => interactuableLayer;

    public LayerMask GrassLayer => grassLayer;
}
