using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdamCode.Advertisement.ViewModels
{
    public class ListAdvertisementsViewModel
    {
        public int? Page { get; set; }
        [BindNever]
        public List<dynamic> ContentItems { get; set; }
        [BindNever]
        public dynamic Pager { get; set; }

        [BindNever]
        public dynamic Header { get; set; }

        public IEnumerable<SelectListItem> Categories { get; set; }

        public bool MyAdvertFilter { get; set; }
        public string CurrentCategory { get; set; }
    }
}
