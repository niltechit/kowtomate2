using AutoMapper;
using CutOutWiz.Core;
using CutOutWiz.Core.Utilities;
using CutOutWiz.Core;
using CutOutWiz.Services.Models.ClientOrders;
using CutOutWiz.Services.Models.Common;
using CutOutWiz.Core.Management;
using CutOutWiz.Services.Models.OrderAndOrderItemStatusChangeLog;
using CutOutWiz.Services.Models.OrderAssignedImageEditors;
using CutOutWiz.Services.Models.Security;
using CutOutWiz.Services.Models.FileUpload;
using FluentFTP;
using KowToMateAdmin.Helper;
using KowToMateAdmin.Models;
using KowToMateAdmin.Models.Security;
using Microsoft.AspNetCore.Components.Authorization;
using Radzen;
using Radzen.Blazor;
using static CutOutWiz.Core.Utilities.Enums;
using CutOutWiz.Services.Security;

namespace KowToMateAdmin.Pages.OrderImages
{
    public partial class OrderImage
	{
		#region Data Store

		List<ClientOrderItemListModel> orderItems;

		IList<ClientOrderItemListModel> selectedFiles = new List<ClientOrderItemListModel>();
		List<CustomEnumTypes> internalOrderItemStatusList = new List<CustomEnumTypes>();
		List<CustomEnumTypes> fileGroupCustomEnumList = new List<CustomEnumTypes>();
		FileServerModel fileServer = new FileServerModel();
		GenericServices _genericService = new GenericServices();
		List<ContactForDropdown> assignContacts = new List<ContactForDropdown>();
		LoginUserInfoViewModel loginUser = null;
		AuthenticationState authState = null;
		int loginUserTeamId = 0;
		bool isLoading = false;
		bool showImageOnList = false;
		bool showActionColumn = false;
		int totalImageCount;
		bool isOrderItemsAssignToEditorPopupVisible = false;
		ClientOrderItemListModel clientOrderItem = new ClientOrderItemListModel();
		bool isAssignOrderSubmitting = false;
		List<TeamMemberListModel> loginUserTeamMembers = new List<TeamMemberListModel>();
		int selectedTeamMemberIdForAssignFiles = 0;
		bool spinShow = false;
        bool isShowProductionDoneImagePopup = false;
		bool isShowCompletedImagePopup = false;
		bool isShowImagePopup=false;
		bool isSubmitting = false;
		bool isSetOrderItemTypePopupVisible = false;
		int selectedOrderItemGroup = 0;
		bool isShowRawImagePopup = false;
       
        #endregion

        #region Filter
        ClientOrderFilter orderFilter = new ClientOrderFilter();
		RadzenDataFilter<ClientOrderItemListModel> dataFilter;
		byte? filterStatus;
		byte? filterGroup;
		string filterFileName;
		int timeLeftFilter;
		IEnumerable<byte> selectedFilterInternalStatus;
		IEnumerable<int> selectedFilterAssignContacts;
		#endregion

		#region Radzen Table Related
		IEnumerable<int> pageSizeOptions = new int[] { 10, 20, 50, 100, 500 };
		RadzenDataGrid<ClientOrderItemListModel> grid;
		bool allowRowSelectOnRowClick = false;
		CssHelper cssHelper = new CssHelper();
		bool searchWithinCompanyImages = false;
		#endregion
		protected override async Task OnInitializedAsync()
		{
			authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
			loginUser = _workContext.LoginUserInfo;
			var teamMemberServiceResponse = await _teamMemberService.GetTeamIdsByContactId(loginUser.ContactId);

			//TODO: Rakib see aminul vai
			loginUserTeamId = teamMemberServiceResponse.FirstOrDefault();

			foreach (InternalOrderItemStatus item in Enum.GetValues(typeof(InternalOrderItemStatus)))
			{
				internalOrderItemStatusList.Add(new CustomEnumTypes { EnumName = item.ToString(), EnumValue = Convert.ToByte((int)item) });
			}

            foreach (OrderItemFileGroup item in Enum.GetValues(typeof(OrderItemFileGroup)))
            {
                fileGroupCustomEnumList.Add(new CustomEnumTypes { EnumName = item.ToString(), EnumValue = Convert.ToByte((int)item) });
            }

            assignContacts = await _contactManager.GetContactsForDropdownByTeamId(loginUserTeamId);
		}
		async Task LoadData(LoadDataArgs args)
		{
			isLoading = true;
			orderFilter.Top = args.Top ?? 20;
			orderFilter.Skip = args.Skip ?? 0;
			orderFilter.IsCalculateTotal = true;
			orderFilter.Where = await PrepareFilterQuery(args.Filters, dataFilter?.Filters);

			PopulateSortFilterQuery(orderFilter, args.Sorts);


			orderItems = await _clientOrderItemService.GetOrderItemsByFilterWithPaging(orderFilter);
			isLoading = false;

			totalImageCount = orderFilter.TotalImageCount;

		}

		private async Task SearchOnAllImages()
		{
            selectedFiles = new List<ClientOrderItemListModel>();
            searchWithinCompanyImages = !searchWithinCompanyImages;
			await ReloadGrid();
		}

		private void OnRowRender(RowRenderEventArgs<ClientOrderItemListModel> args)
		{
			if (args.Data.ArrivalTime != null)
			{
				var arrivalTimePlus1_4Hours = args.Data.ArrivalTime.Value.AddHours(AutomatedAppConstant.VcDeadLineInHour);
				var timeLeft = arrivalTimePlus1_4Hours - DateTime.Now;
				var minLeft = (int)timeLeft.TotalMinutes;
				if (args.Data.Status < (int)InternalOrderItemStatus.Completed && args.Data.FileGroup == (int)OrderItemFileGroup.Work)
				{

					if (minLeft <= AutomatedAppConstant.VcWarningTime && minLeft > AutomatedAppConstant.VcInDangerTime)
					{
						args.Attributes["class"] = "row-warning";
					}
					else if (minLeft > 0 && minLeft <= AutomatedAppConstant.VcInDangerTime)
					{
						args.Attributes["class"] = "row-warning-danger";
					}
					else if (minLeft <= 0)
					{
						args.Attributes["class"] = "row-failed";
					}
				}
			}

		}

