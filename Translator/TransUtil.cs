using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Xml.Linq;

namespace Translator
{
    internal static class LINQExtensions
    {
        public static IEnumerable<T> GetAllSiblings<T>(this T element) where T : XElement
        {
            return (IEnumerable<T>) element.ElementsAfterSelf()
                                           .Concat(element.ElementsBeforeSelf());
        }

        public static XElement GetSiblingElementByName<T>(this T collection, string name)
            where T : IEnumerable<XElement>
        {
            return collection
                   .Single(x => x.Name == name);
        }      
    }
}
