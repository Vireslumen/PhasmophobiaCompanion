﻿using System.ComponentModel.DataAnnotations;

namespace GhostTracker.Data
{
    /// <summary>
    ///     Entity framework модель для таблицы TipsTranslations, содержащей подсказки и их переводы на множество языков.
    /// </summary>
    public class TipsTranslations
    {
        [Key] public int Id { get; set; }
        public string LanguageCode { get; set; }
        public string Level { get; set; }
        public string Tip { get; set; }
    }
}