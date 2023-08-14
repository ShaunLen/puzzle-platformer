using System;
using Godot;
using PuzzlePlatformer.litescript.Frontend;
using PuzzlePlatformer.litescript.Runtime;
using PuzzlePlatformer.ui.themes;
using PuzzlePlatformer.world;

namespace PuzzlePlatformer.autoloads;

public partial class CodeManager : Node
{
    public static CodeManager Instance { get; private set; }
    
    /* Signals */
    [Signal] public delegate void CodeExecutedEventHandler();

    /* Public Properties */
    public bool EditorOpen;

    /* Private Properties */
    private Parser _parser;
    private Interpreter _interpreter;
    private Control _code;
    private Control _codeEditorParent;
    private Control _consoleParent;
    private Control _actionsParent;
    private CodeEdit _codeEdit;
    private RichTextLabel _console;
    private TextureButton _runButton;
    private TextureButton _clearButton;
    private TextureButton _closeButton;
    
    /* Syntax Highlighting */
    private NordColors _nordColors;

    /* Overrides */
    
    public override void _Ready()
    {
        Instance = this;
        
        _parser = new Parser();
        _interpreter = new Interpreter();
        
        _code = GetTree().GetFirstNodeInGroup("Code") as Control;
        _codeEditorParent = _code.GetNode<Control>("CodeEditor");
        _consoleParent = _code.GetNode<Control>("ConsoleWindow");
        _actionsParent = _code.GetNode<Control>("Actions");
        _codeEdit = _code.GetNode<CodeEdit>("CodeEditor/CodeEdit");
        _console = _code.GetNode<RichTextLabel>("ConsoleWindow/Console");
        _runButton = _code.GetNode<TextureButton>("Actions/RunButton");
        _clearButton = _code.GetNode<TextureButton>("Actions/ClearButton");
        _closeButton = _code.GetNode<TextureButton>("Actions/CloseButton");

        _nordColors = new NordColors();
        SetSyntaxHighlighting(_codeEdit.SyntaxHighlighter);
        
        /* Signal Connections */
        _runButton.Pressed += ExecuteCode;
        _clearButton.Pressed += ConsoleClear;
        _closeButton.Pressed += CloseCode;
    }

    public override void _Process(double delta)
    {
        if (InputManager.IsActionJustPressed(InputManager.Action.ToggleCode, true))
        {
            if(EditorOpen)
                CloseCode();
            else
                OpenCode();
        }
        
        if (InputManager.IsActionJustPressed(InputManager.Action.RunCode, true))
            ExecuteCode();
    }
    
    /* Public Methods */
    
    public void ConsoleWrite(string output)
    {
        _console.AddText(output);
    }

    public void ConsoleWriteLine(string output)
    {
        _console.AddText("\n" + output);
    }
    
    public void ConsoleWriteError(string output)
    {
        AudioManager.Instance.PlaySound(AudioManager.Sound.Error);

        _console.PushColor(Color.Color8(200, 0 ,0));
        _console.AddText("\nError: " + output);
    }

    public void ConsoleClear()
    {
        _console.Clear();
    }
    
    /* Private Methods */
    
    private void OpenCode()
    {
        EditorOpen = true;
        _codeEdit.GrabFocus();
        
        InputManager.InputEnabled = false;
        Input.MouseMode = Input.MouseModeEnum.Confined;
        Input.WarpMouse(GetTree().Root.Size / 2);
        
        Console.WriteLine("\n\nCode visible: " + _code.Visible);
        Console.WriteLine("Has focus: " + _codeEdit.HasFocus());
        Console.WriteLine("Input enabled: " + InputManager.InputEnabled);

        var codeTween = CreateTween();
        var consoleTween = CreateTween();
        var actionsTween = CreateTween();
        
        codeTween.TweenProperty(_codeEditorParent, "position:x", 0, 0.2);
        consoleTween.TweenProperty(_consoleParent, "position:x", 0, 0.2);
        actionsTween.TweenProperty(_actionsParent, "position:x", 0, 0.2);
    }
    
    private void CloseCode()
    {
        EditorOpen = false;
        _codeEdit.ReleaseFocus();
        
        InputManager.InputEnabled = true;
        Input.MouseMode = Input.MouseModeEnum.ConfinedHidden;
        
        Console.WriteLine("\n\nCode visible: " + _code.Visible);
        Console.WriteLine("Has focus: " + _codeEdit.HasFocus());
        Console.WriteLine("Input enabled: " + InputManager.InputEnabled);
        
        var codeTween = CreateTween();
        var consoleTween = CreateTween();
        var actionsTween = CreateTween();
        
        codeTween.TweenProperty(_codeEditorParent, "position:x", -585, 0.2);
        consoleTween.TweenProperty(_consoleParent, "position:x", 355, 0.2);
        actionsTween.TweenProperty(_actionsParent, "position:x", 300, 0.2);
    }
    
    private void ExecuteCode()
    {
        EmitSignal(SignalName.CodeExecuted);
        
        ConsoleClear();
        
        var input = _codeEdit.Text;
        var script = _parser.ProduceAst(input);
        var result = _interpreter.Evaluate(script, LevelManager.Instance.Environment);
        
        ConsoleWriteLine("\n" + "Program finished with 0 errors.");
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

        /* Color Regions */
        highlighter.AddColorRegion("#", "", _nordColors.Nord3);
        highlighter.AddColorRegion("\"", "\"", _nordColors.Nord14);
    }
}