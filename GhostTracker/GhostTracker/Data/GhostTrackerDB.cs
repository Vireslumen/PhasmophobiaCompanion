﻿using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Xamarin.Forms;

namespace GhostTracker.Data
{
    /// <summary>
    ///     Контекст базы данных для приложений, наследующий от DbContext.
    /// </summary>
    public class GhostTrackerDb : DbContext
    {
        //Определения DbSet для различных сущностей.
        public DbSet<AchievementBase> AchievementBase { get; set; }
        public DbSet<AchievementCommonTranslations> AchievementCommonTranslations { get; set; }
        public DbSet<AppShellCommonTranslations> AppShellCommonTranslations { get; set; }
        public DbSet<ChallengeModeBase> ChallengeModeBase { get; set; }
        public DbSet<ChallengeModeCommonTranslations> ChallengeModeCommonTranslations { get; set; }
        public DbSet<ClueBase> ClueBase { get; set; }
        public DbSet<ClueCommonTranslations> ClueCommonTranslations { get; set; }
        public DbSet<CursedPossessionBase> CursedPossessionBase { get; set; }
        public DbSet<CursedPossessionCommonTranslations> CursedPossessionCommonTranslations { get; set; }
        public DbSet<DifficultyBase> DifficultyBase { get; set; }
        public DbSet<DifficultyCommonTranslations> DifficultyCommonTranslations { get; set; }
        public DbSet<EquipmentBase> EquipmentBase { get; set; }
        public DbSet<EquipmentCommonTranslations> EquipmentCommonTranslations { get; set; }
        public DbSet<FeedbackCommonTranslations> FeedbackCommonTranslations { get; set; }
        public DbSet<GhostBase> GhostBase { get; set; }
        public DbSet<GhostCommonTranslations> GhostCommonTranslations { get; set; }
        public DbSet<GhostGuessQuestionBase> GhostGuessQuestionBase { get; set; }
        public DbSet<GhostGuessQuestionCommonTranslations> GhostGuessQuestionCommonTranslations { get; set; }
        public DbSet<MainPageCommonTranslations> MainPageCommonTranslations { get; set; }
        public DbSet<MapBase> MapBase { get; set; }
        public DbSet<MapCommonTranslations> MapCommonTranslations { get; set; }
        public DbSet<OtherInfoBase> OtherInfoBase { get; set; }
        public DbSet<PatchBase> PatchBase { get; set; }
        public DbSet<QuestBase> QuestBase { get; set; }
        public DbSet<QuestCommonTranslations> QuestCommonTranslations { get; set; }
        public DbSet<SettingsCommonTranslations> SettingsCommonTranslations { get; set; }
        public DbSet<TipsTranslations> TipsTranslations { get; set; }

        /// <summary>
        ///     Конфигурация подключения к базе данных.
        /// </summary>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            try
            {
                var dbPath = "phasmaDATADB.db"; // Путь к файлу базы данных по умолчанию

                // Проверка, выполняется ли код на Android
                if (Device.RuntimePlatform == Device.Android)
                {
                    // Получение пути к папке для хранения базы данных на устройстве
                    var folderPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                    dbPath = Path.Combine(folderPath, dbPath);
                }

                optionsBuilder.UseSqlite($"Data Source={dbPath}");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка при конфигурации базы данных.");
            }
        }

