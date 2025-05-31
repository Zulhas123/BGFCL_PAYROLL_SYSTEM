using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace BgfclApp.Service
{
    public class ERPIntegrationSettings
    {
        public bool IsIntegrated { get; set; }
    }
    public class BlockIfERPIntegratedAttribute : ActionFilterAttribute
    {
        private readonly bool _isIntegrated;

        public BlockIfERPIntegratedAttribute(IOptions<ERPIntegrationSettings> settings)
        {
            _isIntegrated = settings.Value.IsIntegrated;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (_isIntegrated)
            {
                context.Result = new ContentResult
                {
                    Content = "Action blocked. ERP integration is enabled.",
                    StatusCode = StatusCodes.Status403Forbidden
                };
            }
        }
    }

}
