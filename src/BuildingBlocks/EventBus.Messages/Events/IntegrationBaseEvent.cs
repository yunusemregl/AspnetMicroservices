using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Messages.Events
{
    public class IntegrationBaseEvent
    {
        public IntegrationBaseEvent()
        {
            this.Id = Guid.NewGuid();
            this.CreationDate = DateTime.UtcNow;
        }

        public IntegrationBaseEvent(Guid id, DateTime creationDate)
        {
            Id = id;
            CreationDate = creationDate;
        }

        public Guid Id { get; private set; }
        public DateTime CreationDate { get; private set; }

    }
}
