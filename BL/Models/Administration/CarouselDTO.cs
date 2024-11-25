using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Models.Administration
{
    public class CarouselDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public int Duration { get; set; }

        public List<CarouselPosterMappingDTO> PosterData { get; set; }
    }
}
