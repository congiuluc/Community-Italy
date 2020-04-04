namespace CommunityItaly.Server.Utilities
{
    public class UploadTasks
    {
        public const string ResizeImagePeople = nameof(ResizeImagePeople);
        public const string ResizeImageCommunity = nameof(ResizeImageCommunity);
        public const string ResizeImageEvent = nameof(ResizeImageEvent);
    }

    public class ConfirmationTask
    {
        public const string SendMailEvent = nameof(SendMailEvent);
        public const string CreateEvent = nameof(CreateEvent);
        public const string ApproveFromHttpEvent = nameof(ApproveFromHttpEvent);
        public const string ConfirmEvent_Http = nameof(ConfirmEvent_Http);
        public const string ConfirmEventHuman = nameof(ConfirmEventHuman);
        public const string ConfirmOrchestratorEvent = nameof(ConfirmOrchestratorEvent);
        public const string ApproveCancelEventOnCosmos = nameof(ApproveCancelEventOnCosmos);


        public const string SendMailCommunity = nameof(SendMailCommunity);
        public const string CreateCommunity = nameof(CreateCommunity);
        public const string ApproveFromHttpCommunity = nameof(ApproveFromHttpCommunity);
        public const string ConfirmCommunity_Http = nameof(ConfirmCommunity_Http);
        public const string ConfirmCommunityHuman = nameof(ConfirmCommunityHuman);
        public const string ConfirmOrchestratorCommunity = nameof(ConfirmOrchestratorCommunity);
        public const string ApproveCancelCommunityOnCosmos = nameof(ApproveCancelCommunityOnCosmos);


        public const string SendMailPerson = nameof(SendMailPerson);
        public const string CreatePerson = nameof(CreatePerson);
        public const string ApproveFromHttpPerson = nameof(ApproveFromHttpPerson);
        public const string ConfirmPerson_Http = nameof(ConfirmPerson_Http);
        public const string ConfirmPersonHuman = nameof(ConfirmPersonHuman);
        public const string ConfirmOrchestratorPerson = nameof(ConfirmOrchestratorPerson);
        public const string ApproveCancelPersonOnCosmos = nameof(ApproveCancelPersonOnCosmos);


        public const string SendMailArticle = nameof(SendMailArticle);
        public const string CreateArticle = nameof(CreateArticle);
        public const string ApproveFromHttpArticle = nameof(ApproveFromHttpArticle);
        public const string ConfirmArticle_Http = nameof(ConfirmArticle_Http);
        public const string ConfirmArticleHuman = nameof(ConfirmArticleHuman);
        public const string ConfirmOrchestratorArticle = nameof(ConfirmOrchestratorArticle);
        public const string ApproveCancelArticleOnCosmos = nameof(ApproveCancelArticleOnCosmos);
    }
}
