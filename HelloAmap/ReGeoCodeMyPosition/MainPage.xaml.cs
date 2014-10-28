using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Com.AMap.Api.Maps;
using Com.AMap.Api.Maps.Model;
using Com.AMap.Api.Services;
using Com.AMap.Api.Services.Results;
using Microsoft.Phone.Controls;

namespace ReGeoCodeMyPosition
{
    public partial class MainPage : PhoneApplicationPage
    {
        private readonly AMap amap;
        private LatLng latLng;
        private AMapMarker marker;
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
            txtLat.Text = latLng.latitude.ToString();
            txtLon.Text = latLng.longitude.ToString();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            amap.Clear();
            if (string.IsNullOrWhiteSpace(txtLat.Text) && string.IsNullOrWhiteSpace(txtLon.Text))
            {
                return;
            }
            await GeoCodeToAddress(Convert.ToDouble(txtLon.Text), Convert.ToDouble(txtLat.Text));
        }

        private async Task GeoCodeToAddress(double lon, double lat)
        {
            AMapReGeoCodeResult result = await AMapReGeoCodeSearch.GeoCodeToAddress(lon, lat, 500, "", Extensions.All);

            if (result.Erro == null && result.ReGeoCode != null)
            {
                AMapReGeoCode regeocode = result.ReGeoCode;

                var poiFirst = regeocode.Pois.ToList().First();

                marker = amap.AddMarker(new AMapMarkerOptions
                {
                    Position = new LatLng(poiFirst.Location.Lat, poiFirst.Location.Lon),
                    Title = poiFirst.Name,
                    Snippet = poiFirst.Address,
                });


                string locationHumanReadable = poiFirst.Name;
                var info = new ComeOnEatChicken(locationHumanReadable);
                marker.ShowInfoWindow(info);


                amap.MoveCamera(
                    CameraUpdateFactory.NewLatLngZoom(
                        new LatLng(Convert.ToDouble(txtLon.Text), Convert.ToDouble(txtLat.Text)), 12));
            }
            else
            {
                MessageBox.Show(result.Erro.Message);
            }
        }
    }
}