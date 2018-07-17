using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyConcierge.APIData.Data
{
    public enum ServiceKindCodes
    {
        Unknown,
        Shopping_Grocery,
        Shopping_Household,
        Shopping_General,
        Errand_DryCleaning = 0x10,
        Errand_PetServices,
        Errand_CarServices,
        Errand_Other,
        PersonalService_Laundry = 0x20,
        PersonalService_Waiting,
        PersonalService_HouseSitting,
        PersonalService_Handyman,
        PersonalService_SnowBird,
        SpecialRequest = 0x30,
        Meals_General = 0x40,
        Restaurant_General = 0x50,
        Restaurant_Pickup,
        Restaurant_Delivery,
        Restaurant_GroupedDelivery

    }
}
