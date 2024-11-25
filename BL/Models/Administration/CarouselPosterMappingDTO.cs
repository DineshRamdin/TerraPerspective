using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Models.Administration
{
    public class CarouselPosterMappingDTO
    {
        public long Id { get; set; }
        public long CarouselId { get; set; }
        public long PosterId { get; set; }
        public string? PosterImageBase64 { get; set; }
        public string? PosterName { get; set; }

    }

    public class PreviewCarouselPosterMappingDTO
    {
        public long Id { get; set; }
        public long CarouselId { get; set; }
        public long PosterId { get; set; }
        public string? PosterImageBase64 { get; set; }
        public string? PosterName { get; set; }
        public int Duration { get; set; }
    }
}
