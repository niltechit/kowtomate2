using CutOutWiz.Core;
using CutOutWiz.Services.Models.DynamicReports;
using CutOutWiz.Services.Models.UI;
using CutOutWiz.Services.DbAccess;
using static CutOutWiz.Core.Utilities.Enums;
using CutOutWiz.Data;

namespace CutOutWiz.Services.UI
{
    public class GridViewSetupService : IGridViewSetupService
    {
        private readonly ISqlDataAccess _db;

        public GridViewSetupService(ISqlDataAccess db)
        {
            _db = db;
        }

        /// <summary>
        /// Get List by Contact Id
        /// </summary>
        /// <returns></returns>
        public async Task<List<GridViewSetupModel>> GetListByContactId(GridViewFor gridViewFor, int contactId)
        {
            return await _db.LoadDataUsingProcedure<GridViewSetupModel, dynamic>(storedProcedure: "dbo.SP_UI_GridViewSetup_GetListByContactId",
                new
                {
                    GridViewFor = (byte)gridViewFor,
                    ContactId = contactId
                });
        }

        /// <summary>
        /// Get List by dynamicReportInfoId and contact id
        /// </summary>
        /// <returns></returns>
        public async Task<List<GridViewSetupModel>> GetListByDynamicReportIdContactId(int dynamicReportInfoId, int contactId)
        {
            var query = @"SELECT s.[Id]
                  ,s.[ContactId]
                  ,s.[Name]
	              ,(CASE WHEN s.ContactId = @ContactId THEN s.[Name] + ' (Own)' 	  	  
	              ELSE s.[Name] + ' ('+c.FirstName+')' 	 END )  DisplayName
	              ,s.[GridViewFor]
                  ,s.[OrderByColumn]
                  ,s.[OrderDirection]
                  ,s.[IsDefault]
	              ,s.[IsPublic]
                  ,s.[CreatedDate]
                  ,s.[CreatedByContactId]
                  ,s.[UpdatedDate]
                  ,s.[UpdatedByContactId]
                  ,s.[ObjectId]
	              ,s.[ViewStateJson]
              FROM [dbo].[UI_GridViewSetup] s
              INNER JOIN [dbo].Security_Contact c on c.Id = s.ContactId
              WHERE s.DynamicReportInfoId = @DynamicReportInfoId 
              AND (s.ContactId = @ContactId OR s.IsPublic = 1)
              ORDER BY s.[Name]";

            return await _db.LoadDataUsingQuery<GridViewSetupModel, dynamic>(query,
            
            new
            {
                DynamicReportInfoId = dynamicReportInfoId,
                ContactId = contactId
            });
        }
       
        /// <summary>
        /// Get Columns List By Primary Id
        /// </summary>
        /// <param name="permission"></param>
        /// <returns></returns>
        public async Task<List<GridViewSetupColumnSlim>> GetColumnsByGridViewSetupId(int gridViewSetupId)
        {
            var query = "SELECT g.ColoumnId,g.DisplayOrder, g.Width FROM [dbo].UI_GridViewSetupColumn g WHERE GridViewSetupId = @gridViewSetupId";
            return await _db.LoadDataUsingQuery<GridViewSetupColumnSlim, dynamic>(query, new { gridViewSetupId = gridViewSetupId });
        }

        /// <summary>
        /// Insert gridViewSetup
        /// </summary>
        /// <param name="gridViewSetup"></param>
        /// <returns></returns>
        public async Task<Response<int>> Insert(GridViewSetupModel gridViewSetup)
        {
            gridViewSetup.CreatedDate = DateTime.UtcNow.AddHours(-5);
            gridViewSetup.ObjectId = Guid.NewGuid().ToString();

            var response = new Response<int>();

            try
            {
                var newId = await _db.SaveDataUsingProcedureAndReturnId<short, dynamic>(storedProcedure: "dbo.SP_UI_GridViewSetup_Insert", new
                {
                    gridViewSetup.ContactId,
                    gridViewSetup.Name,
                    gridViewSetup.GridViewFor,
                    gridViewSetup.OrderByColumn,
                    gridViewSetup.OrderDirection,
                    gridViewSetup.IsDefault,
                    gridViewSetup.IsPublic,
                    CreatedDate = DateTime.UtcNow.AddHours(-5),
                    gridViewSetup.CreatedByContactId,
                    ObjectId = Guid.NewGuid().ToString(),
                    gridViewSetup.ViewStateJson,
                    gridViewSetup.DynamicReportInfoId
                });

                response.Result = newId;
                response.IsSuccess = true;
                response.Message = StandardDataAccessMessages.SuccessMessaage;

            }
            catch (Exception ex)
            {
                response.Message = StandardDataAccessMessages.GetSqlErrorMessage(ex);
            }

            return response;
        }

