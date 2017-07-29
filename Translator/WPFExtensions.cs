using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Xml;

namespace Translator
{
    internal static class Extensions
    {
        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }

        public static UIElement FindUid(this DependencyObject parent, string uid)
        {
            var count = VisualTreeHelper.GetChildrenCount(parent);
            if (count == 0) return null;

            for (int i = 0; i < count; i++)
            {
                var el = VisualTreeHelper.GetChild(parent, i) as UIElement;
                if (el == null) continue;

                if (el.Uid == uid) return el;

                el = el.FindUid(uid);
                if (el != null) return el;
            }
            return null;
        }

        public static string GetTranslation(this WebResponse response)
        {
            using (StreamReader sr = new StreamReader(response.GetResponseStream()))
            {
                var nodes = sr.ReadToEnd();
                var doc = new XmlDocument();
                doc.LoadXml(nodes);
                var trans = doc.GetElementsByTagName("text");
                return (trans.Count > 0) ? trans.Item(0).InnerText : String.Empty;
            }
        }
    }
}
