using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Threading.Tasks;

namespace Translator
{
    public partial class MainWindow
    {
        private readonly XDocument cache;
        private readonly XElement root;
        private readonly Dictionary<string, string> languages;

        public MainWindow()
        {
            InitializeComponent();
            cache = new XDocument();
            cache = XDocument.Load(@"..\Debug\Translations.xml"); // it might get moved to another place during app work
            root = cache.Element("root");
            cb.SelectedIndex = 0;
            cb1.SelectedIndex = 1;
            languages = new Dictionary<string, string>
            {
                {"english","en"},
                {"russian","ru"},
            };
        }
    

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ButtonClicked();
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            DragMove();
        }

        private void ExitButtonClicked(object sender, RoutedEventArgs e)
        {
            App.Current.Shutdown();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// You also should consider using reader to retrieve values from cache
        /// </summary>
        private string RetrieveValuesFromCache(string value, string name)
        {
            try
            {
                return root.Descendants()
                      .Single(x => x.Value == value)
                      .GetAllSiblings()
                      .Single(x => x.Name == name)
                      .Value;
            }
            catch (NullReferenceException)
            {
                return String.Empty;
            }
            catch (InvalidOperationException)
            {
                return String.Empty;
            }
        }

        private void SaveToCache()
        {
            XElement data = new XElement(
                "definition",
                new XElement("en"),
                new XElement("ru"));

            foreach (ComboBox combobox in Extensions.FindVisualChildren<ComboBox>(this))
            {
                foreach (var element in data.Elements())
                {
                    if (element.Name.ToString().StartsWith(combobox.Text))
                    {
                        var requieredTextbox = Extensions.FindUid(this, combobox.Uid) as TextBox;
                        element.Value = requieredTextbox.Text;
                    }
                }
            }

            root.Add(data);
            cache.Save("Translations.xml");
        }

        /// <summary>
        /// Gotta make it as a hierarchy(starting from the most popular language)
        /// </summary>
        private string IdentifyLanguage(string textToIdentify)
        {
            var matches = Regex.Matches(textToIdentify, Settings.EndlishPattern);
            if (matches.Count.Equals(textToIdentify.Length))
            {
                return languages["english"];
            }
            else
            {
                matches = Regex.Matches(textToIdentify, Settings.RussianPattern);
                if (matches.Count.Equals(textToIdentify.Length))
                {
                    return languages["russian"];
                }
            }
            return null;
        }

        private bool DoesLanguageDiffers()
        {
           return ((cb.Text != cb1.Text) && (tb.Text != String.Empty));
        }

        private async void ButtonClicked()
        {
            if (DoesLanguageDiffers() && IdentifyLanguage(tb.Text) == cb.Text)
            {
                tb1.Text = (RetrieveValuesFromCache(tb.Text, cb1.Text) == null) ? string.Empty : RetrieveValuesFromCache(tb.Text, cb1.Text);
                if (tb1.Text == String.Empty)
                {
                    var lang = String.Format($"{cb.Text}-{cb1.Text}");
                    tb1.Text = await GetResponse(tb.Text, lang);
                    SaveToCache();
                }
            }
            else
            {
                tb1.Text = tb.Text;
            }           
        }

        private Task<string> GetResponse(string text, string lang)
        {
            return Task.Run(() =>
            {
                WebRequest request = WebRequest.Create("https://translate.yandex.net/api/v1.5/tr/translate?" +
                                                        Settings.APIKey +
                                                       "&ui=ru" +
                                                       "&text=" + text.ToLower() +
                                                       "&lang=" + lang);

                 return request.GetResponse()?.GetTranslation();
            });
        }


    }
}
