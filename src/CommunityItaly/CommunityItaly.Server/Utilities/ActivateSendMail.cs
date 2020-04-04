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

    public class ActivatePersonSendMail :
       ActivateSendMail<PersonViewModelReadOnly>
    {
    }

    public class ActivateArticleSendMail :
      ActivateSendMail<ArticleViewModel>
    {
    }


    public class PersonTemplate
    {
        public string name { get; set; }
        public string surname { get; set; }
    }

    public class MailCommunityTemplateData
    {
        public string confirmurl { get; set; }
        public string aborturl { get; set; }
        public string communityname { get; set; }
        public string communitywebsite { get; set; }
        public List<PersonTemplate> communitymanagers { get; set; }
    }

    public class MailArticleTemplateData
    {
        public string confirmurl { get; set; }
        public string aborturl { get; set; }
        public string articlename { get; set; }
        public string articleurl { get; set; }
        public DateTime articlepublishdate { get; set; }
        public List<PersonTemplate> articleauthors { get; set; }
    }

    public class MailEventTemplateData
    {
        public string confirmurl { get; set; }
        public string aborturl { get; set; }
        public string eventname { get; set; }
        public DateTime eventstartdate { get; set; }
        public DateTime eventenddate { get; set; }
        public string eventbuyticket { get; set; }
        public string eventcfpurl { get; set; }
        public DateTime eventcfpstartdate { get; set; }
        public DateTime eventcfpstartend { get; set; }
        public string eventcommunityname { get; set; }
    }

    public class MailPersonTemplateData
    {
        public string confirmurl { get; set; }
        public string aborturl { get; set; }
        public string personname { get; set; }
        public string personsurname { get; set; }
        public string personmvpcode { get; set; }
    }
}
