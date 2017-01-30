using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shunty.GymMembershipManager
{
    public class AddBookingResult
    {
        public bool Success { get; set; }
        public bool AllowRetry { get; set; }
        public string Message { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string FacilityName { get; set; }
        public string ActivityName { get; set; }
        public string ResourceLocation { get; set; }

        public string ToQueryString()
        {
            string istrue = "true", isfalse = "false";
            var dfrom = StartTime.ToString("ddd, dd MMM yyyy hh:mm:ss") + " GMT";
            var dfromesc = Uri.EscapeDataString(dfrom).Replace("%20", "+");
            var dto = EndTime.ToString("ddd, dd MMM yyyy hh:mm:ss") + " GMT";
            var dtoesc = Uri.EscapeDataString(dto).Replace("%20", "+");
            var sb = new StringBuilder();
            sb.Append($"Success={(Success ? istrue : isfalse)}");
            sb.Append($"&AllowRetry={(AllowRetry ? istrue : isfalse)}");
            sb.Append($"&Message={Uri.EscapeUriString(Message.Replace(" ", "+"))}");
            sb.Append($"&StartTime={dfromesc}");
            sb.Append($"&EndTime={dfromesc}");
            sb.Append($"&FacilityName={Uri.EscapeUriString(FacilityName.Replace(" ", "+"))}");
            sb.Append($"&ActivityName={Uri.EscapeUriString(ActivityName.Replace(" ", "+"))}");
            sb.Append($"&ResourceLocation={Uri.EscapeUriString(ResourceLocation.Replace(" ", "+"))}");
            sb.Append($"&KeepThis={istrue}");
            return sb.ToString();
        }
    }
}
