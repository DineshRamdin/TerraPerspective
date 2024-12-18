using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Models.Administration
{
   public class ProjectTemplateDTO
    {
        public long Id { get; set; }
        public string ProjectTemplateName { get; set; }
        public List<ProjectTemplateMappingDTO> ProjectTemplateData { get; set; }

    }
    public class ProjectTemplateMappingDTO
    {
        public long Id { get; set; }
        public long ProjectTemplateID { get; set; }
        public string TaskName { get; set; }
        public int Duration { get; set; }
        public int Sequence { get; set; }
        public long? ParentTask { get; set; }

    }

}
