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
            AMapReGeoCodeResult rcc = await AMapReGeoCodeSearch.GeoCodeToAddress(lon, lat, 500, "", Extensions.All);

            if (rcc.Erro == null && rcc.ReGeoCode != null)
            {
                AMapReGeoCode regeocode = rcc.ReGeoCode;

                List<AMapPOI> pois = regeocode.Pois.ToList();
                //POI信息点
                foreach (AMapPOI poi in pois)
                {
                    marker = amap.AddMarker(new AMapMarkerOptions
                    {
                        Position = new LatLng(poi.Location.Lat, poi.Location.Lon),
                        Title = poi.Name,
                        Snippet = poi.Address,
                    });
                }

                AMapAddressComponent addressComponent = regeocode.Address_component;
                AMapStreetNumber streetNumber = addressComponent.Stree_number;


                marker = amap.AddMarker(new AMapMarkerOptions
                {
                    Position = new LatLng(streetNumber.Location.Lat, streetNumber.Location.Lon), //amap.Center,//
                    Title = addressComponent.Province,
                    Snippet = regeocode.Formatted_address,
                });


                amap.MoveCamera(
                    CameraUpdateFactory.NewLatLngZoom(
                        new LatLng(Convert.ToDouble(txtLon.Text), Convert.ToDouble(txtLat.Text)), 12));
            }
            else
            {
                MessageBox.Show(rcc.Erro.Message);
            }
        }
    }
}