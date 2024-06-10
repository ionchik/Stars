using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;

public class PreviousData : MonoBehaviour
{
    [SerializeField] private KeyTransfer keyTransfer;
    private DatabaseReference _databaseReference;

    private void Start()
    {
        _databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
        StartCoroutine(GetGames());
    }

    private IEnumerator GetGames()
    {
        var gamesData = _databaseReference.Child("Games").GetValueAsync();
        yield return new WaitUntil(predicate: () => gamesData.IsCompleted);
        DataSnapshot gamesSnapshot = gamesData.Result;
        foreach (DataSnapshot game in gamesSnapshot.Children)
        {
            keyTransfer.Key = game.Key;
            break;
        }
    }
}