		async Task<string> PrepareFilterQuery(IEnumerable<FilterDescriptor> filters, IEnumerable<CompositeFilterDescriptor> topFilters)
		{
			await Task.Yield();
			string where = string.Empty;
			string and = string.Empty;
			orderFilter.JoinClouse = string.Empty;
			//Set group filter
			var items = string.Empty;
			var query = "";

			try
			{
				if (loginUser.CompanyType == (int)CompanyType.Client)
				{
					where = $"{where}{and} o.[CompanyId] = {loginUser.CompanyId} ";
					and = " AND ";
				}
				else
				{
					if (authState.User.IsInRole(PermissionConstants.Order_ViewAllClientOrders))
					{
						//query = $"LEFT JOIN [dbo].[Order_Assigned_Team] AT WITH(NOLOCK) on AT.OrderId=o.Id left JOIN [dbo].Security_Contact CON WITH(NOLOCK) ON CON.Id=AT.[CreatedBy] left JOIN dbo.Management_Team T WITH(NOLOCK) ON T.Id=AT.TeamId LEFT JOIN Management_TeamMember MT WITH(NOLOCK) ON MT.TeamId=AT.TeamId";
						//where = $"{where}{and} m.ContactId = {loginUser.ContactId} ";
					}
					else if (authState.User.IsInRole(PermissionConstants.Order_ViewAllTeamOrders))
					{
                        //TODO: Review again this
                        where = $"{where}{and} m.ContactId = {loginUser.ContactId} and a.IsItemAssignToTeam=1 ";
                        and = " AND ";


      //                  where = $@"{where}{and}  o.Id IN (
      //              SELECT a.OrderId FROM  [dbo].[Order_Assigned_Team] a WITH(NOLOCK)
      //              INNER JOIN Management_TeamMember m WITH(NOLOCK) ON m.TeamId = a.TeamId
      //              WHERE m.ContactId = {loginUser.ContactId} and a.IsItemAssignToTeam=1) ";
						//and = " AND ";
						orderFilter.JoinClouse = @" Inner JOIN[dbo].[Order_Assigned_Team] a WITH(NOLOCK) on a.orderId = o.Id
                    INNER JOIN Management_TeamMember m WITH(NOLOCK) ON m.TeamId = a.TeamId ";

                    }
					else if (authState.User.IsInRole(PermissionConstants.Order_ViewAllQCOrders))
					{
						//where = $@"{where}{and}  o.Id IN (
      //              SELECT a.OrderId FROM  [dbo].[Order_Assigned_Team] a WITH(NOLOCK)
      //              INNER JOIN Management_TeamMember m WITH(NOLOCK) ON m.TeamId = a.TeamId
      //              WHERE m.ContactId = {loginUser.ContactId} and a.IsItemAssignToTeam=1)";

                        where = $"{where}{and} m.ContactId = {loginUser.ContactId} and a.IsItemAssignToTeam=1 ";
                        orderFilter.JoinClouse = @" Inner JOIN[dbo].[Order_Assigned_Team] a WITH(NOLOCK) on a.orderId = o.Id
                    INNER JOIN Management_TeamMember m WITH(NOLOCK) ON m.TeamId = a.TeamId ";

                        if (!searchWithinCompanyImages) {
							where = where + " AND oi.Status in (9,10,13,14,20)";
							if (!string.IsNullOrWhiteSpace(where))
							{
								where = where + " AND oi.Status in (9,10,13,14,20)";
							}
							else
							{
								where = where + "oi.Status in (9,10,13,14,20)";
							}
                        }

                        and = " AND ";
					}
					else if (authState.User.IsInRole(PermissionConstants.Order_ViewAllAssignedOrders))
					{
						where = $@"{where}{and}";

						if (!searchWithinCompanyImages)
						{
							where = where + $@"ed.AssignContactId = {loginUser.ContactId}";
						}
						else
						{

							//where = $@"{where}{and}  o.Id IN (
       //             SELECT a.OrderId FROM  [dbo].[Order_Assigned_Team] a WITH(NOLOCK)
       //             INNER JOIN Management_TeamMember m WITH(NOLOCK) ON m.TeamId = a.TeamId
       //             WHERE m.ContactId = {loginUser.ContactId} and a.IsItemAssignToTeam=1) ";

                            where = $"{where}{and} m.ContactId = {loginUser.ContactId} and a.IsItemAssignToTeam=1 ";
                            orderFilter.JoinClouse = @" Inner JOIN[dbo].[Order_Assigned_Team] a WITH(NOLOCK) on a.orderId = o.Id
                    INNER JOIN Management_TeamMember m WITH(NOLOCK) ON m.TeamId = a.TeamId ";
                        }

                        and = " AND ";
					}
					else  //Default team orders
					{
						where = $@"{where}{and} ed.AssignContactId = {loginUser.ContactId} ";
						and = " AND ";
					}
				}

				//Status wise Filter
				if (selectedFilterInternalStatus != null && selectedFilterInternalStatus.Any())
				{
					if (selectedFilterInternalStatus.Count() == 1)
					{
						where = $"{where}{and} oi.[Status] = {selectedFilterInternalStatus.FirstOrDefault()} ";
						and = " AND ";
					}
					else //count > 1
					{
						items = GetCommaSeperatedByteItems(selectedFilterInternalStatus);
						where = $"{where}{and} oi.[Status] in ({items})";
						and = " AND ";
					}
				}
				



				//Assign ContactName
				if (selectedFilterAssignContacts != null && selectedFilterAssignContacts.Any())
				{
					if (selectedFilterAssignContacts.Count() == 1)
					{
						where = $"{where}{and} ed.[AssignContactId] = {selectedFilterAssignContacts.FirstOrDefault()} ";
						and = " AND ";
					}
					else //count > 1
					{
						items = GetCommaSeperatedIntItems(selectedFilterAssignContacts);
						where = $"{where}{and}  ed.[AssignContactId] in ({items})";
						and = " AND ";
					}
				}

				if (filters != null && filters.Any())
				{
					foreach (var filterItem in filters)
					{
						if (filterItem.Property == "TimeLeft")
						{

							string filterValue = filterItem.FilterValue?.ToString();
							if (int.TryParse(filterValue, out int intValue))
							{
								if (intValue > 0)
								{
									where = $"{where}{and}  {AutomatedAppConstant.VcDeadLineInHour * 60} - DATEDIFF(MINUTE, oi.ArrivalTime, GETDATE()) <= {intValue} and {AutomatedAppConstant.VcDeadLineInHour * 60} - DATEDIFF(MINUTE, oi.ArrivalTime, GETDATE()) > {0}  and oi.[ArrivalTime] IS NOT NULL and oi.Status < {(int)InternalOrderItemStatus.Completed} ";
								}
								else if (intValue == 0)
								{
									where = $"{where}{and}  {AutomatedAppConstant.VcDeadLineInHour * 60} - DATEDIFF(MINUTE, oi.ArrivalTime, GETDATE()) <= {intValue}  and oi.[ArrivalTime] IS NOT NULL and oi.Status < {(int)InternalOrderItemStatus.Completed}";
								}

							}
						}
						else  // String fields
						{
							PopulateStringFiltersForGridHeader(ref where, ref and, filterItem);
						}


					}
				}

				if (!string.IsNullOrWhiteSpace(where))
					where = $" WHERE {where}";
				if (!string.IsNullOrWhiteSpace(query))
					where = $"{query} {where}";

				if (string.IsNullOrWhiteSpace(where))
					where = $"{where}";
			}

			catch (Exception ex)
			{
				throw;
			}

			return where;
		}


