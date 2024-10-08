﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GhostTracker.Data
{
    /// <summary>
    ///     Entity framework модель для таблицы GhostBase.
    /// </summary>
    public class GhostBase
    {
        public ICollection<GhostTranslations> Translations { get; set; }
        [Key] public int Id { get; set; }
        public int MaxGhostSpeed { get; set; }
        public int MaxGhostSpeedLoS { get; set; }
        public int MaxSanityHunt { get; set; }
        public int MinGhostSpeed { get; set; }
        public int MinSanityHunt { get; set; }
        public List<ClueBase> ClueBase { get; set; }
        public List<GhostGuessQuestionBase> GhostGuessQuestionBase { get; set; }
        public List<SpeedRangesBase> SpeedRangesBase { get; set; }
        public List<UnfoldingItemBase> UnfoldingItemBase { get; set; }
        public string ImageFilePath { get; set; }
    }
}