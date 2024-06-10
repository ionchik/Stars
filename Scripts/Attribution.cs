using UnityEngine;

[CreateAssetMenu(fileName = "Attribution", menuName = "Create player attribution", order = 1)]
public class Attribution : ScriptableObject
{
    [SerializeField] private Sprite _cellSprite;
    [SerializeField] private Sprite _stickSprite;
    [SerializeField] private Color _symbolColor;
    [SerializeField] private ParticleSystem _explosion;

    public Sprite CellSprite => _cellSprite;
    public Sprite StickSprite => _stickSprite;
    public Color SymbolColor => _symbolColor;
    public ParticleSystem Explosion => _explosion;
}
