using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Xrm.Sdk.Samples;
using Microsoft.Crm.Sdk.Messages.Samples;



namespace CrmOAuthDemo
{


    [Activity(Label = "CRM OAuth Demo", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {

        int count = 1;

        static public OrganizationDataWebServiceProxy proxy;
        public AuthenticationContext authContext = null;
        public static string DisplayContents = String.Empty;


        public const string ServerUrl = "https://duprepro.agcrm13.com:81";
        public const string ClientId = "fe313022-b19a-4610-a3ff-d504008a66fe";
        public static string redirectUri = "https://apurvgo";
        public static string AuthorityUrl = "https://sts.agcrm13.com/adfs/oauth2/authorize";

        public static string TokenSuccess = String.Empty;


        Button button = null;
        TextView textView = null;


        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            AuthenticationAgentContinuationHelper.SetAuthenticationAgentContinuationEventArgs(requestCode, resultCode, data);
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            button = FindViewById<Button>(Resource.Id.MyButton);
            textView = FindViewById<TextView>(Resource.Id.textView1);

            button.Click += (sender, e) =>
            {

                button.Text = string.Format("{0} clicks!", count++);
                DoCrmLoginAction();
            };

        }

        public void WorkingDisplay(string txt)
        {
            DisplayContents += "\r\n " + txt + "\r\n";
            textView.Text = DisplayContents;
        }
        public async void DoCrmLoginAction()
        {
            WorkingDisplay("Authentication started..");
            String accessToken = string.Empty;

            try
            {

                // If authContext is null, then generate it.
                if (authContext == null)
                    authContext = new AuthenticationContext(AuthorityUrl, false);

                //Clear Cache
                if (authContext.TokenCache.Count > 0)
                    authContext.TokenCache.Clear();

                WorkingDisplay("Token Flushed. Starting Auth..");
                AuthenticationResult result = await authContext.AcquireTokenAsync(
                    resource: ServerUrl,
                    clientId: ClientId,
                    redirectUri: new Uri(redirectUri),
                    parameters: new AuthorizationParameters(this));


                if (result != null)
                {
                    //Result a token here. Storing to local variable
                    WorkingDisplay("Storing Token Completed..");
                    TokenSuccess = result.AccessToken;
                }
                else
                {
                    WorkingDisplay("unknown occurred..");
                }
            }
            catch (Exception ex)
            {
                WorkingDisplay("Error occurred.." + ex.Message);
            }
        }

        void GetLoggedInUser(string tokenValue)
        {
            proxy = new OrganizationDataWebServiceProxy();
            proxy.ServiceUrl = ServerUrl;
            proxy.EnableProxyTypes();
            proxy.AccessToken = tokenValue;
            //TO DO: Write a sample for Logged In User
            //Guid userid = ((Microsoft.Crm.Sdk.Messages.Samples.WhoAmIResponse)proxy.Execute(new Microsoft.Crm.Sdk.Messages.Samples.WhoAmIRequest())).UserId;
        }
    }
}

