using System;
using CoreGraphics;
using Google.Maps;
using UIKit;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Collections.Generic;
using Newtonsoft.Json;
using GMCluster;
using ParkLinkClusteringIOS.Models;

namespace ParkLinkClusteringIOS
{
    public partial class ViewController : UIViewController, IGMUClusterRendererDelegate, IGMUClusterManagerDelegate, IMapViewDelegate
    {
        MapView mapView = null;
        GMUClusterManager clusterManager;

        protected ViewController(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public override void LoadView()
        {
            base.LoadView();
            CameraPosition camera = CameraPosition.FromCamera(latitude: 59.9115, longitude: 10.7579, zoom: 10);
            mapView = MapView.FromCamera(frame: CGRect.Empty, camera: camera);
            this.View = mapView;
        }

        public override async void ViewDidLoad()
        {
            base.ViewDidLoad();

            var googleMapView = mapView; //use the real mapview init'd somewhere else instead of this
            var iconGenerator = new GMUDefaultClusterIconGenerator();
            var algorithm = new GMUNonHierarchicalDistanceBasedAlgorithm();
            var renderer = new GMUDefaultClusterRenderer(googleMapView, iconGenerator);

            renderer.WeakDelegate = this;

            clusterManager = new GMUClusterManager(googleMapView, algorithm, renderer);

            JsonSerializer _serializer = new JsonSerializer()
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Utc
            };

            using (HttpClient httpClient = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip }))
            {
                Token token = null;
                httpClient.BaseAddress = new Uri(AppSettings.BaseAddress);
                FormUrlEncodedContent formUrlEncodedContent = new FormUrlEncodedContent(new Dictionary<String, String> {
                    { "client_id", AppSettings.Client_id },
                    { "client_secret", AppSettings.Client_secret },
                    { "grant_type", AppSettings.Grant_type }
                });
                using (HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "/connect/token"))
                {
                    httpRequestMessage.Content = formUrlEncodedContent;
                    using (HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(httpRequestMessage))
                    {
                        if (httpResponseMessage.IsSuccessStatusCode == true)
                        {
                            using (System.IO.Stream stream = await httpResponseMessage.Content.ReadAsStreamAsync())
                            {
                                using (System.IO.StreamReader streamReader = new System.IO.StreamReader(stream))
                                {
                                    using (JsonTextReader json = new JsonTextReader(streamReader))
                                    {
                                        token = _serializer.Deserialize<Token>(json);
                                    }
                                }
                            }
                        }
                    }
                }
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token.token_type, token.access_token);

                using (HttpResponseMessage httpResponseMessage = await httpClient.GetAsync("/api/v1/pl/Locations/lots"))
                {
                    if (httpResponseMessage.IsSuccessStatusCode == true)
                    {
                        using (System.IO.Stream stream = await httpResponseMessage.Content.ReadAsStreamAsync())
                        {
                            using (System.IO.StreamReader streamReader = new System.IO.StreamReader(stream))
                            {
                                using (JsonTextReader json = new JsonTextReader(streamReader))
                                {
                                    Response<Lot[]> lots = _serializer.Deserialize<Response<Lot[]>>(json);
                                    for (int i = 0; i < lots.result.Length; i++)
                                    {
                                        IGMUClusterItem item = new POIItem(lots.result[i].latitude, lots.result[i].longitude, lots.result[i].name);
                                        clusterManager.AddItem(item);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            clusterManager.Cluster();

            clusterManager.SetDelegate(this, this);
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
    }
}
