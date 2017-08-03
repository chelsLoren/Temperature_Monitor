using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content;
using System;
using System.IO;

namespace MCD_V1
{
    [Activity(Label = "MCD_V1", MainLauncher = true, Icon = "@drawable/babysleeping")]
    public class MainActivity : Activity
    {
        Button sensorbutton;
        Button settingsButton;
        Button showMaps;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);            
            SetContentView(Resource.Layout.Main);

            sensorbutton = FindViewById<Button>(Resource.Id.sensor_button);
            settingsButton = FindViewById<Button>(Resource.Id.settings_button);
            showMaps = FindViewById<Button>(Resource.Id.Google_map);

            sensorbutton.Click += delegate (object sender, EventArgs e)
            {
                var intent = new Intent(this, typeof(weather));
                StartActivity(intent);
                
            };

            settingsButton.Click += delegate (object sender, EventArgs e)
            {
                var intent = new Intent(this, typeof(settings));
                StartActivity(intent);
            };

            // launch event for Google maps
            showMaps.Click += delegate 
            {
                // create Uri with GPS coordinates of Lincoln
                var geoUri = Android.Net.Uri.Parse("geo:53.22683 -0.53792");
                // pass coordinates to intent app that is capable of display geocoordinates
                var mapIntent = new Intent(Intent.ActionView, geoUri);
                // launch Google maps or show list of map apps to choose from
                StartActivity(mapIntent);
            };

            if (! doSettingsExist())
            {
                Toast.MakeText(this, "There doesn't appear any Min/Max settings set, please set them now.", ToastLength.Long).Show();
                var intent = new Intent(this, typeof(settings));
                StartActivity(intent);
            }
        }

        private Boolean doSettingsExist()
        {
            string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            string filename = Path.Combine(path, "mySettings.txt");
            int counter = 0;
            if (! Directory.Exists(filename)) { return false; };

            using (var streamReader = new StreamReader(filename)) //Needs a try catch wrapping around
            {
                string settings = streamReader.ReadToEnd();
                if (settings == null || settings == "") { return false; }
                else while ((settings = streamReader.ReadLine()) != null)
                {
                        if (counter == 0) { GlobalVariables.tmpMin = settings; }
                        if (counter == 1) { GlobalVariables.tmpMax = settings; }
                        if (counter == 2) { GlobalVariables.humMin = settings; }
                        if (counter == 3) { GlobalVariables.humMax = settings; }
                        counter++;
                }
            }

                return true;
        }
    }
}

