﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValidationContextControl.xaml.cs" company="WildGums">
//   Copyright (c) 2008 - 2016 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.Controls
{
    using System.Windows;
    using System.Windows.Media;
    using Catel.Data;
    using Catel.MVVM.Views;

    public sealed partial class ValidationContextView
    {
        static ValidationContextView()
        {
            typeof(ValidationContextView).AutoDetectViewPropertiesToSubscribe();
        }

        public ValidationContextView()
        {
            InitializeComponent();
        }

        #region Dependency properties
        [ViewToViewModel(MappingType = ViewToViewModelMappingType.TwoWayViewWins)]
        public IValidationContext ValidationContext
        {
            get { return (IValidationContext)GetValue(ValidationContextProperty); }
            set { SetValue(ValidationContextProperty, value); }
        }

        public static readonly DependencyProperty ValidationContextProperty = DependencyProperty.Register(
            nameof(ValidationContext), typeof(IValidationContext), typeof(ValidationContextView), new PropertyMetadata(null));


        [ViewToViewModel(MappingType = ViewToViewModelMappingType.TwoWayViewWins)]
        public bool ShowFilterBox
        {
            get { return (bool)GetValue(ShowFilterBoxProperty); }
            set { SetValue(ShowFilterBoxProperty, value); }
        }

        public static readonly DependencyProperty ShowFilterBoxProperty = DependencyProperty.Register(
            nameof(ShowFilterBox), typeof(bool), typeof(ValidationContextView), new PropertyMetadata(true));


        public bool ShowButtons
        {
            get { return (bool)GetValue(ShowButtonsProperty); }
            set { SetValue(ShowButtonsProperty, value); }
        }

        public static readonly DependencyProperty ShowButtonsProperty = DependencyProperty.Register(nameof(ShowButtons),
            typeof(bool), typeof(ValidationContextView), new PropertyMetadata(true));

        [ViewToViewModel(MappingType = ViewToViewModelMappingType.TwoWayViewWins)]
        public bool IsExpandedAllOnStartup
        {
            get { return (bool)GetValue(IsExpandedAllOnStartupProperty); }
            set { SetValue(IsExpandedAllOnStartupProperty, value); }
        }

        public static readonly DependencyProperty IsExpandedAllOnStartupProperty = DependencyProperty.Register(
            nameof(IsExpandedAllOnStartup), typeof(bool), typeof(ValidationContextView), new PropertyMetadata(true));
        #endregion
    }
}
