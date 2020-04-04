using System;
using System.Collections.Generic;

namespace CommunityItaly.Shared.ViewModels
{
	public class ArticleViewModel
	{
        public string Name { get; set; }
        public Uri Url { get; set; }
        public DateTime PublishDate { get; set; }
        public IEnumerable<PersonViewModelReadOnly> Authors { get; set; } = new List<PersonViewModelReadOnly>();
    }

	public class ArticleUpdateViewModel : ArticleViewModel
    {
        public string Id { get; set; }
        public bool Confirmed { get; set; }

        public static ArticleUpdateViewModel Create(ArticleViewModel vm)
        {
            return new ArticleUpdateViewModel
            {
                Url = vm.Url,
                PublishDate = vm.PublishDate,
                Authors = vm.Authors,
            };
        }
    }
}
