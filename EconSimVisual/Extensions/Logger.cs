using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EconSimVisual.Simulation.Base;

namespace EconSimVisual.Extensions
{
    [Serializable]
    internal class Logger
    {
        private readonly List<Log> _logs = new List<Log>();

        public int Count => _logs.Count;

        public void Log(string message, LogType type)
        {
            _logs.Add(new Log()
            {
                Message = message,
                Type = type,
                Date = Entity.Day,
                Created = DateTime.UtcNow
            });
        }

        public void Clear()
        {
            _logs.Clear();
        }

        public List<Log> GetLogs(IList filters, int days)
        {
            return _logs.Where(l => filters.Contains(l.Type)).Where(o => Entity.Day - o.Date <= days).OrderByDescending(l => l.Created).ToList();
        }
    }

    [Serializable]
    internal class Log
    {
        public string Message { get; set; }
        public LogType Type { get; set; }
        public int Date { get; set; }
        public DateTime Created { get; set; }

        public override string ToString()
        {
            return "Day " + Date + ": " + Message;
        }
    }

    internal enum LogType { Hiring, NonPayment, Error, FinancialTransaction, Securities, Death, Birth }
}
