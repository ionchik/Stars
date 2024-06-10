using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfoHandler : MonoBehaviour
{
    [SerializeField] private Player _player;
    [SerializeField] private Text _money;
    [SerializeField] private Text _lands;
    [SerializeField] private Image _timer;

    private void Awake()
    {
        _player.MoneyChange.AddListener(UpdateMoney);
        _player.LandsChange.AddListener(UpdateLands);
        _player.TimeChange.AddListener(UpdateTime);
    }

    private void OnDestroy()
    {
        _player.MoneyChange.RemoveListener(UpdateMoney);
        _player.LandsChange.RemoveListener(UpdateLands);
        _player.TimeChange.RemoveListener(UpdateTime);
    }

    private void UpdateMoney(int money)
    {
        _money.text = money.ToString();
    }

    private void UpdateLands(int lands)
    {
        _lands.text = lands.ToString();
    }

    private void UpdateTime(float time)
    {
        _timer.fillAmount = time / 300;
    }
}
