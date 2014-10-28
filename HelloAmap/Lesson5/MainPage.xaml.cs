using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Com.AMap.Api.Maps;
using Com.AMap.Api.Maps.Model;
using Com.AMap.Api.Services;
using Com.AMap.Api.Services.Results;
using Microsoft.Phone.Controls;

namespace Lesson5
{
    public partial class MainPage : PhoneApplicationPage
    {
        private readonly AMap amap;
        private LatLng latLng;

        // Constructor
        public MainPage()
        {
            InitializeComponent();
            ContentPanel.Children.Add(amap = new AMap());

            amap.Tap += amap_Tap;
        }


        private void amap_Tap(object sender, GestureEventArgs e)
        {
            latLng = amap.GetProjection().FromScreenLocation(e.GetPosition(amap));
            txtLat.Text = "lon:" + latLng.longitude + " lat:" + latLng.latitude;
        }


        private async Task GetPoiAround(double centerX, double centerY, string keywords, string types,
            uint radius, string city)
        {
            AMapPOIResults poir =
                await
                    AMapPOISearch.POIAround(centerX, centerY, keywords, types, null, radius, 0, 20, 1, Extensions.All,
                        city);

            if (poir.Erro == null && poir.POIList != null)
            {
                if (poir.POIList.Count == 0)
                {
                    MessageBox.Show("无查询结果");
                    return;
                }
                IEnumerable<AMapPOI> pois = poir.POIList;

                int i = 0;
                foreach (AMapPOI poi in pois)
                {
                    i++;

                    amap.AddMarker(new AMapMarkerOptions
                    {
                        Position = new LatLng(poi.Location.Lat, poi.Location.Lon), //amap.Center,//
                        Title = poi.Name,
                        Snippet = poi.Address,
                        IconUri = new Uri("Assets/myDownload.jpg",UriKind.Relative)
                        
                    });
                }
            }
            else
            {
                MessageBox.Show(poir.Erro.Message);
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (latLng != null)
            {
                amap.Clear();
                await
                    GetPoiAround(latLng.longitude, latLng.latitude, txtKeyWords.Text, txtTypes.Text, 3000, txtCity.Text);
            }
        }
    }
}