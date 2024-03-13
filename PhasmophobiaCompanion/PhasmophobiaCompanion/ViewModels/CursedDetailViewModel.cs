﻿using System;
using System.Windows.Input;
using PhasmophobiaCompanion.Models;
using PhasmophobiaCompanion.Views;
using Serilog;
using Xamarin.Forms;

namespace PhasmophobiaCompanion.ViewModels
{
    /// <summary>
    ///     ViewModel для подробной страницы проклятого предмета.
    /// </summary>
    public class CursedDetailViewModel : BaseViewModel
    {
        public ICommand ImageTappedCommand;
        private CursedPossession cursedPossession;

        public CursedDetailViewModel(CursedPossession cursedPossession)
        {
            try
            {
                CursedPossession = cursedPossession;
                ImageTappedCommand = new Command<ImageWithDescription>(OpenImagePage);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка во время инициализации CursedDetailViewModel.");
                throw;
            }
        }

        public CursedPossession CursedPossession
        {
            get => cursedPossession;
            set
            {
                cursedPossession = value;
                OnPropertyChanged();
            }
        }

        private async void OpenImagePage(ImageWithDescription image)
        {
            await Application.Current.MainPage.Navigation.PushModalAsync(new ImagePage(image));
        }
    }
}