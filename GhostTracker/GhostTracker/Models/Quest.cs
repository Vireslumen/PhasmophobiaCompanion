﻿namespace GhostTracker.Models
{
    /// <summary>
    ///     Представляет собой квест на игровые сессии для получения игровой валюты.
    /// </summary>
    public class Quest
    {
        public int Id { get; set; }
        public int Reward { get; set; }
        /// <summary>
        ///     Условие выполнения квеста.
        /// </summary>
        public string Clause { get; set; }
        public string Title { get; set; }
        /// <summary>
        ///     Подсказка как выполнить квест.
        /// </summary>
        public string Tip { get; set; }
        /// <summary>
        ///     Еженедельный или ежедневный квест.
        /// </summary>
        public string Type { get; set; }
    }
}