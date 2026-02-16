public interface IInputService
{
    bool IsActive { get; set; }
    GameManager GameplayManager { get; set; }
    InputMethod InputType { get; set; }
}
