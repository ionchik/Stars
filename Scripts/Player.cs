using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Player : MonoBehaviour
{
    [SerializeField] private Attribution _attribution;
    [SerializeField] private float _time;

    public string Title;

    public UnityEvent<int> MoneyChange = new UnityEvent<int>();
    public UnityEvent<int> LandsChange = new UnityEvent<int>();
    public UnityEvent<float> TimeChange = new UnityEvent<float>();
    public UnityEvent TimeEnd = new UnityEvent();

    private Cell _royalCell;
    private List<Cell> _lands;
    private int _money;
    private IEnumerator _timer;

    public Attribution GetAttribution => _attribution;
    public Cell RoyalCell => _royalCell;
    public bool HasRoyal => _royalCell != null;
    public int LandsNumber => _lands.Count;
    public int Money => _money;
    public float Time => _time;

    public void SetRoyal(Cell royal)
    {
        _royalCell = royal;
    }

    public void AddLand(Cell cell)
    {
        _lands.Add(cell);
        LandsChange?.Invoke(LandsNumber);
    }

    public void RemoveLand(Cell cell)
    {
        _lands.Remove(cell);
        LandsChange?.Invoke(LandsNumber);
    }

    public void Income()
    {
        _money += _lands.Select(land => land.GetIncome()).Sum();
        MoneyChange?.Invoke(_money);
    }

    public bool Buy(int price)
    {
        if (_money < price) return false; 
        _money -= price;
        MoneyChange?.Invoke(_money);
        return true;
    }

    private void Awake()
    {
        _money = 0;
        _lands = new List<Cell>();
        _timer = Timer();
    }

    public void StartTimer()
    {
        StartCoroutine(_timer);
    }

    public void StopTimer()
    {
        StopCoroutine(_timer);
    }

    private IEnumerator Timer()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(1);
            _time -= 1;
            TimeChange?.Invoke(_time);
            if( _time <= 0 ) TimeEnd?.Invoke();
        }
    }

    public void SetTime(float time)
    {
        _time = time;
        TimeChange?.Invoke(time);
    }

    public void SetMoney(int money)
    {
        _money = money;
        MoneyChange?.Invoke(money);
    }
}
