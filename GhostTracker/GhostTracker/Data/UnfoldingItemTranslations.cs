﻿using System.ComponentModel.DataAnnotations;

namespace GhostTracker.Data
{
    /// <summary>
    ///     Entity framework модель для таблицы UnfoldingItemTranslations, содержащей переводы на множество языков.
    /// </summary>
    public class UnfoldingItemTranslations
    {
        [Key] public int Id { get; set; }
        public string Body { get; set; }
        public string Header { get; set; }
        public string LanguageCode { get; set; }
        public string Title { get; set; }
    }
}