﻿using Serilog;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PhasmophobiaCompanion.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoadingScreenPage : ContentPage
    {
        public LoadingScreenPage()
        {
            try
            {
                InitializeComponent();
            }
            catch (System.Exception ex)
            {
                Log.Error(ex, "Ошибка во время инициализации LoadingScreenPage.");
                throw;
            }
        }
    }
}