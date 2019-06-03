using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Windows.UI.Xaml.Controls;

namespace WrapPanel.Example
{
    public sealed partial class MainPage : Page
    {
        public ObservableCollection<Block> Blocks { get; }

        public ICommand AddBlockCommand { get; }
        public ICommand OrderByNumberCommand { get; }
        public ICommand OrderBySizeCommand { get; }

        public MainPage()
        {
            this.InitializeComponent();
            Blocks = new ObservableCollection<Block>();
            AddBlockCommand = new Command(AddBlock);
            OrderByNumberCommand = new Command((o) => OrderBlocks(true));
            OrderBySizeCommand = new Command((o) => OrderBlocks(false));
        }

        private void AddBlock(object obj)
        {
            Random r = new Random();
            Blocks.Add(new Block(r.Next(100, 300), r.Next(100)));
        }

        private void OrderBlocks(bool orderByNumber)
        {
            var orderedBlocks = Blocks.OrderBy(b => orderByNumber ? b.Number : b.Size).ToList();
            for(int newIndex = 0; newIndex < orderedBlocks.Count; newIndex++)
            {
                int oldIndex = Blocks.IndexOf(orderedBlocks[newIndex]);
                Blocks.Move(oldIndex, newIndex);
            }
        }
    }

    public class Block : INotifyPropertyChanged
    {
        public int Size { get; }
        public int Number { get; private set; }

        public Block(int size, int number)
        {
            Size = size;
            Number = number;
        }

        public void UpdateNumber(int newNumber)
        {
            Number = newNumber;
            NotifyPropertyChanged(nameof(Number));
        }

        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        // This method is called by the Set accessor of each property.
        // The CallerMemberName attribute that is applied to the optional propertyName
        // parameter causes the property name of the caller to be substituted as an argument.
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion INotifyPropertyChanged Implementation
    }
}
