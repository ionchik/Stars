using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    [SerializeField] private Modificator _royalModificator;
    [SerializeField] private Player _bluePlayer;
    [SerializeField] private Player _redPlayer;
    [SerializeField] private Cell _startCell;
    [SerializeField] private int _fieldSize;
    [SerializeField] private float _delay;

    [SerializeField] private Image _finishScreen;

    public Modificator Mill;
    public Modificator Attack;
    public Modificator Tower;
    public Modificator Scare;

    public UnityEvent Init;
    public UnityEvent<Player> TurnChanged;
    public UnityEvent<Player> Finished;
    public Modificator RoyalModificator => _royalModificator;
    public bool AttackChoosing = false;

    private List<Cell> _field;
    private Player _currentPlayer;
    private string _id;

    public string ID => _id;
    public Player CurrentPlayer => _currentPlayer;
    public Player BluePlayer => _bluePlayer;
    public Player RedPlayer => _redPlayer;
    public List<Cell> Field => _field;

    private void Awake()
    {
        _field = new List<Cell>(72);
        TurnChanged = new UnityEvent<Player>();
        Finished = new UnityEvent<Player>();
        _id = System.DateTime.UtcNow.ToString("HH:mm dd MMMM, yyyy");
    }

    public void SetID(string id)
    {
        _id = id;
    }

    private void Start()
    {
        _bluePlayer.TimeEnd.AddListener(() => { Finish(_redPlayer); });
        _redPlayer.TimeEnd.AddListener(() => { Finish(_bluePlayer); });
        _currentPlayer = _bluePlayer;
        StartCoroutine(GenerateField());
    }

    private IEnumerator GenerateField()
    {
        WaitForSeconds wait = new WaitForSeconds(_delay);
        for (int index = 0; index < _fieldSize; index++)
        {
            _field.Add(Instantiate(_startCell, transform, false));
            _field[index].SetGame(this);
            _field[index].Interacted.AddListener(ChangeTurn);
            if (index < 12) _field[index].RoyalPlayer = _redPlayer;
            if (index > 59) _field[index].RoyalPlayer = _bluePlayer;
            yield return wait;
        }
        Init?.Invoke();
        _currentPlayer.StartTimer();
    }

    private void ChangeTurn()
    {
        _currentPlayer.StopTimer();
        if(_bluePlayer.HasRoyal && _redPlayer.HasRoyal)
        {
            foreach (Cell cell in _field)
            {
                cell.IsRoyalConnected = false;
            }
            _bluePlayer.RoyalCell.ConnectRoyal();
            _redPlayer.RoyalCell.ConnectRoyal();
        }
        _currentPlayer = _currentPlayer == _bluePlayer ? _redPlayer : _bluePlayer;
        _currentPlayer.Income();
        TurnChanged?.Invoke(_currentPlayer);
        _currentPlayer.StartTimer();
    }

    public void StopGame()
    {
        _currentPlayer.StopTimer();
    }

    public void ContinueGame()
    {
        _currentPlayer.StartTimer();
    }

    public void Finish(Player player)
    {
        _finishScreen.color = player.GetAttribution.SymbolColor;
        StartCoroutine(ShowFinishMenu());
    }

    private IEnumerator ShowFinishMenu()
    {
        yield return new WaitForSeconds(1);
        _finishScreen.gameObject.SetActive(true);
    }

    public void SetCurrentPlayer(Player player)
    {
        _currentPlayer.StopTimer();
        _currentPlayer = player;
        TurnChanged?.Invoke(_currentPlayer);
        _currentPlayer.StartTimer();
    }
}
