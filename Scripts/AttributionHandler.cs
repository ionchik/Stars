using UnityEngine;

public class AttributionHandler : MonoBehaviour
{
    [SerializeField] private ModificationStick _modificationStick;
    [SerializeField] private SpriteRenderer _modificatorRenderer;
    [SerializeField] private SpriteRenderer _cellRenderer;
    [SerializeField] private SpriteRenderer _stickRenderer;
    [SerializeField] private Sprite _emptyCell;
    [SerializeField] private Color _emptyColor;
    [SerializeField] private Cell _cell;

    private ParticleSystem _explosion;

    private void OnEnable()
    {
        _cell.OwnerChanged.AddListener(ChangeAttribution);
        _cell.ModificationStarted.AddListener(ShowStick);
        _cell.ModificationFinished.AddListener(HideStick);
        _cell.ModificatorChanged.AddListener(ChangeModificatorIcon);
    }

    private void OnDisable()
    {
        _cell.OwnerChanged.RemoveListener(ChangeAttribution);
        _cell.ModificationStarted.RemoveListener(ShowStick);
        _cell.ModificationFinished.RemoveListener(HideStick);
        _cell.ModificatorChanged.RemoveListener(ChangeModificatorIcon);
    }

    private void ChangeAttribution(Player newOwner)
    {
      
        if (newOwner == null)
        {
            if(_explosion != null) Destroy(Instantiate(_explosion, transform), 2);
            _cellRenderer.sprite = _emptyCell;
        }
        else
        {
            _cellRenderer.sprite = newOwner.GetAttribution.CellSprite;
            _stickRenderer.sprite = newOwner.GetAttribution.StickSprite;
            _explosion = newOwner.GetAttribution.Explosion;
            Destroy(Instantiate(_explosion, transform), 2);
        }
    }

    private void ChangeModificatorIcon(Modificator modificator)
    {
        if (modificator == null)
        {
            _modificatorRenderer.sprite = null;
        }
        else
        {
            _modificatorRenderer.sprite = modificator.Icon;
        }
    }

    private void ShowStick()
    {
        _modificationStick.gameObject.SetActive(true);
    }

    private void HideStick()
    {
        _modificationStick.gameObject.SetActive(false);
    }
}
