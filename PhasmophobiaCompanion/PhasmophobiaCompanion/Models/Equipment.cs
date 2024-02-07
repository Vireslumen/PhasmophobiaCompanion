﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace PhasmophobiaCompanion.Models
{
    /// <summary>
    /// Представляет собой снаряжение.
    /// </summary>
    public class Equipment : BaseDisplayableItem
    {
        public int ID { get; set; }
        // Цена снаряжения.
        public int Cost { get; set; }

        // Максимальное количество снаряжения на одну сессию.
        public int MaxLimit { get; set; }

        // Прочие характеристики снаряжения.
        public ObservableCollection<OtherEquipmentStat> OtherEquipmentStats { get; set; }

        // Тир снаряжения.
        public string Tier { get; set; }
        public ObservableCollection<UnfoldingItem> UnfoldingItems { get; set; }

        // Цена для разблокировки снаряжения.
        public int UnlockCost { get; set; }

        // Уровень для разблокировки снаряжения.
        public int UnlockLevel { get; set; }
    }
}
