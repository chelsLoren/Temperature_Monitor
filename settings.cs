using System;
using Android.App;
using Android.OS;
using Android.Widget;
using System.IO;
using Android.Views;

namespace MCD_V1
{
    [Activity(Label = "Settings", Icon = "@drawable/babysleeping")]
    public class settings : Activity
    {
        Button saveSetting;
        EditText minTmp;
        EditText maxTmp;
        EditText minHum;
        EditText maxHum;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.settings);
            Toast.MakeText(this, "Please enter valid number Values i.e  29", ToastLength.Long);

            ActionBar.SetHomeButtonEnabled(true);
            ActionBar.SetDisplayHomeAsUpEnabled(true);

            saveSetting = FindViewById<Button>(Resource.Id.settingsSave);
            minTmp = FindViewById<EditText>(Resource.Id.tempMin);
            maxTmp = FindViewById<EditText>(Resource.Id.tempMax);
            minHum = FindViewById<EditText>(Resource.Id.humMin);
            maxHum = FindViewById<EditText>(Resource.Id.humMax);

            saveSetting.Click += (object sender, EventArgs e) =>
            {
                                                
                string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
                string filename = Path.Combine(path, "mySettings.txt");
                using (var streamWriter = new System.IO.StreamWriter(filename, false))

                    for (int counter = 0; counter < 4; counter++)
                    {
                        if (counter == 0) { streamWriter.WriteLine(minTmp.Text); GlobalVariables.tmpMin = minTmp.Text; }
                        if (counter == 1) { streamWriter.WriteLine(maxTmp.Text); GlobalVariables.tmpMax = maxTmp.Text; }
                        if (counter == 2) { streamWriter.WriteLine(minHum.Text); GlobalVariables.humMin = minHum.Text; }
                        if (counter == 3) { streamWriter.WriteLine(maxHum.Text); GlobalVariables.humMax = maxHum.Text; }
                    }
                if (!Directory.Exists(filename)) { Console.WriteLine("Error file does not exist!"); }

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

        public bool validForm ()
        {
            decimal d;

            if (!Decimal.TryParse(minTmp.Text, out d)) { return false; }
            if (!Decimal.TryParse(maxTmp.Text, out d)) { return false; }
            if (!Decimal.TryParse(minHum.Text, out d)) { return false; }
            if (!Decimal.TryParse(maxHum.Text, out d)) { return false; }
            return true;
        }
        

    }
}