using CutOutWiz.Services.HR;
using CutOutWiz.Services.Models.HR;
using KowToMate.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace KowToMate.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DesignationController : ControllerBase
    {

        #region Private Members     
        public readonly IDesignationService _designationService;
        #endregion

        #region Ctor
        public DesignationController(IDesignationService designationService)
        {
            _designationService = designationService;
        }
        #endregion

        #region APIs
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var response = new ApiGenericResponse<List<DesignationModel>>
            {
                IsSuccess = false
            };

            var designations = await _designationService.GetAll();

            if (!designations.Any())
            {
                response.Notification = "There are no designation to show.";
                return Ok(response);
            }

            response.Result = designations;
            response.IsSuccess = true;
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var response = new ApiGenericResponse<DesignationModel>
            {
                IsSuccess = false
            };

            var designation = await _designationService.GetById(id);
            if (designation == null)
            {
                response.Notification = "Designation not found. Please try again.";
                return Ok(response);
            }

            response.Result = designation;

            response.IsSuccess = true;
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string objectId)
        {
            var response = new ApiBaseResponse
            {
                IsSuccess = false
            };

            var deleteResposne = await _designationService.Delete(objectId);

            if (!deleteResposne.IsSuccess)
            {
                response.Notification = "Designation not found. Please try again.";
                return Ok(response);
            }

            response.IsSuccess = true;
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Create(DesignationModel designation)
        {
            var response = new ApiGenericResponse<int>
            {
                IsSuccess = false
            };
            
            var addResponse = await _designationService.Insert(designation);

            if(!addResponse.IsSuccess)
            {
                response.Notification = addResponse.Message;
                return Ok(response);
            }

            response.Result = addResponse.Result;
            response.IsSuccess = true;
            return Ok(response);
        }

        [HttpPut]
        public async Task<IActionResult> Update(DesignationModel designation)
        {
            var response = new ApiBaseResponse
            {
                IsSuccess = false
            };

            var updateResponse = await _designationService.Update(designation);

            if (!updateResponse.IsSuccess)
            {
                response.Notification = updateResponse.Message;
                return Ok(response);
            }

            response.IsSuccess = true;
            return Ok(response);
        }
        #endregion
    }
}
