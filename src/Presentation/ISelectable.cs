using Godot;

public interface ISelectable
{
    int PlayerId { get; }
    bool Selected { get; set; }

    bool IsClicked(Vector2 position);
}