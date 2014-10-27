﻿using System;
using System.Windows;
using System.Windows.Media;
using Com.AMap.Api.Maps;
using Com.AMap.Api.Maps.Model;
using Microsoft.Phone.Controls;

namespace Lesson3
{
    public partial class MainPage : PhoneApplicationPage
    {
        private AMap amap;
        private AMapGeolocator mylocation;
        private LatLng location;
        private AMapMarker marker;
        private AMapCircle circle;
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
            ContentPanel.Children.Add(amap = new AMap());
            Loaded += MyLocation_Loaded;
            Unloaded += MyLocation_Unloaded;
        }

        private void MyLocation_Unloaded(object sender, RoutedEventArgs e)
        {
            if (mylocation != null)
            {
                mylocation.PositionChanged -= mylocation_PositionChanged;
                mylocation.Stop();
            }
        }

        private void MyLocation_Loaded(object sender, RoutedEventArgs e)
        {
            mylocation = new AMapGeolocator();
            mylocation.Start();
            //触发位置改变事件
            mylocation.PositionChanged += mylocation_PositionChanged;
        }

        private void mylocation_PositionChanged(AMapGeolocator sender, AMapPositionChangedEventArgs args)
        {
            location = args.LngLat;
            //todo 是否应该给用户直接转向UI线程??类似amap_CameraChangeListener
            Dispatcher.BeginInvoke(() =>
            {
                //GeoSearch(args.LngLat);

                if (marker == null)
                {
                    //添加圆
                    circle = amap.AddCircle(new AMapCircleOptions
                    {
                        Center = args.LngLat, //圆点位置
                        Radius = (float)args.Accuracy, //半径
                        FillColor = Color.FromArgb(80, 100, 150, 255),
                        StrokeWidth = 2, //边框粗细
                        StrokeColor = Color.FromArgb(80, 0, 0, 255), //边框颜色
                    });

                    //添加点标注，用于标注地图上的点
                    marker = amap.AddMarker(
                        new AMapMarkerOptions
                        {
                            Position = args.LngLat, //图标的位置
                            Title = "我的位置",
                            Snippet = args.LngLat.ToString(),
                            IconUri = new Uri("Assets/marker_gps_no_sharing.png", UriKind.Relative),//图标的URL
                            Anchor = new Point(0.5, 0.5), //图标中心点
                        });
                }
                else
                {
                    //点标注和圆的位置在当前经纬度
                    marker.Position = args.LngLat;
                    circle.Center = args.LngLat;
                    circle.Radius = (float)args.Accuracy; //圆半径
                }

                //设置当前地图的经纬度和缩放级别
                amap.MoveCamera(CameraUpdateFactory.NewLatLngZoom(args.LngLat, 15));
            });
        }

       
    }
}