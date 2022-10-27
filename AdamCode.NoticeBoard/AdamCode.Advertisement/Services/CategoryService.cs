using OrchardCore;
using OrchardCore.Taxonomies.Indexing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YesSql;

namespace AdamCode.Advertisement.Services
{
    public class CategoryService
    {
        private readonly IOrchardHelper _orchardHelper;
        private readonly ISession _session;
        public CategoryService(IOrchardHelper orchardHelper, ISession session)
        {
            _orchardHelper = orchardHelper;
            _session = session;
        }

        public async Task<IDictionary<string, string>> GetCategoriesAsync()
        {
            Dictionary<string, string> categories = new Dictionary<string, string>();
            var taxonomyIndexes = await _session.Query<TaxonomyIndex, TaxonomyIndex>().Where(x => x.ContentField == "Category").ListAsync();
            foreach (var taxonomy in taxonomyIndexes)
            {
                var term = await _orchardHelper.GetTaxonomyTermAsync(taxonomy.TaxonomyContentItemId, (string)taxonomy.TermContentItemId);
                categories.TryAdd(term.ContentItemId, term.DisplayText);
            }
            return categories;
        }
    }
}
