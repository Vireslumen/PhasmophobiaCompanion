﻿using AutoFixture;
using PhasmophobiaCompanion.Interfaces;
using PhasmophobiaCompanion.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;
using static Xamarin.Essentials.Permissions;

namespace PhasmophobiaCompanion.ViewModels
{
    class CursedViewModel : BaseViewModel, ISearchable
    {
        private ObservableCollection<CursedPossession> curseds;
        private ObservableCollection<CursedPossession> filteredCurseds;
        private string searchQuery;
        public ObservableCollection<CursedPossession> Curseds
        {
            get { return filteredCurseds; }
            set
            {
                SetProperty(ref filteredCurseds, value);
            }
        }
        public CursedViewModel()
        {
            var fixture = new Fixture();
            var items = fixture.CreateMany<CursedPossession>(10);
            curseds = new ObservableCollection<CursedPossession>(items);
            for (int i = 0; i < curseds.Count; i++)
            {
                Random rand = new Random();
                int x = rand.Next(3);
                switch (x)
                {
                    case 0:
                        curseds[i].ImageUrl = "asylum.jpg";
                        break;
                    case 1:
                        curseds[i].ImageUrl = "bframhouse.jpg";
                        break;
                    case 2:
                        curseds[i].ImageUrl = "campwood.jpg";
                        break;
                }
            }
            Curseds = new ObservableCollection<CursedPossession>(curseds);
            SearchCommand = new Command<string>(query => Search(query));
        }
        public string SearchQuery
        {
            get { return searchQuery; }
            set
            {
                SetProperty(ref searchQuery, value);
                SearchCurseds();
            }
        }

        private void SearchCurseds()
        {
            if (string.IsNullOrWhiteSpace(SearchQuery))
            {
                Curseds = new ObservableCollection<CursedPossession>(curseds);
            }
            else
            {
                var filtered = curseds.Where(cursed => cursed.Title.ToLowerInvariant().Contains(SearchQuery.ToLowerInvariant())).ToList();
                Curseds = new ObservableCollection<CursedPossession>(filtered);
            }
        }

        public ICommand SearchCommand { get; set; }

        public void Search(string query)
        {
            SearchQuery = query;
        }
    }
}
