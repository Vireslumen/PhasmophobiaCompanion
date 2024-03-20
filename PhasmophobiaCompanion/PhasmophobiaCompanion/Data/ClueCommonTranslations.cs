﻿using System.ComponentModel.DataAnnotations;

namespace PhasmophobiaCompanion.Data
{
    /// <summary>
    ///     Entity framework модель для таблицы ClueCommonTranslations.
    /// </summary>
    public class ClueCommonTranslations
    {
        [Key] public int ID { get; set; }
        public string LanguageCode { get; set; }
        public string OtherGhosts { get; set; }
        public string RelatedEquipment { get; set; }
    }
}