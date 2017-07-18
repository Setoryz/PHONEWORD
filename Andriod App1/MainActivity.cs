using Android.App;
using Android.Widget;
using Android.OS;
using System;
using Android.Content;
using System.Collections.Generic;

namespace Phoneword
{
    [Activity(Label = "Phoneword", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        static readonly List<string> phoneNumbers = new List<string>();

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            //New code went here
            // GET THE UI CONTROLS FROM THE LOADED LAYOUT;
            EditText phoneNumberText = FindViewById<EditText>(Resource.Id.PhoneNumberText);
            Button translateButton = FindViewById<Button>(Resource.Id.TranslateButton);
            Button callButton = FindViewById<Button>(Resource.Id.CallButton);
            Button callHistoryButton = FindViewById<Button>(Resource.Id.CallHistoryButton);

            // DISABLE THE CALL CONTROL
            callButton.Enabled = false;

            // ADD CODE TO TRANSLATE NUMBER
            string translatedNumber = string.Empty;

            translateButton.Click += (object sender, EventArgs e) =>
            {
                // TRANSLATE USER'S ALPHANUMERIC PHONE NUMBER TO NUMERIC
                translatedNumber = Core.PhonewordTranslator.ToNumber(phoneNumberText.Text);
                if (string.IsNullOrWhiteSpace(translatedNumber))
                {
                    callButton.Text = "Call";
                    callButton.Enabled = false;
                }
                else
                {
                    callButton.Text = $"Call {translatedNumber}";
                    callButton.Enabled = true;
                }
            };

            callButton.Click += (object sender, EventArgs e) =>
            {
                // ON "CALL" BUTTON CLICK TRY, TRY TO DIAL PHONE NUMBER.
                var callDialog = new AlertDialog.Builder(this);
                callDialog.SetMessage("Call " + translatedNumber + "?");
                callDialog.SetNeutralButton("Call", delegate
                    {
                        // ADD DIALED NUMBER TO LIST OF CALLED NUMBERS
                        phoneNumbers.Add(translatedNumber);
                        // ENABLE THE CALL HISTORY BUTTON
                        callHistoryButton.Enabled = true;
                        // CREATE INTENT TO DIAL PHONE
                        var callIntent = new Intent(Intent.ActionCall);
                        callIntent.SetData(Android.Net.Uri.Parse("tel:" + translatedNumber));
                        StartActivity(callIntent);
                    });
                callDialog.SetNegativeButton("Cancel", delegate { });

                // SHOW THE ALERT DIALOG TO THE USER AND WAIT FOR RESPONSE
                callDialog.Show();
            };

            callHistoryButton.Click += (sender, e) =>
            {
                var intent = new Intent(this, typeof(CallHistoryActivity));
                intent.PutStringArrayListExtra("phone_numbers", phoneNumbers);
                StartActivity(intent);
            };
        }
    }
}

