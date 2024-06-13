﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using PhasmophobiaCompanion.Models;
using PhasmophobiaCompanion.Services;
using PhasmophobiaCompanion.Views;
using Serilog;
using Xamarin.Forms;

namespace PhasmophobiaCompanion.ViewModels
{
    /// <summary>
    ///     ViewModel для подробной страницы Особого режима.
    /// </summary>
    public class ChallengeModeDetailViewModel : BaseViewModel
    {
        public readonly DataService dataService;
        private ChallengeMode challengeMode;
        private ChallengeModeCommon challengeModeCommon;
        private DifficultyCommon difficultyCommon;
        private EquipmentCommon equipmentCommon;

        public ChallengeModeDetailViewModel(ChallengeMode challengeMode)
        {
            try
            {
                dataService = DependencyService.Get<DataService>();
                DifficultyCommon = dataService.GetDifficultyCommon();
                EquipmentCommon = dataService.GetEquipmentCommon();
                ChallengeMode = challengeMode;
                ChallengeModeCommon = dataService.GetChallengeModeCommon();
                SetChallengeModeData();
                EquipmentSelectedCommand = new Command<Equipment>(OnEquipmentSelected);
                MapSelectedCommand = new Command(OnMapSelected);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка во время инициализации ChallengeModeDetailViewModel.");
                throw;
            }
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
        public EquipmentCommon EquipmentCommon
        {
            get => equipmentCommon;
            set => SetProperty(ref equipmentCommon, value);
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
        public ICommand EquipmentSelectedCommand { get; private set; }
        public ICommand MapSelectedCommand { get; private set; }

        /// <summary>
        ///     Переход на подробную страницу выбранного снаряжения.
        /// </summary>
        /// <param name="selectedEquipment">Выбранное снаряжение.</param>
        private async void OnEquipmentSelected(Equipment selectedEquipment)
        {
            try
            {
                if (isNavigating) return;
                if (selectedEquipment != null)
                {
                    // Логика для открытия страницы деталей снаряжения
                    var detailPage = new EquipmentDetailPage(selectedEquipment);
                    await NavigateWithLoadingAsync(detailPage);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex,
                    "Ошибка во время перехода на подробную страницу снаряжения из страницы особого режима ChallengeModeDetailPage.");
                throw;
            }
        }

        /// <summary>
        ///     Переход на подробную страницу выбранной карты.
        /// </summary>
        private async void OnMapSelected()
        {
            try
            {
                if (isNavigating) return;
                if (ChallengeMode.ChallengeMap != null)
                {
                    // Логика для открытия страницы деталей карты
                    var detailPage = new MapDetailPage(ChallengeMode.ChallengeMap);
                    await NavigateWithLoadingAsync(detailPage);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex,
                    "Ошибка во время перехода на подробную страницу карты из страницы особого режима ChallengeModeDetailPage.");
                throw;
            }
        }

        /// <summary>
        ///     Установка карты, снаряжения для выбранного особого режима.
        /// </summary>
        private void SetChallengeModeData()
        {
            challengeMode.ChallengeMap = dataService.GetMaps()
                .Where(m => m.ID == challengeMode.MapID)
                .FirstOrDefault();
            challengeMode.ChallengeEquipments = new List<Equipment>
            (dataService.GetEquipments().Where(e => challengeMode.EquipmentsID.Contains(e.ID))
                .ToList());
        }
    }
}