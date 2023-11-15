using System;
using System.Collections.Generic;
using System.Linq;
using EconSimVisual.Extensions;
using EconSimVisual.Managers;
using EconSimVisual.Simulation.Agents;
using EconSimVisual.Simulation.Base;
using EconSimVisual.Simulation.Polities;

namespace EconSimVisual.Simulation.Banks
{
    [Serializable]
    /// <summary>
    ///     Businesses that that take deposits and provide loans.
    /// </summary>
    internal class CommercialBank : Business, IDepository
    {
        public CommercialBank()
        {
            Deposits = new Deposits(this);
        }

        public override Town Town
        {
            get => base.Town;
            set
            {
                base.Town = value;
            }
        }

        private static int count = 1;
        private readonly int id = count++;
        protected override string DefaultName => "Bank " + id;
        public Deposits Deposits { get; }
        public double ReserveRatio => Deposits.Total == 0 ? 0 : Reserves / Deposits.Total;
        public double RequiredReserves => Deposits.Total * Town.Agents.CentralBank.RequiredReserveRatio;
        public double Reserves => Cash + BankAccounts.Sum(o => o.Balance);

        public override void FirstTick()
        {
            Income.InterestExpense += Deposits.DailyInterestExpenses;
            Deposits.ApplyInterest();
            base.FirstTick();
        }
    }
}