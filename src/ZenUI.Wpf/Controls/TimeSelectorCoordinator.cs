using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Controls;

namespace ZenUI.Wpf.Controls
{
    /// <summary>
    /// 协调时间选择面板中的选项、选中值和范围状态。
    /// </summary>
    /// <remarks>
    /// 该类型不拥有视觉树，使时间选择逻辑可以复用于不同控件，同时保留宿主控件的模板部件契约。
    /// </remarks>
    internal sealed class TimeSelectorCoordinator
    {
        private readonly ListBox hourList;
        private readonly ListBox minuteList;
        private readonly ListBox secondList;
        private readonly ListBox periodList;
        private readonly Action<TimeSpan> selectedTimeChanged;
        private bool isSynchronizing;
        private bool is24Hour;
        private bool isSecondVisible;
        private int minuteIncrement;
        private int secondIncrement;
        private TimeSpan minimum;
        private TimeSpan maximum;
        private TimeSpan selectedTime;

        internal TimeSelectorCoordinator(
            ListBox hourList,
            ListBox minuteList,
            ListBox secondList,
            ListBox periodList,
            Action<TimeSpan> selectedTimeChanged)
        {
            this.hourList = hourList;
            this.minuteList = minuteList;
            this.secondList = secondList;
            this.periodList = periodList;
            this.selectedTimeChanged = selectedTimeChanged;

            AddSelectionChangedHandler(hourList);
            AddSelectionChangedHandler(minuteList);
            AddSelectionChangedHandler(secondList);
            AddSelectionChangedHandler(periodList);
        }

        internal void Configure(
            bool use24Hour,
            bool showSecond,
            int minuteStep,
            int secondStep,
            TimeSpan minimumValue,
            TimeSpan maximumValue)
        {
            is24Hour = use24Hour;
            isSecondVisible = showSecond;
            minuteIncrement = minuteStep;
            secondIncrement = secondStep;
            minimum = minimumValue;
            maximum = maximumValue;

            PopulateOptions();
        }

        internal void Synchronize(TimeSpan value)
        {
            if (isSynchronizing)
            {
                return;
            }

            selectedTime = value;
            isSynchronizing = true;
            try
            {
                var displayHour = is24Hour
                    ? value.Hours
                    : (value.Hours % 12 == 0 ? 12 : value.Hours % 12);

                SelectNearest(hourList, displayHour, 1);
                SelectNearest(minuteList, value.Minutes, minuteIncrement);
                SelectNearest(secondList, value.Seconds, secondIncrement);
                SelectOption(periodList, value.Hours >= 12 ? 1 : 0);
                UpdateOptionStates();
            }
            finally
            {
                isSynchronizing = false;
            }
        }

        internal void ScrollSelectedItemsIntoView()
        {
            ScrollSelectedItemIntoView(hourList);
            ScrollSelectedItemIntoView(minuteList);
            if (isSecondVisible)
            {
                ScrollSelectedItemIntoView(secondList);
            }

            if (!is24Hour)
            {
                ScrollSelectedItemIntoView(periodList);
            }
        }

        internal void Detach()
        {
            RemoveSelectionChangedHandler(hourList);
            RemoveSelectionChangedHandler(minuteList);
            RemoveSelectionChangedHandler(secondList);
            RemoveSelectionChangedHandler(periodList);
        }

        private void PopulateOptions()
        {
            isSynchronizing = true;
            try
            {
                if (hourList != null)
                {
                    hourList.ItemsSource = CreateRange(
                        is24Hour ? 0 : 1,
                        is24Hour ? 23 : 12,
                        1);
                }

                if (minuteList != null)
                {
                    minuteList.ItemsSource = CreateRange(0, 59, minuteIncrement);
                }

                if (secondList != null)
                {
                    secondList.ItemsSource = CreateRange(0, 59, secondIncrement);
                }

                if (periodList != null)
                {
                    periodList.ItemsSource = new[]
                    {
                        new TimeSelectorOption(
                            0,
                            CultureInfo.CurrentCulture.DateTimeFormat.AMDesignator),
                        new TimeSelectorOption(
                            1,
                            CultureInfo.CurrentCulture.DateTimeFormat.PMDesignator)
                    };
                }
            }
            finally
            {
                isSynchronizing = false;
            }
        }

        private static List<TimeSelectorOption> CreateRange(int start, int end, int increment)
        {
            var values = new List<TimeSelectorOption>();
            for (var value = start; value <= end; value += increment)
            {
                values.Add(
                    new TimeSelectorOption(
                        value,
                        value.ToString("00", CultureInfo.CurrentCulture)));
            }

            return values;
        }

        private static void SelectNearest(ListBox selector, int value, int increment)
        {
            var selected = increment <= 1 ? value : value - (value % increment);
            SelectOption(selector, selected);
        }