		private void PopulateStringFiltersForGridHeader(ref string where, ref string and, FilterDescriptor filterItem)
		{
			try
			{
				if (!string.IsNullOrEmpty(filterItem.FilterValue?.ToString()))
				{
					if (filterItem.FilterOperator == FilterOperator.Contains)
					{
						if (!string.IsNullOrWhiteSpace(where))
						{
							where = $"{where}{and} {GetActualFieldName(filterItem.Property)} like ''%{filterItem.FilterValue}%''";
						}
						else
						{
							where = $"{GetActualFieldName(filterItem.Property)} like ''%{filterItem.FilterValue}%''";
						}
						
						and = " AND ";
					}
					else if (filterItem.FilterOperator == FilterOperator.DoesNotContain)
					{
						if (!string.IsNullOrWhiteSpace(where))
						{
							where = $"{where}{and} {GetActualFieldName(filterItem.Property)} NOT like ''%{filterItem.FilterValue}%''";
						}
						else
						{
							where = $"{GetActualFieldName(filterItem.Property)} NOT like ''%{filterItem.FilterValue}%''";
						}
					
						and = " AND ";
					}
					else if (filterItem.FilterOperator == FilterOperator.StartsWith)
					{
						if (!string.IsNullOrWhiteSpace(where))
						{
							where = $"{where}{and} {GetActualFieldName(filterItem.Property)} like ''{filterItem.FilterValue}%''";
						}
						else
						{
							where = $"{GetActualFieldName(filterItem.Property)} like ''{filterItem.FilterValue}%''";
						}
						
						and = " AND ";
					}
					else if (filterItem.FilterOperator == FilterOperator.EndsWith)
					{
						if (!string.IsNullOrWhiteSpace(where))
						{
							where = $"{where}{and} {GetActualFieldName(filterItem.Property)}  like ''%{filterItem.FilterValue}''";
						}
						else
						{
							where = $"{GetActualFieldName(filterItem.Property)}  like ''%{filterItem.FilterValue}''";
						}
						
						and = " AND ";
					}
					else if (filterItem.FilterOperator == FilterOperator.Equals)
					{
					    if (!string.IsNullOrWhiteSpace(where))
						{
							where = $"{where}{and} {GetActualFieldName(filterItem.Property)} = ''{filterItem.FilterValue}''";
						}
						else
						{
							where = $"{GetActualFieldName(filterItem.Property)} = ''{filterItem.FilterValue}''";
						}
						and = " AND ";
					}
					else if (filterItem.FilterOperator == FilterOperator.NotEquals)
					{
						if (!string.IsNullOrWhiteSpace(where))
						{
							where = $"{where}{and} {GetActualFieldName(filterItem.Property)} <> ''{filterItem.FilterValue}''";
						}
						else
						{
							where = $"{GetActualFieldName(filterItem.Property)} = ''{filterItem.FilterValue}''";
						}
						
						and = " AND ";
					}
					else if (filterItem.FilterOperator == FilterOperator.LessThan)
					{
						if (!string.IsNullOrWhiteSpace(where))
						{
							where = $"{where}{and} {GetActualFieldName(filterItem.Property)} < ''{filterItem.FilterValue}''";
						}
						else
						{
							where = $"{GetActualFieldName(filterItem.Property)} = ''{filterItem.FilterValue}''";
						}
						and = " AND ";
					}
					else if (filterItem.FilterOperator == FilterOperator.LessThanOrEquals)
					{
						if (!string.IsNullOrWhiteSpace(where))
						{
							where = $"{where}{and} {GetActualFieldName(filterItem.Property)} <= ''{filterItem.FilterValue}''";
						}
						else
						{
							where = $"{GetActualFieldName(filterItem.Property)} = ''{filterItem.FilterValue}''";
						}
						and = " AND ";
					}
					else if (filterItem.FilterOperator == FilterOperator.GreaterThan)
					{
						if (!string.IsNullOrWhiteSpace(where))
						{
							where = $"{where}{and} {GetActualFieldName(filterItem.Property)} > ''{filterItem.FilterValue}''";
						}
						else
						{
							where = $"{GetActualFieldName(filterItem.Property)} = ''{filterItem.FilterValue}''";
						}
						
						and = " AND ";
					}
					else if (filterItem.FilterOperator == FilterOperator.GreaterThanOrEquals)
					{
						if (!string.IsNullOrWhiteSpace(where))
						{
							where = $"{where}{and} {GetActualFieldName(filterItem.Property)} >= ''{filterItem.FilterValue}''";
						}
						else
						{
							where = $"{GetActualFieldName(filterItem.Property)} = ''{filterItem.FilterValue}''";
						}
						
						and = " AND ";
					}
				}
				else
				{
					if (filterItem.FilterOperator == FilterOperator.IsEmpty || filterItem.FilterOperator == FilterOperator.IsNull)
					{
						if (!string.IsNullOrWhiteSpace(where))
						{
							where = $"{where}{and} ({GetActualFieldName(filterItem.Property)} = '''' OR {GetActualFieldName(filterItem.Property)} IS NULL)";

						}
						else
						{
							where = $"({GetActualFieldName(filterItem.Property)} = '''' OR {GetActualFieldName(filterItem.Property)} IS NULL)";
						}
						and = " AND ";
					}
					else if (filterItem.FilterOperator == FilterOperator.IsNotEmpty || filterItem.FilterOperator == FilterOperator.IsNotNull)
					{
						if (!string.IsNullOrWhiteSpace(where))
						{
							where = $"{where}{and} ({GetActualFieldName(filterItem.Property)} <> '''' AND {GetActualFieldName(filterItem.Property)} IS NOT NULL)";
						}
						else
						{
							where = $"({GetActualFieldName(filterItem.Property)} <> '''' AND {GetActualFieldName(filterItem.Property)} IS NOT NULL)";
						}
						and = " AND ";
					}
				}
			}
			catch (Exception ex)
			{
				CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
				{
					PrimaryId = (int)0,
					ActivityLogFor = (int)ActivityLogForConstants.Order,
					loginUser = loginUser,
					ErrorMessage = ex.Message,
					MethodName = "PopulateStringFiltersForGridHeader",
					RazorPage = "OrderList.razor.cs",
					Category = (int)ActivityLogCategory.OrderListError,
				};
				_activityLogCommonMethodService.InsertErrorActivityLog(activity);
				js.DisplayMessage($"{ex.Message}");
			}
		}
		void PopulateSortFilterQuery(ClientOrderFilter filter, IEnumerable<SortDescriptor> sorts)
		{
			try
			{
				string sortClouse = string.Empty;

				//Set group filter
				if (sorts != null && sorts.Any())
				{
					var sortColumn = sorts.FirstOrDefault();
					filter.SortColumn = GetActualFieldName(sortColumn.Property);

					if (sortColumn.SortOrder == SortOrder.Ascending)
					{
						filter.SortDirection = "ASC";
					}
					else
					{
						filter.SortDirection = "DESC";
					}
				}
				else
				{
					filter.SortColumn = "";
					filter.SortDirection = "";
				}
			}
			catch (Exception ex)
			{
				CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
				{
					PrimaryId = (int)0,
					ActivityLogFor = (int)ActivityLogForConstants.Order,
					loginUser = loginUser,
					ErrorMessage = ex.Message,
					MethodName = "PopulateSortFilterQuery",
					RazorPage = "OrderImages.razor.cs",
					Category = (int)ActivityLogCategory.OrderListError,
				};
				_activityLogCommonMethodService.InsertErrorActivityLog(activity);
				//js.DisplayMessage($"{ex.Message}");
			}
		}

		string GetActualFieldName(string uiFieldName)
		{
			try
			{
				if (uiFieldName == "OrderPlaceDate" || uiFieldName == "OrderPlaceDateOnly")
				{
					return "o.[OrderPlaceDate]";
				}
				else if (uiFieldName == "CompanyName")
				{
					return "c.[Name]";
				}
				//else if (uiFieldName == "ContactName")
				//{
				//	return "assignby.[FirstName]";
				//}
				else if (uiFieldName == "TeamName")
				{
					return "T.[Name]";
				}
				else if (uiFieldName == "TeamAssignedDate")
				{
					return "o.[AssignedDateToTeam]";
				}
				else if (uiFieldName == "FileName")
				{
					return "oi.[FileName]";
				}
			}
			catch (Exception ex)
			{
				CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
				{
					PrimaryId = (int)0,
					ActivityLogFor = (int)ActivityLogForConstants.Order,
					loginUser = loginUser,
					ErrorMessage = ex.Message,
					MethodName = "GetActualFieldName",
					RazorPage = "OrderList.razor.cs",
					Category = (int)ActivityLogCategory.OrderListError,
				};
				_activityLogCommonMethodService.InsertErrorActivityLog(activity);
				js.DisplayMessage($"{ex.Message}");
			}

			return $"o.[{uiFieldName}]";
		}

		async Task ShowImagePopup(ClientOrderItemListModel clientOrderItemVM)
		{
			spinShow = true;
			StateHasChanged();
			clientOrderItem = new ClientOrderItemListModel();
			clientOrderItem = clientOrderItemVM;
			isShowRawImagePopup = true;
			this.StateHasChanged();
			spinShow = false;
			StateHasChanged();
		}

		private async Task ViewOrderItemStatusLog(int orderItemId)
		{

		}

		private async Task ViewOrderItemActivityLog(int orderItemId)
		{

		}

		//Drop Down Filter
		async Task OnSelectedInternalOrderStatusChange(object value)
		{
			try
			{
				if (selectedFilterInternalStatus != null && !selectedFilterInternalStatus.Any())
				{
					selectedFilterInternalStatus = null;
				}

				await grid.FirstPage(true);
			}
			catch (Exception ex)
			{
				CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
				{
					PrimaryId = (int)0,
					ActivityLogFor = (int)ActivityLogForConstants.Order,
					loginUser = loginUser,
					ErrorMessage = ex.Message,
					MethodName = "OnSelectedInternalOrderStatusChange",
					RazorPage = "OrderList.razor.cs",
					Category = (int)ActivityLogCategory.OrderListError,
				};
				await _activityLogCommonMethodService.InsertErrorActivityLog(activity);
				await js.DisplayMessage($"{ex.Message}");
			}
		}

		async Task onTimeLeftSearch(object value)
		{
			try
			{
				await grid.FirstPage(true);
			}
			catch (Exception)
			{

				throw;
			}
		}

		async Task OnSelectedContactNameChange(object value)
		{
			try
			{
				if (selectedFilterAssignContacts != null && !selectedFilterAssignContacts.Any())
				{
					selectedFilterAssignContacts = null;
				}

				await grid.FirstPage(true);
			}
			catch (Exception ex)
			{
				CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
				{
					PrimaryId = (int)0,
					ActivityLogFor = (int)ActivityLogForConstants.Order,
					loginUser = loginUser,
					ErrorMessage = ex.Message,
					MethodName = "OnSelectedContactNameChange",
					RazorPage = "OrderList.razor.cs",
					Category = (int)ActivityLogCategory.OrderListError,
				};
				await _activityLogCommonMethodService.InsertErrorActivityLog(activity);
				await js.DisplayMessage($"{ex.Message}");
			}
		}

		string GetCommaSeperatedByteItems(IEnumerable<byte> filterItems)
		{
			var items = "";
			try
			{
				var seperator = "";

				foreach (var item in filterItems)
				{
					items = $"{items}{seperator}{item}";
					seperator = ",";
				}
			}
			catch (Exception ex)
			{
				CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
				{
					PrimaryId = (int)0,
					ActivityLogFor = (int)ActivityLogForConstants.Order,
					loginUser = loginUser,
					ErrorMessage = ex.Message,
					MethodName = "GetCommaSeperatedByteItems",
					RazorPage = "OrderList.razor.cs",
					Category = (int)ActivityLogCategory.OrderListError,
				};
				_activityLogCommonMethodService.InsertErrorActivityLog(activity);
				js.DisplayMessage($"{ex.Message}");
			}
			return items;
		}

