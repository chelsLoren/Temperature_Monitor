using System;
using System.Collections.Generic;
using System.Json;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using Android.App;
using Android.OS;
using Android.Widget;
using Newtonsoft.Json;
using Android.Content;
using Android.Views;

namespace MCD_V1
{
    [Activity(Label = "Temperature and Humidity", Icon = "@drawable/babysleeping")]
    public class weather : Activity
    {
        Button get_data_Temp;
        Button get_data_Hum;
        Button refresh_Temp;
        Button refresh_Hum;
        ListView data_view;
        
        Android.Media.MediaPlayer _player;

        public class jsonSensor
        {
            //public string sensor_id { get; set; }
            public string time_stamp { get; set; }
            public string sensor_type { get; set; }
            public string value { get; set; }
        }

        public class RootObject
        {
            //[JsonProperty("sensor_id")]
            //public string sensor_id { get; set; }
            [JsonProperty("time_stamp")]
            public string time_stamp { get; set; }
            [JsonProperty("sensor_type")]
            public string sensor_type { get; set; }
            [JsonProperty("value")]
            public string value { get; set; }
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.sensordata);

            ActionBar.SetHomeButtonEnabled(true);
            ActionBar.SetDisplayHomeAsUpEnabled(true);

            _player = Android.Media.MediaPlayer.Create(this, Resource.Raw.test);
            data_view = FindViewById<ListView>(Resource.Id.dataView);

            get_data_Temp = FindViewById<Button>(Resource.Id.button1);
            get_data_Temp.Click += async (object sender, EventArgs e) =>
            {
                get_data_Temp.Enabled = false; //Set enabled to false so we can't force through multiple requests
                Toast.MakeText(this, "Loading...", ToastLength.Long).Show();
                try
                {
                    string sensorURL = "http://mcd14541565.azurewebsites.net/index.php/weather/getsensordata?sensorid=1";
                    JsonValue json = await GetTempData(sensorURL);

                    //Temperature Data
                    TextView jsondata_1 = FindViewById<TextView>(Resource.Id.textView1);
                    TextView jsondata_2 = FindViewById<TextView>(Resource.Id.textView2);
                    //jsondata_1.Text = json.ToString();           // Output Pure json as returned by GET
                    jsondata_2.Text = ParseAndDisplay(json);       // Output formatted JSON
                    _player.Start();
                }
                catch (Exception ex)    //This will capture the issue
                {
                    Toast.MakeText(this, "Error Loading Data:" + ex.ToString(), ToastLength.Long).Show();
                }
                finally //regardless of any error this code gets executed.
                {
                    get_data_Temp.Enabled = true; //so the button always ends up enabled.
                }

            };

            get_data_Hum = FindViewById<Button>(Resource.Id.button2);
            get_data_Hum.Click += async (object sender, EventArgs e) =>
            {
                get_data_Hum.Enabled = false; //Set enabled to false so we can't force through multiple requests
                Toast.MakeText(this, "Loading...", ToastLength.Long).Show();
                try
                {
                    string sensorURL2 = "http://mcd14541565.azurewebsites.net/index.php/weather/getsensordata?sensorid=2";
                    //sensorURL2 = "N..!"; //this shows error handling if uncommented
                    JsonValue json2 = await GetHumData(sensorURL2);

                    //Humidity Data
                    TextView jsondata_3 = FindViewById<TextView>(Resource.Id.textView3);
                    TextView jsondata_4 = FindViewById<TextView>(Resource.Id.textView4);
                    // jsondata_3.Text = json2.ToString();
                    jsondata_4.Text = PAndD(json2);
                    _player.Start();
                    //loadData();
                }
                catch (Exception ex)    //This will capture the issue
                {
                    Toast.MakeText(this, "Error Loading Data:" + ex.ToString(), ToastLength.Long).Show();
                }
                finally //regardless of any error this code gets executed.
                {
                    get_data_Hum.Enabled = true; //so the button always ends up enabled.
                }
            
            };

            refresh_Temp = FindViewById<Button>(Resource.Id.button3);
            refresh_Temp.Click += (object sender, EventArgs e) =>
            {
                var intent = new Intent(this, typeof(weather));
                StartActivity(intent);
            };

            refresh_Hum = FindViewById<Button>(Resource.Id.button4);
            refresh_Hum.Click += (object sender, EventArgs e) =>
            {             
                var intent = new Intent(this, typeof(weather));
                StartActivity(intent);
                //DisplayAlert("Alert", "You have been alerted", "OK");
            };           

        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    Finish();
                    return true;

