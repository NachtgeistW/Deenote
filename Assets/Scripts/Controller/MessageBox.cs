﻿using UnityEngine;
using UnityEngine.UI;

public class MessageBox : Window
{
    public static MessageBox Instance { get; private set; }
    [SerializeField] private LocalizedText _content;
    [SerializeField] private Button[] _buttons;
    [SerializeField] private LocalizedText[] _buttonTexts;
    [SerializeField] private RectTransform[] _buttonTransforms;
    private float[] _buttonWidths = { 0.0f, 200.0f, 150.0f, 110.0f, 85.0f };
    public class ButtonInfo
    {
        public Callback callback = null;
        public string[] texts;
    }
    protected override void Open()
    {
        base.Open();
        MoveToCenter();
        operations.Clear();
        operations.Add(new Operation
        {
            shortcut = new Shortcut { key = KeyCode.Return },
            callback = () => { _buttons[0].onClick.Invoke(); }
        });
    }
    public static void Activate(string[] title, string[] content, params ButtonInfo[] buttonInfos)
    {
        Instance.Open(title, content, buttonInfos);
    }
    private void Open(string[] title, string[] content, ButtonInfo[] buttonInfos)
    {
        Open();
        int length = buttonInfos.Length;
        if (length == 0 || length > 4)
            Debug.LogError("Error: Too many or no selections for a MessageBox");
        _content.Strings = content;
        SetTagContent(title);
        for (int i = 0; i < 4; i++)
            if (i < length)
            {
                _buttons[i].gameObject.SetActive(true);
                _buttons[i].onClick.RemoveAllListeners();
                int temp = i;
                _buttons[i].onClick.AddListener(() =>
                {
                    Close();
                    buttonInfos[temp].callback?.Invoke();
                });
                _buttonTexts[i].Strings = buttonInfos[i].texts;
                Vector2 sizeDelta = _buttonTransforms[i].sizeDelta;
                sizeDelta.x = _buttonWidths[length];
                _buttonTransforms[i].sizeDelta = sizeDelta;
            }
            else
                _buttons[i].gameObject.SetActive(false);
    }
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(this);
            Debug.LogError("Error: Unexpected multiple instances of MessageBox");
        }
    }
}