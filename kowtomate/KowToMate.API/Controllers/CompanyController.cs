using CutOutWiz.Data.Common;
using CutOutWiz.Services.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KowToMate.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        public ICompanyService companyService { get; set; }

        public CompanyController(ICompanyService companyService)
        {
            this.companyService = companyService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var companys = await companyService.GetAll();
                return Ok(companys);
            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var company = await companyService.GetById(id);
                return Ok(company);
            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await companyService.Delete(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(Company company)
        {
            try
            {
                await companyService.Insert(company);
                return Ok(company);
            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update(Company company)
        {
            try
            {
                await companyService.Update(company);
                return Ok(company);
            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }
    }
}
