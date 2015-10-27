using KS.CRMWorkflow.Component.Cache.Implementation;
using KS.CRMWorkflow.Component.Cache.Interface;
using KS.CRMWorkflow.Component.Logging.Implemetation;
using KS.CRMWorkflow.Component.Logging.Interface;
using KS.CRMWorkflow.Data.DataAccess.Repository.Implementation;
using KS.CRMWorkflow.Data.DataAccess.Repository.Interface;
using Microsoft.AspNet.Identity;
using System;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Web.Http;

namespace KS.CRMWorkflow.WebAPI.Controllers.api
{
    /// <summary>
    /// Contains the logic used across all of the api controllers
    /// </summary>
    [Authorize]
    public class BaseApiController : ApiController
    {
        protected ICacheProvider CacheProvider { get; set; }
        protected IRepositoryCollection RepositoryCollection { get; set; }
        protected ILogger Logger { get; set; }

        public BaseApiController()
        {
            CacheProvider = MemoryCacheProvider.Instance;
            Logger = EnterpriseLogger.Instance;
            RepositoryCollection = new DapperRepositoryCollection(CacheProvider);
        }

        protected string AuthenticatedUserId
        {
            get
            {
                if (User != null)
                {
                    IIdentity identity = User.Identity;
                    if (identity != null)
                    {
                        return identity.GetUserId();
                    }
                }

                return string.Empty;
            }
        }

        public HttpResponseMessage CreateAccessError(string errorMessage)
        {
            errorMessage = string.Format(
                "{0}: UserId: {1} URL: {2}",
                errorMessage,
                AuthenticatedUserId,
                Request.RequestUri);

            Logger.Error(this, errorMessage);
            HttpError err = new HttpError(errorMessage);
            return Request.CreateResponse(HttpStatusCode.BadRequest, err);
        }

        public HttpResponseMessage CreateResponseError(string errorMessage, Exception ex)
        {
            Logger.Error(this, errorMessage, ex);
            HttpError err = new HttpError(errorMessage);
            return Request.CreateResponse(HttpStatusCode.BadRequest, err);
        }
    }        
}