		string GetCommaSeperatedIntItems(IEnumerable<int> filterItems)
		{
			var items = "";
			try
			{
				var seperator = "";

				foreach (var item in filterItems)
				{
					items = $"{items}{seperator}{item}";
					seperator = ",";
				}
			}
			catch (Exception ex)
			{
				CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
				{
					PrimaryId = (int)0,
					ActivityLogFor = (int)ActivityLogForConstants.Order,
					loginUser = loginUser,
					ErrorMessage = ex.Message,
					MethodName = "GetCommaSeperatedIntItems",
					RazorPage = "OrderList.razor.cs",
					Category = (int)ActivityLogCategory.OrderListError,
				};
				_activityLogCommonMethodService.InsertErrorActivityLog(activity);
				js.DisplayMessage($"{ex.Message}");
			}
			return items;
		}



		//Assign Order Item TO editor
		private async Task AssignToEditor()
		{
			
			
			await ShowAssignOrderItemsToEditor();

		}

		public async Task ShowAssignOrderItemsToEditor()
		{

			ContactListModel contact = await _teamService.GetByContactId(loginUser.ContactId);
			if (contact != null)
			{
				if (loginUserTeamId > 0)
				{
					loginUserTeamMembers.AddRange(await _teamMemberService.GetTeamMemberListWithDetailsByTeamId(loginUserTeamId));
				}
			}
			isOrderItemsAssignToEditorPopupVisible = true;
		}

		private async Task InsertAssingOrderToEditor()
		{

			//try
			//{
			isAssignOrderSubmitting = true;
			spinShow = true;
			StateHasChanged();

			foreach (var orderItem in selectedFiles)
			{
				if (orderItem.Status > (int)InternalOrderItemStatus.InProduction && orderItem.Status != (int)InternalOrderItemStatus.ReworkDistributed && orderItem.Status != (int)InternalOrderItemStatus.ReworkInProduction)
				{
					spinShow = false;
					StateHasChanged();
					await js.DisplayMessage("Unable to distribute due to Status Protocal Policy");
					CloseAssignOrderItemToEditorPopup();
					return;
				}
			}
			if (selectedTeamMemberIdForAssignFiles <= 0)
			{
				spinShow = false;
				isAssignOrderSubmitting = false;
				StateHasChanged();
				return;
			}
			await Task.Yield();
			List<OrderAssignedImageEditorModel> assignedImages = new List<OrderAssignedImageEditorModel>();
			var clientOrderItemIds = new List<string>();

			var assignclientOrderItems = await LeaveAssignImage();

			if (!assignclientOrderItems.Any())
			{
				spinShow = false;
				isAssignOrderSubmitting = false;
				StateHasChanged();
				await js.DisplayMessage("Unable to distribute due to Status Protocal Policy ");
				CloseAssignOrderItemToEditorPopup();
				
				return;
			}

			List<long> orderIds = new List<long>();

			foreach (var selectedFile in assignclientOrderItems)
			{

				var orderItem = await _clientOrderItemService.GetById((int)selectedFile.Id);

				OrderAssignedImageEditorModel assignedImage = new OrderAssignedImageEditorModel
				{
					OrderId = selectedFile.ClientOrderId,
					AssignByContactId = loginUser.ContactId,
					AssignContactId = selectedTeamMemberIdForAssignFiles,
					Order_ImageId = selectedFile.Id,
					ObjectId = Guid.NewGuid().ToString(),
					UpdatedByContactId = loginUser.ContactId

				};
				assignedImages.Add(assignedImage);

				clientOrderItemIds.Add(selectedFile.Id.ToString());

				if (!orderIds.Contains(selectedFile.ClientOrderId))
				{
					orderIds.Add(selectedFile.ClientOrderId);
				}
			}

			var addResponse = await _orderAssignedImageEditorService.Insert(assignedImages);

			if (!addResponse.IsSuccess)
			{
				//ModalNotification.ShowMessage("Error", addResponse.Message);
				//Folder Structure Data Update
				isAssignOrderSubmitting = false;
				return;
			}

			//Update Status after assign to editor
			await _updateOrderItemBLLService.UpdateOrderItemsStatusByClientItemListModel(selectedFiles.ToList(), InternalOrderItemStatus.Distributed, loginUser.ContactId);


			foreach (var orderId in orderIds)
			{
				await _orderStatusService.UpdateOrderStatusByOrderId(orderId,loginUser.ContactId);
			}

			selectedFiles = new List<ClientOrderItemListModel>();

			isAssignOrderSubmitting = false;
			CloseAssignOrderItemToEditorPopup();
			spinShow = false;
			await ReloadGrid();
			StateHasChanged();
			//await js.DisplayMessage("Successfully Assigned");
			//}
			//catch (Exception ex)
			//{
			//	CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
			//		{
			//			PrimaryId = (int)order.Id,
			//			ActivityLogFor = (int)ActivityLogForConstants.Order,
			//			loginUser = loginUser,
			//			ErrorMessage = ex.Message,
			//			MethodName = "InsertAssingOrderToEditor",
			//			RazorPage = "OrderDetails.razor.cs",
			//			Category = (int)ActivityLogCategory.InsertAssingOrderToEditorError,
			//		};
			//	await _activityLogCommonMethodService.InsertErrorActivityLog(activity);
			//	await js.DisplayMessage($"{ex.Message}");
			//}
		}
		void CloseAssignOrderItemToEditorPopup()
		{
			isOrderItemsAssignToEditorPopupVisible = false;
		}

		private async Task<List<ClientOrderItemListModel>> LeaveAssignImage()
		{
			List<ClientOrderItemListModel> assignedAbleItemList = new List<ClientOrderItemListModel>();

			foreach (var orderItem in selectedFiles)
			{
				
				if (orderItem.Status <= (int)InternalOrderItemStatus.InProduction || orderItem.Status == (int)InternalOrderItemStatus.ReworkDistributed || orderItem.Status == (int)InternalOrderItemStatus.ReworkInProduction)
				{
					assignedAbleItemList.Add(orderItem);
				}
			}
			List<long> orderIds = new List<long>();
			if (assignedAbleItemList.Any())
			{
				long orderId = 0;
				foreach (var orderItem in assignedAbleItemList)
				{

					var response = await _orderAssignedImageEditorService.Delete((int)orderItem.Id);

					if (response.IsSuccess)
					{
						orderItem.Status = (byte)InternalOrderItemStatus.Assigned;
						orderItem.ExternalStatus = (byte)(EnumHelper.ExternalOrderItemStatusChange(InternalOrderItemStatus.Assigned));
						await _clientOrderItemService.UpdateClientOrderItemListModelStatus(orderItem);


						orderId = orderItem.ClientOrderId;

						if (!orderIds.Contains(orderItem.ClientOrderId))
						{
							orderIds.Add(orderItem.ClientOrderId);
						}
					}
					else
					{
						assignedAbleItemList.Remove(orderItem);
					}

				}
				//await _updateOrderItemBLLService.UpdateOrderItemsStatusByClientItemListModel(assignedAbleItemList, InternalOrderItemStatus.Assigned, loginUser.ContactId);

				foreach (var clientOrderId in orderIds)
				{
					await _orderStatusService.UpdateOrderStatusByOrderId(clientOrderId,loginUser.ContactId);
				}
			}
			return assignedAbleItemList;


		}

