using Xunit;
using Whiteboard;

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
        public string mess {  get; private set; }

        public void Log(string messenger)
        {
            mess = messenger;
            WasCalled = true;
        }

    }

    public class Test_WhiteBoard
    {
        [Theory]
        [InlineData(1, "red", 3, "A to B")]
        [InlineData(2, "blue", 5, "A to B")]
        [InlineData(3, "red", 3, "C to D")]
        [InlineData(4, "red", 3, "B to C")]
        [InlineData(5, "red", 3, "D to E")]
        public void AddStroke_Valid_Add(int id, string color, int thickness, string content)
        {
            var log = new FakeILogger();
            var _stroke = new Stroke(id, color, thickness, content);
            var whiteboard = new WhiteBoard(log);

            whiteboard.AddStroke(_stroke);
            Assert.Equal(1, whiteboard.GetStrokes().Count());
        }

        [Fact]
        public void AddStrokeLog_Valid_CallLog()
        {
            var log = new FakeILogger();
            var _stroke = new Stroke(1, "red", 3, "A to B");
            var whiteboard = new WhiteBoard(log);

            whiteboard.AddStroke(_stroke);

            Assert.True(log.WasCalled);
        }

        [Theory]
        [InlineData(1, "red", 3, "A to B")]
        [InlineData(2, "blue", 5, "A to B")]
        [InlineData(3, "red", 3, "C to D")]
        [InlineData(4, "red", 3, "B to C")]
        [InlineData(5, "red", 3, "D to E")]

        public void Undo_Valid_undo(int id, string color, int thickness, string content)
        {
            var log = new FakeILogger();
            var _stroke = new Stroke(id, color, thickness, content);
            var whiteboard = new WhiteBoard(log);
            whiteboard.AddStroke(_stroke);
            whiteboard.Undo();
            Assert.Equal(0, whiteboard.GetStrokes().Count());
        }
        [Fact]
        public void Undo_Invalid_ThrowEx()
        {
            var log = new FakeILogger();
            var whiteboard = new WhiteBoard(log);

            Assert.Throws<InvalidOperationException>(() => whiteboard.Undo());
        }

        [Theory]
        [InlineData(1, "red", 3, "A to B")]
        [InlineData(2, "blue", 5, "A to B")]
        [InlineData(3, "red", 3, "C to D")]
        [InlineData(4, "red", 3, "B to C")]
        [InlineData(5, "red", 3, "D to E")]
        public void Redo_Valid_redo(int id, string color, int thickness, string content)
        {
            var log = new FakeILogger();
            var _stroke = new Stroke(id, color, thickness, content);
            var whiteboard = new WhiteBoard(log);
            whiteboard.AddStroke(_stroke);
            whiteboard.Undo();
            whiteboard.Redo();
            Assert.Equal(1, whiteboard.GetStrokes().Count());
        }

        [Fact]
        public void Redo_Invalid_ThrowEx() {
            var log = new FakeILogger();
            var _stroke = new Stroke(1, "red", 3, "A to B");
            var whiteboard = new WhiteBoard(log);
            whiteboard.AddStroke(_stroke);
            Assert.Throws<InvalidOperationException>(() => whiteboard.Redo());
        }
    }
}
