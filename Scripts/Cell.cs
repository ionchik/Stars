using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(TouchHandler))]
public class Cell : MonoBehaviour
{
    [SerializeField] private TouchHandler _touchHandler;
    [SerializeField] private NeighbourHandler _neighbourHandler;

    public Player RoyalPlayer;
    public bool IsRoyal = false;
    public UnityEvent Interacted;
    public bool IsRoyalConnected = true;
    public UnityEvent<Player> OwnerChanged;
    public UnityEvent ModificationStarted;
    public UnityEvent ModificationFinished;
    public UnityEvent<Modificator> ModificatorChanged;

    private Game _game;
    private Player _owner;
    private List<Cell> _neighbours;
    private Modificator _modificator;

    public Player Owner => _owner;
    public Modificator GetModificator => _modificator;

    public int GetIncome()
    {
        return _neighbours.Any(n => n.GetModificator == _game.Mill) ? 2 : 1;
    }

    public void SetGame(Game game)
    {
        _game = game;
        _game.Init.AddListener(SetNeighbours);
        _game.TurnChanged.AddListener(OnTurnChanged);
    }

    public bool CanCapture(Player player)
    {
        if (_owner == player) return false;
        if (_modificator == _game.Tower) return false;
        if (_neighbours.Any(n => n.GetModificator == _game.Scare && n.Owner != player)) return false;
        return _neighbours.Any(n => n.Owner == player);
    }

    public void ConnectRoyal()
    {
        if (IsRoyal || IsRoyalConnected)
        {
            foreach (Cell cell in _neighbours.Where(n => n.Owner == _owner && !n.IsRoyalConnected).ToList())
            {
                cell.IsRoyalConnected = true;
                cell.ConnectRoyal();
            }
        }
    }

    private void Awake()
    {
        Interacted = new UnityEvent();
        ModificationStarted = new UnityEvent();
        ModificationFinished = new UnityEvent();
        OwnerChanged = new UnityEvent<Player>();
    }

    private void OnEnable()
    {
        _touchHandler.TouchDown.AddListener(ChangeOwner);
        _touchHandler.TouchDown.AddListener(StartModification);
        _touchHandler.TouchUp.AddListener(FinishModification);
        _touchHandler.Choose.AddListener(ChangeModificator);
        _touchHandler.TouchDown.AddListener(Attack);
        _touchHandler.TouchDown.AddListener(Act);
    }

    private void OnDisable()
    {
        _touchHandler.TouchDown.RemoveListener(ChangeOwner);
        _touchHandler.TouchDown.RemoveListener(StartModification);
        _touchHandler.TouchUp.RemoveListener(FinishModification);
        _touchHandler.Choose.RemoveListener(ChangeModificator);
        _touchHandler.TouchDown.RemoveListener(Attack);
        _touchHandler.TouchDown.RemoveListener(Act);     
    }

    private void ChangeOwner()
    {
        if (_game.CurrentPlayer.HasRoyal == false)
        {
            if (RoyalPlayer != _game.CurrentPlayer) return;
            _modificator = _game.RoyalModificator;
            ModificatorChanged?.Invoke(_modificator);
            _game.CurrentPlayer.SetRoyal(this);
            IsRoyal = true;
            _owner = _game.CurrentPlayer;
            OwnerChanged?.Invoke(_owner);
            Interacted?.Invoke();
        }
        else if (CanCapture(_game.CurrentPlayer))
        {
            _owner = _game.CurrentPlayer;
            if (IsRoyal) _game.Finish(_owner);
            _owner.AddLand(this);
            OwnerChanged?.Invoke(_owner);
            Interacted?.Invoke();
        }
    }

    private void StartModification()
    {
        if (_game.AttackChoosing) return;
        if (_owner != _game.CurrentPlayer) return;
        if (_modificator != null) return;
        ModificationStarted?.Invoke();
    }

    private void ChangeModificator(Modificator modificator)
    {
        if (_owner != _game.CurrentPlayer) return;
        if (!_owner.Buy(modificator.Price)) return;

        _modificator = modificator;
        ModificatorChanged?.Invoke(_modificator);
        Interacted?.Invoke();
    }

    private void FinishModification()
    {
        if (_owner != _game.CurrentPlayer) return;
        ModificationFinished?.Invoke();
    }

    private void SetNeighbours()
    {
        _neighbours = _neighbourHandler.GetNeighbours();
    }

    private void OnTurnChanged(Player player)
    {
        if (!IsRoyal && !IsRoyalConnected && _owner != null)
        {
            Clear();
        }
    }

    private void Clear()
    {
        _owner.RemoveLand(this);
        _owner = null;
        _modificator = null;
        OwnerChanged?.Invoke(_owner);
        ModificatorChanged?.Invoke(_modificator);
    }

    private void Act()
    {
        if (_owner != _game.CurrentPlayer) return;
        if (_modificator == _game.Attack)
        {
            _game.AttackChoosing = true;
            _modificator = null;
            ModificatorChanged?.Invoke(_modificator);
        }
    }

    private void Attack()
    {
        if (!_game.AttackChoosing) return;
        if (IsRoyal) return;
        if (_modificator)
        {
            _modificator = null;
            ModificatorChanged?.Invoke(_modificator);
        }
        else 
        {
            Clear();
        }
        _game.AttackChoosing = false;
        Interacted?.Invoke();
    }

    public void SetModificator(Modificator mod)
    {
        _modificator = mod;
        ModificatorChanged?.Invoke(mod);
    }

    public void SetOwner(Player owner) 
    {
        _owner = owner;
        OwnerChanged?.Invoke(owner);
    }
}
