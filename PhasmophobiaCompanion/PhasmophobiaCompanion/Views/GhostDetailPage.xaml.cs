﻿using System;
using PhasmophobiaCompanion.Models;
using PhasmophobiaCompanion.ViewModels;
using Serilog;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PhasmophobiaCompanion.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GhostDetailPage : ContentPage
    {
        public GhostDetailPage(Ghost ghost)
        {
            try
            {
                InitializeComponent();
                var viewModel = new GhostDetailViewModel(ghost);
                BindingContext = viewModel;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка во время инициализации GhostDetailPage.");
                throw;
            }
        }
    }
}