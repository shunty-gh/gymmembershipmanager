using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Shunty.Logging;

namespace Shunty.GymMembershipManager
{
    public class CancelAction : MembershipAction
    {
        private static readonly ILog _log = LogProvider.For<CancelAction>();

        public CancelAction(IConnectionManager manager) : base(manager)
        { }

        public async Task<bool> CancelBooking(int bookingId)
        {
            var queryparams = new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>("bookingId", bookingId.ToString()) };

            // First need to call the cancel booking url and then the cancel confirmation url
            var cancelResult = await Manager.GetResultAsync(Consts.CancelUrl, queryparams);
            _log.TraceFormat("Cancel result: {CancelResult}", cancelResult);

            cancelResult = await Manager.GetResultAsync(Consts.CancelConfirmationUrl, queryparams);
            _log.TraceFormat("Cancel confirmation result: {CancelResult}", cancelResult);

            // If successful, the result should contain "Your booking has been cancelled"
            return cancelResult.Contains("Your booking has been cancelled");
        }
    }
}
