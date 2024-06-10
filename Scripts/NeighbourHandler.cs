using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeighbourHandler : MonoBehaviour
{
    [SerializeField] private Vector2[] _directions;

    public List<Cell> GetNeighbours()
    {
        Vector2 startPosition;
        List<Cell> neighbours = new List<Cell>();

        foreach (Vector2 direction in _directions)
        {
            startPosition = new Vector2(transform.position.x, transform.position.y) + direction;
            RaycastHit2D hit = Physics2D.Raycast(startPosition, direction, 1);
            if(hit == false) continue;
            if (hit.collider.TryGetComponent<Cell>(out Cell neighbour))
            {
                neighbours.Add(neighbour);
            }
        }
        
        return neighbours;
    }
}
