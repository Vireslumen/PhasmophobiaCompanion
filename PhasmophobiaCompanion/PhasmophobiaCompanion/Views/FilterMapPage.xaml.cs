﻿using System;
using System.ComponentModel;
using PhasmophobiaCompanion.ViewModels;
using Rg.Plugins.Popup.Pages;
using Serilog;
using Xamarin.Forms.Xaml;

namespace PhasmophobiaCompanion.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    [DesignTimeVisible(false)]
    public partial class FilterMapPage : PopupPage
    {
        public FilterMapPage(MapsViewModel viewModel)
        {
            try
            {
                InitializeComponent();
                BindingContext = viewModel;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка во время инициализации FilterMapPage.");
                throw;
            }
        }
    }
}