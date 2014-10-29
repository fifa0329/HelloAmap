using Com.AMap.Api.Maps;
using Microsoft.Phone.Controls;

namespace HelloAmap
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Sample code to localize the ApplicationBar
            ContentPanel.Children.Add(new AMap());
        }


    }
}