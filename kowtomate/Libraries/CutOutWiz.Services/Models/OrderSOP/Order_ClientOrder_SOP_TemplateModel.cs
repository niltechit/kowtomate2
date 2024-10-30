using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutOutWiz.Services.Models.OrderSOP
{
	public class Order_ClientOrder_SOP_TemplateModel
	{
		public int Id { get; set; }
		public int Order_ClientOrder_Id { get; set; }
		public int SOP_Template_Id { get; set; }
		public int SortOrder { get; set; }
		public bool IsDeleted { get; set; }
		public int OrderSOP_Template_Id { get; set; }
	}
}
