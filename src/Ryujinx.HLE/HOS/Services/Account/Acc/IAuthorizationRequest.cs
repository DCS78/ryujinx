using Gommon;
using Ryujinx.Common.Logging;
using Ryujinx.HLE.HOS.Kernel.Threading;
using Ryujinx.HLE.HOS.Services.Account.Acc.AsyncContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ryujinx.HLE.HOS.Services.Account.Acc
{
    class IAuthorizationRequest : IpcService
    {

        [CommandCmif(0)]
        // GetSessionId()
        public ResultCode GetSessionId(ServiceCtx context)
        {
            Logger.Stub?.PrintStub(LogClass.ServiceAcc);

            return ResultCode.Success;
        }


        [CommandCmif(10)]
        // Returns an #IAsyncContext.()
        public ResultCode InvokeWithoutInteractionAsync(ServiceCtx context)
        {
            KEvent asyncEvent = new(context.Device.System.KernelContext);
            AsyncExecution asyncExecution = new(asyncEvent);

            asyncExecution.Initialize(1000, EnsureIdTokenCacheAsyncImpl);
            MakeObject(context, new IAsyncContext(asyncExecution));

            return ResultCode.Success;
        }


        private async Task EnsureIdTokenCacheAsyncImpl(CancellationToken token)
        {
            // NOTE: This open the file at "su/baas/USERID_IN_UUID_STRING.dat" (where USERID_IN_UUID_STRING is formatted as "%08x-%04x-%04x-%02x%02x-%08x%04x")
            //       in the "account:/" savedata.
            //       Then its read data, use dauth API with this data to get the Token Id and probably store the dauth response
            //       in "su/cache/USERID_IN_UUID_STRING.dat" (where USERID_IN_UUID_STRING is formatted as "%08x-%04x-%04x-%02x%02x-%08x%04x") in the "account:/" savedata.
            //       Since we don't support online services, we can stub it.

            Logger.Stub?.PrintStub(LogClass.ServiceAcc);

            // TODO: Use a real function instead, with the CancellationToken.
            await Task.CompletedTask;
        }


        [CommandCmif(19)]

        public ResultCode IsAuthorized(ServiceCtx context)
        {
            Logger.Stub?.PrintStub(LogClass.ServiceAcc);

            return ResultCode.Success;
        }

        [CommandCmif(20)]
        public ResultCode GetAuthorizationCode(ServiceCtx context)
        {
            Logger.Stub?.PrintStub(LogClass.ServiceAcc);

            return ResultCode.Success;
        }

        [CommandCmif(21)]
        public ResultCode GetIdToken(ServiceCtx context)
        {
            Logger.Stub?.PrintStub(LogClass.ServiceAcc);

            return ResultCode.Success;
        }

        [CommandCmif(22)]
        public ResultCode GetState(ServiceCtx context)
        {
            Logger.Stub?.PrintStub(LogClass.ServiceAcc);

            return ResultCode.Success;
        }

    }
}
