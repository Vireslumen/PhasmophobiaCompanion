﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using GhostTracker.Models;
using GhostTracker.Services;
using GhostTracker.Views;
using Serilog;
using Xamarin.Forms;

namespace GhostTracker.ViewModels
{
    /// <summary>
    ///     ViewModel для подробной страницы доказательств.
    /// </summary>
    public class ClueDetailViewModel : UnfoldingItemsViewModel
    {
        private Clue clue;
        private ClueCommon clueCommon;

        public ClueDetailViewModel(Clue clue)
        {
            var dataService = DependencyService.Get<DataService>();
            Clue = clue;
            ClueCommon = dataService.GetClueCommon();
            Clue.ClueRelatedEquipments = new List<Equipment>
            (dataService.GetEquipments().Where(e => Clue.EquipmentsId.Contains(e.Id))
                .ToList());
            foreach (var item in Clue.UnfoldingItems) item.IsExpanded = true;
            foreach (var item in Clue.ExpandFieldsWithImages) item.IsExpanded = true;
            ClueSelectedCommand = new Command<Clue>(OnClueSelected);
            GhostSelectedCommand = new Command<Ghost>(OnGhostSelected);
            ImageTappedCommand = new Command<ImageWithDescription>(OpenImagePage);
            EquipmentSelectedCommand = new Command<Equipment>(OpenEquipPage);
        }

        public Clue Clue
        {
            get => clue;
            set
            {
                clue = value;
                OnPropertyChanged();
            }
        }
        public ClueCommon ClueCommon
        {
            get => clueCommon;
            set
            {
                clueCommon = value;
                OnPropertyChanged();
            }
        }
        public ICommand ClueSelectedCommand { get; protected set; }
        public ICommand EquipmentSelectedCommand { get; protected set; }
        public ICommand GhostSelectedCommand { get; protected set; }
        public ICommand ImageTappedCommand { get; protected set; }

        public void Cleanup()
        {
            ToggleExpandCommand = null;
            ClueSelectedCommand = null;
            GhostSelectedCommand = null;
            ImageTappedCommand = null;
            EquipmentSelectedCommand = null;
        }

        /// <summary>
        ///     Переход на страницу доказательства при нажатии на неё.
        /// </summary>
        private async void OnClueSelected(Clue clueItem)
        {
            try
            {
                if (IsNavigating) return;
                var page = new ClueDetailPage(clueItem);
                await NavigateWithLoadingAsync(page);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка во время перехода на страницу доказательства с подробной страницы доказательства.");
            }
        }

        /// <summary>
        ///     Переход на страницу призрака при нажатии на него.
        /// </summary>
        private async void OnGhostSelected(Ghost ghostItem)
        {
            try
            {
                if (IsNavigating) return;
                var page = new GhostDetailPage(ghostItem);
                await NavigateWithLoadingAsync(page);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка во время перехода на страницу призрака с подробной страницы доказательства.");
            }
        }

        /// <summary>
        ///     Переход на страницу связанного с доказательством снаряжения при нажатии на него.
        /// </summary>
        private async void OpenEquipPage(Equipment equipItem)
        {
            try
            {
                if (IsNavigating) return;
                var page = new EquipmentDetailPage(equipItem);
                await NavigateWithLoadingAsync(page);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка во время перехода на страницу снаряжения с подробной страницы доказательства.");
            }
        }

        private async void OpenImagePage(ImageWithDescription image)
        {
            try
            {
                await Application.Current.MainPage.Navigation.PushModalAsync(new ImagePage(image));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка при открытии страницы изображения.");
            }
        }
    }
}