		async Task ReloadGrid()
		{
			await grid.Reload();
		}
		private async Task SingleDownloadEditor(bool canStatusChange = false)
		{

            List<ClientOrderItemListModel> downloadedItemList = new List<ClientOrderItemListModel>();
            try
			{
				if (selectedFiles != null && !selectedFiles.Any())
				{
					await js.DisplayMessage("Please Select At Least One File");
					return;
				}
				DateTime currentDateTime = DateTime.Now;
				string formattedDateTimeForDownload = currentDateTime.ToString("dd-MM-yyyy-HHmmss");

				ContactModel contactInfo = await _contactManager.GetById(loginUser.ContactId);
				var sharedFolderDownloadPath = "";

				if (contactInfo != null)
				{
					sharedFolderDownloadPath = contactInfo.DownloadFolderPath;
				}

				if (!Directory.Exists(sharedFolderDownloadPath))
				{
					await js.DisplayMessage("Shared folder is not Enable !");
					return;
				}

				spinShow = true;
				StateHasChanged();

				if (canStatusChange)
				{
                    if (selectedFiles != null && selectedFiles.Any())
                    {
                        foreach (var item in selectedFiles)
                        {
                            if (item.Status > (byte)InternalOrderItemStatus.InProduction)
                            {
                                await js.DisplayMessage("Status out of range , Unable to download!");
                                return;
                            }
                        }

                    }
                }

                if (selectedFiles != null && selectedFiles.Any())
				{
					//var contactInfo = await _contactManager.GetById(loginUser.ContactId);
					List<long> orderIds = new List<long>();
					foreach (var item in selectedFiles)
					{
						var itemOrderInfo = await _clientOrderService.GetById(item.OrderId);
						var serverInfo = await _fileServerService.GetById((int)itemOrderInfo.FileServerId);

						FileUploadModel model = new FileUploadModel();

						model.FtpUrl = serverInfo.Host;
						model.userName = serverInfo.UserName;
						model.password = serverInfo.Password;
						model.SubFolder = serverInfo.SubFolder;
						model.OrderNumber = itemOrderInfo.OrderNumber;
						model.ContactName = contactInfo.FirstName.Trim() + " " + contactInfo.Id;

						using (var client = _ftpService.CreateFtpClient(model))
						{
							client.Config.EncryptionMode = FtpEncryptionMode.Auto;
							client.Config.ValidateAnyCertificate = true;
							client.Connect();

							var dataSavePath = sharedFolderDownloadPath + $"\\{model.ContactName}\\" + "Raw\\" + $"{formattedDateTimeForDownload}" + "\\" + $"{itemOrderInfo.OrderNumber}";

							var localPath = $"{dataSavePath}/{item.FileName}";
							var remotePath = $"{item.InternalFileInputPath}";

							if (!string.IsNullOrWhiteSpace(serverInfo.SubFolder))
							{
								remotePath = $"{serverInfo.SubFolder}/{remotePath}";
							}
							if (!Directory.Exists(localPath))
							{
								Directory.CreateDirectory(Path.GetDirectoryName(localPath));
							}

							var downloadResponse = client.DownloadFile(localPath, remotePath);

							if (downloadResponse.Equals(FtpStatus.Success))
							{
								await _activityAppLogService.InsertAppDownloadToEditorPcActivityLog((int)item.Id, localPath, loginUser.ContactId);

								var uploadedPath = sharedFolderDownloadPath + $"\\{model.ContactName}\\" + "Completed\\" + "_uploaded\\";

								if (!Directory.Exists(uploadedPath))
								{
									Directory.CreateDirectory(uploadedPath);
								}

								long orderId = item.ClientOrderId;

								if (!orderIds.Contains(item.ClientOrderId))
								{
									orderIds.Add(item.ClientOrderId);
								}

								downloadedItemList.Add(item);
							}

							client.Disconnect();

						}
					}

					if(canStatusChange)
					{
						await _updateOrderItemBLLService.UpdateOrderItemsStatusByClientItemListModel(downloadedItemList, InternalOrderItemStatus.InProduction, loginUser.ContactId);

						foreach (var clientOrderId in orderIds)
						{
							await _orderStatusService.UpdateOrderStatusByOrderId(clientOrderId, loginUser.ContactId);
						}
					}
					
				}
			}

			catch (Exception ex)
			{
				spinShow = false;
				StateHasChanged();
				await js.DisplayMessage(ex.ToString());
				await js.DisplayMessage("Download Failed");
			}
			spinShow = false;
			StateHasChanged();
			await js.DisplayMessage($"Successfully Downloaded {downloadedItemList.Count} Out Of {selectedFiles.Count} Images");
		}

        private async Task SingleCompletedFileDownloadByEditor()
        {

            List<ClientOrderItemListModel> downloadedItemList = new List<ClientOrderItemListModel>();
            try
            {
                if (selectedFiles != null && !selectedFiles.Any())
                {
                    await js.DisplayMessage("Please Select At Least One File");
                    return;
                }

                if (selectedFiles != null && selectedFiles.Any())
                {
                    foreach (var item in selectedFiles)
                    {
                        if (item.Status <= (byte)InternalOrderItemStatus.InProduction)
                        {
                            await js.DisplayMessage("Not completed image unable to download");
                            return;
                        }
                    }

                }
                DateTime currentDateTime = DateTime.Now;
                string formattedDateTimeForDownload = currentDateTime.ToString("dd-MM-yyyy-HHmmss");

                ContactModel contactInfo = await _contactManager.GetById(loginUser.ContactId);
                var sharedFolderDownloadPath = "";

                if (contactInfo != null)
                {
                    sharedFolderDownloadPath = contactInfo.DownloadFolderPath;
                }

                if (!Directory.Exists(sharedFolderDownloadPath))
                {
                    await js.DisplayMessage("Shared folder is not Enable !");
                    return;
                }

                spinShow = true;
                StateHasChanged();
               

                if (selectedFiles != null && selectedFiles.Any())
                {
                    //var contactInfo = await _contactManager.GetById(loginUser.ContactId);
                    List<long> orderIds = new List<long>();
                    foreach (var item in selectedFiles)
                    {
                        var itemOrderInfo = await _clientOrderService.GetById(item.OrderId);
                        var serverInfo = await _fileServerService.GetById((int)itemOrderInfo.FileServerId);

                        FileUploadModel model = new FileUploadModel();

                        model.FtpUrl = serverInfo.Host;
                        model.userName = serverInfo.UserName;
                        model.password = serverInfo.Password;
                        model.SubFolder = serverInfo.SubFolder;
                        model.OrderNumber = itemOrderInfo.OrderNumber;
                        model.ContactName = contactInfo.FirstName.Trim() + " " + contactInfo.Id;

                        using (var client = _ftpService.CreateFtpClient(model))
                        {
                            client.Config.EncryptionMode = FtpEncryptionMode.Auto;
                            client.Config.ValidateAnyCertificate = true;
                            client.Connect();

                            var dataSavePath = sharedFolderDownloadPath + $"\\{model.ContactName}\\" + "Raw\\" + $"{formattedDateTimeForDownload}" + "\\" + $"{itemOrderInfo.OrderNumber}";

                            var localPath = $"{dataSavePath}/{item.FileName}";
                            var remotePath = $"{item.InternalFileOutputPath}";

                            if (!string.IsNullOrWhiteSpace(serverInfo.SubFolder))
                            {
                                remotePath = $"{serverInfo.SubFolder}/{remotePath}";
                            }
                            if (!Directory.Exists(localPath))
                            {
                                Directory.CreateDirectory(Path.GetDirectoryName(localPath));
                            }

							//If downladed file not exist , continue other for download
							if (!client.FileExists(remotePath))
							{
								continue;
							}

                            var downloadResponse = client.DownloadFile(localPath, remotePath);

                            if (downloadResponse.Equals(FtpStatus.Success))
                            {
                                await _activityAppLogService.InsertAppDownloadToEditorPcActivityLog((int)item.Id, localPath, loginUser.ContactId);

                                var uploadedPath = sharedFolderDownloadPath + $"\\{model.ContactName}\\" + "Completed\\" + "_uploaded\\";

                                if (!Directory.Exists(uploadedPath))
                                {
                                    Directory.CreateDirectory(uploadedPath);
                                }

                                long orderId = item.ClientOrderId;

                                if (!orderIds.Contains(item.ClientOrderId))
                                {
                                    orderIds.Add(item.ClientOrderId);
                                }

                                downloadedItemList.Add(item);
                            }

                            client.Disconnect();

                        }
                    }

                }
            }

            catch (Exception ex)
            {
                spinShow = false;
                StateHasChanged();
                await js.DisplayMessage(ex.ToString());
                await js.DisplayMessage("Download Failed");
            }
            spinShow = false;
			StateHasChanged();
            await js.DisplayMessage($"Successfully Downloaded {downloadedItemList.Count} Out Of {selectedFiles.Count} Images");
        }

        #region QC Functionality 
        private async Task UpdateOrderItemInQc(InternalOrderItemStatus status, bool FileMove = false)
		 {
			try
			{

				spinShow = true;
				if (selectedFiles == null || !selectedFiles.Any())
				{
					spinShow = false;
					this.StateHasChanged();
					await js.DisplayMessage("Select at least one Item !");
					return;

				}
				if (selectedFiles.Any(i => i.Status != (byte)InternalOrderItemStatus.ProductionDone && i.Status != (byte)InternalOrderItemStatus.ReworkDone))
				{
					spinShow = false;
					this.StateHasChanged();
					await js.DisplayMessage("Unable to Take in Take In Qc . Due To Status Protocol");
					return;
				}
				
				await _updateOrderItemBLLService.UpdateOrderItemsStatusByClientItemListModel(selectedFiles.ToList(), InternalOrderItemStatus.InQc, loginUser.ContactId);

				var orderIds = selectedFiles.Select(f => f.ClientOrderId).Distinct().ToList();
				foreach (var clientOrderId in orderIds)
				{
					await _orderStatusService.UpdateOrderStatusByOrderId(clientOrderId,loginUser.ContactId);
				}

				selectedFiles = new List<ClientOrderItemListModel>();



				spinShow = false;
				this.StateHasChanged();
			}

			catch (Exception ex)
			{
				CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
				{
					PrimaryId = 0,
					ActivityLogFor = (int)ActivityLogForConstants.Order,
					loginUser = loginUser,
					ErrorMessage = ex.Message,
					MethodName = "UpdateOrderItemByQc",
					RazorPage = "OrderImages.razor.cs",
					Category = (int)ActivityLogCategory.UpdateOrderItemByQcError,
				};
				await _activityLogCommonMethodService.InsertErrorActivityLog(activity);
				await js.DisplayMessage($"{ex.Message}");
			}
		}

