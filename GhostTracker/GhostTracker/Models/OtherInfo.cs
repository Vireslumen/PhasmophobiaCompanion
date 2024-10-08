﻿using System.Collections.Generic;

namespace GhostTracker.Models
{
    /// <summary>
    ///     Представляет собой данные для некатегоризируемой страницы.
    /// </summary>
    public class OtherInfo : BaseTitledItem
    {
        public int Id { get; set; }
        public List<ExpandFieldWithImages> ExpandFieldsWithImages { get; set; }
        public List<UnfoldingItem> UnfoldingItems { get; set; }
    }
}