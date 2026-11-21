
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleUIController : MonoBehaviour
{
    [Header("Enemy UI")]
    [SerializeField] private TMP_Text enemyNameText;
    [SerializeField] private TMP_Text enemyHPText;
    [SerializeField] private Image enemyHPFill;

    [Header("Player UI")]
    [SerializeField] private TMP_Text playerNameText;
    [SerializeField] private TMP_Text playerHPText;
    [SerializeField] private Image playerHPFill;

    private EnemyEncounter currentEnemy;
    private PlayerStats currentPlayer;

    public void ShowBattle(EnemyEncounter enemy, PlayerStats player)
    {
        currentEnemy = enemy;
        currentPlayer = player;
        RefreshUI();
    }

    public void RefreshUI()
    {
        if (currentEnemy == null || currentPlayer == null)
            return;

        enemyNameText.text = currentEnemy.EnemyName;
        enemyHPText.text = $"HP: {currentEnemy.CurrentHP} / {currentEnemy.MaxHP}";

        enemyHPFill.fillAmount = CalculateHP(currentEnemy.CurrentHP, currentEnemy.MaxHP);

        playerNameText.text = currentPlayer.PlayerName;
        playerHPText.text = $"HP: {currentPlayer.CurrentHP} / {currentPlayer.MaxHP}";

        playerHPFill.fillAmount = CalculateHP(currentPlayer.CurrentHP, currentPlayer.MaxHP);
    }

    private float CalculateHP(int current, int max)
    {
        if (max <= 0)
            return 0f;

        return Mathf.Clamp01((float)current / max);
    }
}
