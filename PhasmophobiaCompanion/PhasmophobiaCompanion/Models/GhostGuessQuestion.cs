﻿using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace GhostTracker.Models
{
    /// <summary>
    ///     Представляет собой вопрос указывающий насколько призраки подходит под варианты ответа на вопрос.
    /// </summary>
    public class GhostGuessQuestion : INotifyPropertyChanged
    {
        private bool isVisible;
        private int visibility;

        public GhostGuessQuestion()
        {
            Ghosts = new List<Ghost>();
        }

        public bool IsVisible
        {
            get => isVisible;
            set
            {
                isVisible = value;
                OnPropertyChanged();
            }
        }
        public int Answer { get; set; }
        public int AnswerMeaning { get; set; }
        public int AnswerNegativeMeaning { get; set; }
        public int ID { get; set; }
        public int Visibility
        {
            get => visibility;
            set
            {
                visibility = value;
                OnPropertyChanged();
            }
        }
        public List<Ghost> Ghosts { get; set; }
        public List<int> GhostsID { get; set; }
        public string QuestionText { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        ///     Связывает вопрос  - GhostGuessQuestion с призраками Ghost через имеющийся список Id призраков - GhostsID.
        /// </summary>
        /// <param name="allghosts">Список всех призраков Ghost.</param>
        public void PopulateAssociatedGhosts(List<Ghost> allghosts)
        {
            foreach (var ghostId in GhostsID)
            {
                var ghost = allghosts.FirstOrDefault(c => c.ID == ghostId);
                if (ghost != null) Ghosts.Add(ghost);
            }
        }
    }
}