
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class EnemyEncounter : MonoBehaviour
{
    [Header("Enemy Info")]
    [SerializeField] private string enemyName = "Enemy";
    [SerializeField] private int maxHP = 15;
    private int currentHP;
    public int CurrentHP => currentHP;
    private bool encounterTriggered = false;
    public string EnemyName => enemyName;
    public int MaxHP => maxHP;

    private void Reset()
    {
        GetComponent<SphereCollider>().isTrigger = true;
    }
    
    private void Awake()
    {
        maxHP = Mathf.Max(1, maxHP); //nie moze byc mniejsze niz 1
        currentHP = maxHP;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (encounterTriggered)
            return;

        PlayerStats player = other.GetComponentInParent<PlayerStats>();

        if (player == null)
            return;

        if (GameModeController.Instance == null)
        {
            Debug.LogError("GameModeController not found!");
            return;
        }
        encounterTriggered = true;
        GameModeController.Instance.StartBattle(this, player);
    }
}
