using CoreTweetCommon;
using CoreTweet;
using System;
using System.Threading;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// 空白ページのアイテム テンプレートについては、http://go.microsoft.com/fwlink/?LinkId=234238 を参照してください

namespace CoreTweetTest
{
	/// <summary>
	/// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
	/// </summary>
	public sealed partial class MainPage : Page
	{
		#region Fields

		internal const string API_CONSUMER_TOKEN = Const.API_CONSUMER_TOKEN;
		internal const string API_CONSUMER_SECRET = Const.API_CONSUMER_SECRET;
		internal const string API_CONSUMER_CALLBACK_URL = "http://127.0.0.1:64003/Account/ExternalLoginCallback";

		#endregion Fields

		#region Properies
		public OAuth.OAuthSession Session { get; private set; }
		public Tokens Tokens { get; private set; }
		#endregion Properies

		#region Constructor
		public MainPage()
		{
			this.InitializeComponent();
			_debugMessage.Text = "Start..";
		}
		#endregion Constructor

		#region Methods
		/// <summary>
		/// 1st.Step OnAuthorize
		/// </summary>
		private async void OnAuthorize(object sender, Windows.UI.Xaml.RoutedEventArgs args)
		{
			Session = await OAuth.AuthorizeAsync(API_CONSUMER_TOKEN, API_CONSUMER_SECRET);
			string uriToLaunch = Session.AuthorizeUri.ToString();
			var uri = new Uri(uriToLaunch);
			var success = await Windows.System.Launcher.LaunchUriAsync(uri);
			if (success)
			{
				// URI launched
				return;
			}
			else
			{
				// URI launch failed
				MessageDialog md = new MessageDialog("Failed External Web Browser Open.");
				await md.ShowAsync();
			}
		}

		/// <summary>
		/// 2nd.Step PIN Comparison Button
		/// </summary>
		private async void PinExeCute(object sender, RoutedEventArgs e)
		{
			string pin = _pincodeTextBox.Text;
			Tokens = await Session.GetTokensAsync(pin);
			if (Tokens != null)
			{
				_debugMessage.Text +=
					"\r" +
					"\r Token:" + Tokens.AccessToken.ToString() +
					"\r TokenSecret :" + Tokens.AccessTokenSecret.ToString() +
					"\r UseerId :" + Tokens.UserId.ToString() +
					"\r ScreenName :" + Tokens.ScreenName.ToString();
			}
		}

		/// <summary>
		/// OnTweet Button
		/// </summary>
		private async void OnTweet(object sender, Windows.UI.Xaml.RoutedEventArgs args)
		{
			var tokenSource = new CancellationTokenSource();
			var res = await Tokens.Statuses.UpdateAsync(new { status = _statusTextBox.Text + " #Test #CoreTweet #KunappWSA" }, tokenSource.Token);
			tokenSource.Cancel();
			if (res != null)
			{
				_debugMessage.Text +=
					"\r result :" +
					"\r Id:" + res.Id.ToString() +
					"\r HashCode:" + res.GetHashCode().ToString() +
					"\r Json:" + res.Json.ToString() +
					"\r Text:" + res.Text;
				return;
			}
		}
		#endregion Methods
	}
}