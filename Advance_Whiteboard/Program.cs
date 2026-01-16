using System.Collections;
using System.Collections.Generic;


public interface Ilogger
{
    void Log(string message);
}

public class ConsoleLogger : Ilogger
{
    public void Log(string message)
    {
        Console.WriteLine(message); 
    }
}

public class Activity
{
    public int Id { get; private set; }
    public string Title { get; private set; }
    public int Duration { get; private set; }
    enum ActivityType{
        Lecture, Quiz, Discussion
    }

    public Activity(int _id, string _title, int _duration)
    {
        if (_id <= 0)
        {
            throw new ArgumentException("Invalid id");
        }
        else if (_duration <= 0)
        {
            throw new ArgumentException("Invalid duration");
        }
        else if (string.IsNullOrEmpty(_title)) {
            throw new ArgumentException("Invalid title");
        }
        Id = _id;
        Title = _title;
        Duration = _duration;
    }
}

public interface ICommand
{
    void Excute();
    void Undo();
}

public class Lesson
{

    public int _id {  get; private set; }
    public string _name {  get; private set; }
    private List<Activity> activities = new();

    public Lesson(int id, string name)
    {
        _id = id;
        _name = name;
    }

    
    internal void AddActivity_In(Activity act)
    {
        activities.Add(act);

    }
    internal void RemoveActivity_In(int id)
    {
        if (activities.Count > 0) {
            for (int i = 0; i < activities.Count; i++) {
                if (activities[i].Id == id) { 
                    activities.RemoveAt(i);
                    break;
                }
            }
        } else
        {
            throw new InvalidOperationException("Nothing to remove");
        }
    }
    public IReadOnlyList<Activity> GetActivity()
    {
        return activities.AsReadOnly();
    }
    internal Activity GetActivityById(int id)
    {
        return activities.FirstOrDefault(x => x.Id == id);
    }

}

public class LessonManager
{
    public Ilogger _log;
    private Stack<ICommand> UndoStack = new();
    private Stack<ICommand> RedoStack = new();

    public void Excute(ICommand command)
    {
        command.Excute();
        UndoStack.Push(command);
        RedoStack.Clear();
    }
    public void Undo() { 
        
        var tmp = UndoStack.Pop();
        RedoStack.Push(tmp);
    }
    public void Redo()
    {
        var tmp = RedoStack.Pop();
        UndoStack.Push(tmp);
    }
}

public class AddActivityCmd : ICommand
{
    private readonly Lesson _lesson;
    private readonly Activity _activity;
    public Ilogger _log;

    public AddActivityCmd(Ilogger log ,Lesson lesson, Activity activity)
    {
        _log = log;
        _lesson = lesson;
        _activity = activity;
    }
    public void Excute()
    {
        _lesson.AddActivity_In(_activity);
        _log.Log($"Added activity: {_activity.Id}");
    }
    public void Undo()
    {
        _lesson.RemoveActivity_In(_activity.Id);
        _log.Log($"Removed activity: {_activity.Id}");
    }
}

public class RemoveActivityCmd : ICommand
{
    private readonly Lesson _lesson;
    private readonly Activity _activity;
    private Activity _activityBackup;
    private readonly Ilogger _log;

    public RemoveActivityCmd(Ilogger log, Lesson lesson, Activity activity)
    {
        _lesson = lesson;
        _activity = activity;
        _log = log;
    }

    public void Excute()
    {
        _activityBackup = _lesson.GetActivityById(_activity.Id);
        _lesson.RemoveActivity_In(_activity.Id);
        _log.Log($"Removed activity: {_activity.Id}");
    }
    public void Undo()
    {
        _lesson.AddActivity_In( _activityBackup);
        _log.Log($"Added activity: {_activity.Id}");
    }
}

class Program
{
    static void Main(string[] args)
    {

    }
}