using UnityEngine;

public class NoteVisualizer
{
    private readonly GameObject notePrefab;
    private readonly Transform parentTransform;

    public NoteVisualizer(GameObject _notePrefab, Transform _parentTransform)
    {
        notePrefab = _notePrefab;
        parentTransform = _parentTransform;
    }

    public void VisualizeNotePlacement(Note _note)
    {
        var noteVisual = Object.Instantiate(notePrefab, parentTransform);
        noteVisual.transform.position = new Vector3(_note.Pos.x, _note.Pos.y, 0f);
    }

    public void RemoveNoteVisual(Vector2Int _pos)
    {
        foreach (Transform child in parentTransform)
        {
            if (child == null) continue;
            var childPosition = child.position;
            if (!Mathf.Approximately(childPosition.x, _pos.x) ||
                !Mathf.Approximately(childPosition.y, _pos.y)) continue;
            Object.Destroy(child.gameObject);
            return;
        }
    }
}