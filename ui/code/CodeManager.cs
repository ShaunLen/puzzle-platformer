using System.Collections.Generic;
using System.Linq;
using Godot;
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
    public Env Environment;
    public List<Interactable> Objects = new();
    public bool CodeInterfaceOpen => UiManager.Instance.CodeInterfaceOpen;

    /* Private Fields */
    [Export] private CodeInterface _codeInterface;
    private ErrorReporter _reporter;
    private TextReader _reader;
    private Tokenizer _tokenizer;
    private Parser _parser;
    private Interpreter _interpreter;

    /* Overrides */
    
    public override void _Ready()
    {
        Instance = this;
        
        CreateEnvironment();
        
        /* Signal Connections */
        _codeInterface.RequestCodeExecute += ExecuteCode;
    }
    
    /* Public Methods */

    public void SetCode(string code) =>_codeInterface.SetCode(code);
    
    public void ConsoleWrite(string output) => _codeInterface.ConsoleWrite(output);

    public void ConsoleWriteLine(string output) => _codeInterface.ConsoleWriteLine("\n" + output);
    
    public void ConsoleWriteError(string output, bool addErrorPrefix = true) => _codeInterface.ConsoleWriteError(output, addErrorPrefix);
    
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

    private void ExecuteCode(string code)
    {
        EmitSignal(SignalName.CodeExecuted);
        CreateEnvironment();
        _reporter = new ErrorReporter();
        _reader = new TextReader(code);
        _tokenizer = new Tokenizer(_reporter, _reader);
        _parser = new Parser(_reporter);
        _interpreter = new Interpreter(_reporter);
        GetTree().Root.AddChild(_interpreter);

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

        _reporter.WriteErrorsIfAny();
        
        _interpreter.ExecutionFinished += () =>
        {
            GD.Print("caught signal");
            ConsoleWriteLine("\n" + "Program finished with 0 errors.");
        
            if (!UiManager.Instance.CodeInterfaceOpen)
                HudManager.Instance.SendNotification("Program finished with 0 errors.");
        };
    }
}