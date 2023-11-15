using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using EconSimVisual.Extensions;
using EconSimVisual.Simulation.Accounting;
using EconSimVisual.Simulation.Base;
using EconSimVisual.Simulation.Instruments.Loans;
using EconSimVisual.Simulation.Polities;

namespace EconSimVisual.Simulation.Banks
{

    [Serializable]
    internal class CentralBank : Agent, IDepository, ILender
    {

        public override Town Town
        {
            get => base.Town;
            set
            {
                base.Town = value;
            }
        }

        public CentralBank()
        {
            BalanceSheet = new CentralBankBalanceSheet(this);
            Loans = new Loans(this);
            Deposits = new Deposits(this);
        }

        public override void FirstTick()
        {
            base.FirstTick();
            Loans.Tick();
        }

        public Loans Loans { get; set; }
        public Deposits Deposits { get; }

        protected override string DefaultName => "CentralBank";
        public double RequiredReserveRatio { get; set; } = 0.1;

        public override bool CanPay(double amount) => true;
        public override bool CanPayCash(double amount) => true;
        public override bool CanPayCredit(double amount) => true;
        public override void Pay(Agent payee, double amount)
        {
            PayCash(payee, amount);
        }
        public override void PayCredit(Agent payee, double amount)
        {
            throw new Exception();
        }
    }
}