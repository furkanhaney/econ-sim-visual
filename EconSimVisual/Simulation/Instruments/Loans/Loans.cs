using System.Collections.Generic;
using System.Linq;
using EconSimVisual.Simulation.Banks;
using EconSimVisual.Simulation.Base;

namespace EconSimVisual.Simulation.Instruments.Loans
{
    internal class Loans
    {
        public Loans(Agent agent)
        {
            Agent = agent;
            MadeLoans = new List<SimpleLoan>();
        }

        public Agent Agent { get; }
        public List<SimpleLoan> MadeLoans { get; }
        public double InterestRate { get; set; }
        public double TotalAmount => 0;

        public void Tick()
        {
            foreach (var loan in MadeLoans.ToList())
            {
                loan.MaturityDays--;
                if (loan.MaturityDays <= 0)
                    loan.Mature();
            }
        }


        public bool Apply(Agent borrower, double amount, int period)
        {
            return Agent is CentralBank;
        }

        public void Take(Agent borrower, double amount, int period)
        {
            var loan = new SimpleLoan()
            {
                Owner = Agent,
                Borrower = borrower,
                Principal = amount,
                TotalLength = period,
                MaturityDays = period,
                InterestRate = InterestRate
            };
            loan.TransferFunds();
            MadeLoans.Add(loan);
        }
    }
}