        /// <summary>
        /// Update GridViewSetup
        /// </summary>
        /// <param name="gridViewSetup"></param>
        /// <returns></returns>
        public async Task<Response<int>> Update(GridViewSetupModel gridViewSetup)
        {
            var response = new Response<int>();

            try
            {
                await _db.SaveDataUsingProcedure(storedProcedure: "dbo.SP_UI_GridViewSetup_Update", new
                {
                    gridViewSetup.Name,
                    gridViewSetup.GridViewFor,
                    gridViewSetup.OrderByColumn,
                    gridViewSetup.OrderDirection,
                    gridViewSetup.IsDefault,
                    gridViewSetup.IsPublic,
                    UpdatedDate = DateTime.UtcNow.AddHours(-5),
                    gridViewSetup.UpdatedByContactId,
                    gridViewSetup.ObjectId,
                    gridViewSetup.ViewStateJson,
                    gridViewSetup.DynamicReportInfoId
                });

                response.IsSuccess = true;
                response.Result = gridViewSetup.Id;
                response.Message = StandardDataAccessMessages.SuccessMessaage;
            }
            catch (Exception ex)
            {
                response.Message = StandardDataAccessMessages.GetSqlErrorMessage(ex);
            }

            return response;
        }

        /// <summary>
        /// Delete GridViewSetup by id
        /// </summary>
        /// <param name="gridViewSetupId"></param>
        /// <returns></returns>
        public async Task<Response<bool>> Delete(string objectId)
        {
            var response = new Response<bool>();

            try
            {
                await _db.SaveDataUsingProcedure(storedProcedure: "dbo.SP_UI_GridViewSetup_Delete", new { ObjectId = objectId });
                response.IsSuccess = true;
                response.Message = StandardDataAccessMessages.SuccessMessaage;
            }
            catch (Exception ex)
            {
                response.Message = StandardDataAccessMessages.GetSqlErrorMessage(ex);
            }

            return response;
        }


        /// <summary>
        /// Insert module permissions by permission id
        /// </summary>
        /// <param name="permission"></param>
        /// <returns></returns>
        public async Task<Response<bool>> TemplateColumnsInsertOrUpdateBySetupId(int gridViewSetupId, List<GridViewSetupColumnSlim> columns)
        {
            var response = new Response<bool>();
            if (columns == null || !columns.Any())
            {
                return response;
            }
            //string columnsIds = "";

            //if (columns != null)
            //{
            //    columnsIds = string.Join(",", columns);
            //}

            //try
            //{
            //    await _db.SaveDataUsingProcedure(storedProcedure: "dbo.SP_UI_GridViewSetupColumn_InsertOrUpdateBySetupId", new
            //    {
            //        GridViewSetupId = gridViewSetupId,
            //        ColumnIds = columnsIds
            //    });

            //    response.IsSuccess = true;
            //    response.Message = StandardDataAccessMessages.SuccessMessaage;
            //}
            //catch (Exception ex)
            //{
            //    response.Message = StandardDataAccessMessages.GetSqlErrorMessage(ex);
            //}


            try
            {
                string query = string.Empty;

                query = $"DELETE FROM UI_GridViewSetupColumn WHERE GridViewSetupId = {gridViewSetupId} {Environment.NewLine}";

                query = $"{query} INSERT INTO dbo.UI_GridViewSetupColumn (GridViewSetupId,ColoumnId,DisplayOrder,Width) VALUES ";

                string values = string.Empty;
                string seperator = "";

                foreach (var colum in columns)
                {
                    values = $"{values}{seperator}({gridViewSetupId},{colum.ColoumnId},{colum.DisplayOrder},{colum.Width})";
                    seperator = ",";
                }

                query = $"{query} {values};";

                await _db.SaveDataUsingQuery(query, new
                {
                });

                response.IsSuccess = true;
                response.Message = StandardDataAccessMessages.SuccessMessaage;
            }
            catch (Exception ex)
            {
                response.Message = StandardDataAccessMessages.GetSqlErrorMessage(ex);
            }

            return response;
        }

