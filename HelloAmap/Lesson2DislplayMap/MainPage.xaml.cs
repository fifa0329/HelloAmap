using Com.AMap.Api.Maps;
using Microsoft.Phone.Controls;

namespace Lesson2DislplayMap
{
    public partial class MainPage : PhoneApplicationPage
    {
        private AMap amap;
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();


            ContentPanel.Children.Add(amap = new AMap());

            amap.CameraChangeListener += amap_CameraChangeListener;
        }

        private void amap_CameraChangeListener(object sender, AMapEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                //显示信息
                cameraloutput1.Text = "target：" + e.CameraPosition.target;
                cameraloutput2.Text = "tilt：" + e.CameraPosition.tilt;
                cameraloutput3.Text = "bearing：" + e.CameraPosition.bearing;
                cameraloutput4.Text = "zoom:" + e.CameraPosition.zoom;
            });
        }
    }
}