		private async Task UpdateOrderItemReadyToDeliver()
		{
			try
			{
				spinShow = true;

				if (selectedFiles == null || !selectedFiles.Any())
				{
					spinShow = false;
					this.StateHasChanged();
					await js.DisplayMessage("Select at least one Item !");
					return;
				}

				if (selectedFiles.Any(i => i.Status != (byte)InternalOrderItemStatus.InQc))
				{
					spinShow = false;
					this.StateHasChanged();
					await js.DisplayMessage("One of selected image not In Qc");
					return;
				}

               var orderItemGroups = from p in selectedFiles
                                     group p by p.ClientOrderId into g
                                     select new { ClientOrderId = g.Key, Items = g.ToList() };

				foreach (var groupItem in orderItemGroups)
				{
					var orderInfo = await _clientOrderService.GetById(groupItem.ClientOrderId);
					var itemIds = groupItem.Items.Select(i => i.Id).ToList();

                    var response = await _autoOrderDeliveryService.MoveOrderItemIdsAsCompletedOrder(itemIds, orderInfo);
					await _updateOrderItemBLLService.UpdateOrderItemsStatus(response.Result, InternalOrderItemStatus.ReadyToDeliver,loginUser.ContactId);
					await _orderStatusService.UpdateOrderStatus(orderInfo,loginUser.ContactId);
				}
                selectedFiles = new List<ClientOrderItemListModel>();
				spinShow = false;
				await ReloadGrid();
				this.StateHasChanged();
			}

			catch (Exception ex)
			{
				CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
				{
					PrimaryId = 0,
					ActivityLogFor = (int)ActivityLogForConstants.Order,
					loginUser = loginUser,
					ErrorMessage = ex.Message,
					MethodName = "UpdateOrderItemReadyToDeliver",
					RazorPage = "OrderImages.razor.cs",
					Category = (int)ActivityLogCategory.UpdateOrderItemByQcError,
				};
				await _activityLogCommonMethodService.InsertErrorActivityLog(activity);
				await js.DisplayMessage($"{ex.Message}");
			}
		}


		private async Task Reject(InternalOrderItemStatus status = InternalOrderItemStatus.InQc)
		{
			try
			{
				spinShow = true;
				if (selectedFiles != null)
				{
					if (selectedFiles.Count <= 0)
					{
						spinShow = false;
						this.StateHasChanged();
						await js.DisplayMessage("Select at least One File");
						return;
					}

					foreach (var file in selectedFiles)
					{
						if (file.Status == (byte)InternalOrderItemStatus.ProductionDone || file.Status == (byte)InternalOrderItemStatus.ReworkDone)
						{
							spinShow = false;
							this.StateHasChanged();
							await js.DisplayMessage("Dear User One of your selected File is not in Qc . To Reject First Take them in QC");
							return;
						}
						
						var order = await _clientOrderService.GetById(file.ClientOrderId);
						var serverInfo = await _fileServerService.GetById((int)order.FileServerId);
						var contactInfo = await _contactManager.GetById(loginUser.ContactId);

						var fileInfo = await _clientOrderItemService.GetById((int)file.Id);
						var imageInfo = await _assignService.GetById((int)file.Id);
						var editorInfo = await _contactManager.GetById(imageInfo.AssignContactId);
						FileUploadModel model = new FileUploadModel();
						model.fileName = Path.GetFileName(fileInfo.ProductionDoneFilePath);
						model.FtpUrl = serverInfo.Host;
						model.userName = serverInfo.UserName;
						model.password = serverInfo.Password;
						model.OrderNumber = order.OrderNumber;
						var path = $"{fileInfo.InternalFileInputPath}";
						var dividePath = path.Split("/");
						model.RootDirectory = dividePath[0];
						model.Date = order.CreatedDate;
						model.ContactName = editorInfo.FirstName + editorInfo.Id;
						var baseDirectory = "";
						var productionDoneFilePathAfterInProgress = "";


						var dividePathFor = fileInfo.ProductionDoneFilePath.Split("In Progress");
						baseDirectory = dividePathFor[0];


						productionDoneFilePathAfterInProgress = fileInfo.ProductionDoneFilePath.Split("/In Progress")[1];
						productionDoneFilePathAfterInProgress = productionDoneFilePathAfterInProgress.Replace("\\", "/");
						//Source Path
						model.UploadDirectory = $"{baseDirectory}/In Progress{Path.GetDirectoryName(productionDoneFilePathAfterInProgress)}";
						//Destination Path
						var destinationPath = "";

						destinationPath = fileInfo.ProductionDoneFilePath.Replace("/Production Done", "/Rejected");



						model.ReturnPath = Path.GetDirectoryName(destinationPath);


						await _fluentFtpService.FolderCreateAtApprovedTime(model);
						FileUploadModel modell = new FileUploadModel()
						{
							FtpUrl = serverInfo.Host,
							userName = serverInfo.UserName,
							password = serverInfo.Password,
							SubFolder = serverInfo.SubFolder,
							ReturnPath = $"{model.ReturnPath}/{model.fileName}",
							UploadDirectory = $"{model.UploadDirectory}/{model.fileName}"
						};

						var fileMoveResult = await _fluentFtpService.MoveFile(modell);

						if (fileMoveResult.IsSuccess)
						{
							ClientOrderItemModel clientOrderItem = new ClientOrderItemModel()
							{
								CompanyId = fileInfo.CompanyId,
								FileName = file.FileName,
								ProductionDoneFilePath = destinationPath,
								PartialPath = fileInfo.PartialPath,
								ClientOrderId = order.Id,
							};

							var updateItemInDBResponse = await _clientOrderItemService.UpdateItemByQCForReject(clientOrderItem);

							//TODO: Need to update items
							if (updateItemInDBResponse.IsSuccess)
							{
								var orderItemForUpdatePath = orderItems.FirstOrDefault(f => f.Id == file.Id);

								if (orderItemForUpdatePath.ProductionDoneFilePath != null)
								{
									//Update currnt path and status
									orderItems.FirstOrDefault(f => f.Id == file.Id);
								}
							}
						}

						await UpdateOrderItem(status, file);
						await UpdateOrder(order, await GetInternalOrderStatus((int)order.Id)); //ToDo:RakibStatus

					}
					// Rakib Vai
					//await UpdateOrderItem(status);
					selectedFiles = new List<ClientOrderItemListModel>();

					//Order Status Update
					//await UpdateOrder(order, await GetInternalOrderStatus((int)order.Id)); //ToDo:RakibStatus

					//await LoadOrderItemForLoginUser();
					//Order Status Update
					//isShowImagePopup = false;
				}
				else
				{
					spinShow = false;
					this.StateHasChanged();
					await js.DisplayMessage("Select at least one file or item !");
				}
				spinShow = false;
				this.StateHasChanged();
			}
			catch (Exception ex)
			{
				CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
				{
					PrimaryId = 0,
					ActivityLogFor = (int)ActivityLogForConstants.Order,
					loginUser = loginUser,
					ErrorMessage = ex.Message,
					MethodName = "Reject",
					RazorPage = "OrderDetails.razor.cs",
					Category = (int)ActivityLogCategory.RejectError,
				};
				await _activityLogCommonMethodService.InsertErrorActivityLog(activity);
				await js.DisplayMessage($"{ex.Message}");
			}

		}
		#endregion