        #region Grid View Filter
        /// <summary>
        /// Insert grid View Filter
        /// </summary>
        /// <param name="gridVewFilter"></param>
        /// <returns></returns>
        public async Task<Response<int>> GridViewFilterInsert(GridViewFilterModel gridVewFilter)
        {
            var response = new Response<int>();
            try
            {
                var newId = await _db.SaveDataUsingProcedureAndReturnId<short, dynamic>(storedProcedure: "dbo.SP_UI_GridViewFilter_Insert", new
                {
                    gridVewFilter.GridViewSetupId,
                    gridVewFilter.ContactId,
                    gridVewFilter.Name,
                    gridVewFilter.FilterJson,
                    gridVewFilter.IsDefault,
                    gridVewFilter.IsPublic,
                    gridVewFilter.LogicalOperator,
                    gridVewFilter.SortColumn,
                    gridVewFilter.SortOrder,
                    UpdatedDate = DateTime.UtcNow.AddHours(-5)
                });

                response.Result = newId;
                response.IsSuccess = true;
                response.Message = StandardDataAccessMessages.SuccessMessaage;

            }
            catch (Exception ex)
            {
                response.Message = StandardDataAccessMessages.GetSqlErrorMessage(ex);
            }

            return response;
        }

        /// <summary>
        /// Update GridViewSetup
        /// </summary>
        /// <param name="gridViewSetup"></param>
        /// <returns></returns>
        public async Task<Response<int>> GridViewFilterUpdate(GridViewFilterModel gridVewFilter)
        {
            var response = new Response<int>();

            try
            {
                await _db.SaveDataUsingProcedure(storedProcedure: "dbo.SP_UI_GridViewFilter_Update", new
                {
                    gridVewFilter.Id,
                    gridVewFilter.GridViewSetupId,
                    gridVewFilter.Name,
                    gridVewFilter.FilterJson,
                    gridVewFilter.IsDefault,
                    gridVewFilter.IsPublic,
                    gridVewFilter.LogicalOperator,
                    gridVewFilter.SortColumn,
                    gridVewFilter.SortOrder,
                    UpdatedDate = DateTime.UtcNow.AddHours(-5)
                });

                response.IsSuccess = true;
                response.Result = gridVewFilter.Id;
                response.Message = StandardDataAccessMessages.SuccessMessaage;
            }
            catch (Exception ex)
            {
                response.Message = StandardDataAccessMessages.GetSqlErrorMessage(ex);
            }

            return response;
        }

        /// <summary>
        /// Get gridVewFilter by Id
        /// </summary>
        /// <param name="FileServerId"></param>
        /// <returns></returns>
        public async Task<GridViewFilterModel> GetById(int gridVewFilterId)
        {
            var result = await _db.LoadDataUsingProcedure<GridViewFilterModel, dynamic>(storedProcedure: "dbo.SP_UI_GridViewFilter_GetById",
                new { GridVewFilterId = gridVewFilterId });
            return result.FirstOrDefault();
        }

        /// <summary>
        /// Get GridVewFilter List by gridViewSetupId
        /// </summary>
        /// <returns></returns>
        public async Task<List<GridViewFilterModel>> GetGridViewFiltersBySetupId(int contactId, int gridViewSetupId)
        {
            return await _db.LoadDataUsingProcedure<GridViewFilterModel, dynamic>(storedProcedure: "dbo.SP_UI_GridViewFilter_GetListByGridViewSetupId",
                new
                {
                    GridViewSetupId = gridViewSetupId,
                    ContactId = contactId
                });
        }

        /// <summary>
        /// Delete GridViewSetup by id
        /// </summary>
        /// <param name="gridViewSetupId"></param>
        /// <returns></returns>
        public async Task<Response<bool>> GridViewFilterDelete(int gridViewFilterId)
        {
            var response = new Response<bool>();

            try
            {
                await _db.SaveDataUsingProcedure(storedProcedure: "dbo.SP_UI_GridViewFilter_Delete", new { GridVewFilterId = gridViewFilterId });
                response.IsSuccess = true;
                response.Message = StandardDataAccessMessages.SuccessMessaage;
            }
            catch (Exception ex)
            {
                response.Message = StandardDataAccessMessages.GetSqlErrorMessage(ex);
            }

            return response;
        }

        #endregion
    }
}
