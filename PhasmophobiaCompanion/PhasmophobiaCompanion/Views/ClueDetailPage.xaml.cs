﻿using System;
using System.Linq;
using PhasmophobiaCompanion.Models;
using PhasmophobiaCompanion.ViewModels;
using Serilog;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PhasmophobiaCompanion.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ClueDetailPage : ContentPage
    {
        public ClueDetailPage(Clue clue)
        {
            try
            {
                InitializeComponent();
                var viewModel = new ClueDetailViewModel(clue);
                BindingContext = viewModel;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка во время инициализации ClueDetailPage.");
                throw;
            }
        }

        /// <summary>
        ///     Раскрытие или свертывание раскрывающегося элемента по нажатию на него.
        /// </summary>
        private void OnItemTapped(object sender, EventArgs e)
        {
            try
            {
                if (sender is StackLayout layout && layout.BindingContext is UnfoldingItem unfoldingItem)
                    unfoldingItem.IsExpanded = !unfoldingItem.IsExpanded;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка во время раскрытия или сворачивания списка на странице ClueDetailPage.");
                throw;
            }
        }
        private void OnImageTapped(object sender, EventArgs e)
        {
            var gesture = (TapGestureRecognizer)((Image)sender).GestureRecognizers.FirstOrDefault();
            if (gesture != null && gesture.CommandParameter is ImageWithDescription imageWithDescription)
            {
                if (this.BindingContext is ClueDetailViewModel viewModel)
                {
                    if (viewModel.ImageTappedCommand.CanExecute(imageWithDescription))
                    {
                        viewModel.ImageTappedCommand.Execute(imageWithDescription);
                    }
                }
            }
        }
    }
}