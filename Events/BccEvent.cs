using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rock.Model;
using Newtonsoft.Json;

namespace com.baysideonline.BccWeb.Events
{
    public class BccEvent
    {
        /// <summary>
        /// <see cref="Rock.Model.EventItem"/>
        /// </summary>
        public string Name { get; set; }

        /// <see cref="Rock.Model.EventItem"/>
        public string Summary { get; set; }

        /// <see cref="Rock.Model.EventItem"/>
        public string Description { get; set; }

        /// <see cref="Rock.Model.EventItemOccurrence"/>
        public string Note { get; set; }

        /// <see cref="Rock.Model.EventItemOccurrence"/>
        public string Location { get; set; }

        /// <see cref="Rock.Model.EventItemOccurrence"/>
        public int? CampusId { get; set; }

        //Parsed iCal data
        public DateTimeOffset StartTime { get; set; }

        //Parsed iCal data
        public DateTimeOffset? EndTime { get; set; }

        /// <see cref="Rock.Model.EventItemOccurrence.Schedule"/>
        public Schedule Schedule { get; set; }

        /// <summary>
        /// <see cref="Rock.Model.EventItemOccurrence.Linkages"/>
        /// </summary>
        public List<EventItemOccurrenceGroupMap> Linkages { get; set; }

        public string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
