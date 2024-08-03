﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Timers;
using System.Windows.Input;
using PhasmophobiaCompanion.Interfaces;
using PhasmophobiaCompanion.Models;
using PhasmophobiaCompanion.Services;
using PhasmophobiaCompanion.Views;
using Serilog;
using Xamarin.Forms;

namespace PhasmophobiaCompanion.ViewModels
{
    /// <summary>
    ///     ViewModel для страницы определения призрака.
    /// </summary>
    public class GhostGuessViewModel : BaseViewModel
    {
        private const int MaxClicks = 6;
        private const int MimicId = 20;
        private const int SpeedPoints = 10;
        private const int GhostListCount = 6;
        private const int ValueForVisibility = 90;
        private const double SpeedCoef = 0.88;
        private const int ResetTime = 2;
        public readonly DataService dataService;
        private readonly List<DateTime> lastClickTimes;
        private readonly Timer resetTimer;
        public List<Ghost> Ghosts;
        public List<GhostGuessQuestion> Questions;
        public List<SupposedGhost> SupposedGhosts;
        private bool isSpeedMatter;
        private double averageFrequency;
        private GhostGuessQuestionCommon ghostGuessQuestionCommon;
        private ObservableCollection<GhostGuessQuestion> displayedQuestions;
        private ObservableCollection<object> selectedClues;
        private ObservableCollection<string> items;
        private ObservableCollection<SupposedGhost> sortedGhosts;

        public GhostGuessViewModel()
        {
            //Загрузка всех данных для страницы
            dataService = DependencyService.Get<DataService>();
            ghostGuessQuestionCommon = dataService.GetGhostGuessQuestionCommon();
            Ghosts = dataService.GetGhosts();
            Questions = dataService.GetGhostGuessQuestions();
            AllClues = dataService.GetClues();
            //Настройка данных для страницы
            SelectedClues = new ObservableCollection<object>();
            SupposedGhosts = Ghosts.Select(ghost => new SupposedGhost
            {
                Ghost = ghost,
                Points = 0,
                Percent = 0
            }).ToList();
            displayedQuestions = new ObservableCollection<GhostGuessQuestion>(Questions);
            Items = new ObservableCollection<string>
            {
                GhostGuessQuestionCommon.AnswerDontKnow,
                GhostGuessQuestionCommon.AnswerYes,
                GhostGuessQuestionCommon.AnswerThinkSo,
                GhostGuessQuestionCommon.AnswerNo
            };

            //Настройка данных для вычисления скорости призрака
            resetTimer = new Timer(ResetTime * 1000);
            resetTimer.Elapsed += (sender, e) => OnUpdateGhost();
            resetTimer.AutoReset = false;
            lastClickTimes = new List<DateTime>();

            // Инициализация команд
            PressButtonCommand = new Command(OnButtonPressed);
            UpdateGhost = new Command(OnUpdateGhost);
            GhostTappedCommand = new Command<SupposedGhost>(OnGhostTapped);
        }

