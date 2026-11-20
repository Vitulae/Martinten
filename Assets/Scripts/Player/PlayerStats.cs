
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Player Info")] //wszystko pewnie jasne
    [SerializeField] private string playerName = "Player";
    [SerializeField] private int maxHP = 20;
    [SerializeField] private int currentHP = 20;

    public string PlayerName => playerName;
    public int MaxHP => maxHP;
    public int CurrentHP => currentHP;

    private void OnValidate()
    {
        maxHP = Mathf.Max(1, maxHP); //max hp nie moze byc mniejsze niz 1
        currentHP = Mathf.Clamp(currentHP, 0, maxHP); //nie moze byc mniejsze niz 0
    }
}
