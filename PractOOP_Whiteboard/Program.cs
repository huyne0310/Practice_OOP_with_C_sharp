using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace Whiteboard
{
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

    class ConsoleLogger : ILogger
    {
        public void Log(string message) { 
            Console.WriteLine( "[Log] " + message);
        }
    }

    public class WhiteBoard
    {
        private List<Stroke> strokes = new List<Stroke>();
        private Stack<Stroke> undoStack = new Stack<Stroke>();
        private Stack<Stroke> redoStack = new Stack<Stroke>();
        private readonly ILogger _log;

        public WhiteBoard(ILogger log)
        {
            _log = log ?? throw new ArgumentNullException(nameof(log));
        }

        public void AddStroke(Stroke tmp)
        {
            
            strokes.Add(tmp);
            undoStack.Push(tmp);
            redoStack.Clear();
            _log.Log("Added stroke " + tmp.Id);
        }

        public void Undo()
        {
            if (undoStack.Count() == 0)
            {
                throw new InvalidOperationException();
            }
            
            redoStack.Push(undoStack.Peek());
            strokes.Remove(undoStack.Peek());
            
            _log.Log("Undo add " + undoStack.Peek().Id);
            undoStack.Pop();
        }

        public void Redo()
        {
            if (redoStack.Count() == 0)
            {
                throw new InvalidOperationException();
            }
            
            
            undoStack.Push (redoStack.Peek());
            strokes.Add (redoStack.Peek());

            _log.Log("Redo add " + redoStack.Peek().Id);

            redoStack.Pop();
        }

        public IReadOnlyList<Stroke> GetStrokes()
        {
            return new List<Stroke>(strokes);
        }

    }


    class Program
    {
        static void Main(string[] args)
        {
            ILogger logger = new ConsoleLogger();
            var _Strokes = new Stroke(1, "blue", 3, "A to B");
            var workspace = new WhiteBoard(logger);
            workspace.AddStroke(_Strokes);
            workspace.Undo();
        }
    }
}