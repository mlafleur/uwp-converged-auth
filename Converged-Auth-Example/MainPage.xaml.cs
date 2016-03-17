using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Security.Authentication.Web;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Converged_Auth_Example
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void button_Click(object sender, RoutedEventArgs e)
        {
            string applicationId = "2a4dbf13-4a77-471a-8519-a8bd56f85728";
            string redirectUri = "urn:ietf:wg:oauth:2.0:oob";
            string scopes = "https://graph.microsoft.com/User.Read openid offline_access";

            string authorizeUri = string.Format("https://login.microsoftonline.com/common/oauth2/v2.0/authorize?client_id={0}&response_type=code&redirect_uri={1}&scope={2}", applicationId, redirectUri, scopes);
            string tokenUri = "https://login.microsoftonline.com/common/oauth2/v2.0/token";

            WebAuthenticationResult WebAuthenticationResult =
                await WebAuthenticationBroker.AuthenticateAsync(
                        WebAuthenticationOptions.None,
                        new Uri(authorizeUri),
                        new Uri(redirectUri));

            if (WebAuthenticationResult.ResponseStatus == WebAuthenticationStatus.Success)
            {
                // Parse out the auth code from the response
                string authorization_code = WebAuthenticationResult.ResponseData.ToString().Split('=')[1].Split('&')[0];

                // Buld a data set to send to the Token URI for processing
                var data = new Dictionary<string, string>();
                data.Add("grant_type", "authorization_code");
                data.Add("code", authorization_code);
                data.Add("client_id", applicationId);
                data.Add("redirect_uri", redirectUri);
                data.Add("scope", scopes);

                // Create an HTTP Client
                var client = new System.Net.Http.HttpClient();

                // Post the data we compiled above to the TokenURI
                var result = await client.PostAsync(tokenUri, new System.Net.Http.FormUrlEncodedContent(data));

                // This next part is only for the demo. It the JSON object returned by the TokenURI
                // and converts it to a raw string. We then copy it to the UI so you can see the results
                var tokenObject = await result.Content.ReadAsStringAsync();
                this.resultText.Text = tokenObject;
            }
            else if (WebAuthenticationResult.ResponseStatus == WebAuthenticationStatus.ErrorHttp)
            {
                this.resultText.Text = "HTTP Error returned by AuthenticateAsync() : " + WebAuthenticationResult.ResponseErrorDetail.ToString();
            }
            else
            {
                this.resultText.Text = "Error returned by AuthenticateAsync() : " + WebAuthenticationResult.ResponseStatus.ToString();
            }
        }
    }
}