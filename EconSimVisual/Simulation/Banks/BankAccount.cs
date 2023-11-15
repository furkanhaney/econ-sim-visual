using System;
using System.Reflection;
using EconSimVisual.Extensions;
using EconSimVisual.Simulation.Agents;
using EconSimVisual.Simulation.Base;
using EconSimVisual.Simulation.Helpers;
using EconSimVisual.Simulation.Polities;

namespace EconSimVisual.Simulation.Banks
{
    [Serializable]
    internal class BankAccount : Entity
    {
        public IDepository Bank { get; set; }
        public Agent Owner { get; set; }
        public double Balance { get; set; }
        public double SavingsRate { get; set; }
        public double CreditRate { get; set; }
        public double CreditLimit { get; set; }
        public double AvailableCredit => Balance + CreditLimit;
        public override Town Town {
            get => Owner.Town;
            set
            {
                throw new Exception();
            } }

        public void Deposit(double amount)
        {
            if (amount <= 0)
                throw new ArgumentOutOfRangeException(nameof(amount));
            Assert(Owner.Cash >= amount);

            Owner.PayCash((Agent)Bank, amount);
            Balance += amount;
            Log(Owner + " deposited " + amount.FormatMoney() + " at " + Bank + ".", LogType.FinancialTransaction);
        }

        public void Pay(Agent agent, double amount)
        {
            Assert(amount > 0);
            Assert(AvailableCredit >= amount);

            if (Bank.Deposits.HasAccount(agent))
                TransferCredit(agent, amount);
            else if (((Agent)Bank).CanPay(amount))
                TransferCash(agent, amount);
            else
                Log(Bank + " could not pay " + amount.FormatMoney() + " to " + agent + " on " + Owner + "'s request.", LogType.NonPayment);
        }

        public void Withdraw(double amount)
        {
            if (amount <= 0)
                throw new ArgumentOutOfRangeException(nameof(amount));
            Assert(AvailableCredit >= amount);

            if (((Agent)Bank).CanPay(amount))
            {
                ((Agent)Bank).Pay(Owner, amount);
                Balance -= amount;
                Log(Owner + " withdrew " + amount.FormatMoney() + " from " + Bank + ".", LogType.FinancialTransaction);
            }
            else
                Log(Bank + " could not carry out withdrawal request.", LogType.NonPayment);
        }

        public void Close()
        {
            if (Balance > 0)
                ((Agent)Bank).Income.BadDebt += Balance;
            Owner.BankAccounts.Remove(this);
            Bank.Deposits.Accounts.Remove(Owner);
        }

        private void TransferCash(Agent agent, double amount)
        {
            Balance -= amount;
            ((Agent)Bank).Pay(agent, amount);
        }

        private void TransferCredit(Agent agent, double amount)
        {
            Balance -= amount;
            Bank.Deposits.Accounts[agent].Balance += amount;
        }

        public static void Test()
        {
            /*var person = new Person()
            {
                Cash = 250
            };
            var bank = new CommercialBank()
            {
                MinimumPaymentRate = 0.1,
                MinimumPaymentConstant = 20
            };
            bank.OpenAccount(person);
            var account = person.BankAccounts[0];

            // Test Deposit
            account.Deposit(100);
            Assert(person.Cash == 150);
            Assert(account.Balance == 100);
            Assert(bank.Cash == 100);

            // Test Withdraw
            account.Withdraw(50);
            Assert(person.Cash == 200);
            Assert(account.Balance == 50);
            Assert(bank.Cash == 50);

            // Test cash payment
            var person2 = new Person();
            account.Pay(person2, 20);
            Assert(person.Cash == 200);
            Assert(account.Balance == 30);
            Assert(bank.Cash == 30);
            Assert(person2.Cash == 20);

            // Test credit payment
            var person3 = new Person();
            bank.OpenAccount(person3);
            account.Pay(person3, 10);
            Assert(person.Cash == 200);
            Assert(account.Balance == 20);
            Assert(person3.BankAccounts[0].Balance == 10);
            Assert(bank.Cash == 30);

            // Test minimum payment
            var manufacturer = new Manufacturer(ManufacturingProcess.Get("Potato")) { Cash = 100 };
            bank.OpenAccount(manufacturer);
            var account2 = manufacturer.BankAccounts[0];
            account2.CreditLimit = 500;
            Assert(account2.MinimumPayment == 0);
            account2.Withdraw(20);
            Assert(account2.MinimumPayment == 20);
            account2.Withdraw(10);
            Assert(account2.MinimumPayment == 23);*/
        }
    }
}