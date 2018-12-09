using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace VSProjectManager
{
    /// <summary>
    /// Логика взаимодействия для UserControl1.xaml
    /// </summary>
    public partial class OptionBox2 : UserControl
    {
        public Property GetProperty
        {
            get
            {
                return (new Property(Property.Text, Value.Text));
            }
        }
        public OptionBox2()
        {
            InitializeComponent();
            Margin = new Thickness(2.5, 5, 2.5, 0);
            Property.ItemsSource = new List<string>(Settings.Instance.KnownParamsAndValues.Keys);
            Value.ItemsSource = new List<string>();
            Value.Focusable = false;
        }

        public OptionBox2(Property property) : this()
        {
            Property.Text = property.Name;
            Value.Text = property.Value;
        }

        public void SetFocus()
        {
            Property.Focus();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            (Parent as StackPanel).Children.Remove(this);
        }

        public void OnComboboxTextChanged(object sender, RoutedEventArgs e)
        {
            var CB = sender as ComboBox;
            CB.IsDropDownOpen = true;
            
            var tb = (TextBox)e.OriginalSource;
            tb.Select(tb.SelectionStart + tb.SelectionLength, 0);
            CollectionView cv = (CollectionView)CollectionViewSource.GetDefaultView(CB.ItemsSource);
            cv.Filter = s =>
                ((string)s).IndexOf(CB.Text, StringComparison.CurrentCultureIgnoreCase) >= 0;
        }

        private void PropertyLostFocus(object sender, RoutedEventArgs e)
        {
            var settings = Settings.Instance;
            if (settings.KnownParamsAndValues.ContainsKey(Property.Text))
            {
                Value.ItemsSource = new List<string>(settings.KnownParamsAndValues[Property.Text]);
            }
            Value.Focusable = true;
        }
    }
}