		#region Update Order Item
		private async Task UpdateOrderItem(InternalOrderItemStatus status, ClientOrderItemListModel item =null)
		{
			if (item == null)
			{

				if (selectedFiles.Count > 0)
				{
					foreach (var file in selectedFiles)
					{
						var statusHasChanged = false;

						// Mapping Model to Entity
						var configuration = new MapperConfiguration(cfg =>
						   {
							   cfg.CreateMap<ClientOrderItemListModel, ClientOrderItemModel>();
						   });
						var mapper = configuration.CreateMapper();

						//Get user info
						var selectedFile = mapper.Map<ClientOrderItemModel>(file);

						if (selectedFile.Status == (byte)InternalOrderItemStatus.ReworkDistributed)
						{
							selectedFile.Status = (byte)InternalOrderItemStatus.ReworkInProduction;
							selectedFile.ExternalStatus = (byte)(EnumHelper.ExternalOrderItemStatusChange(InternalOrderItemStatus.ReworkInProduction));
							await _clientOrderItemService.UpdateClientOrderItemStatus(selectedFile);
							statusHasChanged = true;
						}
						else if (selectedFile.Status == (byte)InternalOrderItemStatus.ReworkDone)
						{
							selectedFile.Status = (byte)InternalOrderItemStatus.ReworkQc;
							selectedFile.ExternalStatus = (byte)(EnumHelper.ExternalOrderItemStatusChange(InternalOrderItemStatus.ReworkQc));
							await _clientOrderItemService.UpdateClientOrderItemStatus(selectedFile);
							statusHasChanged = true;

						}
						else if (selectedFile.Status == (byte)InternalOrderItemStatus.ReworkQc && status == InternalOrderItemStatus.ReworkDistributed)
						{
							selectedFile.Status = (byte)InternalOrderItemStatus.ReworkDistributed;
							selectedFile.ExternalStatus = (byte)(EnumHelper.ExternalOrderItemStatusChange(InternalOrderItemStatus.ReworkDistributed));
							await _clientOrderItemService.UpdateClientOrderItemStatus(selectedFile);
							statusHasChanged = true;

						}
						else
						{
							if (selectedFile.Status < (byte)status)
							{
								selectedFile.Status = (byte)status;
								selectedFile.ExternalStatus = (byte)(EnumHelper.ExternalOrderItemStatusChange(status));
								await _clientOrderItemService.UpdateClientOrderItemStatus(selectedFile);
								statusHasChanged = true;

							}

						}
						if (statusHasChanged)
						{
							await AddOrderItemStatusChangeLog(selectedFile, status);// Order Item Status Change log
						}


					}
				}
				else
				{
					foreach (var file in orderItems)
					{

						// Mapping Model to Entity
						var configuration = new MapperConfiguration(cfg =>
						{
							cfg.CreateMap<ClientOrderItemListModel, ClientOrderItemModel>();
						});
						var mapper = configuration.CreateMapper();

						//Get user info
						var selectedFile = mapper.Map<ClientOrderItemModel>(file);

						if (selectedFile.Status == (byte)InternalOrderItemStatus.ReworkDistributed)
						{
							selectedFile.Status = (byte)InternalOrderItemStatus.ReworkInProduction;
							selectedFile.ExternalStatus = (byte)(EnumHelper.ExternalOrderItemStatusChange(InternalOrderItemStatus.ReworkInProduction));
							await _clientOrderItemService.UpdateClientOrderItemStatus(selectedFile);
						}
						else
						{
							if (selectedFile.Status < (byte)status)
							{
								selectedFile.Status = (byte)status;
								selectedFile.ExternalStatus = (byte)(EnumHelper.ExternalOrderItemStatusChange(status));
								await _clientOrderItemService.UpdateClientOrderItemStatus(selectedFile);
							}
						}
						await AddOrderItemStatusChangeLog(selectedFile, status);// Order Item Status Change log

					}
				}
			}
			else
			{
				var statusHasChanged = false;

				// Mapping Model to Entity
				var configuration = new MapperConfiguration(cfg =>
				{
					cfg.CreateMap<ClientOrderItemListModel, ClientOrderItemModel>();
				});
				var mapper = configuration.CreateMapper();

				//Get user info
				var selectedFile = mapper.Map<ClientOrderItemModel>(item);

				if (selectedFile.Status == (byte)InternalOrderItemStatus.ReworkDistributed)
				{
					selectedFile.Status = (byte)InternalOrderItemStatus.ReworkInProduction;
					selectedFile.ExternalStatus = (byte)(EnumHelper.ExternalOrderItemStatusChange(InternalOrderItemStatus.ReworkInProduction));
					await _clientOrderItemService.UpdateClientOrderItemStatus(selectedFile);
					statusHasChanged = true;
				}
				else if (selectedFile.Status == (byte)InternalOrderItemStatus.ReworkDone)
				{
					selectedFile.Status = (byte)InternalOrderItemStatus.ReworkQc;
					selectedFile.ExternalStatus = (byte)(EnumHelper.ExternalOrderItemStatusChange(InternalOrderItemStatus.ReworkQc));
					await _clientOrderItemService.UpdateClientOrderItemStatus(selectedFile);
					statusHasChanged = true;

				}
				else if (selectedFile.Status == (byte)InternalOrderItemStatus.ReworkQc && status == InternalOrderItemStatus.ReworkDistributed)
				{
					selectedFile.Status = (byte)InternalOrderItemStatus.ReworkDistributed;
					selectedFile.ExternalStatus = (byte)(EnumHelper.ExternalOrderItemStatusChange(InternalOrderItemStatus.ReworkDistributed));
					await _clientOrderItemService.UpdateClientOrderItemStatus(selectedFile);
					statusHasChanged = true;

				}
				else
				{
					if (selectedFile.Status < (byte)status)
					{
						selectedFile.Status = (byte)status;
						selectedFile.ExternalStatus = (byte)(EnumHelper.ExternalOrderItemStatusChange(status));
						await _clientOrderItemService.UpdateClientOrderItemStatus(selectedFile);
						statusHasChanged = true;

					}

				}
				if (statusHasChanged)
				{
					await AddOrderItemStatusChangeLog(selectedFile, status);// Order Item Status Change log
				}

			}

			//CanUploadProductionDoneImages();
		}


		public async Task AddOrderItemStatusChangeLog(ClientOrderItemModel clientOrderItem, InternalOrderItemStatus internalOrderItemStatus)
		{
			var previousLog = await _orderItemStatusChangeLogService.OrderItemStatusLastChangeLogByOrderFileId((int)clientOrderItem.Id);

			if (previousLog != null || previousLog.NewInternalStatus != (byte)internalOrderItemStatus)
			{
				OrderItemStatusChangeLogModel orderItemStatusChangeLog = new OrderItemStatusChangeLogModel
				{
					OrderFileId = (int)clientOrderItem.Id,
					NewInternalStatus = (byte)internalOrderItemStatus,
					NewExternalStatus = (byte)EnumHelper.ExternalOrderItemStatusChange(internalOrderItemStatus),
					ChangeByContactId = loginUser.ContactId,
					ChangeDate = DateTime.Now
				};

				if (previousLog != null)
				{
					orderItemStatusChangeLog.OldExternalStatus = previousLog.NewExternalStatus;
					orderItemStatusChangeLog.OldInternalStatus = previousLog.NewInternalStatus;
					orderItemStatusChangeLog.TimeDurationInMinutes = (int)(orderItemStatusChangeLog.ChangeDate.Subtract(previousLog.ChangeDate).TotalMinutes);
				}
				await _orderItemStatusChangeLogService.Insert(orderItemStatusChangeLog);
			}

		}

		private async Task UpdateOrder(ClientOrderModel clientOrder, InternalOrderStatus status)
		{
			if (status == InternalOrderStatus.AssignedForSupport)
			{
				status = InternalOrderStatus.Assigned;
			}
			clientOrder.InternalOrderStatus = (byte)status;
			clientOrder.ExternalOrderStatus = (byte)(EnumHelper.ExternalOrderStatusChange(status));
			await _clientOrderService.UpdateClientOrderStatus(clientOrder);

			await AddOrderStatusChangeLog(clientOrder, status);
		}

		public async Task AddOrderStatusChangeLog(ClientOrderModel clientOrder, InternalOrderStatus internalOrderStatus)
		{
			var previousLog = await _orderStatusChangeLogService.OrderStatusLastChangeLogByOrderId((int)clientOrder.Id);
			if (previousLog.NewInternalStatus != (byte)internalOrderStatus)
			{
				OrderStatusChangeLogModel orderStatusChangeLog = new OrderStatusChangeLogModel
				{
					OrderId = (int)clientOrder.Id,
					NewInternalStatus = (byte)internalOrderStatus,
					NewExternalStatus = (byte)EnumHelper.ExternalOrderStatusChange(internalOrderStatus),
					ChangeByContactId = loginUser.ContactId,
					ChangeDate = DateTime.Now
				};

				if (previousLog != null)
				{
					orderStatusChangeLog.OldExternalStatus = previousLog.NewExternalStatus;
					orderStatusChangeLog.OldInternalStatus = previousLog.NewInternalStatus;
					orderStatusChangeLog.TimeDurationInMinutes = (int)(orderStatusChangeLog.ChangeDate.Subtract(previousLog.ChangeDate).TotalMinutes);
				}
				await _orderStatusChangeLogService.Insert(orderStatusChangeLog);
			}

		}

