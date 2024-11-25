using BL.Models.Administration;
using BL.Models.Common;
using BL.Services.Administration;
using DAL.Context;
using DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UI.Controllers.Common;

namespace UI.Controllers
{
    public class CarouselController : BaseController
    {
        public CarouselService service;
        public CarouselController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> rolemanager, PerspectiveContext Dbcontext) : base(userManager, signInManager, rolemanager, Dbcontext)
        {
            service = new CarouselService();
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Preview(long Id)
        {
            BaseResponseDTO<List<PreviewCarouselPosterMappingDTO>> dt = new BaseResponseDTO<List<PreviewCarouselPosterMappingDTO>>();
            dt.Data = new List<PreviewCarouselPosterMappingDTO>();
            if (Id > 0)
            {
                dt = service.PreviewCarouselByParentId(Id);
            }
            return View(dt.Data);
        }

        [HttpPost]
        public ActionResult<BaseResponseDTO<List<CarouselDTO>>> GetAll()
        {
            try
            {
                BaseResponseDTO<List<CarouselDTO>> dt = new BaseResponseDTO<List<CarouselDTO>>();

                dt = service.GetAll();

                return Ok(dt);
            }
            catch (Exception)
            {
                return BadRequest();
            }


        }

        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<CarouselDTO>>> GetById(long Id)
        {
            try
            {
                BaseResponseDTO<CarouselDTO> dt = new BaseResponseDTO<CarouselDTO>();
                dt.Data = new CarouselDTO();
                if (Id > 0)
                {
                    dt = service.GetById(Id);
                }

                return Ok(dt);
            }
            catch (Exception)
            {
                return BadRequest();
            }


        }


        [HttpPost]
        public ActionResult<BaseResponseDTO<List<DropDown>>> GetAllDropdownValues()
        {
            try
            {
                BaseResponseDTO<List<DropDown>> List = new BaseResponseDTO<List<DropDown>>();
                List = service.GetAllDropDownValues();
                return Ok(List);


            }
            catch (Exception ex)
            {

                return BadRequest();

            }
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<bool>>> CreateUpdate(CarouselDTO dto)
        {
            try
            {
                BaseResponseDTO<bool> dt = new BaseResponseDTO<bool>();

                if (dto.Id == 0)
                {
                    dt = await service.SaveAsync(dto);
                }
                else
                {
                    dt = await service.UpdateAsync(dto);
                }

                return Ok(dt);
            }
            catch (Exception)
            {
                return BadRequest();
            }


        }



        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<List<CarouselPosterMappingDTO>>>> CarouselChildDataByParentId(long Id)
        {
            try
            {
                BaseResponseDTO<List<CarouselPosterMappingDTO>> dt = new BaseResponseDTO<List<CarouselPosterMappingDTO>>();
                dt.Data = new List<CarouselPosterMappingDTO>();
                if (Id > 0)
                {
                    dt = service.CarouselChildDataByParentId(Id);
                }

                return Ok(dt);
            }
            catch (Exception)
            {
                return BadRequest();
            }


        }
        

    }
}
