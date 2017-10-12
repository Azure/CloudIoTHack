using FlySim.Translation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Web.Http;

namespace FlySim.Helpers
{
    public static class TranslationHelper
    {
        public static async Task<string> GetTextTranslationAsync(string content, string languageCode)
        {
            string translatedText = "";

            HttpClient client = new HttpClient();
            
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", Common.CoreConstants.TranslatorTextSubscriptionKey);

            var result = await client.GetAsync(new Uri($"{Common.CoreConstants.TranslatorServicesBaseUrl}v2/Http.svc/Translate?text={Uri.EscapeDataString(content)}&to={languageCode}"));

            var resultDocument = await result.Content.ReadAsStringAsync();

            System.Xml.XmlDocument xTranslation = new System.Xml.XmlDocument();

            xTranslation.LoadXml(resultDocument);

            translatedText = xTranslation.InnerText;

            return translatedText;
        }

        private static async Task<List<string>> GetTextTranslationLanguagesAsync()
        {
            HttpClient client = new HttpClient();
            
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", Common.CoreConstants.TranslatorTextSubscriptionKey);

            var result = await client.GetAsync(new Uri($"{Common.CoreConstants.TranslatorServicesBaseUrl}v2/Http.svc/GetLanguagesForTranslate"));

            var resultDocument = await result.Content.ReadAsStringAsync();

            Windows.Data.Xml.Dom.XmlDocument languageDocument = new Windows.Data.Xml.Dom.XmlDocument();

            languageDocument.LoadXml(resultDocument);

            var translationLanguages = (from node in languageDocument.DocumentElement.ChildNodes
                select node.InnerText).ToList();

            return translationLanguages;
        }
        
        public static async Task<List<LanguageInformation>> GetTextTranslationLanguageNamesAsync()
        {
            List<LanguageInformation> translationLanguages = new List<LanguageInformation>();

            var languageAbbreviations = await Helpers.TranslationHelper.GetTextTranslationLanguagesAsync();
           
            HttpClient client = new HttpClient();
 
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", Common.CoreConstants.TranslatorTextSubscriptionKey);

            var result = await client.GetAsync(new Uri($"{Common.CoreConstants.TranslatorServicesBaseUrl}v1/http.svc/GetLanguageNames"));

            var resultDocument = await result.Content.ReadAsStringAsync();

            var languageNames = resultDocument.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < languageAbbreviations.Count; i++)
            {
                translationLanguages.Add(new LanguageInformation() { DisplayName = languageNames[i], Abbreviation = languageAbbreviations[i] });
            }

            return translationLanguages.OrderBy(o => o.DisplayName).ToList();
        }
    }
}
