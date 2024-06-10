using UnityEngine;

[CreateAssetMenu(fileName = "Modificator", menuName = "Create cell modificator", order = 2)]
public class Modificator: ScriptableObject
{
    [SerializeField] private string _title;
    [SerializeField] private Sprite _icon;
    [SerializeField] private int _price;
    public Sprite Icon => _icon;
    public int Price => _price;
    public string Title => _title;
}
