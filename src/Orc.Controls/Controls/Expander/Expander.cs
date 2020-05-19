﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Expander.cs" company="WildGums">
//   Copyright (c) 2008 - 2016 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.Controls
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media.Animation;
    using Catel.Logging;
    
    [TemplateVisualState(Name = "Expanded", GroupName = "Expander")]
    [TemplateVisualState(Name = "Collapsed", GroupName = "Expander")]
    [TemplatePart(Name = "PART_ExpandSite", Type = typeof(ContentPresenter))]
    [TemplatePart(Name = "PART_HeaderSiteBorder", Type = typeof(Border))]
    public class Expander : HeaderedContentControl
    {
        #region Constants
        private const double MinimumDuration = 150d;
        #endregion 

        #region Fields
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private GridLength? _expandDistance;
        private double? _previousActualLength;
        private double? _previousMaxLenght;
        private ContentPresenter _expandSite;
        private Border _headerSiteBorder;
        #endregion

        #region Constructors
        public Expander()
        {
            DefaultStyleKey = typeof(Expander);
        }
        #endregion

        #region Properties
        public bool IsExpanded
        {
            get { return (bool)GetValue(IsExpandedProperty); }
            set { SetValue(IsExpandedProperty, value); }
        }

        public static readonly DependencyProperty IsExpandedProperty = DependencyProperty.Register(nameof(IsExpanded),
            typeof(bool), typeof(Expander),
            new PropertyMetadata(true, (sender, args) => ((Expander)sender).OnIsExpandedChanged(args)));

        public ExpandDirection ExpandDirection
        {
            get { return (ExpandDirection)GetValue(ExpandDirectionProperty); }
            set { SetValue(ExpandDirectionProperty, value); }
        }

        public static readonly DependencyProperty ExpandDirectionProperty = DependencyProperty.Register(nameof(ExpandDirection),
            typeof(ExpandDirection), typeof(Expander), new PropertyMetadata(ExpandDirection.Left));
        
        public double ExpandDuration
        {
            get { return (double)GetValue(ExpandDurationProperty); }
            set { SetValue(ExpandDurationProperty, value); }
        }

        public static readonly DependencyProperty ExpandDurationProperty = DependencyProperty.Register(
            nameof(ExpandDuration), typeof(double), typeof(Expander), new PropertyMetadata(0d));


        public bool AutoResizeGrid
        {
            get { return (bool)GetValue(AutoResizeGridProperty); }
            set { SetValue(AutoResizeGridProperty, value); }
        }

        public static readonly DependencyProperty AutoResizeGridProperty = DependencyProperty.Register(nameof(AutoResizeGrid),
            typeof(bool), typeof(Expander), new PropertyMetadata(false));
        #endregion

        #region Methods
        private void OnIsExpandedChanged(DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                OnExpanded();
            }
            else
            {
                OnCollapsed();
            }

            UpdateStates(true);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _expandSite = GetTemplateChild("PART_ExpandSite") as ContentPresenter;
            if (_expandSite is null)
            {
                throw Log.ErrorAndCreateException<InvalidOperationException>($"Can't find template part 'PART_ExpandSite'");
            }

            _headerSiteBorder = GetTemplateChild("PART_HeaderSiteBorder") as Border;
            if (_headerSiteBorder is null)
            {
                throw Log.ErrorAndCreateException<InvalidOperationException>($"Can't find template part 'PART_HeaderSiteBorder'");
            }
            
            UpdateStates(false);
        }

        private void AnimateMaxHeight(RowDefinition rowDefinition, double from, double to, double duration)
        {
            rowDefinition.BeginAnimation(RowDefinition.MaxHeightProperty, null);
            if (duration < MinimumDuration)
            {
                duration = MinimumDuration;
            }

            var storyboard = new Storyboard();

            var anumationDuration = new Duration(TimeSpan.FromMilliseconds(duration));
            var ease = new CubicEase { EasingMode = EasingMode.EaseOut };

            var animation = new DoubleAnimation { EasingFunction = ease, Duration = anumationDuration };
            storyboard.Children.Add(animation);
            animation.From = from;
            animation.To = to;
            Storyboard.SetTarget(animation, rowDefinition);
            Storyboard.SetTargetProperty(animation, new PropertyPath("(RowDefinition.MaxHeight)"));

            storyboard.Completed += OnRowMaxHeightAnimationCompleted;

            storyboard.Begin();
        }

        private void AnimateMaxWidth(ColumnDefinition columnDefinition, double from, double to, double duration)
        {
            columnDefinition.BeginAnimation(ColumnDefinition.MaxWidthProperty, null);
            if (duration < MinimumDuration)
            {
                duration = MinimumDuration;
            }

            var storyboard = new Storyboard();

            var anumationDuration = new Duration(TimeSpan.FromMilliseconds(duration));
            var ease = new CubicEase { EasingMode = EasingMode.EaseOut };

            var animation = new DoubleAnimation { EasingFunction = ease, Duration = anumationDuration };
            storyboard.Children.Add(animation);
            animation.From = from;
            animation.To = to;
            Storyboard.SetTarget(animation, columnDefinition);
            Storyboard.SetTargetProperty(animation, new PropertyPath("(ColumnDefinition.MaxWidth)"));

            storyboard.Completed += OnColumnMaxWidthAnimationCompleted;

            storyboard.Begin();
        }

        private void OnRowMaxHeightAnimationCompleted(object sender, EventArgs e)
        {
            var isExpanded = IsExpanded;

            if (!isExpanded)
            {
                return;
            }

            if (_previousMaxLenght is null)
            {
                return;
            }

            if (!AutoResizeGrid)
            {
                return;
            }

            if (!(Parent is Grid grid))
            {
                return;
            }

            var row = Grid.GetRow(this);
            var rowDefinition = grid.RowDefinitions[row];

            rowDefinition.BeginAnimation(RowDefinition.MaxHeightProperty, null);
        }

        private void OnColumnMaxWidthAnimationCompleted(object sender, EventArgs e)
        {
            var isExpanded = IsExpanded;

            if (!isExpanded)
            {
                return;
            }

            if (_previousMaxLenght is null)
            {
                return;
            }

            if (!AutoResizeGrid)
            {
                return;
            }

            if (!(Parent is Grid grid))
            {
                return;
            }

            var column = Grid.GetColumn(this);
            var columnDefinition = grid.ColumnDefinitions[column];

            columnDefinition.BeginAnimation(ColumnDefinition.MaxWidthProperty, null);
        }

        protected virtual void OnCollapsed()
        {
            if (!AutoResizeGrid)
            {
                return;
            }

            if (!(Parent is Grid grid))
            {
                return;
            }

            switch (ExpandDirection)
            {
                case ExpandDirection.Left:
                case ExpandDirection.Right:
                {
                    var column = Grid.GetColumn(this);
                    var columnDefinition = grid.ColumnDefinitions[column];
                    _expandDistance = columnDefinition.Width;

                    if (_previousMaxLenght is null)
                    {
                        _previousMaxLenght = columnDefinition.MaxWidth;
                    }
                    
                    _previousActualLength = columnDefinition.ActualWidth;
                    AnimateMaxWidth(columnDefinition, columnDefinition.ActualWidth, _headerSiteBorder.ActualWidth, ExpandDuration);

                    break;
                }

                case ExpandDirection.Up:
                case ExpandDirection.Down:
                {
                    var row = Grid.GetRow(this);
                    var rowDefinition = grid.RowDefinitions[row];
                    _expandDistance = rowDefinition.Height;
                        
                    if (_previousMaxLenght is null)
                    {
                        _previousMaxLenght = rowDefinition.MaxHeight;
                    }

                    _previousActualLength = rowDefinition.ActualHeight;
                    AnimateMaxHeight(rowDefinition, rowDefinition.ActualHeight, _headerSiteBorder.ActualHeight, ExpandDuration);

                    break;
                }
            }
        }

        protected virtual void OnExpanded()
        {
            if (!AutoResizeGrid)
            {
                return;
            }

            if (!(Parent is Grid grid))
            {
                return;
            }

            switch (ExpandDirection)
            {
                case ExpandDirection.Left:
                case ExpandDirection.Right:
                    {
                        var column = Grid.GetColumn(this);

                        if (_previousActualLength.HasValue)
                        {
                            var columnDefinition = grid.ColumnDefinitions[column];
                            AnimateMaxWidth(columnDefinition, _headerSiteBorder.ActualWidth, _previousActualLength.Value, ExpandDuration);
                        }
                        break;
                    }

                case ExpandDirection.Up:
                case ExpandDirection.Down:
                {
                    var row = Grid.GetRow(this);

                    if (_previousActualLength.HasValue)
                    {
                        var rowDefinition = grid.RowDefinitions[row];
                        AnimateMaxHeight(rowDefinition, _headerSiteBorder.ActualHeight, _previousActualLength.Value, ExpandDuration);
                    }
                    break;
                }
            }
        }

        private void UpdateStates(bool useTransitions)
        {
            VisualStateManager.GoToState(this, IsExpanded ? "Expanded" : "Collapsed", useTransitions);
        }
        #endregion
    }
}
