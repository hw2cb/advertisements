using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdamCode.Advertisement.Services.Utility
{
    public enum FilterType
    {
        AuthCategoryFilterAdvertisementsCountQuery, //
        PublishedAndCategoryCount, //
        AuthCategoryMyAdvertsFilterCountQuery, //
        AuthMyAdvertsFilterCountQuery,
        PublishedCountQuery, //
        AuthAdvertisementsQuery,

        AuthAdvertisementCountQuery,
        AuthCategoryMyAdvertsFilterQuery,//
        AuthCategoryFilterAdvertisementsQuery,
        AuthMyAdvertsFilterQuery, //
        PublishedAndCategory, //
        PublishedQuery,//
        none
    }
}
