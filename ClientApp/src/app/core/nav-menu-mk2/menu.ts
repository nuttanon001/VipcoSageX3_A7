export interface MenuModel {
  label?: string;
  link?: string;
  icon?: string;
  children?: Array<MenuModel>
}

export const ModulesList: Array<MenuModel> = [{
  label: "SageX3 Data(s)",
  children: [
    {
      label: "Purchase Request",
      link: "/purchase-request",
      icon: "add_shopping_cart"
    },
    {
      label: "For Stock",
      children: [
        {
          label: "Stock Onhand",
          link: "/stock-onhand",
          icon: "swap_horiz"
        },
        {
          label: "Stock Onhand By Loc",
          link: "/stock-onhand/onhandmk2",
          icon: "surround_sound"
        },
        {
          label: "Stock Balance",
          link: "/stock-onhand/stock-balance",
          icon: "local_drink"
        },
        {
          label: "Issue By Workgroup",
          link: "/stock-onhand/issus-workgroup",
          icon: "folder_shared_black"
        },
        {
          label: "Stock Movement",
          link: "/stock-movement",
          icon: "settings_ethernet"
        },

      ]
    },
    {
      label: "For Account",
      children: [
        {
          label: "Payment",
          link: "/payment",
          icon: "account_balance_wallet"
        },
        {
          label: "Retention",
          link: "/payment/retention",
          icon: "chrome_reader_mode"
        },
        {
          label: "SubPayment",
          link: "/payment/payment-sub",
          icon: "multiline_chart"
        },
        {
          label: "Issue & Journal",
          link: "/mics-account",
          icon: "card_membership"
        },
        {
          label: "Supplier BP Invoice",
          link: "/mics-account/invoices",
          icon: "library_books"
        },
        {
          label: "Journal Entry",
          link: "/mics-account/journal2",
          icon: "gesture"
        },
        {
          label: "OutStanding/OverDue",
          link: "/mics-account/out-due",
          icon: "open_in_new"
        },
      ]
    },
  ]
},
{
  label: "For Purchase",
  children: [
    {
      label: "PurchaseOrder Report-1",
      link: "/purchase-request/posub-report",
      icon: "shopping_basket"
    },
    {
      label: "PurchaseRequest Outstanding",
      link: "/purchase-request/pr-outstanding",
      icon: "toll_black"
    },
    {
      label: "PurchaseOrder Outstanding",
      link: "/purchase-request/po-outstanding",
      icon: "fiber_smart_record"
    },
    {
      label: "PurchaseRequest Received",
      link: "/purchase-extend",
      icon: "event"
    }

  ]
},
//{
//  label: "Admin",
//  children: [{
//    label: "Admin 1"
//  }, {
//    label: "Admin 2"
//  }, {
//    label: "Admin 3"
//  }, {
//    label: "Admin 4"
//  }]
//  }
];
