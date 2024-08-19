using Godot;


public interface IClickable{
    int PlayerId { get; }
    bool IsClicked(Vector2 position);
}

public interface ISelectable : IClickable
{
    bool Selected { get; set; }
}

public interface IDoubleClickable : IClickable
{
    void DoubleClicked();
}