using Microsoft.AspNetCore.Mvc;

namespace MaReSy2_Api.Controllers
{
    public static class helperMethod
    {
        public static ActionResult<APIResponse<T>> ToActionResult<T> (APIResponse<T> result, ControllerBase controller)
        {
            if (!result.Success)
            {
                return controller.StatusCode(result.StatusCode ?? 400, result);
            }

            return controller.StatusCode(result.StatusCode ?? 200, result.Data);
        }

        public static ActionResult<APIResponse<T>> ToActionResultBasic<T>(APIResponse<T> result, ControllerBase controller)
        {
            if (!result.Success)
            {
                return controller.StatusCode(result.StatusCode ?? 400, result);
            }

            return controller.StatusCode(result.StatusCode ?? 200, result);
        }
    }
}
