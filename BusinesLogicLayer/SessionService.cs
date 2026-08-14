using DataAccessLayer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SolarVolt.DTOs;
using SolarVolt.Models;

namespace BusinesLogicLayer
{
    public class SessionService
    {
        private readonly ApplicationDbContext _context;
        public SessionService(ApplicationDbContext context)
        {
            _context = context; 
        }

        /*  double ConvertToHours(string Time)
          {
              if (string.IsNullOrEmpty(Time))
              {
                  return 0;
              }
              char unit =char.ToLower( Time[0]);
              string TimePart=Time.Substring(1);  
              if (!double.TryParse(TimePart, out double TimeAfterExtacting))
              {
                  return 0;
              }

              if (unit == 'h')
                  return TimeAfterExtacting;
              else if (unit == 'm')
                  return (TimeAfterExtacting/60);
              return -1;
          }*/
        double ConvertToHours(string time)
        {
            if (string.IsNullOrWhiteSpace(time))
                return 0;

            time = time.ToLower().Trim();
            double totalHours = 0;

            // التعامل مع الساعات
            // (مثال
            // : "6h"
            // أو
            // "h6")
            if (time.Contains("h"))
            {
                var parts = time.Split('h');
                // بناخد الجزء الرقمي سواء قبل الـ
                // h
                // أو بعدها
                string numStr = string.IsNullOrWhiteSpace(parts[0]) ? parts[1] : parts[0];
                if (double.TryParse(numStr.Trim(), out double hours))
                {
                    totalHours += hours;
                }
            }

            if (time.Contains("m"))
            {
                // استخراج الجزء الخاص بالدقائق
                string mPart = time.Contains("h") ? time.Split('h')[1] : time;
                mPart = mPart.Replace("m", "").Trim();

                if (double.TryParse(mPart, out double minutes))
                {
                    totalHours += minutes / 60.0;
                }
            }

            return Math.Round( totalHours,2);
               // بدل ما ترجع كسر طويل مثل 0.333333333333
              // برجعلك 0.33 فقط
             // لان الديسمل عاملو شي 16 انا
        }

        public async Task<bool> CreateSession(CreateSessionDTO createSessionDTO,int UserID )
        {
            int TotalWatt = 0;
            var SessionItems=new List<Energy_Input_Item>();    

            foreach (var item in createSessionDTO.Items)
            {
                int ActualWatt = item.Watt ?? await _context.Appliances.Where(a => a.ApplianceID == item.ApplianceID).Select(a => a.DefaultWattage).FirstOrDefaultAsync();   //ادا كان صفر رح نجيب القيمة الافتراضية من الداتابيز 
                TotalWatt += ActualWatt * item.Quantity;



                SessionItems.Add
                    (
                        new Energy_Input_Item()
                            {
                                ApplianceID= item.ApplianceID,  
                                Quantity=item.Quantity,
                                WattOverride= item.Watt,
                                OperatingHours= ConvertToHours(item.OpeatingTime),

                        }
                    );
            }
            Energy_Input_Session energy_Input_Session = new Energy_Input_Session()
            {
                UserID = 1,              //            //                   //                 //            //          // 
                CreatedAt = DateTime.Now,
                SourceType = createSessionDTO.SourceType,
                TotalWatt=TotalWatt,
                energy_Input_Items_List = SessionItems  ,
            };

            await _context.Energy_Input_Sessions.AddAsync(energy_Input_Session);
            await _context.SaveChangesAsync();  


            return true;
        }



    }
}
