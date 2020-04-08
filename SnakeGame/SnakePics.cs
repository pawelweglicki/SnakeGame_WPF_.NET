using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Windows.Media.Imaging;

namespace SnakeGame
{
	class SnakePics
	{
		public List<string> pics = new List<string>();

		public SnakePics()
		{
			pics.Add("https://pixabay.com/get/50e5d645495bb108feda8460da293276133cdae45b5470_1920.jpg");
			pics.Add("https://pixabay.com/get/57e6d647485baf14f6d1867dda35367b1d39dde15259774a_1920.jpg");
			pics.Add("https://pixabay.com/get/53e4d6414e51b108feda8460da293276133cdae45b5674_1920.jpg");
			pics.Add("https://pixabay.com/get/57e0d4474850a514f6d1867dda35367b1d39dde152597941_1920.jpg");
			pics.Add("https://pixabay.com/get/55e9d24a4c52ad14f6d1867dda35367b1d39dde152587148_1920.jpg");
		}

		public BitmapImage DiplayPic()
		{
			string picUrl = RandomPic();

			return SaveImage(picUrl);

		}


		private BitmapImage SaveImage(string url)
		{
			using(WebClient client = new WebClient())
			{
				Stream stream = client.OpenRead(url);
				Bitmap bitmap = new Bitmap(stream);

				if (bitmap != null)
				{
					bitmap.Save(Directory.GetCurrentDirectory() + @"\snake" + ConfigurationManager.AppSettings["incremental"] + ".bmp", ImageFormat.Bmp);
					int value = int.Parse(ConfigurationManager.AppSettings["incremental"]);
					value++;

					var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
					var settings = configFile.AppSettings.Settings;
					settings["incremental"].Value = value.ToString();
					configFile.Save(ConfigurationSaveMode.Modified);
					ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
				}

				BitmapImage bitmapImage = ToBitmapImage(bitmap);
				stream.Close();

				return bitmapImage;
			}
		}

		private string RandomPic()
		{
			Random rnd = new Random();
			int index = rnd.Next(0, 4);

			return pics[index];
		}

		private BitmapImage ToBitmapImage(Bitmap bitmap)
		{
			using (var memory = new MemoryStream())
			{
				bitmap.Save(memory, ImageFormat.Png);
				memory.Position = 0;

				var bitmapImage = new BitmapImage();
				bitmapImage.BeginInit();
				bitmapImage.StreamSource = memory;
				bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
				bitmapImage.EndInit();
				bitmapImage.Freeze();

				return bitmapImage;
			}
		}
	};

}
