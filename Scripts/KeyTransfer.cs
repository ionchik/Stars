using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyTransfer : MonoBehaviour
{
    public string Key;
    public bool UseKey;

    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public void Use()
    {
        UseKey = true;
    }
}
