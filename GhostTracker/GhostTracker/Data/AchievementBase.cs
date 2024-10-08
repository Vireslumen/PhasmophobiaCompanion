﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GhostTracker.Data
{
    /// <summary>
    ///     Entity framework модель для таблицы AchievementBase.
    /// </summary>
    public class AchievementBase
    {
        [Key] public int Id { get; set; }
        public string ImageFilePath { get; set; }
        public ICollection<AchievementTranslations> Translations { get; set; }
    }
}