﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PhasmophobiaCompanion.Data
{
    /// <summary>
    /// Entity framework модель для таблицы ExpandFieldWithImagesTranslations, содержащей переводы на множество языков.
    /// </summary>
    public class ExpandFieldWithImagesTranslations
    {
        public string Body { get; set; }

        public ExpandFieldWithImagesBase ExpandFieldWithImages { get; set; }

        public int ExpandFieldWithImagesBaseID { get; set; }

        public string Header { get; set; }

        [Key]
        public int ID { get; set; }

        public string LanguageCode { get; set; }
        public string Title { get; set; }
    }
}
