using System;
using System.Collections.Generic;
using System.IO;

namespace JDP {
	public static class Settings {
		private static Dictionary<string, string> _settings;

		static Settings() {
			Load();
		}

		public static string SettingsDirectory {
			get {
				string dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), General.ApplicationName);
				if (!Directory.Exists(dir)) {
					Directory.CreateDirectory(dir);
				}
				return dir;
			}
		}

		public static string SettingsFileName {
			get { return "settings.txt"; }
		}

		public static string SettingsPath {
			get { return Path.Combine(SettingsDirectory, SettingsFileName); }
		}

		public static string CaptchasFileName {
			get { return "captchas.txt"; }
		}

		public static string CaptchasPath {
			get { return Path.Combine(SettingsDirectory, CaptchasFileName); }
		}

		public static string ImageFolder {
			get { return Get("ImageFolder"); }
			set { Set("ImageFolder", value); }
		}

		public static string ThreadURL {
			get { return Get("ThreadURL"); }
			set { Set("ThreadURL", value); }
		}

		public static int? PostInterval {
			get { return GetInt("PostInterval"); }
			set { SetInt("PostInterval", value); }
		}

		public static bool? NumberPosts {
			get { return GetBool("NumberPosts"); }
			set { SetBool("NumberPosts", value); }
		}

		public static int? PostNumberStart {
			get { return GetInt("PostNumberStart"); }
			set { SetInt("PostNumberStart", value); }
		}

		public static bool? RandomizeOrder {
			get { return GetBool("RandomizeOrder"); }
			set { SetBool("RandomizeOrder", value); }
		}

		public static bool? MovePostedToSubfolder {
			get { return GetBool("MovePostedToSubfolder"); }
			set { SetBool("MovePostedToSubfolder", value); }
		}

		public static string UserName {
			get { return Get("UserName"); }
			set { Set("UserName", value); }
		}

		public static string EmailAddress {
			get { return Get("EmailAddress"); }
			set { Set("EmailAddress", value); }
		}

		public static string Password {
			get { return Get("Password"); }
			set { Set("Password", value); }
		}

		public static string ChanPassToken {
			get { return Get("ChanPassToken"); }
			set { Set("ChanPassToken", value); }
		}

		public static string ChanPassPIN {
			get { return Get("ChanPassPIN"); }
			set { Set("ChanPassPIN", value); }
		}

		private static string Get(string name) {
			lock (_settings) {
				string value;
				return _settings.TryGetValue(name, out value) ? value : null;
			}
		}

		private static bool? GetBool(string name) {
			string value = Get(name);
			if (value == null) return null;
			return value == "1";
		}

		private static int? GetInt(string name) {
			string value = Get(name);
			if (value == null) return null;
			int x;
			return Int32.TryParse(value, out x) ? x : (int?)null;
		}

		private static void Set(string name, string value) {
			lock (_settings) {
				if (value == null) {
					_settings.Remove(name);
				}
				else {
					_settings[name] = value;
				}
			}
		}

		private static void SetBool(string name, bool? value) {
			Set(name, value.HasValue ? (value.Value ? "1" : "0") : null);
		}

		private static void SetInt(string name, int? value) {
			Set(name, value.HasValue ? value.Value.ToString() : null);
		}

		private static void Load() {
			string path = SettingsPath;

			_settings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

			if (!File.Exists(path)) {
				return;
			}

			using (StreamReader sr = File.OpenText(path)) {
				string line;

				while ((line = sr.ReadLine()) != null) {
					int pos = line.IndexOf('=');

					if (pos != -1) {
						string name = line.Substring(0, pos);
						string value = line.Substring(pos + 1);

						if (!_settings.ContainsKey(name)) {
							_settings.Add(name, value);
						}
					}
				}
			}
		}

		public static void Save() {
			using (StreamWriter sw = File.CreateText(SettingsPath)) {
				lock (_settings) {
					foreach (KeyValuePair<string, string> item in _settings) {
						sw.WriteLine(item.Key + "=" + item.Value);
					}
				}
			}
		}
	}
}