		private async Task<InternalOrderStatus> GetInternalOrderStatus(int orderId)
		{
			InternalOrderStatus internalOrderItemStatus = 0; //Todo:Rakib
			try
			{
				await Task.Yield();
				ClientOrderItemStatus clientOrderItemMinStatus = await _clientOrderItemService.GetOrderItemMinStatusByOrderId(orderId);
				internalOrderItemStatus = (InternalOrderStatus)clientOrderItemMinStatus.Status;

			}
			catch (Exception ex)
			{
				CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
				{
					PrimaryId = (int)orderId,
					ActivityLogFor = (int)ActivityLogForConstants.Order,
					loginUser = loginUser,
					ErrorMessage = ex.Message,
					MethodName = "GetInternalOrderStatus",
					RazorPage = "OrderDetails.razor.cs",
					Category = (int)ActivityLogCategory.GetInternalOrderStatusError,
				};
				await _activityLogCommonMethodService.InsertErrorActivityLog(activity);
				await js.DisplayMessage($"{ex.Message}");

			}

			return internalOrderItemStatus;

		}
		#endregion

		private async Task ShowHideImgaeOnList()
		{
			await Task.Yield();
			showImageOnList = !showImageOnList;
			StateHasChanged();
		}

		bool CheckListContainsFile<T>(IList<T> selectedFiles)
		{
			bool isContains = true;
			if (!selectedFiles.Any())
			{
				//spinShow = false;
				//this.StateHasChanged();
				//await js.DisplayMessage("Select at least One File");
				isContains = false;
			}
			return isContains;

		}
        async Task ShowProductionDoneImagePopup(ClientOrderItemListModel clientOrderItemVM)
        {
            spinShow = true;
			StateHasChanged();
			clientOrderItem = new ClientOrderItemListModel();
            clientOrderItem = clientOrderItemVM;
            isShowProductionDoneImagePopup = true;
            this.StateHasChanged();
            spinShow = false;
            StateHasChanged();
        }
		private async Task PreviousImageShow(ClientOrderItemListModel clientOrderItemVM)
		{
			spinShow = true;
			StateHasChanged();
			int clientItemIndex = orderItems.FindIndex(x => x.Id.Equals(clientOrderItemVM.Id));
			if ((clientItemIndex - 1) == -1)
			{
				CloseProductionDoneImagePopup();
			}
			else
			{
				clientOrderItem = new ClientOrderItemListModel();
				clientOrderItem = orderItems[clientItemIndex - 1];
			}
			spinShow = false;
			this.StateHasChanged();
		}
		private async Task NextImageShow(ClientOrderItemListModel clientOrderItemVM)
		{
			spinShow = true;
			StateHasChanged();
			int clientItemIndex = orderItems.FindIndex(x => x.Id.Equals(clientOrderItemVM.Id));
			int LastIndex = orderItems.Count() - 1;
			if ((clientItemIndex + 1) > LastIndex)
			{
				CloseProductionDoneImagePopup();
			}
			else
			{
				clientOrderItem = new ClientOrderItemListModel();
				clientOrderItem = orderItems[clientItemIndex + 1];
			}
			spinShow = false;
			this.StateHasChanged();
		}


		void CloseProductionDoneImagePopup()
		{
			isShowProductionDoneImagePopup = false;
			isShowCompletedImagePopup = false;
			this.StateHasChanged();
		}
		private async Task ApprovedQCForPopupView(InternalOrderItemStatus status, ClientOrderItemListModel orderItem, bool FileMove = true)
		{
			if (clientOrderItem.Id > 0)
			{
				try
				{
					var order = await _clientOrderService.GetById(orderItem.ClientOrderId);
					var serverInfo = await _fileServerService.GetById((int)order.FileServerId);
					var contactInfo = await _contactManager.GetById(loginUser.ContactId);
					var fileInfo = await _clientOrderItemService.GetById((int)orderItem.Id);

					var imageInfo = await _assignService.GetById((int)fileInfo.Id);
					var editorInfo = await _contactManager.GetById(imageInfo.AssignContactId);

					spinShow = true;


					if (fileInfo.Status == (byte)InternalOrderItemStatus.ProductionDone || fileInfo.Status == (byte)InternalOrderItemStatus.ReworkDone)
					{
						spinShow = false;
						StateHasChanged();
						await js.DisplayMessage("Dear User One of your selected File is not in Qc . To Approve or Reject First Take them in QC");
						return;
					}

					List<long> itemIds = new List<long>();
					itemIds.Add(orderItem.Id);

					var response = await _autoOrderDeliveryService.MoveOrderItemIdsAsCompletedOrder(itemIds, order);
					await _updateOrderItemBLLService.UpdateOrderItemsStatus(response.Result, status);
					await _orderStatusService.UpdateOrderStatus(order,loginUser.ContactId);

					selectedFiles = new List<ClientOrderItemListModel>();
					spinShow = false;
					await ReloadGrid();
					this.StateHasChanged();

					isShowImagePopup = false;
				}
				catch(Exception ex)
				{ 

				}
			}
			else
			{
				await js.DisplayMessage("Select at least one Item !");
				return;
			}
			spinShow = true;
			int clientItemIndex = orderItems.FindIndex(x => x.Id.Equals(clientOrderItem.Id));
			int LastIndex = orderItems.Count() - 1;
			if ((clientItemIndex + 1) > LastIndex)
			{
				CloseProductionDoneImagePopup();
			}
			else
			{
				clientOrderItem = new ClientOrderItemListModel();
				clientOrderItem = orderItems[clientItemIndex + 1];
			}

			this.StateHasChanged();
			spinShow = false;
		}

		private async Task RejectFromPopupView(InternalOrderItemStatus status, ClientOrderItemListModel orderItem)
		{
			spinShow = true;
			if (orderItem.Id > 0)
			{
				try
				{
					var order = await _clientOrderService.GetById(orderItem.ClientOrderId);
					//var fileInfo = await _clientOrderItemService.GetById((int));

					if (orderItem.Status == (byte)InternalOrderItemStatus.ProductionDone || orderItem.Status == (byte)InternalOrderItemStatus.ReworkDone)
					{
						spinShow = false;
						StateHasChanged();
						await js.DisplayMessage("Dear User One of your selected File is not in Qc . To Reject First Take them in QC");
						return;
					}

					List<long> itemIds = new List<long>();
					itemIds.Add(orderItem.Id);

					var response = await _autoOrderDeliveryService.MoveOrderItemIdsAsCompletedOrder(itemIds, order);
					await _updateOrderItemBLLService.UpdateOrderItemsStatus(response.Result, status);
					await _orderStatusService.UpdateOrderStatus(order,loginUser.ContactId);

					selectedFiles = new List<ClientOrderItemListModel>();
					spinShow = false;
					await ReloadGrid();
					this.StateHasChanged();

					isShowImagePopup = false;
				}
				catch (Exception ex)
				{

				}
			}
			else
			{
				await js.DisplayMessage("Select at least one Item !");
				return;
			}

			spinShow = true;
			int clientItemIndex = orderItems.FindIndex(x => x.Id.Equals(clientOrderItem.Id));
			clientOrderItem = new ClientOrderItemListModel();
			clientOrderItem = orderItems[clientItemIndex + 1];
			this.StateHasChanged();
			spinShow = false;
		}

		private async Task ShowSetOrderItemTypePopup()
		{
			await Task.Yield();
			isSetOrderItemTypePopupVisible = true;
			foreach (OrderItemFileGroup item in Enum.GetValues(typeof(OrderItemFileGroup)))
			{
				fileGroupCustomEnumList.Add(new CustomEnumTypes { EnumName = item.ToString(), EnumValue = Convert.ToByte((int)item) });
			}
		}
		private async Task CloseSetOrderItemTypePopup()
		{
			fileGroupCustomEnumList = new List<CustomEnumTypes>();
			selectedFiles = null;
			await Task.Yield();
			isSetOrderItemTypePopupVisible = false;

		}

		private async Task UpdateOrderItemFileType()
		{

			foreach (var orderItem in selectedFiles)
			{
				if (orderItem.Status > (int)InternalOrderItemStatus.InProduction)
				{
					spinShow = false;
					StateHasChanged();
					await js.DisplayMessage("Unable to Add Group due to Status Protocal Policy");
					await CloseSetOrderItemTypePopup();
					return;
				}
			}


			List<long> orderIds = new List<long>();
			foreach (var selectedFile in selectedFiles)
			{
				long orderId = 0;
				selectedFile.FileGroup = (byte)selectedOrderItemGroup;
				await _clientOrderItemService.UpdateClientOrderItemListModelStatus(selectedFile);

				orderId = selectedFile.ClientOrderId;

				if (!orderIds.Contains(selectedFile.ClientOrderId))
				{
					orderIds.Add(selectedFile.ClientOrderId);
				}
				//await UpdateOrder(order, await GetInternalOrderStatus((int)order.Id));
			}

			foreach (var clientOrderId in orderIds)
			{
				await _orderStatusService.UpdateOrderStatusByOrderId(clientOrderId,loginUser.ContactId);
			}

			await ReloadGrid();
			await CloseSetOrderItemTypePopup();
		}

		void CloseImagePopup()
		{
			isShowRawImagePopup = false;
		}
	}
}

