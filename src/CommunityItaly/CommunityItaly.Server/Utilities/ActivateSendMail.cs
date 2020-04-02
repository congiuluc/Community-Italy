using CommunityItaly.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommunityItaly.Server.Utilities
{

    public class ActivateSendMail<T>
        where T : class
    {
        public T Data { get; set; }
        public string InstanceId { get; set; }
    }

    public class ActivateEventSendMail : 
        ActivateSendMail<EventUpdateViewModel>
    {
    }

    public class ActivateCommunitySendMail :
       ActivateSendMail<CommunityUpdateViewModel>
    {
    }
}
