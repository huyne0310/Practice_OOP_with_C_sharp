using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

public class Stroke
{
    public int Id {  get; private set; }
    public string Color { get; private set; }
    public int Thickness { get; private set; }
    public string Content { get; private set; }

    public Stroke(int id, string color, int thickness, string content)
    {
        if (id <= 0)
        {
            throw new ArgumentException(nameof(id));
        }

        else if (String.IsNullOrEmpty(color))
        {
            throw new ArgumentException(nameof(color));
        }
        else if (thickness <= 0)
        {
            throw new ArgumentException(nameof(thickness));
        }
        else if (String.IsNullOrEmpty(content)) { 
            throw new ArgumentException(nameof(content));
        }

        Id = id;
        Color = color;
        Thickness = thickness;
        Content = content;
    }
}

public interface ILogger
{
    void Log(string message);
}

public interface ICommand
{
    void Execute();
    void Undo();
}

public class Whiteboard
{
    public List<Stroke> strokes = new();
    public Stack<ICommand> UndoStack = new();
    public Stack<ICommand> RedoStack = new();
    public ILogger _log;

    public Whiteboard(ILogger log)
    {
        _log = log;
    }

    public void ExcuteCommand(ICommand cmd)
    {
        cmd.Execute();
        UndoStack.Push(cmd);
        RedoStack.Clear();
    }

    public void Undo()
    {
        if (UndoStack.Count() == 0) throw new InvalidOperationException("Nothing to Undo");
        var tmp = UndoStack.Pop();
        RedoStack.Push(tmp);
        tmp.Undo();
    }
    public void Redo()
    {
        if (RedoStack.Count() == 0) throw new InvalidOperationException("Nothing to Redo");
        var tmp = RedoStack.Pop();
        UndoStack.Push(tmp);
        tmp.Execute();
    }
    internal void AddStroke_Internal(Stroke _stroke)
    {
        strokes.Add(_stroke);
        _log.Log($"Added stroke: {_stroke.Id}");
    }
    internal void RemoveStroke_Internal(Stroke _stroke)
    {
        strokes.Remove(_stroke);
        _log.Log($"Removed stroke: {_stroke.Id}");
    }
    public IReadOnlyList<Stroke> GetStroke()
    {
        return strokes.AsReadOnly();
    }
}

public class AddStrokes : ICommand
{
    private readonly Whiteboard _board;
    private readonly Stroke _stroke;

    public AddStrokes(Whiteboard board, Stroke stroke)
    {
        _board = board;
        _stroke = stroke;
    }
    public void Execute() { 
        _board.AddStroke_Internal(_stroke);
    }
    public void Undo()
    {
        _board.RemoveStroke_Internal(_stroke) ;
    }
}

public class ConsoleLogger : ILogger {
    public void Log(string message) { 
        Console.WriteLine(message);
    }
}

class Program
{
    static void Main(string[] args)
    {
        ILogger loger = new ConsoleLogger();
        var board = new Whiteboard(loger);

        var stroke = new Stroke(1, "blue", 3, "A to B");
        var addStroke = new AddStrokes(board, stroke);

        board.ExcuteCommand(addStroke);
        board.Undo();
        board.Redo();

    }
}
