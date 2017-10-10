using Dyn365Samples;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Dyn365OAuthTshooter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string OrgUrl { get; set; }
        public string clientId { get; set; }
        public string redirectUrl { get; set; }


        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            OrgUrl = txtOrgUrl.Text;
            clientId = txtClientId.Text;
            redirectUrl = txtRedirectUrl.Text;

            Thread sThread = new Thread(DoWork);
            sThread.Start();
        }

        async void DoWork()
        {
            await txtOutput.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => this.txtOutput.Text = string.Empty));
            await txtOutput.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => this.txtOutput.Text = "Connecting..."));
            try
            {
                WebAPIHelper apiHelper = new WebAPIHelper();
                apiHelper.BaseOrganizationApiUrl = OrgUrl;                
                AuthenticationParameters authParams =  apiHelper.DiscoverAuthority();
                await txtOutput.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                {
                    this.txtoauthEndpoint.Text = authParams.Authority;
                    this.txtDiscoveryUrl.Text = authParams.Resource;
                }));

                apiHelper.BaseOrganizationApiUrl = OrgUrl;
               await apiHelper.ObtainOAuthToken(clientId, redirectUrl, authParams);
                await txtOutput.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => this.txtOutput.Text = "Finished Obtaining Access Token"));
                if (!(string.IsNullOrEmpty(apiHelper.AccessToken)))
                {
                    //await txtOutput.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => this.txtOutput.Text = "Creating Sample Account in CRM"));

                    //Create Sample Contact
                    JObject accountJobj = new JObject();
                    accountJobj.Add("name", "SDK Account");
                    //await apiHelper.CreateEntityRecords("accounts", accountJobj, "v8.1");

                    await txtOutput.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => this.txtOutput.Text = $"Printing Access Token {apiHelper.AccessToken}"));

                    await txtOutput.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => this.txtOutput.Text = "Completed Acount Creation"));
                }
            }
            catch (Exception e)
            {
                await txtOutput.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => this.txtOutput.Text = e.Message));
            }

        }

    }
}
