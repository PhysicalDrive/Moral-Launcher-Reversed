using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;
using Moral;
using TiltedLauncher.Resources;
using Wpf.Ui.Controls;

public class Window1 : Window, IComponentConnector
{
	internal PasswordBox PasswordBox;

	internal TextBox EmailBox;

	private bool _contentLoaded;

	public Window1()
	{
		InitializeComponent();
		string text = UpdateINI.ReadValue("Auth", "Email");
		string text2 = UpdateINI.ReadValue("Auth", "Password");
		if (text != "NONE")
		{
			((TextBox)(object)EmailBox).Text = text;
		}
		if (text2 != "NONE")
		{
			PasswordBox.Password = text2;
		}
	}

	public string GetJsonBody(string url, string body)
	{
		using HttpClient httpClient = new HttpClient();
		StringContent content = new StringContent(body, Encoding.UTF8, "application/json");
		HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, url);
		httpRequestMessage.Content = content;
		FieldInfo field = typeof(HttpMethod).GetField("_content", BindingFlags.Instance | BindingFlags.NonPublic);
		if (field != null && field.GetValue(httpRequestMessage.Method) is HttpMethod httpMethod)
		{
			httpMethod.GetType().GetProperty("ContentBodyNotAllowed", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(httpMethod, false);
		}
		HttpResponseMessage result = httpClient.SendAsync(httpRequestMessage).Result;
		result.EnsureSuccessStatusCode();
		return result.Content.ReadAsStringAsync().Result;
	}

	private bool ValidateCredentialsAsync(string email, string password)
	{
		try
		{
			return GetJsonBody("http://172.93.101.73:3551/login", $"{{\"email\":\"{email}\", \"password\":\"{password}\"}}") == "true";
		}
		catch (Exception ex)
		{
			MessageBox.Show("Error: " + ex.Message);
			return false;
		}
	}

	private void Button_Click(object sender, RoutedEventArgs e)
	{
		string text = ((TextBox)(object)EmailBox).Text;
		string password = PasswordBox.Password;
		if (ValidateCredentialsAsync(text, password))
		{
			UpdateINI.WriteToConfig("Auth", "Email", text);
			UpdateINI.WriteToConfig("Auth", "Password", password);
			new MainWindow().Show();
			Close();
		}
		else
		{
			UpdateINI.WriteToConfig("Auth", "Email", text);
			UpdateINI.WriteToConfig("Auth", "Password", password);
			MessageBox.Show("Incorrect E-Mail or Password");
		}
	}


	private void Button_Click_4(object sender, RoutedEventArgs e)
	{
		Close();
	}

	private void Button_Click_2(object sender, RoutedEventArgs e)
	{
		base.WindowState = WindowState.Minimized;
	}

	private void Hyperlink_Click(object sender, RoutedEventArgs e)
	{
		string fileName = "https://discord.gg/moralfn";
		Process.Start(new ProcessStartInfo
		{
			UseShellExecute = true,
			FileName = fileName
		});
	}
