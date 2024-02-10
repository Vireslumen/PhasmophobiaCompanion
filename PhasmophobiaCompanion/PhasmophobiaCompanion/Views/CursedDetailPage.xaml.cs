﻿using System;
using PhasmophobiaCompanion.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PhasmophobiaCompanion.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CursedDetailPage : ContentPage
    {
        public CursedDetailPage(CursedPossession selectedCursed)
        {
            InitializeComponent();
            BindingContext = selectedCursed;
        }

        /// <summary>
        ///     Раскрытие или свертывание раскрывающегося элемента по нажатию на него.
        /// </summary>
        private void OnItemTapped(object sender, EventArgs e)
        {
            if (sender is StackLayout layout && layout.BindingContext is UnfoldingItem unfoldingItem)
                unfoldingItem.IsExpanded = !unfoldingItem.IsExpanded;
        }
    }
}