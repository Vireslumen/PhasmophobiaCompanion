﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using PhasmophobiaCompanion.Interfaces;
using PhasmophobiaCompanion.Models;
using PhasmophobiaCompanion.Services;
using PhasmophobiaCompanion.Views;
using Rg.Plugins.Popup.Services;
using Serilog;
using Xamarin.Forms;

namespace PhasmophobiaCompanion.ViewModels
{
    /// <summary>
    ///     ViewModel для страницы списка призраков, поддерживает поиск и фильтрацию.
    /// </summary>
    public class GhostsViewModel : SearchableViewModel, IFilterable
    {
        private readonly DataService dataService;
        private readonly List<Ghost> ghosts;
        private GhostCommon ghostCommon;
        private List<Clue> allClues;
        private List<object> selectedCluesSaved;
        private ObservableCollection<Ghost> filteredGhosts;
        private ObservableCollection<object> selectedClues;

        public GhostsViewModel()
        {
            try
            {
                dataService = DependencyService.Get<DataService>();
                //Загрузка всех призраков и улик.
                ghosts = dataService.GetGhosts().OrderBy(g => g.Title).ToList();
                allClues = dataService.GetClues();
                SelectedClues = new ObservableCollection<object>();
                selectedCluesSaved = new List<object>();
                Ghosts = new ObservableCollection<Ghost>(ghosts);
                AllClues = new List<Clue>(allClues);
                //Загрузка данных для интерфейса.
                GhostCommon = dataService.GetGhostCommon();
                // Инициализация команд
                GhostSelectedCommand = new Command<Ghost>(OnGhostSelected);
                FilterCommand = new Command(OnFilterTapped);
                FilterApplyCommand = new Command(OnFilterApplyTapped);
                FilterClearCommand = new Command(OnFilterClearTapped);
                BackgroundClickCommand = new Command(ExecuteBackgroundClick);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка во время инициализации GhostsViewModel.");
                throw;
            }
        }

        /// <summary>
        ///     Общие текстовые данные для интерфейса относящегося к призракам.
        /// </summary>
        public GhostCommon GhostCommon
        {
            get => ghostCommon;
            set
            {
                ghostCommon = value;
                OnPropertyChanged();
            }
        }
        public ICommand BackgroundClickCommand { get; private set; }
        public ICommand FilterApplyCommand { get; private set; }
        public ICommand FilterClearCommand { get; private set; }
        public ICommand FilterCommand { get; private set; }
        public ICommand GhostSelectedCommand { get; private set; }
        public List<Clue> AllClues
        {
            get => allClues;
            set => SetProperty(ref allClues, value);
        }
        /// <summary>
        ///     Отображаемая коллекция призраков, которая может быть отфильтрована на основе заданных критериев.
        /// </summary>
        public ObservableCollection<Ghost> Ghosts
        {
            get => filteredGhosts;
            set => SetProperty(ref filteredGhosts, value);
        }
        public ObservableCollection<object> SelectedClues
        {
            get => selectedClues;
            set => SetProperty(ref selectedClues, value);
        }
        public string FilterColor
        {
            get
            {
                var currentTheme = Application.Current.RequestedTheme;
                return SelectedClues.Any() ? currentTheme == OSAppTheme.Dark ? "#FD7E14" : "#FD7E14" : "White";
            }
        }

        /// <summary>
        ///     Фильтрация списка призраков на основе выбранных улик.
        /// </summary>
        public void Filter()
        {
            try
            {
                UpdateFilteredGhosts();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка во время фильтрации призраков.");
                throw;
            }
        }

        /// <summary>
        ///     При нажатии на фон сбросить параметры фильтра до состояния на момента его открытия.
        /// </summary>
        private void ExecuteBackgroundClick()
        {
            try
            {
                SelectedClues = new ObservableCollection<object>(selectedCluesSaved);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка при сбрасывании параметров фильтра до состояния на момент открытия.");
                throw;
            }
        }

        /// <summary>
        ///     Принятие фильтрации и закрытие окна фильтра.
        /// </summary>
        private void OnFilterApplyTapped()
        {
            try
            {
                selectedCluesSaved = new List<object>(SelectedClues);
                Filter();
                OnPropertyChanged(nameof(FilterColor));
                PopupNavigation.Instance.PopAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка при принятии фильтрации.");
                throw;
            }
        }

        /// <summary>
        ///     Сброс параметров фильтрации и закрытие окна фильтрации.
        /// </summary>
        private void OnFilterClearTapped()
        {
            try
            {
                selectedCluesSaved = new List<object>();
                SelectedClues = new ObservableCollection<object>();
                Filter();
                OnPropertyChanged(nameof(FilterColor));
                PopupNavigation.Instance.PopAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка при сбросе фильтрации.");
                throw;
            }
        }

        /// <summary>
        ///     Открытие страницы фильтра призраков.
        /// </summary>
        private async void OnFilterTapped()
        {
            try
            {
                // Логика для открытия страницы фильтра
                var filterPage = new FilterPage(this);
                await PopupNavigation.Instance.PushAsync(filterPage);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка во время открытия фильтра на странице призраков GhostPage.");
                throw;
            }
        }

        /// <summary>
        ///     Переход на подробную страницу выбранного призрака.
        /// </summary>
        /// <param name="selectedGhost">Выбранный призрак.</param>
        private async void OnGhostSelected(Ghost selectedGhost)
        {
            try
            {
                if (selectedGhost == null) return;
                // Логика для открытия страницы деталей призрака
                var detailPage = new GhostDetailPage(selectedGhost);
                await Application.Current.MainPage.Navigation.PushAsync(detailPage);
            }
            catch (Exception ex)
            {
                Log.Error(ex,
                    "Ошибка во время перехода на подробную страницу призрак из страницы призраков GhostPage.");
                throw;
            }
        }

        /// <summary>
        ///     Фильтрация коллекции в соответствии с поисковым запросом.
        /// </summary>
        protected override void PerformSearch()
        {
            try
            {
                UpdateFilteredGhosts();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка во время поиска призраков.");
                throw;
            }
        }

        /// <summary>
        ///     Фильтрация призраков по поисковому запросу и выбранным параметрам фильтра.
        /// </summary>
        private void UpdateFilteredGhosts()
        {
            try
            {
                var filtered = ghosts.Where(ghost => (string.IsNullOrWhiteSpace(SearchText) ||
                                                      ghost.Title.ToLowerInvariant()
                                                          .Contains(SearchText.ToLowerInvariant()))
                                                     &&
                                                     (!selectedCluesSaved.Any() ||
                                                      selectedCluesSaved.All(selectedClue =>
                                                          ghost.Clues.Any(clue =>
                                                              clue.Title == ((Clue) selectedClue).Title))))
                    .ToList();

                Ghosts = new ObservableCollection<Ghost>(filtered);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка во время обновления отфильтрованных призраков.");
                throw;
            }
        }
    }
}