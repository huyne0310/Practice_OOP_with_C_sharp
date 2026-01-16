using Xunit;

namespace Test_Whiteboard
{
    public class Test_Stroke
    {
        [Fact]
        public void AddStroke_validData_CreateStroke()
        {
            var _stroke = new Stroke(1, "red", 3, "A to B");
            Assert.Equal(1, _stroke.Id);
            Assert.Equal("red", _stroke.Color);
            Assert.Equal(3, _stroke.Thickness);
            Assert.Equal("A to B", _stroke.Content);
        }

        [Fact]
        public void AddStroke_InvalidId_ThrowEx()
        {
            Assert.Throws<ArgumentException>(() => new Stroke(-1, "blue", 4, "B to C"));
        }
        [Fact]
        public void AddStroke_InvalidColr_ThrowEx()
        {
            Assert.Throws<ArgumentException>(() => new Stroke(2, "", 5, "C to D"));
        }
        [Fact]
        public void AddStroke_InvalidThck_ThrowEx()
        {
            Assert.Throws<ArgumentException>(() => new Stroke(3, "red", 0, "C to D"));
        }
        [Fact]
        public void AddStroke_InvalidCntn_ThrowEx()
        {
            Assert.Throws<ArgumentException>(() => new Stroke(4, "yellow", 1, ""));
        }
        // không gán được giá trị null nên không test

    }

    public class FakeILogger : ILogger
    {
        public bool WasCalled { get; private set; }
        public string mess { get; private set; }

        public void Log (string message)
        {
            WasCalled = true;
            mess = message;
        }
    }
    
    public class Test_Whiteboard
    {
        [Fact]
        public void AddStroke_Valid_Add()
        {
            var logger = new FakeILogger();
            var board = new Whiteboard(logger);
            var _stroke = new Stroke(1, "blue", 2, "A to B");
            
            var addcmd = new AddStrokes(board, _stroke);
            board.ExcuteCommand(addcmd);

            Assert.True(logger.WasCalled);
            Assert.Equal(1, board.GetStroke().Count());

        }
        [Fact]
        public void UndoStroke_Valid_Undo()
        {
            var logger = new FakeILogger();
            var board = new Whiteboard(logger);
            var _stroke = new Stroke(2, "blue", 5, "B to C");
            var addCmd = new AddStrokes(board, _stroke);
            board.ExcuteCommand(addCmd);
            board.Undo();

            Assert.True(logger.WasCalled);
            Assert.Equal(0, board.GetStroke().Count());

        }
        [Fact]
        public void UndoStroke_Invalid_ThrowEx()
        {
            var logger = new FakeILogger();
            var board = new Whiteboard(logger);
            
            Assert.Throws<InvalidOperationException>(() =>  board.Undo());
        }

        [Fact]
        public void RedoStroke_Valid_Undo()
        {
            var logger = new FakeILogger();
            var board = new Whiteboard(logger);
            var _stroke = new Stroke(2, "blue", 5, "B to C");
            var addCmd = new AddStrokes(board, _stroke);
            board.ExcuteCommand(addCmd);
            board.Undo();
            board.Redo();

            Assert.True(logger.WasCalled);
            Assert.Equal(1, board.GetStroke().Count());

        }
        [Fact]
        public void RedoStroke_Invalid_ThrowEx()
        {
            var logger = new FakeILogger();
            var board = new Whiteboard(logger);
            var _stroke = new Stroke(2, "blue", 5, "B to C");
            var addCmd = new AddStrokes(board, _stroke);
            board.ExcuteCommand(addCmd);
            Assert.Throws<InvalidOperationException>(() => board.Redo());
        }
    }  
}
