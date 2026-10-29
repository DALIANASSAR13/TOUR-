using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourGuide.BLL.ViewModels
{
    public class GuideListViewModel
    {
        public List<GuideCardViewModel> Guides { get; set; } = new();
        public string? Search { get; set; }

    }
}
