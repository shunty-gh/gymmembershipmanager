using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Shunty.Logging;

namespace Shunty.GymMembershipManager
{
    public class ActivitiesAction : MembershipAction
    {
        private static readonly ILog _log = LogProvider.For<ActivitiesAction>();

        public ActivitiesAction(IConnectionManager manager)
            : base(manager)
        { }

        public async Task<IEnumerable<Activity>> GetActivitiesAsync()
        {
            // Need to call the behaviours URL first
            var response = await Manager.PostResultAsync(Consts.BehavioursUrl, new List<KeyValuePair<string, string>> {
                new KeyValuePair<string, string>("club", Consts.ClubId.ToString())
            });
            _log.TraceFormat("Behaviours response: {BehavioursResponse}", response);

            // Now call the activities url
            var content = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("behaviours", Consts.FitnessClassesBehaviour.ToString()),
                new KeyValuePair<string, string>("bookingType", "0"),
                new KeyValuePair<string, string>("X-Requested-With", "XMLHttpRequest"),
            };
            response = await Manager.PostResultAsync(Consts.ActivitiesUrl, content);
            _log.TraceFormat("Activities response content: {ActivitiesResponse}", response);
            var result = ParseActivitiesText(response);
            return result;
        }

        private IEnumerable<Activity> ParseActivitiesText(string source)
        {
            try
            {
                // The source is an HTML fragment of a load of DIV elements so we need to wrap it in a root element
                var el = XElement.Parse("<activities>" + source + "</activities>");

                var result = el.Elements("div")
                    .Where(x => x.Attributes().Any(a => a.Name == "class" && a.Value.Contains("activityItem")))
                    .Select(x => x.Elements("label").FirstOrDefault())
                    .Select(x => new Activity
                    {
                        Id = int.Parse(x.Element("input").Attribute("value").Value),
                        Title = x.Value.Trim()
                    });
                return result;
            }
            catch (Exception ex)
            {
                _log.ErrorException("Error parsing activities", ex);
                return new List<Activity>();
            }
        }
    }
}
