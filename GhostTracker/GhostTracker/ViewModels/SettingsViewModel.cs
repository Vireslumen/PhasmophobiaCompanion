﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Input;
using GhostTracker.Models;
using GhostTracker.Services;
using GhostTracker.Views;
using Newtonsoft.Json;
using Rg.Plugins.Popup.Services;
using Serilog;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace GhostTracker.ViewModels
{
    /// <summary>
    ///     ViewModel для страницы настроек.
    /// </summary>
    public class SettingsViewModel : BaseViewModel
    {
        private readonly DataService dataService;
        private bool isLoggingEnabled;
        private bool shakeActive;
        private List<string> languages;
        private List<string> tipLevels;
        private SettingsCommon settingsCommon;
        private string selectedLanguage;
        private string selectedTipLevel;

        public SettingsViewModel()
        {
            dataService = DependencyService.Get<DataService>();
            TipLevels = dataService.GetTipLevels();
            Languages = new List<string>(LanguageDictionary.LanguageMap.Keys);
            selectedLanguage = LanguageDictionary.GetLanguageNameByCode(dataService.LanguageCode);
            shakeActive = ShakeHelper.GetShakeActive();
            SettingsCommon = dataService.GetSettingsCommon();
            isLoggingEnabled = LoggerHelper.GetServerLogActive();
            selectedTipLevel = SettingsCommon.SelectedLevel;
            ReportBugCommand = new Command(ReportBug);
        }

        public bool IsLoggingEnabled
        {
            get => isLoggingEnabled;
            set
            {
                if (isLoggingEnabled == value) return;
                isLoggingEnabled = value;
                LoggerHelper.SaveServerLogActive(value);
                LoggerConfigurationManager.EnableServerLogging(value);
                OnPropertyChanged();
            }
        }
        public bool ShakeActive
        {
            get => shakeActive;
            set
            {
                shakeActive = value;
                dataService.SetShakeActive(shakeActive);
                OnPropertyChanged();
            }
        }
        public ICommand ReportBugCommand { get; protected set; }
        public List<string> Languages
        {
            get => languages;
            set => SetProperty(ref languages, value);
        }
        public List<string> TipLevels
        {
            get => tipLevels;
            set => SetProperty(ref tipLevels, value);
        }
        public SettingsCommon SettingsCommon
        {
            get => settingsCommon;
            set
            {
                settingsCommon = value;
                OnPropertyChanged();
            }
        }
        public string AppVersion => $"{AppInfo.VersionString}";
        public string SelectedLanguage
        {
            get => selectedLanguage;
            set
            {
                if (selectedLanguage == value) return;
                selectedLanguage = value;
                OnPropertyChanged();
                var selectedLanguageCode = LanguageDictionary.LanguageMap[value];
                LanguageHelper.SaveUserLanguage(selectedLanguageCode);
                ShowLoadingAndInitializeApp();
            }
        }
        public string SelectedTipLevel
        {
            get => selectedTipLevel;
            set
            {
                if (selectedTipLevel == value) return;
                selectedTipLevel = value;
                SettingsCommon.SelectedLevel = value;
                dataService.SelectedTipLevel = value;
                var serializedData = JsonConvert.SerializeObject(SettingsCommon);
                var filePath = Path.Combine(dataService.FolderPath,
                    dataService.LanguageCode + "_" + "settings_common_cache.json");
                File.WriteAllText(filePath, serializedData);
                OnPropertyChanged();
            }
        }

        public void Cleanup()
        {
            ReportBugCommand = null;
        }

        /// <summary>
        ///     Открытие страницы отправки фидбэка.
        /// </summary>
        private static async void ReportBug()
        {
            await PopupNavigation.Instance.PushAsync(new FeedbackPopupPage());
        }

        /// <summary>
        ///     Показ загрузочного экрана и перезагрузка приложения для смены языка.
        /// </summary>
        private static async void ShowLoadingAndInitializeApp()
        {
            try
            {
                var loadingPopup = new LoadingPopup();
                await PopupNavigation.Instance.PushAsync(loadingPopup);
                ((AppShell) Shell.Current).StopShakeDetector();
                await App.CurrentApp.InitializeAppShellAsync();
                await PopupNavigation.Instance.PopAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка при перезагрузке приложения после смены языка");
            }
        }
    }
}