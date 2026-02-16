using UnityEngine;
using Zenject;

public class InputService : MonoBehaviour, IInputService
{
    [Inject] private IGameService _gameService;

    public bool isActive;
    public GameManager gameplayManager;
    public InputMethod inputType;

    public bool IsActive { get => isActive; set => isActive = value; }
    public GameManager GameplayManager { get => gameplayManager; set => gameplayManager = value; }
    public InputMethod InputType { get => inputType; set => inputType = value; }

    void Awake()
    {
#if UNITY_EDITOR
        inputType = InputMethod.MouseInput;
#else
        inputType = InputMethod.MouseInput;
#endif
    }

    void Update()
    {
        if (isActive)
        {
            if (inputType == InputMethod.KeyboardInput)
                KeyboardInput();
            else if (inputType == InputMethod.MouseInput)
                MouseInput();
        }
    }

    #region KEYBOARD
    void KeyboardInput()
    {
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.UpArrow))
            RotateClockWise(false);
        else if (Input.GetKeyDown(KeyCode.D))
            RotateClockWise(true);

        if (Input.GetKeyDown(KeyCode.LeftArrow))
            MoveHorizontal(Vector2.left);
        else if (Input.GetKeyDown(KeyCode.RightArrow))
            MoveHorizontal(Vector2.right);
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            InstantFall();
        }
    }
    #endregion

    #region MOUSE
    Vector2 _firstPressPosition;
    Vector2 _startPressPosition;
    Vector2 _endPressPosition;
    Vector2 _currentSwipe;
    float _buttonDownPhaseStart;
    public float tapInterval;

    void MouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _firstPressPosition = _startPressPosition = gameplayManager.gameplayCamera.ScreenToWorldPoint(Input.mousePosition);
            _buttonDownPhaseStart = Time.time;
        }

        if (Input.GetMouseButton(0))
        {
            _endPressPosition = gameplayManager.gameplayCamera.ScreenToWorldPoint(Input.mousePosition);
            _currentSwipe = new Vector2(_endPressPosition.x - _startPressPosition.x, _endPressPosition.y - _startPressPosition.y);

            if (_currentSwipe.x < -0.75f && _currentSwipe.y > -0.75f && _currentSwipe.y < 0.75f)
            {
                _startPressPosition = _endPressPosition;
                MoveHorizontal(Vector2.left);
            }
            if (_currentSwipe.x > 0.75f && _currentSwipe.y > -0.75f && _currentSwipe.y < 0.75f)
            {
                _startPressPosition = _endPressPosition;
                MoveHorizontal(Vector2.right);
            }
            if (_currentSwipe.y < -0.75f && _currentSwipe.x > -0.75f && _currentSwipe.x < 0.75f)
            {
                InstantFall();
            }
            if (_currentSwipe.y > 0.75f && _currentSwipe.x > -0.75f && _currentSwipe.x < 0.75f)
            {
                NormalFall();
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            NormalFall();
            _endPressPosition = gameplayManager.gameplayCamera.ScreenToWorldPoint(Input.mousePosition);
            _currentSwipe = new Vector2(_endPressPosition.x - _firstPressPosition.x, _endPressPosition.y - _firstPressPosition.y);

            if (_currentSwipe.x > -0.05f && _currentSwipe.x < 0.05f && _currentSwipe.y > -0.05f && _currentSwipe.y < 0.05f)
            {
                RotateClockWise(true);
            }
        }
    }
    #endregion

    #region Direction Callbacks

    private void MoveHorizontal(Vector2 vector2)
    {
        if (_gameService.GameStatus != GameStatus.ONGOING)
            return;
    }

    private void InstantFall()
    {
        if (_gameService.GameStatus != GameStatus.ONGOING)
            return;
    }

    private void NormalFall()
    {
        if (_gameService.GameStatus != GameStatus.ONGOING)
            return;
    }

    private void RotateClockWise(bool rotateClockwise)
    {
        if (_gameService.GameStatus != GameStatus.ONGOING)
            return;
    }
    #endregion
}