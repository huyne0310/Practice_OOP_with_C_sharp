using System;

namespace BankAccountSystem
{
    public class BankAccount
    {
        public string AccountNumber { get; private set; }
        public string OwnerName { get; }
        public decimal Balance { get; private set; }

        public BankAccount(string accountNumber, string ownerName)
        {
            Balance = 0;
            //if (accountNumber == "" || ownerName == "" || accountNumber == null || ownerName == null)
            if (string.IsNullOrWhiteSpace(accountNumber) || string.IsNullOrWhiteSpace(ownerName))
            {
                throw new ArgumentException();
            }
            AccountNumber = accountNumber;
            OwnerName = ownerName;
        }

        public void Deposit(decimal amount)
        {
            if (amount <= 0)
            {
                throw new InvalidOperationException();
            }
            Balance += amount;
        }

        public void Withdraw(decimal amount)
        {
            if (amount <= 0 || amount > Balance)
            {
                throw new InvalidOperationException();
            }
            Balance -= amount;
        }
    }

    public interface ITransactionLogger
    {
        void Log(string message);
    }


    public class BankService {

        private readonly ITransactionLogger _logger;
        public BankService(ITransactionLogger logger)
        {
            //if (logger == null)
            //{
            //    throw new ArgumentNullException();
            //}
            //_logger = logger;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));


        }

        public void Transfer(BankAccount from, BankAccount to, decimal amount)
        {
            if (from == null || to == null)
            {
                throw new ArgumentNullException();
            }

            if (amount <= 0 )
            {
                throw new InvalidOperationException();
            }
            from.Withdraw(amount);
            to.Deposit(amount);

            _logger.Log($"Transfer {amount} from {from.AccountNumber} to {to.AccountNumber}");

        }
    }


    class Program
    {
        static void Main(string[] args)
        {
        }

    }
}