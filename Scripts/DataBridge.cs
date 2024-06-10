using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;

public class DataBridge : MonoBehaviour
{
    [SerializeField] private Modificator[] _modificators;
    [SerializeField] private Game _game;
    private DatabaseReference _databaseReference;

    private void Start()
    {
        _databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
        KeyTransfer transfer = FindObjectOfType<KeyTransfer>();
        if(transfer != null)
        {
            if(transfer.Key != null && transfer.UseKey) 
            {
                _game.Init.AddListener(() => LoadData(transfer.Key));
            }
            Destroy(transfer.gameObject);
        }
    }

    private void OnApplicationQuit()
    {
        SaveData();
    }

    public void SaveData()
    {
        DatabaseReference gameInfo = _databaseReference.Child("Games").Child(_game.ID);
        DatabaseReference playersInfo = gameInfo.Child("Players");
        DatabaseReference fieldInfo = gameInfo.Child("Field");

        string jsonBluePlayer = JsonUtility.ToJson(new PlayerData(_game.BluePlayer));
        playersInfo.Child("Blue").SetRawJsonValueAsync(jsonBluePlayer);
        string jsonRedPlayer = JsonUtility.ToJson(new PlayerData(_game.RedPlayer));
        playersInfo.Child("Red").SetRawJsonValueAsync(jsonRedPlayer);

        gameInfo.Child("CurrentPlayer").SetValueAsync(_game.CurrentPlayer.Title);

        string jsonCurrentCell;
        for (int index = 0; index < 72; index++)
        {
            jsonCurrentCell = JsonUtility.ToJson(new CellData(_game.Field[index]));
            fieldInfo.Child("Cell-" + index).SetRawJsonValueAsync(jsonCurrentCell);
        }
    }

    private void LoadData(string gameID)
    {
        StartCoroutine(LoadDataEnum(gameID));
    }

    private IEnumerator LoadDataEnum(string gameID)
    {
        _game.SetID(gameID);
        var gameData = _databaseReference.Child("Games").Child(gameID).GetValueAsync();
        yield return new WaitUntil(predicate: () => gameData.IsCompleted);

        DataSnapshot snapshotBluePlayer = gameData.Result.Child("Players").Child("Blue");
        string jsonBluePlayerData = snapshotBluePlayer.GetRawJsonValue();
        if (jsonBluePlayerData != null)
        {
            PlayerData bluePlayer = JsonUtility.FromJson<PlayerData>(jsonBluePlayerData);
            _game.BluePlayer.SetMoney(bluePlayer.Money);
            _game.BluePlayer.SetTime(bluePlayer.Time);
        }

        DataSnapshot snapshotRedPlayer = gameData.Result.Child("Players").Child("Red");
        string jsonRedPlayerData = snapshotRedPlayer.GetRawJsonValue();
        if (jsonRedPlayerData != null)
        {
            PlayerData redPlayer = JsonUtility.FromJson<PlayerData>(jsonRedPlayerData);
            _game.BluePlayer.SetMoney(redPlayer.Money);
            _game.BluePlayer.SetTime(redPlayer.Time);
        }


        for (int index = 0; index < 72; index++)
        {
            DataSnapshot snapshotCell = gameData.Result.Child("Field").Child("Cell-" + index);
            string jsonCellData = snapshotCell.GetRawJsonValue();
            if (jsonCellData != null)
            {
                CellData cell = JsonUtility.FromJson<CellData>(jsonCellData);
                _game.Field[index].IsRoyal = cell.IsRoyal;
                if (cell.Owner == "Nobody")
                {
                    _game.Field[index].SetOwner(null);
                }
                else
                {
                    _game.Field[index].SetOwner(cell.Owner == "Blue" ? _game.BluePlayer : _game.RedPlayer);
                }
                if (cell.Modificator == "Empty")
                {
                    _game.Field[index].SetModificator(null);
                }
                else
                {
                    foreach (Modificator mod in _modificators)
                    {
                        if (mod.Title == cell.Modificator)
                        {
                            _game.Field[index].SetModificator(mod);
                        }
                        if (mod.Title == "Royal")
                        {
                            _game.Field[index].Owner.SetRoyal(_game.Field[index]);
                        }
                    }

                }
            }
        }

        DataSnapshot snapshotCurrentPlayer = gameData.Result.Child("CurrentPlayer");
        string CurrentPlayerData = snapshotCurrentPlayer.Value.ToString();
        Debug.Log(CurrentPlayerData);
        _game.SetCurrentPlayer(CurrentPlayerData == "Red" ? _game.RedPlayer : _game.BluePlayer);
    }

}

[Serializable]
public class PlayerData
{
    public int Money;
    public float Time;

    public PlayerData(Player player)
    {
        Money = player.Money;
        Time = player.Time;
    }
}

[Serializable]
public class CellData
{
    public bool IsRoyal;
    public string Owner;
    public string Modificator;


    public CellData(Cell cell)
    {
        IsRoyal = cell.IsRoyal;
        Owner = cell.Owner != null ? cell.Owner.Title : "Nobody";
        Modificator = cell.GetModificator != null ? cell.GetModificator.Title : "Empty";
    }
}
