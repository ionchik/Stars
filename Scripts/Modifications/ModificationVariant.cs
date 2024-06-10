using UnityEngine;

public class ModificationVariant : MonoBehaviour
{
    [SerializeField] private Modificator _modificator;
    public Modificator GetModificator => _modificator;
}
