using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;

namespace RustSkinsEditor.UserControls
{
    /// <summary>
    /// Editable combo box which uses the text in its editable textbox to perform a lookup
    /// in its data source.
    /// </summary>
    public class AutoFilteredComboBox : ComboBox
    {
        private int _silenceEvents;
        private ICollectionView _collView;
        private string _savedText;
        private bool _textSaved;
        private int _start;
        private int _length;
        private bool _keyboardSelectionGuard;

        /// <summary>
        /// Creates a new instance of <see cref="AutoFilteredComboBox" />.
        /// </summary>
        public AutoFilteredComboBox()
        {
            var textProperty = DependencyPropertyDescriptor.FromProperty(
                TextProperty,
                typeof(AutoFilteredComboBox));
            textProperty.AddValueChanged(this, OnTextChanged);
            RegisterIsCaseSensitiveChangeNotification();
        }

        // IsCaseSensitive Dependency Property

        /// <summary>
        /// The <see cref="DependencyProperty"/> object of the
        /// <see cref="IsCaseSensitive" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsCaseSensitiveProperty =
            DependencyProperty.Register(
                "IsCaseSensitive",
                typeof(bool),
                typeof(AutoFilteredComboBox),
                new UIPropertyMetadata(false));

        /// <summary>
        /// Gets or sets the way the combo box treats the case sensitivity of
        /// typed text.
        /// </summary>
        /// <value>
        /// The way the combo box treats the case sensitivity of typed text.
        /// </value>
        [Description("The way the combo box treats the case sensitivity of typed text")]
        [Category("AutoFiltered ComboBox")]
        [DefaultValue(true)]
        public bool IsCaseSensitive
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return (bool)GetValue(IsCaseSensitiveProperty); }
            [System.Diagnostics.DebuggerStepThrough]
            set { SetValue(IsCaseSensitiveProperty, value); }
        }

        protected virtual void OnIsCaseSensitiveChanged(object sender,
                                                        EventArgs e)
        {
            if (IsCaseSensitive)
                IsTextSearchEnabled = false;
            RefreshFilter();
        }

        private void RegisterIsCaseSensitiveChangeNotification()
        {
            DependencyPropertyDescriptor.FromProperty(
                IsCaseSensitiveProperty,
                typeof(AutoFilteredComboBox)).AddValueChanged(
                    this, OnIsCaseSensitiveChanged);
        }

        // DropDownOnFocus Dependency Property

        /// <summary>
        /// The <see cref="DependencyProperty"/> object of the
        /// <see cref="DropDownOnFocus" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty DropDownOnFocusProperty =
            DependencyProperty.Register(
                "DropDownOnFocus",
                typeof(bool),
                typeof(AutoFilteredComboBox),
                new UIPropertyMetadata(true));

        /// <summary>
        /// Gets or sets the way the combo box behaves when it receives focus.
        /// </summary>
        /// <value>The way the combo box behaves when it receives focus.</value>
        [Description("The way the combo box behaves when it receives focus")]
        [Category("AutoFiltered ComboBox")]
        [DefaultValue(true)]
        public bool DropDownOnFocus
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return (bool)GetValue(DropDownOnFocusProperty); }
            [System.Diagnostics.DebuggerStepThrough]
            set { SetValue(DropDownOnFocusProperty, value); }
        }

        // Handle selection

        /// <summary>
        /// Called when <see cref="ComboBox.ApplyTemplate()"/> is called.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            EditableTextBox.SelectionChanged += EditableTextBox_SelectionChanged;
            ItemsPopup.Focusable = true;
        }

        /// <summary>
        /// Gets the text box in charge of the editable portion of the combo box.
        /// </summary>
        private TextBox EditableTextBox
        {
            get { return (TextBox)GetTemplateChild("PART_EditableTextBox"); }
        }

        private Popup ItemsPopup
        {
            get { return (Popup)GetTemplateChild("PART_Popup"); }
        }

        private ScrollViewer ItemsScrollViewer
        {
            get
            {
                var border = ItemsPopup.FindName("DropDownBorder") as Border;
                if (border == null) return null;
                return border.Child as ScrollViewer;
            }
        }

        private void EditableTextBox_SelectionChanged(object sender,
                                                      RoutedEventArgs e)
        {
            var origTextBox = (TextBox)e.OriginalSource;
            var origStart = origTextBox.SelectionStart;
            var origLength = origTextBox.SelectionLength;

            if (_silenceEvents > 0) return;
            _start = origStart;
            _length = origLength;
            RefreshFilter();
            ScrollItemsToTop();
        }

        private void RestoreSavedText()
        {
            Text = _textSaved ? _savedText : "";
            EditableTextBox.SelectAll();
        }

        private void ClearFilter()
        {
            _length = 0;
            _start = 0;
            RefreshFilter();
            Text = "";
            ScrollItemsToTop();
        }

        private void SilenceEvents()
        {
            ++_silenceEvents;
        }

        private void UnSilenceEvents()
        {
            if (_silenceEvents > 0)
                --_silenceEvents;
        }

        // Handle focus

        /// <summary>
        /// Invoked whenever an unhandled <see cref="UIElement.GotFocus" />
        /// event reaches this element in its route.
        /// </summary>
        /// <param name="e">
        /// The <see cref="RoutedEventArgs" /> that contains the event data.
        /// </param>
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
            if (ItemsSource == null) return;
            if (DropDownOnFocus)
                IsDropDownOpen = true;
        }

        /// <summary>
        /// Restores initial text on focus loss if the current text is empty.
        /// Otherwise saves the currently selected item text as the new saved
        /// text, which will be used when the control is empty on lost focus. 
        /// </summary>
        /// <param name="e">Event parameters</param>
        protected override void OnPreviewLostKeyboardFocus(
            KeyboardFocusChangedEventArgs e)
        {
            if (Text.Length == 0)
            {
                RestoreSavedText();
            }
            else if (SelectedItem != null)
            {
                _savedText = SelectedItem.ToString();
            }
            base.OnPreviewLostKeyboardFocus(e);
        }

        // Handle filtering

        private void ScrollItemsToTop()
        {
            // need to find the scroll viewer containing list items and scroll
            // it to the top whenever filter is updated; otherwise user won't
            // see the top part of the filtered list of choices
            // See http://social.msdn.microsoft.com/forums/en-US/wpf/thread/5b788897-669c-4d1f-8744-9ace6e5c4b38
            var scrollViewer = ItemsScrollViewer;
            if (scrollViewer == null) return;
            scrollViewer.ScrollToTop();
        }

        private void RefreshFilter()
        {
            if (ItemsSource == null) return;
            _collView = CollectionViewSource.GetDefaultView(ItemsSource);
            _collView.Refresh();
            IsDropDownOpen = true;
        }

        private bool FilterPredicate(object value)
        {
            // We don't like nulls.
            if (value == null) return false;

            // If there is no text, there's no reason to filter.
            if (Text.Length == 0)
                return true;

            var prefix = Text;

            // If the end of the text is selected, do not mind it.
            if (_length > 0 && _start + _length == Text.Length)
            {
                prefix = prefix.Substring(0, _start);
            }

            return value.ToString().StartsWith(prefix,
                                               !IsCaseSensitive,
                                               CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Called when the source of an item in a selector changes.
        /// </summary>
        /// <param name="oldValue">Old value of the source.</param>
        /// <param name="newValue">New value of the source.</param>
        protected override void OnItemsSourceChanged(
            System.Collections.IEnumerable oldValue,
            System.Collections.IEnumerable newValue)
        {
            if (newValue != null)
            {
                _collView = CollectionViewSource.GetDefaultView(newValue);
                _collView.Filter += FilterPredicate;
            }

            if (oldValue != null)
            {
                _collView = CollectionViewSource.GetDefaultView(oldValue);
                _collView.Filter -= FilterPredicate;
            }

            base.OnItemsSourceChanged(oldValue, newValue);
        }

        private void OnTextChanged(object sender, EventArgs e)
        {
            if (!_textSaved)
            {
                _savedText = Text;
                _textSaved = true;
            }

            if (IsTextSearchEnabled || _silenceEvents != 0) return;

            RefreshFilter();

            // Manually simulate the automatic selection that would have been
            // available if the IsTextSearchEnabled dependency property was set.
            if (Text.Length <= 0) return;
            var prefix = Text.Length;
            _collView = CollectionViewSource.GetDefaultView(ItemsSource);
            foreach (var item in _collView)
            {
                var text = item.ToString().Length;
                SelectedItem = item;

                SilenceEvents();
                EditableTextBox.Text = item.ToString();
                EditableTextBox.Select(prefix, text - prefix);
                UnSilenceEvents();
                break;
            }
        }

        // Handling keyboard

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                case Key.Tab:
                    IsDropDownOpen = false;
                    break;
                case Key.Escape:
                    // Escape removes current filter
                    _keyboardSelectionGuard = false;
                    UnSilenceEvents();
                    ClearFilter();
                    IsDropDownOpen = true;
                    return;
                case Key.Down:
                case Key.Up:
                    // Open dropdown
                    IsDropDownOpen = true;
                    if (!_keyboardSelectionGuard)
                    {
                        _keyboardSelectionGuard = true;
                        SilenceEvents();
                    }
                    break;
                default:
                    break;
            }
            base.OnPreviewKeyDown(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    // Escape removes current filter
                    _keyboardSelectionGuard = false;
                    UnSilenceEvents();
                    ClearFilter();
                    IsDropDownOpen = true;
                    return;
                default:
                    break;
            }
            base.OnKeyDown(e);
        }
    }
}
