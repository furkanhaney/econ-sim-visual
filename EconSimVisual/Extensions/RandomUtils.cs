﻿using System;
using System.Collections.Generic;
using System.Windows.Media;
using RandomNameGenerator;
using EconSimVisual.Simulation.Agents;
using System.Linq;

namespace EconSimVisual.Extensions
{
    internal static class RandomUtils
    {
        private static readonly Random Rnd = new Random();
        public static Simulation.Agents.Gender GetRandomGender()
        {
            return (Simulation.Agents.Gender)Rnd.Next(2);
        }
        public static string GetRandomFirstName(Simulation.Agents.Gender gender)
        {
            var gender2 = (gender == Simulation.Agents.Gender.Male) ? RandomNameGenerator.Gender.Male : RandomNameGenerator.Gender.Female;
            return NameGenerator.GenerateFirstName(gender2).FormatName();
        }
        public static string GetRandomLastName()
        {
            return NameGenerator.GenerateLastName().FormatName();
        }
        public static SolidColorBrush GetRandomBackgroundColor()
        {
            return new SolidColorBrush(Color.FromRgb((byte)(255 - Rnd.Next(75)), (byte)(255 - Rnd.Next(50)), (byte)(255 - Rnd.Next(25))));
        }

        public static T GetRandom<T>(this T[] array)
        {
            return array[Rnd.Next(array.Length)];
        }
        public static T GetRandom<T>(this IList<T> array)
        {
            return array[Rnd.Next(array.Count)];
        }
        public static IList<T> Shuffle<T>(this IList<T> list)
        {
            var newList = new List<T>(list);
            var n = newList.Count;
            while (n > 1)
            {
                n--;
                var k = Rnd.Next(n + 1);
                var value = newList[k];
                newList[k] = newList[n];
                newList[n] = value;
            }
            return newList;
        }
        public static IList<T> Sample<T>(this IList<T> list, int size)
        {
            var count = list.Count;
            var nums = new HashSet<int>();
            while (nums.Count < size)
                nums.Add(Rnd.Next(count));
            IList<T> elements = new List<T>();
            foreach (var num in nums)
                elements.Add(list[num]);
            return elements;
        }
    }
}
