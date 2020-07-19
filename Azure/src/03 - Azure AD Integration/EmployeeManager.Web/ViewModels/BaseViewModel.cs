using System.Collections.Generic;

namespace EmployeeManager.Web.ViewModels
{
    public class BaseViewModel
    {
        public List<string> Errors { get; set; }

        public BaseViewModel()
        {
            Errors = new List<string>();
        }
    }
}