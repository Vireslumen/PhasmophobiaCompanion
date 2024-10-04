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
    ///     ViewModel для подробной страницы Особого режима.
    /// </summary>
    public class ChallengeModeDetailViewModel : BaseViewModel
    {
        public readonly DataService DataService;
        private ChallengeMode challengeMode;
        private ChallengeModeCommon challengeModeCommon;
        private DifficultyCommon difficultyCommon;
        private EquipmentCommon equipmentCommon;

        public ChallengeModeDetailViewModel(ChallengeMode challengeMode)
        {
            DataService = DependencyService.Get<DataService>();
            DifficultyCommon = DataService.GetDifficultyCommon();
            EquipmentCommon = DataService.GetEquipmentCommon();
            ChallengeMode = challengeMode;
            ChallengeModeCommon = DataService.GetChallengeModeCommon();
            SetChallengeModeData();
            EquipmentSelectedCommand = new Command<Equipment>(OnEquipmentSelected);
            MapSelectedCommand = new Command(OnMapSelected);
        }

        public ChallengeMode ChallengeMode
        {
            get => challengeMode;
            set
            {
                challengeMode = value;
                OnPropertyChanged();
            }
        }
        public ChallengeModeCommon ChallengeModeCommon
        {
            get => challengeModeCommon;
            set => SetProperty(ref challengeModeCommon, value);
        }
        public DifficultyCommon DifficultyCommon
        {
            get => difficultyCommon;
            set
            {
                difficultyCommon = value;
                OnPropertyChanged();
            }
        }
        public EquipmentCommon EquipmentCommon
        {
            get => equipmentCommon;
            set => SetProperty(ref equipmentCommon, value);
        }
        public ICommand EquipmentSelectedCommand { get; private set; }
        public ICommand MapSelectedCommand { get; private set; }

        public void Cleanup()
        {
            EquipmentSelectedCommand = null;
            MapSelectedCommand = null;
        }

        /// <summary>
        ///     Переход на подробную страницу выбранного снаряжения.
        /// </summary>
        /// <param name="selectedEquipment">Выбранное снаряжение.</param>
        private async void OnEquipmentSelected(Equipment selectedEquipment)
        {
            try
            {
                if (IsNavigating) return;
                if (selectedEquipment == null) return;
                // Логика для открытия страницы деталей снаряжения
                var detailPage = new EquipmentDetailPage(selectedEquipment);
                await NavigateWithLoadingAsync(detailPage);
            }
            catch (Exception ex)
            {
                Log.Error(ex,
                    "Ошибка во время перехода на подробную страницу снаряжения из страницы особого режима ChallengeModeDetailPage.");
            }
        }

        /// <summary>
        ///     Переход на подробную страницу выбранной карты.
        /// </summary>
        private async void OnMapSelected()
        {
            try
            {
                if (IsNavigating) return;
                if (ChallengeMode.ChallengeMap == null) return;
                // Логика для открытия страницы деталей карты
                var detailPage = new MapDetailPage(ChallengeMode.ChallengeMap);
                await NavigateWithLoadingAsync(detailPage);
            }
            catch (Exception ex)
            {
                Log.Error(ex,
                    "Ошибка во время перехода на подробную страницу карты из страницы особого режима ChallengeModeDetailPage.");
            }
        }

        /// <summary>
        ///     Установка карты, снаряжения для выбранного особого режима.
        /// </summary>
        private void SetChallengeModeData()
        {
            try
            {
                challengeMode.ChallengeMap = DataService
                    .GetMaps()
                    .FirstOrDefault(m => m.Id == challengeMode.MapId);
                challengeMode.ChallengeEquipments = new List<Equipment>
                (DataService.GetEquipments().Where(e => challengeMode.EquipmentsId.Contains(e.Id))
                    .ToList());
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка при установке данных в ChallengeModeDetailViewModel.");
                challengeMode.ChallengeMap = null;
                challengeMode.ChallengeEquipments = new List<Equipment>();
            }
        }
    }
}