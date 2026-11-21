
using System.Collections;
using UnityEngine;

public class GameModeController : MonoBehaviour
{
    public static GameModeController Instance { get; private set; }

    [Header("Game Modes")]
    [SerializeField] private GameObject explorationRoot;
    [SerializeField] private GameObject battleRoot;

    [Header("Battle UI")]
    [SerializeField] private BattleUIController battleUIController;

    private bool isBattleActive = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        explorationRoot.SetActive(true);
        battleRoot.SetActive(false);

        isBattleActive = false;
    }

    public void StartBattle(
        EnemyEncounter enemy,
        PlayerStats player)
    {
        if (isBattleActive || enemy == null || player == null)
            return;

        isBattleActive = true;

        StartCoroutine(SwitchToBattle(enemy, player));
    }

    private IEnumerator SwitchToBattle(
        EnemyEncounter enemy,
        PlayerStats player)
    {
        yield return null;

        explorationRoot.SetActive(false);
        battleRoot.SetActive(true);

        battleUIController.ShowBattle(enemy, player);

        Debug.Log("Battle Mode activated!");
    }

    public void ReturnToExploration()
    {
        if (!isBattleActive)
            return;

        battleRoot.SetActive(false);
        explorationRoot.SetActive(true);

        isBattleActive = false;

        Debug.Log("Returned to Exploration Mode!");
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}
