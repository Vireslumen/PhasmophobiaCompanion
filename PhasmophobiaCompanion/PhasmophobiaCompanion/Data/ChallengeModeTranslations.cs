﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PhasmophobiaCompanion.Data
{
    /// <summary>
    /// Entity framework модель для таблицы ChallengeModeTranslations, содержащей переводы на множество языков.
    /// </summary>
    public class ChallengeModeTranslations
    {
        [Key]
        public int ID { get; set; }
        public int ChallengeModeBaseID { get; set; }
        public string LanguageCode { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }
}
