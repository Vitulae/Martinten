
using TMPro;
using UnityEngine;
using System.Collections;

public class GameModeController : MonoBehaviour
{
    public static GameModeController Instance { get; private set; }

    [Header("Game Modes")]
    [SerializeField] private GameObject explorationRoot;
    [SerializeField] private GameObject battleRoot;

    [Header("Battle UI")]
    [SerializeField] private TMP_Text enemyNameText;
    [SerializeField] private TMP_Text enemyHPText;
    [SerializeField] private TMP_Text playerNameText;
    [SerializeField] private TMP_Text playerHPText;

    private bool isBattleActive = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        battleRoot.SetActive(false);
    }

    public void StartBattle(EnemyEncounter enemy, PlayerStats player)
    {
        if (isBattleActive || enemy == null || player == null)
            return;

        isBattleActive = true;

        //Enemy info
        enemyNameText.text = enemy.EnemyName;
        enemyHPText.text = "HP: " + enemy.MaxHP + " / " + enemy.MaxHP;

        // Player info
        playerNameText.text = player.PlayerName;
        playerHPText.text = "HP: " + player.CurrentHP + " / " + player.MaxHP;

        StartCoroutine(SwitchToBattle());
    }

    private IEnumerator SwitchToBattle()
    {
        // czekamy na fizyczke zeby sie ogarnela
        yield return null;

        battleRoot.SetActive(true);
        explorationRoot.SetActive(false);

        Debug.Log("Battle STAAAART!");
    }

    public void ReturnToExploration()
    {
        if (!isBattleActive)
            return;

        //chowamy battle screen
        battleRoot.SetActive(false);
        //i siup do eksploracji
        explorationRoot.SetActive(true);
        isBattleActive = false;

        Debug.Log("Returned to Exploration!");
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}
