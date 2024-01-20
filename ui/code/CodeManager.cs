using System.Collections.Generic;
using System.Linq;
using Godot;
using PuzzlePlatformer.autoloads;
using PuzzlePlatformer.litescript_two;
using PuzzlePlatformer.litescript_two.IO;
using PuzzlePlatformer.litescript_two.Runtime;
using PuzzlePlatformer.litescript_two.Runtime.Values;
using PuzzlePlatformer.litescript_two.Tokenization;
using PuzzlePlatformer.objects.interactable;
using PuzzlePlatformer.ui.hud;
using PuzzlePlatformer.world;

namespace PuzzlePlatformer.ui.code;

//TODO: Decouple this
public partial class CodeManager : Node
{
    public static CodeManager Instance { get; private set; }
    
    /* Signals */
    [Signal] public delegate void CodeExecutedEventHandler();

    /* Public Fields */
    public Env GlobalEnvironment;
    public List<Interactable> Objects = new();
    public bool CodeInterfaceOpen => UiManager.Instance.CodeInterfaceOpen;
    public bool Executing;
    public bool FinishedExecuting;

    /* Private Fields */
    [Export] private CodeInterface _codeInterface;
    private ErrorReporter _reporter;
    private TextReader _reader;
    private Tokenizer _tokenizer;
    private Parser _parser;
    private Interpreter _interpreter;
    private Interpreter _initialInterpreter;

    /* Overrides */
    
    public override void _Ready()
    {
        Instance = this;
        
        CreateEnvironment();
        
        /* Signal Connections */
        _codeInterface.RequestCodeExecute += ExecuteCode;
        GameManager.Instance.SceneRestarted += () => FinishedExecuting = false;
    }
    
    /* Public Methods */

    public void SetCode(string code)
    {
        _codeInterface.SetCode(GameManager.Instance.PlayerWrittenCode == ""
            ? code
            : GameManager.Instance.PlayerWrittenCode);
    }

    public void ConsoleWrite(string output) => _codeInterface.ConsoleWrite(output);

    public void ConsoleWriteLine(string output) => _codeInterface.ConsoleWriteLine("\n" + output);
    
    public void ConsoleWriteError(string output, bool addErrorPrefix = true) => _codeInterface.ConsoleWriteError(output, addErrorPrefix);
    
    /* Private Methods */
    
    private void CreateEnvironment()
    {
        // Create LS environment
        GlobalEnvironment = Env.CreateStandardEnvironment();
        
        // Get list of all interactable objects in level
        var interactables = GetTree().GetNodesInGroup("Interactable").ToList();

        foreach (Interactable obj in interactables)
        {
            Objects.Add(obj);
            GlobalEnvironment.DeclareVariable(obj.Name, new ObjectValue(GetObjectProperties(obj.Name)), false);

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

        obj.GetType().GetMethod(methodName)?.Invoke(obj, new object[]{ new List<IRuntimeValue>(), GlobalEnvironment });

        return true;
    }

    private void ExecuteCode(string code)
    {
        if (Executing)
            return;
        Executing = true;
        
        EmitSignal(SignalName.CodeExecuted);
        CreateEnvironment();
        _reporter = new ErrorReporter();
        _reader = new TextReader(code);
        _tokenizer = new Tokenizer(_reporter, _reader);
        _parser = new Parser(_reporter);
        _interpreter = new Interpreter(_reporter);
        _initialInterpreter ??= _interpreter;
        GetTree().Root.AddChild(_interpreter);

        if (_initialInterpreter.IsLooping)
            return;

        if (FinishedExecuting)
            return;

        if (_reporter.WriteErrorsIfAny())
            return;

        var tokens = _tokenizer.GetAllTokens();

        if (_reporter.WriteErrorsIfAny())
            return;

        var program = _parser.Parse(tokens);

        if (_reporter.WriteErrorsIfAny())
            return;

        if (!LevelManager.Instance.CheckRequirementsMet(program))
            return;

        var result = _initialInterpreter.Evaluate(program, GlobalEnvironment);

        if (result == null)
            return;

        _reporter.WriteErrorsIfAny();
        
        GD.Print("Made it to just before the event catch");
        
        _initialInterpreter.ExecutionFinished += () =>
        {
            GD.Print("CodeManager knows execution has finished.");
            Executing = false;
            FinishedExecuting = true;
            ConsoleWriteLine("\n" + "Program finished with 0 errors.");
        
            if (!UiManager.Instance.CodeInterfaceOpen)
                HudManager.Instance.SendNotification("Program finished with 0 errors");
        };
    }
}