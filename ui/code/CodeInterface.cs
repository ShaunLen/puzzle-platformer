using System;
using Godot;
using PuzzlePlatformer.autoloads;
using PuzzlePlatformer.ui.hud;
using PuzzlePlatformer.ui.themes;

namespace PuzzlePlatformer.ui.code;

public partial class CodeInterface : Control
{
	/* Signals */
	[Signal] public delegate void RequestCodeExecuteEventHandler(string code);
	[Signal] public delegate void CodeWindowOpenedEventHandler();
	[Signal] public delegate void CodeWindowClosedEventHandler();
	
	/* Private Fields */
	[Export] private Control _codeEditorParent;
	[Export] private Control _consoleParent;
	[Export] private Control _actionsParent;
	[Export] private CodeEdit _codeEdit;
	[Export] private RichTextLabel _console;
	[Export] private TextureButton _runButton;
	[Export] private TextureButton _clearButton;
	[Export] private TextureButton _closeButton;
	
	/* Syntax Highlighting */
	private NordColors _nordColors;

	/* Override Methods */
	
	public override void _Ready()
	{
		_nordColors = new NordColors();
		SetSyntaxHighlighting(_codeEdit.SyntaxHighlighter);
		
		Show();
		
		GetTree().CurrentScene.Ready += () =>
		{
			_codeEdit.SetCaretLine(500);
			_codeEdit.SetCaretColumn(500);
		};
		
		/* Signal Connections */
		_clearButton.Pressed += ConsoleClear;
		_runButton.Pressed += RequestCodeExecution;
		_closeButton.Pressed += CloseCode;
		UiManager.Instance.OpenCodeInterface += OpenCode;
		UiManager.Instance.CloseCodeInterface += CloseCode;
	}

	public override void _ExitTree()
	{
		((CodeHighlighter)_codeEdit.SyntaxHighlighter).ColorRegions = null;
	}

	public override void _Process(double delta)
	{
		if (InputManager.IsActionJustPressed(InputManager.Action.RunCode, true))
		{
			ConsoleClear();
			EmitSignal(SignalName.RequestCodeExecute, _codeEdit.Text);
		}
	}
	
	/* Public Methods */
	
	public void SetCode(string code)
	{
		_codeEdit.Text = code;
	}
	
	public void ConsoleWrite(string output)
	{
		_console.AddText(output);
	}

	public void ConsoleWriteLine(string output)
	{
		_console.AddText("\n" + output);
	}
	
	public void ConsoleWriteError(string output, bool addErrorPrefix)
	{
		AudioManager.Instance.PlaySound(AudioManager.Sound.Error);

		_console.PushColor(Color.Color8(181, 91 ,91));
		_console.AddText(addErrorPrefix ? $"\nError: {output}" : $"\n{output}");
	}
	
	/* Private Methods */
	
	private void OpenCode()
	{
		UiManager.Instance.CodeInterfaceOpen = true;
		
		EmitSignal(SignalName.CodeWindowOpened);
		_codeEdit.GrabFocus();
		
		InputManager.InputEnabled = false;
		Input.MouseMode = Input.MouseModeEnum.Confined;
		Input.WarpMouse(GetTree().Root.Size / 2);

		var codeTween = CreateTween();
		var consoleTween = CreateTween();
		var actionsTween = CreateTween();
		
		codeTween.TweenProperty(_codeEditorParent, "position:x", 0, 0.1);
		consoleTween.TweenProperty(_consoleParent, "position:x", 0, 0.1);
		actionsTween.TweenProperty(_actionsParent, "position:x", 0, 0.1);
	}
	
	public void CloseCode()
	{
		UiManager.Instance.CodeInterfaceOpen = false;
		
		EmitSignal(SignalName.CodeWindowClosed);
		
		_codeEdit.ReleaseFocus();
		
		InputManager.InputEnabled = true;
		Input.MouseMode = Input.MouseModeEnum.ConfinedHidden;
		
		var codeTween = CreateTween();
		var consoleTween = CreateTween();
		var actionsTween = CreateTween();
		
		codeTween.TweenProperty(_codeEditorParent, "position:x", -585, 0.1);
		consoleTween.TweenProperty(_consoleParent, "position:x", 355, 0.1);
		actionsTween.TweenProperty(_actionsParent, "position:x", 300, 0.1);
	}

	private void RequestCodeExecution()
	{
		ConsoleClear();
		EmitSignal(SignalName.RequestCodeExecute, _codeEdit.Text);
	}
	
	private void ConsoleClear()
	{
		Console.WriteLine("Clearing console");
		_console.Clear();
	}
	
	private void SetSyntaxHighlighting(SyntaxHighlighter syntaxHighlighter)
	{
		var highlighter = (CodeHighlighter) syntaxHighlighter;
		
		/* General Colors */
		highlighter.NumberColor = _nordColors.Nord15;
		highlighter.SymbolColor = _nordColors.Nord10;
		highlighter.FunctionColor = _nordColors.Nord8;
		highlighter.MemberVariableColor = _nordColors.Nord7;
		
		/* Keywords */
		highlighter.AddKeywordColor("var", _nordColors.Nord7);
		highlighter.AddKeywordColor("const", _nordColors.Nord7);
		highlighter.AddKeywordColor("else", _nordColors.Nord7);
		highlighter.AddKeywordColor("true", _nordColors.Nord7);
		highlighter.AddKeywordColor("false", _nordColors.Nord7);

		/* Color Regions */
		highlighter.AddColorRegion("#", "", _nordColors.Nord3);
		highlighter.AddColorRegion("\"", "\"", _nordColors.Nord14);
	}
}