                default:
                    return base.OnOptionsItemSelected(item);
            }
        }

        public override void OnBackPressed()
        {
            var intent = new Intent(this, typeof(MainActivity));
            StartActivity(intent);

            //base.OnBackPressed(); // DO NOT CALL THIS LINE OR WILL NAVIGATE BACK
        }

        private async Task<JsonValue> GetTempData(string url)
        {
            // Create an HTTP web request using the URL:
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(url));
            request.ContentType = "application/json";
            request.Method = "GET";

            // Send the request to the server and wait for the response:
            using (WebResponse response = await request.GetResponseAsync())
            {
                // Get a stream representation of the HTTP web response:
                using (Stream stream = response.GetResponseStream())
                {
                    // Use this stream to build a JSON document object:
                    JsonValue jsonDoc = await Task.Run(() => JsonValue.Load(stream));
                    Console.Out.WriteLine("Response: {0}", jsonDoc.ToString());
                    return jsonDoc;
                }
            }
        }

        //Full data from the database is parsed and formatted
        private string ParseAndDisplay(JsonValue json)
        {
            try
            {
                string jsonlist = "";
                if (json == null)
                {
                    return "Error unable to retieve data";
                }
                var items = JsonConvert.DeserializeObject<List<jsonSensor>>(json.ToString());
                TextView outputjson = FindViewById<TextView>(Resource.Id.textView1);

            // loop through entire json document and list each item
            for (int x = 0; x < items.Count; x++)
            {
                Console.Out.WriteLine("Response {0}: {1};", x, items[x].sensor_type);
                jsonlist += $"Data{x}: type={items[x].sensor_type}, val={items[x].value}, time ={items[x].time_stamp} \n"; //id={items[x].sensor_id}
            }
                return jsonlist;
            }
            catch (JsonException jex)//Try to catch any possible errors thrown by converting  json to string;
            {
                Toast.MakeText(this, "Error invalid data type: " + jex.ToString(), ToastLength.Long).Show();
            }
            return "";          
        }

        private async Task<JsonValue> GetHumData(string url)
        {
            // Create an HTTP web request using the URL:
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(url));
            request.ContentType = "application/json";
            request.Method = "GET";

            // Send the request to the server and wait for the response:
            using (WebResponse response = await request.GetResponseAsync())
            {
                // Get a stream representation of the HTTP web response:
                using (Stream stream = response.GetResponseStream())
                {
                    // Use this stream to build a JSON document object:
                    JsonValue jsonDoc = await Task.Run(() => JsonValue.Load(stream));
                    Console.Out.WriteLine("Response: {0}", jsonDoc.ToString());
                    return jsonDoc;
                }
            }
        }      

        //Full data from the database is parsed and formatted
        private string PAndD(JsonValue json2)
        {
            try
            {
                string jsonlist = "";
                if (json2 == null)
                {
                    return "Error unable to retieve data";
                }              
                var items = JsonConvert.DeserializeObject<List<jsonSensor>>(json2.ToString());
                TextView outputjson1 = FindViewById<TextView>(Resource.Id.textView3);

                // loop through entire json document and list each item
                for (int x = 0; x < items.Count; x++)
                {
                    Console.Out.WriteLine("Response {0}: {1};", x, items[x].sensor_type);
                    jsonlist += $"Data{x}: type={items[x].sensor_type}, val={items[x].value}, time ={items[x].time_stamp} \n"; //id={items[x].sensor_id}
                }
                return jsonlist;
            }
            catch (JsonException jex)//Try to catch any possible errors thrown by converting  json to string;
            {
                Toast.MakeText(this, "Error invalid data type: " + jex.ToString(), ToastLength.Long).Show();
            }
            return "";
        }

        private void loadData()
        {
            string[] items = new string[] { "test", "Test" };
            ArrayAdapter listAdapter = new ArrayAdapter<string>(this, Resource.Id.dataView, items);
            data_view.Adapter = listAdapter;
        }

        //private void loadData()
        //{
        //    string[] items = new string[] { "test", "Test" };
        //    ArrayAdapter listAdapter = new ArrayAdapter<string>(this, Resource.Id.dataView, items);
        //    data_view.Adapter = listAdapter;
        //}


    }

    
}