        public bool IsSpeedMatter
        {
            get => isSpeedMatter;
            set
            {
                isSpeedMatter = value;
                OnUpdateGhost();
                OnPropertyChanged();
            }
        }
        public double AverageFrequency
        {
            get => averageFrequency;
            set
            {
                if (averageFrequency != value)
                {
                    averageFrequency = value;
                    OnPropertyChanged();
                }
            }
        }
        public GhostGuessQuestionCommon GhostGuessQuestionCommon
        {
            get => ghostGuessQuestionCommon;
            set
            {
                ghostGuessQuestionCommon = value;
                OnPropertyChanged();
            }
        }
        public ICommand GhostTappedCommand { get; private set; }
        public ICommand PressButtonCommand { protected set; get; }
        public ICommand UpdateGhost { protected set; get; }
        public List<Clue> AllClues { get; set; }
        public ObservableCollection<GhostGuessQuestion> DisplayedQuestions
        {
            get => displayedQuestions;
            set => SetProperty(ref displayedQuestions, value);
        }
        public ObservableCollection<object> SelectedClues
        {
            get => selectedClues;
            set => SetProperty(ref selectedClues, value);
        }
        public ObservableCollection<string> Items
        {
            get => items;
            set => SetProperty(ref items, value);
        }
        public ObservableCollection<SupposedGhost> SortedGhosts
        {
            get => sortedGhosts;
            set
            {
                sortedGhosts = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Добавление или вычет поинтов Points за ответы на вопросы.
        /// </summary>
        private void AddPointsForQuestionAnswers()
        {
            try
            {
                foreach (var question in DisplayedQuestions)
                foreach (var ghost in question.Ghosts)
                {
                    var supposedGhost = SupposedGhosts.FirstOrDefault(sg => sg.Ghost == ghost);
                    if (supposedGhost != null)
                        supposedGhost.Points += RoundAwayFromZero(CalculateAnswerResult(question.Answer,
                            question.AnswerMeaning,
                            question.AnswerNegativeMeaning, question.Ghosts.Count > 1 && ghost.ID == MimicId));
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex,
                    "Ошибка при добавлении или вычетании поинтов за ответы на вопросы на странице определения прирака.");
            }
        }

        /// <summary>
        ///     Добавление поинтов Points за совпадающую скорость.
        /// </summary>
        private void AddPointsForSpeed()
        {
            try
            {
                if (IsSpeedMatter)
                    foreach (var supposedGhost in SupposedGhosts)
                    foreach (var speedRange in supposedGhost.Ghost.SpeedRanges)
                        if (AverageFrequency > speedRange.Min / 100.0 && AverageFrequency < speedRange.Max / 100.0)
                        {
                            supposedGhost.Points += SpeedPoints;
                            break;
                        }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка при добавлении поинтов за совпадающую скорость на странице определения прирака.");
            }
        }

        /// <summary>
        ///     Расчёт начисляемых Points за ответ на данный вопрос.
        /// </summary>
        /// <param name="index">Индекс ответа</param>
        /// <param name="answerValue">Коэффициент начисляемых поинтов на положительный ответ</param>
        /// <param name="answerNegativeValue">Коэффициент начисляемых поинтов на отрицательный ответ</param>
        /// <param name="mimic">Является ли призрак мимиком</param>
        /// <returns>Число начисляемых поинтов.</returns>
        private double CalculateAnswerResult(int index, int answerValue, int answerNegativeValue, bool mimic)
        {
            double[] values = {0, 1, 0.01, 1};
            switch (index)
            {
                case 0: return 0;
                case 1: return values[index] * answerValue;
                case 2: return values[index] * answerValue;
                case 3:
                    if (mimic) return 0;
                    return values[index] * answerNegativeValue;
                default: return 0;
            }
        }

        /// <summary>
        ///     Предотвращение утечек памяти.
        /// </summary>
        public void Cleanup()
        {
            PressButtonCommand = null;
            UpdateGhost = null;
            GhostTappedCommand = null;
        }

        /// <summary>
        ///     Очистить видимость вопросов на странице.
        /// </summary>
        private void ClearQuestionVisibility()
        {
            try
            {
                for (var i = 0; i < DisplayedQuestions.Count; i++)
                {
                    DisplayedQuestions[i].Visibility = -1;
                    DisplayedQuestions[i].IsVisible = true;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка очищения видимости вопросов на странице.");
            }
        }

        /// <summary>
        ///     Метод операций связанных с вычислением скорости по нажатию на кнопку.
        /// </summary>
        private void OnButtonPressed()
        {
            try
            {
                DependencyService.Get<IHapticFeedback>().ExecuteHapticFeedback();
                var now = DateTime.Now;

                // Очистить расчёт скорости, если время между последним и текущим кликом больше ResetTime
                if (lastClickTimes.Any() && (now - lastClickTimes.Last()).TotalSeconds > ResetTime)
                    lastClickTimes.Clear();

                // Добавляем текущее время клика
                lastClickTimes.Add(now);

                // Если кликов больше, чем MaxClicks, удаляем самый старый
                if (lastClickTimes.Count > MaxClicks) lastClickTimes.RemoveAt(0);

                UpdateAverageFrequency();
                resetTimer.Stop();
                resetTimer.Start();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка во время вычисления скорости призрака по нажатию на кнопку.");
            }
        }

        /// <summary>
        ///     Переход на подробную страницу призрака по нажатию на него.
        /// </summary>
        private async void OnGhostTapped(SupposedGhost ghostItem)
        {
            try
            {
                if (isNavigating) return;
                var page = new GhostDetailPage(ghostItem.Ghost);
                await NavigateWithLoadingAsync(page);
            }
            catch (Exception ex)
            {
                Log.Error(ex,
                    "Ошибка во время перехода на подробную страницу призрака GhostDetailPage с страницы определения призрака GhostGuessQuestionPage.");
            }
        }

        /// <summary>
        ///     Метод обновления страницы на основе данных на ней
        /// </summary>
        /// <param name="selectedItem">Выбранный элемент в Picker</param>
        private void OnUpdateGhost(object selectedItem = null)
        {
            try
            {
                ClearQuestionVisibility();
                UpdateQuestionVisibilityBasedOnClues();
                UpdateSupposedGhostsBasedOnClues();
                AddPointsForSpeed();
                AddPointsForQuestionAnswers();
                UpdateQuestionVisibilityBasedOnGhostPoints();
                UpdateGhostsPercentAndSort();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка во время обновления страницы на основе данных на ней.");
            }
        }

        /// <summary>
        ///     Метод округления в большую сторону от нуля.
        /// </summary>
        /// <param name="value">Число для округления</param>
        /// <returns>Число полученное в результате округления.</returns>
        private static int RoundAwayFromZero(double value)
        {
            if (value > 0)
                return (int) Math.Ceiling(value);
            return (int) Math.Floor(value);
        }

        /// <summary>
        ///     Вычисление текущей скорости призрака по нажатию на кнопку.
        /// </summary>
        private void UpdateAverageFrequency()
        {
            try
            {
                if (lastClickTimes.Count > 1)
                {
                    var intervals = lastClickTimes
                        .Zip(lastClickTimes.Skip(1), (previous, next) => (next - previous).TotalSeconds).ToList();
                    if (intervals.Count > 0)
                    {
                        var averageInterval = intervals.Average() / SpeedCoef;
                        AverageFrequency = averageInterval > 0 ? 1 / averageInterval : 0;
                    }
                    else
                    {
                        AverageFrequency = 0;
                    }
                }
                else
                {
                    AverageFrequency = 0;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка во время вычисления скорости призрака в методе UpdateAverageFrequency.");
            }
        }

        /// <summary>
        ///     Обновление шансов призраков и сортировки по шансу
        /// </summary>
        private void UpdateGhostsPercentAndSort()
        {
            try
            {
                double totalPoints = SupposedGhosts.Where(sg => sg.Points > 0).Sum(sg => sg.Points);
                foreach (var supposedGhost in SupposedGhosts)
                    if (totalPoints > 0 && supposedGhost.Points > 0)
                        supposedGhost.Percent = RoundAwayFromZero(supposedGhost.Points * 100 / totalPoints);
                    else
                        supposedGhost.Percent = 0;
                SortedGhosts = new ObservableCollection<SupposedGhost>(SupposedGhosts
                    .OrderByDescending(sg => sg.Percent)
                    .Where(sg => sg.Percent > 0).Take(GhostListCount).ToList());
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка при обновлении шансов и сортировки призраков на странице определения призрака.");
            }
        }

        /// <summary>
        ///     Обновление видимости вопросов на основе улик Clue.
        /// </summary>
        private void UpdateQuestionVisibilityBasedOnClues()
        {
            try
            {
                foreach (var question in DisplayedQuestions)
                {
                    var isVisible = question.Ghosts.Where(ghost => !(ghost.ID == MimicId && question.Ghosts.Count > 1))
                        .Any(
                            ghost =>
                                !SelectedClues.Any() ||
                                SelectedClues.All(selectedClue =>
                                    ghost.Clues.Any(clue =>
                                        clue.Title == ((Clue) selectedClue).Title)));
                    question.Visibility = isVisible ? -1 : 0;
                    question.IsVisible = isVisible;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex,
                    "Ошибка во время обновления видимости вопросов на основе улик Clue на странице определения призрака.");
            }
        }

        /// <summary>
        ///     Обновление видимости вопросов на основе полученных ответов на вопросы.
        /// </summary>
        private void UpdateQuestionVisibilityBasedOnGhostPoints()
        {
            try
            {
                var validSupposedGhosts = SupposedGhosts
                    .Where(supposedGhost => supposedGhost.Points > ValueForVisibility)
                    .Select(sg => sg.Ghost.Title).ToList();
                if (validSupposedGhosts.Count > 0)
                    foreach (var question in DisplayedQuestions)
                    {
                        var shouldBeVisible = question.Ghosts
                            .Where(ghost => !(ghost.ID == MimicId && question.Ghosts.Count > 1))
                            .Any(ghost => SupposedGhosts.Any(sg => sg.Ghost == ghost) &&
                                          (validSupposedGhosts.Contains(ghost.Title) || (question.Ghosts.Count == 1 &&
                                              question.Ghosts.FirstOrDefault().ID == MimicId))) || question.Answer != 0;

                        if (shouldBeVisible)
                        {
                            question.Visibility = -1;
                            question.IsVisible = true;
                        }
                        else
                        {
                            question.Visibility = 0;
                            question.IsVisible = false;
                        }
                    }
            }
            catch (Exception ex)
            {
                Log.Error(ex,
                    "Ошибка при обновлении видимости вопросов на основе данных ответов на странице определения призрака.");
            }
        }

        /// <summary>
        ///     Обновление списка предполагаемых призраков на основе улик Clue.
        /// </summary>
        private void UpdateSupposedGhostsBasedOnClues()
        {
            try
            {
                SupposedGhosts = Ghosts.Where(ghost =>
                    !SelectedClues.Any() ||
                    SelectedClues.All(selectedClue =>
                        ghost.Clues.Any(clue =>
                            clue.Title == ((Clue) selectedClue).Title))).Select(ghost => new SupposedGhost
                {
                    Ghost = ghost,
                    Points = 5,
                    Percent = 0
                }).ToList();
            }
            catch (Exception ex)
            {
                Log.Error(ex,
                    "Ошибка во время обновления списка предполагаемых призраков на основе улик Clue на странице определения призрака.");
            }
        }
    }
}