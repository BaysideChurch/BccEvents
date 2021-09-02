using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

using Rock.Model;
using Rock.Data;
using Rock.Rest;

namespace com.baysideonline.BccWeb.Events
{
    public class BccEventController : ApiControllerBase
    {
        [HttpGet]
        [System.Web.Http.Route("api/bccweb/events")]
        public IEnumerable<BccEvent> GetEventsInTimeFrame([FromUri] DateTime start, [FromUri] DateTime end, [FromUri] int? campusId = null)
        {
            //var eventOccurrenceService = new EventItemOccurrenceService(context);
            //var eventCalendar = new EventCalendarService(context).Get(Guid.Empty);
            DateTime start_ = DateTime.Now;
            DateTime end_ = DateTime.Now.AddDays(30);
            //int? campusId_ = null;

            using (var context = new RockContext())
            {
                var eventOccurrenceService = new EventItemOccurrenceService(context);
                var eventCalendar = new EventCalendarService(context).Get(Guid.Empty);

                var qry = eventOccurrenceService
                        .Queryable("EventItem, Schedule");
                /*.Where(m => m.EventItem.EventCalendarItems.Any(i => i.EventCalendarId == eventCalendar.Id) &&
                    m.EventItem.IsActive &&
                    m.EventItem.IsApproved);*/
                Console.WriteLine("");

                if (campusId.HasValue)
                {
                    var campus = new CampusService(context).Get(campusId.Value);
                    if (campus != null)
                    {
                        //qry equals qry where the EventItemOccurrence's CampusId doesn't have a value, or where the campusId is equal to campus.Id
                        qry = qry.Where(x => !x.CampusId.HasValue || x.CampusId == campus.Id);
                    }
                }

                //Pagination:
                //.Skip(()=> skip).Take(() => take) where skip and take are ints.

                var occurrences = qry.ToList()
                    .SelectMany(x =>
                    {
                        //return 0 if Schedule == null or Schedule.DurationInMinutes == null
                        //  otherwise return Schedule.DurationInMinutes
                        var duration = x.Schedule?.DurationInMinutes ?? 0;

                        //return the start times
                        //where the datetime in [start, end)
                        // and return an anonymous type with Date, Duration, and the EventItemOccurrence
                        return x.GetStartTimes(start, end)
                             .Where(dt => dt >= start && dt < end)
                             .Select(b => new
                             {
                                 Date = (DateTimeOffset)b,
                                 Duration = duration,
                                 EventItemOccurrence = x
                             });
                    })
                    .Select(x => new BccEvent
                    {
                        Name = x.EventItemOccurrence.EventItem.Name,
                        Summary = x.EventItemOccurrence.EventItem.Summary,
                        Description = x.EventItemOccurrence.EventItem.Description,
                        Note = x.EventItemOccurrence.Note,
                        Location = x.EventItemOccurrence.Location,
                        CampusId = x.EventItemOccurrence.CampusId.GetValueOrDefault(),
                        StartTime = x.Date,
                        EndTime = x.Duration > 0 ? (DateTimeOffset?)x.Date.AddMinutes(x.Duration) : null,
                        Schedule = x.EventItemOccurrence.Schedule,
                        Linkages = x.EventItemOccurrence.Linkages.ToList()
                    }).ToList();

                //string json = JsonConvert.SerializeObject(occurrences);
                //List<BccEvent> jsonDeserialize = JsonConvert.DeserializeObject<List<BccEvent>>(json);
                Console.WriteLine("");
                return occurrences;
            }
        }
    }
}