        /// <summary>
        ///     Конфигурация моделей с помощью Fluent API.
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ConfigureGhostEntity(modelBuilder);
            ConfigureClueEntity(modelBuilder);
            ConfigureImageWithDescriptionEntity(modelBuilder);
            ConfigureCursedPossessionEntity(modelBuilder);
            ConfigureEquipmentEntity(modelBuilder);
            ConfigureMapEntity(modelBuilder);
            ConfigureOtherInfoEntity(modelBuilder);
            ConfigureChallengeModeEntity(modelBuilder);
            ConfigureGhostGuessQuestionEntity(modelBuilder);
        }

        private static void ConfigureChallengeModeEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ChallengeModeBase>()
                .HasMany(c => c.EquipmentBase)
                .WithMany(c => c.ChallengeModeBase)
                .UsingEntity(j => j.ToTable("ChallengeModeToEquipmentLink"));
        }

        private static void ConfigureClueEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ClueBase>()
                .HasMany(c => c.EquipmentBase)
                .WithMany(c => c.ClueBase)
                .UsingEntity(j => j.ToTable("ClueToEquipmentLink"));
            modelBuilder.Entity<ClueBase>()
                .HasMany(c => c.UnfoldingItemBase)
                .WithMany(u => u.ClueBase)
                .UsingEntity(j => j.ToTable("ClueToUnfoldingItemLink"));

            modelBuilder.Entity<ClueBase>()
                .HasMany(c => c.ExpandFieldWithImagesBase)
                .WithMany(e => e.ClueBase)
                .UsingEntity(j => j.ToTable("ClueToExpandFieldWithImagesLink"));
        }

        private static void ConfigureCursedPossessionEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CursedPossessionBase>()
                .HasMany(c => c.ExpandFieldWithImagesBase)
                .WithMany(e => e.CursedPossessionBase)
                .UsingEntity(j => j.ToTable("CursedPossessionToExpandFieldWithImagesLink"));

            modelBuilder.Entity<CursedPossessionBase>()
                .HasMany(c => c.UnfoldingItemBase)
                .WithMany(u => u.CursedPossessionBase)
                .UsingEntity(j => j.ToTable("CursedPossessionToUnfoldingItemLink"));
        }

        private static void ConfigureEquipmentEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EquipmentBase>()
                .HasMany(e => e.UnfoldingItemBase)
                .WithMany(u => u.EquipmentBase)
                .UsingEntity(j => j.ToTable("EquipmentToUnfoldingItemLink"));
        }

        private static void ConfigureGhostEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GhostBase>()
                .HasMany(g => g.ClueBase)
                .WithMany(c => c.GhostBase)
                .UsingEntity(j => j.ToTable("GhostToClueLink"));

            modelBuilder.Entity<GhostBase>()
                .HasMany(g => g.UnfoldingItemBase)
                .WithMany(u => u.GhostBase)
                .UsingEntity(j => j.ToTable("GhostToUnfoldingItemLink"));

            modelBuilder.Entity<SpeedRangesBase>()
                .HasOne(sp => sp.GhostBase)
                .WithMany(g => g.SpeedRangesBase)
                .HasForeignKey(sp => sp.GhostBaseId);
        }

        private static void ConfigureGhostGuessQuestionEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GhostGuessQuestionBase>()
                .HasMany(c => c.GhostBase)
                .WithMany(c => c.GhostGuessQuestionBase)
                .UsingEntity(j => j.ToTable("GhostGuessQuestionToGhostLink"));
        }

        private static void ConfigureImageWithDescriptionEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ImageWithDescriptionBase>()
                .HasOne(iwd => iwd.ExpandFieldWithImagesBase)
                .WithMany(expandFieldWithImagesBase => expandFieldWithImagesBase.ImageWithDescriptionBase)
                .HasForeignKey(iwd => iwd.ExpandFieldWithImagesBaseId);
        }

        private static void ConfigureMapEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MapBase>()
                .HasMany(m => m.UnfoldingItemBase)
                .WithMany(u => u.MapBase)
                .UsingEntity(j => j.ToTable("MapToUnfoldingItemsLink"));

            modelBuilder.Entity<MapBase>()
                .HasMany(m => m.ExpandFieldWithImagesBase)
                .WithMany(e => e.MapBase)
                .UsingEntity(j => j.ToTable("MapToExpandFieldWithImagesLink"));
        }

        private static void ConfigureOtherInfoEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OtherInfoBase>()
                .HasMany(o => o.UnfoldingItemBase)
                .WithMany(u => u.OtherInfoBase)
                .UsingEntity(j => j.ToTable("OtherInfoToUnfoldingItemLink"));

            modelBuilder.Entity<OtherInfoBase>()
                .HasMany(o => o.ExpandFieldWithImagesBase)
                .WithMany(e => e.OtherInfoBase)
                .UsingEntity(j => j.ToTable("OtherInfoToExpandFieldWithImagesLink"));
        }
    }
}