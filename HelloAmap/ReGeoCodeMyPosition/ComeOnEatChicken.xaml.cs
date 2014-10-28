using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace ReGeoCodeMyPosition
{
    public partial class ComeOnEatChicken : UserControl
    {
        private string location;

        public ComeOnEatChicken()
        {
            InitializeComponent();
        }


        public ComeOnEatChicken(string location)
        {
            this.location = location;
            InitializeComponent();

        }


        private void LayoutRoot_OnLoaded(object sender, RoutedEventArgs e)
        {
            LocationBlock.Text = location;
        }
    }
}
