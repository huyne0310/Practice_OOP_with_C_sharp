using BankAccountSystem;
using Xunit;

public class BankAccountTests
{
    [Theory]
    [InlineData(50)]
    [InlineData(100)]
    [InlineData(100.8)]
    public void Deposit_ValidAmount_IncreasesBalance(decimal amount)
    {
        var acc = new BankAccount("123", "Test1");

        acc.Deposit(amount);

        Assert.Equal(amount, acc.Balance);
    }
    [Fact]
    public void Deposit_InValidAmount_ThrowEx()
    {
        var acc = new BankAccount("ab2", "test2");
        
        Assert.Throws<InvalidOperationException>(() => acc.Deposit(-50));
    }

    [Fact]
    public void Withdraw_Valid_DecreaseBalance()
    {
        var acc = new BankAccount("abc", "test3");
        acc.Deposit(200);
        acc.Withdraw(100);
        Assert.Equal(100, acc.Balance);
    }

    [Fact]
    public void Withdraw_Invalid_ThrowEx()
    {
        var acc = new BankAccount("dasda", "test4");
        //acc.Withdraw(50);
        Assert.Throws<InvalidOperationException>(() => acc.Withdraw(50));
    }

}

public class FakeITransactionLogger : ITransactionLogger
{
    public bool WasCalled { get; private set; }
    public string mess { get; private set;  }

    public void Log(string message)
    {
        WasCalled = true;
        mess = message;
    }

}

public class BankService_Test
{


    [Theory]
    [InlineData(1000,10,200)]
    [InlineData(900, 10, 300)]
    [InlineData(1200, 54.3, 200)]
    [InlineData(1300.88, 0.55, 197.36)]
    [InlineData(1818, 36.36, 18.36)]
    public void Transfer_Valid_Sendlog(decimal bal1, decimal bal2, decimal amount)
    {
        var log = new FakeITransactionLogger();
        var service = new BankService(log);
        var acc1 = new BankAccount("A", "nguoi gui");
        var acc2 = new BankAccount("B", "nguoi nhan");
        acc1.Deposit(bal1);
        acc2.Deposit(bal2);
        service.Transfer(acc1, acc2, amount);
        Assert.Equal((bal1 - amount), acc1.Balance);
        Assert.Equal((bal2 + amount), acc2.Balance);

        Assert.True(log.WasCalled);
        Assert.Equal($"Transfer {amount} from {acc1.AccountNumber} to {acc2.AccountNumber}", log.mess);
    }
    [Theory]
    [InlineData(1000, 10, 2000)]
    [InlineData(900, 10, 3000)]
    public void Transfer_InValidAmount_ThrowEx(decimal bal1, decimal bal2, decimal amount)
    {
        var log = new FakeITransactionLogger();
        var service = new BankService(log);
        var acc1 = new BankAccount("A", "nguoi gui");
        var acc2 = new BankAccount("B", "nguoi nhan");
        acc1.Deposit(bal1);
        acc2.Deposit(bal2);
        
        Assert.Throws<InvalidOperationException>(() => service.Transfer(acc1, acc2, amount));
        Assert.False(log.WasCalled);
        Assert.Throws<ArgumentNullException>(() => new BankService(null));
    }
}
