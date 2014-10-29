using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Com.AMap.Api.Maps;
using Com.AMap.Api.Maps.Model;
using Com.AMap.Api.Services;
using Com.AMap.Api.Services.Results;
using Microsoft.Phone.Controls;

namespace Lesson6
{
    public partial class MainPage : PhoneApplicationPage
    {
        private readonly AMap amap;
        private LatLng endLatLng;
        private LatLng startLatLng;
        private LatLng latLng;


        // Constructor
        public MainPage()
        {
            InitializeComponent();
            ContentPanel.Children.Add(amap = new AMap());

            amap.Tap += amap_Tap;


            //小明住在一个小岛上
            startLatLng = new LatLng(39.910785, 116.385842);
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
                AMapPOI poi = pois.First();

                amap.AddMarker(new AMapMarkerOptions
                {
                    Position = new LatLng(poi.Location.Lat, poi.Location.Lon),
                    Title = poi.Name,
                    Snippet = poi.Address,
                    IconUri = new Uri("Assets/myDownload.jpg", UriKind.Relative)
                });


                endLatLng = new LatLng(poi.Location.Lat, poi.Location.Lon);

                await GetNavigationWalking(startLatLng, endLatLng);
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


        private async Task GetNavigationWalking(LatLng start, LatLng end)
        {
            AMapRouteResults rts =
                await
                    AMapNavigationSearch.WalkingNavigation(start.longitude, start.latitude, end.longitude, end.latitude,
                        0);
            if (rts.Erro == null)
            {
                if (rts.Count == 0)
                {
                    MessageBox.Show("无查询结果");
                    return;
                }

                AMapRoute route = rts.Route;
                List<AMapPath> paths = route.Paths.ToList();

                var lnglats = new List<LatLng>();
                foreach (AMapPath item in paths)
                {
                    //画路线
                    List<AMapStep> steps = item.Steps.ToList();
                    foreach (AMapStep st in steps)
                    {
                        amap.AddMarker(new AMapMarkerOptions
                        {
                            Position = latLagsFromString(st.Polyline).FirstOrDefault(),
                            Title = "Title",
                            Snippet = "Snippet",
                            IconUri = new Uri("Assets/man.png", UriKind.Relative),
                        });
                        lnglats = latLagsFromString(st.Polyline);
                        amap.AddPolyline(new AMapPolylineOptions
                        {
                            Points = latLagsFromString(st.Polyline),
                            Color = Color.FromArgb(255, 0, 0, 255),
                            Width = 4,
                        });
                    }
                }
            }
            else
            {
                MessageBox.Show(rts.Erro.Message);
            }
        }


        private List<LatLng> latLagsFromString(string polyline)
        {
            var latlng = new List<LatLng>();

            string[] arrystring = polyline.Split(new[] { ';' });
            foreach (String str in arrystring)
            {
                String[] lnglatds = str.Split(new[] { ',' });
                latlng.Add(new LatLng(Double.Parse(lnglatds[1]), Double.Parse(lnglatds[0])));
            }
            return latlng;
        }
    }
}