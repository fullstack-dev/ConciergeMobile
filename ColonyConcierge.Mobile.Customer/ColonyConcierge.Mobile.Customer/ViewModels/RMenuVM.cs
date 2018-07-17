//using ColonyConcierge.APIData.Data;
//using System;
//using System.Collections.Generic;
//using System.Linq;

//namespace ColonyConcierge.Mobile.Customer.ViewModels
//{
//    public class RestaurantVM
//    {
//        public int LocationId { get; set; }
//        public bool Closed { get; set; } = false;
//        public string Address { get; set; }
//        public string LogoURL { get; set; }
//        public Restaurant Restaurant { get; set; }
//        public List<RMenuVM> Menus { get; set; }
//        public RestaurantSearchResultData SearchResultData { get; set; }
//        public Address AddressObj { get; set;}
//    }

//    public class RMenuVM
//    {
//        public RMenu Menu { get; set; }
//        public bool IsAvailable { get; set; } = true;
//        public List<RMenuGroupVM> MenuGroups { get; set; }

//    }
//    public class RMenuGroupVM
//    {
//        public RMenuGroup Group { get; set; }
//        public List<MenuItemVM> MenuItems { get; set; }
//    }
//    public class MenuItemVM
//    {
//        public List<decimal?> ModifierPrices { get; set; }
//        public decimal? MenuItemOriginalPrice { get; set; }
//        public RMenuItem MenuItem { get; set; }
//    }
//    public class RMenuGroupModifierVM
//    {
//        public RMenuModifierGroup MenuModifierGroup { get; set; }
//        public List<RMenuModifierVM> MenuModifiers { get; set; }
//    }
//    public class RMenuModifierVM
//    {
//        public decimal? ModifierPrice { get; set; }
//        public bool Additive { get; set; } = true;
//        public bool IsSelected { get; set; } = false;
//        public int Quantity { get; set; } = 1;
//        public RMenuModifier MenuModifier { get; set; }
//        public RMenuModifierGroup SubMenuModifierGroup { get; set; }
//        public List<RMenuModifierVM> SubMenuModifiers { get; set; }
//    }

//    public class RMenuItemVM
//    {
//        public int LocationId { get; set; }
//        public int MenuId { get; set; }
//        public RMenuItem MenuItem { get; set; }
//    }
//}