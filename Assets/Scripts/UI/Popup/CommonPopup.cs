using UnityEngine.UI;
using UnityEngine;
using System;
using TMPro;
using UniRx;
using Zenject;

public class CommonPopup : GameMonoBehaviour
{
    [Inject] private IAudioService _audioService;
    [Inject] private IBackLogService _backLogService;

    public Button closeButton, leftButton, rightButton;
    public TextMeshProUGUI titleText, descriptionText, leftButtonText, rightButtonText;
    Action leftButtonListener, rightButtonListener;

    public IReactiveProperty<string> title = new ReactiveProperty<string>();
    public IReactiveProperty<string> description = new ReactiveProperty<string>();
    public IReactiveProperty<string> leftButtonContent = new ReactiveProperty<string>();
    public IReactiveProperty<string> rightButtonContent = new ReactiveProperty<string>();

    private void Awake()
    {
        closeButton.onClick.AsObservable().Subscribe(x =>
        {
            _audioService.PlayUIClick();
            _backLogService.CloseLastUI();
        });

        rightButton.onClick.AsObservable().Subscribe(x =>
        {
            rightButtonListener?.Invoke();
            _audioService.PlayUIClick();
            _backLogService.CloseLastUI();
        });

        leftButton.onClick.AsObservable().Subscribe(x =>
        {
            leftButtonListener?.Invoke();
            _audioService.PlayUIClick();
            _backLogService.CloseLastUI();
        });

        title.AsObservable().SubscribeToText(titleText);
        description.AsObservable().SubscribeToText(descriptionText);
        leftButtonContent.AsObservable().SubscribeToText(leftButtonText);
        rightButtonContent.AsObservable().SubscribeToText(rightButtonText);
    }

    public void OpenPopup(string title, string description, string leftButtonContent, string rightButtonContent, Action leftButtonCallback, Action rightButtonCallback)
    {
        leftButtonListener = leftButtonCallback;
        rightButtonListener = rightButtonCallback;

        this.title.Value = title;
        this.description.Value = description;
        this.leftButtonContent.Value = leftButtonContent;
        this.rightButtonContent.Value = rightButtonContent;

        this.Show(BacklogType.KeepPreviousScreen);
    }
}
