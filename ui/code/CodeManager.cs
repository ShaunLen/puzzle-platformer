using System.Collections.Generic;
using System.Linq;
using Godot;
using PuzzlePlatformer.litescript_two;
using PuzzlePlatformer.litescript_two.IO;
using PuzzlePlatformer.litescript_two.Runtime;
using PuzzlePlatformer.litescript_two.Runtime.Values;
using PuzzlePlatformer.litescript_two.Tokenization;
using PuzzlePlatformer.objects.interactable;
using PuzzlePlatformer.ui.guide;
using PuzzlePlatformer.ui.themes;
using PuzzlePlatformer.world;

namespace PuzzlePlatformer.autoloads;

//TODO: Decouple this
public partial class CodeManager : Node
{
    public static CodeManager Instance { get; private set; }
    
    /* Signals */
    [Signal] public delegate void CodeExecutedEventHandler();
    [Signal] public delegate void CodeWindowOpenedEventHandler();
    [Signal] public delegate void CodeWindowClosedEventHandler();

    /* Public Fields */
    public bool EditorOpen;
    public Env Environment;
    public List<Interactable> Objects = new();

    /* Private Fields */
    private ErrorReporter _reporter;
    private TextReader _reader;
    private Tokenizer _tokenizer;
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
    private Guide _guide;
    
    /* Syntax Highlighting */
    private NordColors _nordColors;

    /* Overrides */
    
    public override void _Ready()
    {
        Instance = this;
        _code = GetTree().GetFirstNodeInGroup("Code") as Control;
        _codeEditorParent = _code.GetNode<Control>("CodeEditor");
        _consoleParent = _code.GetNode<Control>("ConsoleWindow");
        _actionsParent = _code.GetNode<Control>("Actions");
        _codeEdit = _code.GetNode<CodeEdit>("CodeEditor/CodeEdit");
        _console = _code.GetNode<RichTextLabel>("ConsoleWindow/Console");
        _runButton = _code.GetNode<TextureButton>("Actions/RunButton");
        _clearButton = _code.GetNode<TextureButton>("Actions/ClearButton");
        _closeButton = _code.GetNode<TextureButton>("Actions/CloseButton");
        _guide = (Guide) GetTree().GetFirstNodeInGroup("Guide");
        
        CreateEnvironment();

        _nordColors = new NordColors();
        SetSyntaxHighlighting(_codeEdit.SyntaxHighlighter);
        
        _code.Show();
        
        /* Signal Connections */
        _runButton.Pressed += ExecuteCode;
        _clearButton.Pressed += ConsoleClear;
        _closeButton.Pressed += CloseCode;
        _guide.GuideOpened += CloseCode;
        
        GetTree().CurrentScene.Ready += () =>
        {
            _codeEdit.SetCaretLine(500);
            _codeEdit.SetCaretColumn(500);
        };
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
    
    public void ConsoleWriteError(string output, bool errorText = true)
    {
        AudioManager.Instance.PlaySound(AudioManager.Sound.Error);

        _console.PushColor(Color.Color8(181, 91 ,91));
        _console.AddText(errorText ? $"\nError: {output}" : $"\n{output}");
    }

    public void ConsoleClear()
    {
        _console.Clear();
    }
    
    public void OpenCode()
    {
        EmitSignal(SignalName.CodeWindowOpened);
        
        EditorOpen = true;
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
        EmitSignal(SignalName.CodeWindowClosed);
        
        EditorOpen = false;
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
    
    /* Private Methods */
    
    private void CreateEnvironment()
    {
        // Create LS environment
        Environment = Env.CreateStandardEnvironment();
        
        // Get list of all interactable objects in level
        var interactables = GetTree().GetNodesInGroup("Interactable").ToList();

        foreach (Interactable obj in interactables)
        {
            Objects.Add(obj);
            Environment.DeclareVariable(obj.Name, new ObjectValue(GetObjectProperties(obj.Name)), false);

        }
    }

    private Dictionary<string, IRuntimeValue> GetObjectProperties(string objName) {
        
        var obj = GetObject(objName);
        Dictionary<string, IRuntimeValue> propertyList = obj.GetType().GetMethod("GetProperties")?.Invoke(obj, null) as Dictionary<string, IRuntimeValue>;

        return propertyList;
    }

    private bool ObjectExists(string objName)
    {
        return Objects.Any(obj => obj.Name.Equals(objName));
    }

    private Interactable GetObject(string objName)
    {
        return Objects.Find(obj => obj.Name.Equals(objName));
    }
    
    private bool CallMethod(string objName, string methodName)
    {
        if (!ObjectExists(objName))
            return false;

        var obj = GetObject(objName); 

        obj.GetType().GetMethod(methodName)?.Invoke(obj, new object[]{ new List<IRuntimeValue>(), Environment });

        return true;
    }
    
    private void ExecuteCode()
    {
        EmitSignal(SignalName.CodeExecuted);
        CreateEnvironment();
        ConsoleClear();
        
        var input = _codeEdit.Text;
        _reporter = new ErrorReporter();
        _reader = new TextReader(input);
        _tokenizer = new Tokenizer(_reporter, _reader);
        _parser = new Parser(_reporter);
        _interpreter = new Interpreter(_reporter);

        var tokens = _tokenizer.GetAllTokens();
        
        if (_reporter.WriteErrorsIfAny())
            return;
        
        var program = _parser.Parse(tokens);
        
        if (_reporter.WriteErrorsIfAny())
            return;

        if (!LevelManager.Instance.CheckRequirementsMet(program))
            return;
        
        var result = _interpreter.Evaluate(program, Environment);

        if (result == null)
            return;

        if (_reporter.WriteErrorsIfAny())
            return;
        
        ConsoleWriteLine("\n" + "Program finished with 0 errors.");
            
        if(!EditorOpen)
            ui.hud.HudManager.Instance.SendNotification("Program finished with 0 errors.");
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