        private static void SelectOption(ListBox selector, int value)
        {
            if (selector == null)
            {
                return;
            }

            foreach (var item in selector.Items)
            {
                if (item is TimeSelectorOption option && option.Value == value)
                {
                    selector.SelectedItem = option;
                    return;
                }
            }
        }

        private void HandleSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isSynchronizing ||
                hourList?.SelectedItem == null ||
                minuteList?.SelectedItem == null ||
                (isSecondVisible && secondList?.SelectedItem == null))
            {
                return;
            }

            if (sender is ListBox selector &&
                selector.SelectedItem is TimeSelectorOption selectedOption &&
                !selectedOption.IsEnabled)
            {
                Synchronize(selectedTime);
                return;
            }

            var hour = ParseSelector(hourList);
            var minute = ParseSelector(minuteList);
            var second = isSecondVisible ? ParseSelector(secondList) : 0;
            if (!is24Hour && periodList != null)
            {
                hour %= 12;
                if (periodList.SelectedIndex == 1)
                {
                    hour += 12;
                }
            }

            selectedTimeChanged(new TimeSpan(hour, minute, second));
        }

        private static int ParseSelector(ListBox selector)
        {
            return ((TimeSelectorOption)selector.SelectedItem).Value;
        }

        private void UpdateOptionStates()
        {
            var period = periodList?.SelectedItem is TimeSelectorOption periodOption
                ? periodOption.Value
                : (selectedTime.Hours >= 12 ? 1 : 0);
            var selectedHour = hourList?.SelectedItem is TimeSelectorOption hourOption
                ? hourOption.Value
                : (is24Hour
                    ? selectedTime.Hours
                    : (selectedTime.Hours % 12 == 0 ? 12 : selectedTime.Hours % 12));
            var actualHour = ConvertToActualHour(selectedHour, period);
            var selectedMinute = minuteList?.SelectedItem is TimeSelectorOption minuteOption
                ? minuteOption.Value
                : selectedTime.Minutes;

            SetOptionStates(
                hourList,
                option =>
                {
                    var hour = ConvertToActualHour(option.Value, period);
                    return IntersectsRange(
                        new TimeSpan(hour, 0, 0),
                        new TimeSpan(hour, 59, 59));
                });
            SetOptionStates(
                minuteList,
                option =>
                {
                    var start = new TimeSpan(actualHour, option.Value, 0);
                    var end = isSecondVisible
                        ? new TimeSpan(actualHour, option.Value, 59)
                        : start;
                    return IntersectsRange(start, end);
                });
            SetOptionStates(
                secondList,
                option => IsWithinRange(
                    new TimeSpan(actualHour, selectedMinute, option.Value)));
            SetOptionStates(
                periodList,
                option => IntersectsRange(
                    new TimeSpan(option.Value == 0 ? 0 : 12, 0, 0),
                    new TimeSpan(option.Value == 0 ? 11 : 23, 59, 59)));
        }

        private int ConvertToActualHour(int displayHour, int period)
        {
            if (is24Hour)
            {
                return displayHour;
            }

            var hour = displayHour % 12;
            return period == 1 ? hour + 12 : hour;
        }

        private bool IntersectsRange(TimeSpan start, TimeSpan end)
        {
            return end >= minimum && start <= maximum;
        }

        private bool IsWithinRange(TimeSpan value)
        {
            return value >= minimum && value <= maximum;
        }

        private static void SetOptionStates(
            ListBox selector,
            Func<TimeSelectorOption, bool> predicate)
        {
            if (selector == null)
            {
                return;
            }

            foreach (var item in selector.Items)
            {
                if (item is TimeSelectorOption option)
                {
                    option.IsEnabled = predicate(option);
                }
            }
        }

        private void AddSelectionChangedHandler(ListBox selector)
        {
            if (selector != null)
            {
                selector.SelectionChanged += HandleSelectionChanged;
            }
        }

        private void RemoveSelectionChangedHandler(ListBox selector)
        {
            if (selector != null)
            {
                selector.SelectionChanged -= HandleSelectionChanged;
            }
        }

        private static void ScrollSelectedItemIntoView(ListBox selector)
        {
            if (selector?.SelectedItem != null)
            {
                selector.ScrollIntoView(selector.SelectedItem);
            }
        }
    }

    internal sealed class TimeSelectorOption : INotifyPropertyChanged
    {
        private bool isEnabled = true;

        internal TimeSelectorOption(int value, string displayText)
        {
            Value = value;
            DisplayText = displayText;
        }

        internal int Value { get; }

        public string DisplayText { get; }

        public bool IsEnabled
        {
            get { return isEnabled; }
            set
            {
                if (isEnabled == value)
                {
                    return;
                }

                isEnabled = value;
                PropertyChanged?.Invoke(
                    this,
                    new PropertyChangedEventArgs(nameof(IsEnabled)));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public override string ToString()
        {
            return DisplayText;
        }
    }
}
